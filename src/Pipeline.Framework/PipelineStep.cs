﻿using System.Threading.Channels;
using System.Threading.Tasks;

namespace Pipeline.Framework
{
    /// <summary>
    /// 
    /// </summary>
    internal class PipelineStep
    {
        private readonly int _batchSize;
        private readonly Func<IList<Message>, Task<IList<Message>>> _domainStep;
        private readonly Task _completionTask;
        private readonly Channel<Message> _channel;
        private PipelineStep _nextStep;

        /// <summary>
        /// 
        /// </summary>
        internal PipelineStep(Func<IList<Message>, Task<IList<Message>>> domainStep, int batchSize)
        {
            _channel = Channel.CreateUnbounded<Message>();
            _batchSize = batchSize;
            _domainStep = domainStep;
            _completionTask = CreateChannelReaderTask();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Task CreateChannelReaderTask()
        {
            return Task.Run(async () =>
            {
                var messageBatch = new List<Message>(); 
                var readerCompletion = _channel.Reader.Completion;
                while (!readerCompletion.IsCompleted)
                {
                    await foreach (var message in _channel.Reader.ReadAllAsync())
                    {
                        messageBatch.Add(message);
                        if (messageBatch.Count < _batchSize)
                        {
                            continue;
                        }
                        await _domainStep(messageBatch);
                        messageBatch = new List<Message>();
                    }
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainStep"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        internal PipelineStep AddNextStep(Func<IList<Message>, Task<IList<Message>>> domainStep, int batchSize)
        {
            var step = new PipelineStep(domainStep, batchSize);
            return AddNextStep(step);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainStep"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        internal PipelineStep AddNextStep(PipelineStep step)
        {
            _nextStep = step;
            return step;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task ProcessMessagesAsync(IList<Message> messages)
        {
            foreach (var message in messages)
            {
                await _channel.Writer.WriteAsync(message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal async Task Complete()
        {
            _channel.Writer.Complete();
            await _completionTask;
            if (_nextStep != null)
            {
                await _nextStep.Complete();
            }
        }
    }
}
