namespace Pipeline.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        Task ProcessMessagesAsync(IList<Message> messages);
    }

    /// <summary>
    /// 
    /// </summary>
    public class Pipeline : IPipeline
    {
        private readonly string _name;
        private PipelineStep _rootStep;

        /// <summary>
        /// Ctor for pipeline
        /// </summary>
        /// <param name="name"></param>
        public Pipeline(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Processes messages
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task ProcessMessagesAsync(IList<Message> messages)
        {
            await _rootStep.ProcessMessagesAsync(messages);
            await _rootStep.Complete();
        }

        /// <summary>
        /// Adds root domain step for pipeline object
        /// </summary>
        /// <param name="pipelineStep"></param>
        private void AddRoot(PipelineStep pipelineStep) => _rootStep = pipelineStep;

        /// <summary>
        /// Builer for creating pipeline object
        /// </summary>
        public class Builder
        {
            private string _pipelineName;
            private PipelineStep _rootStep;
            private PipelineStep _currentStep;

            /// <summary>
            /// Adds domain step to pipeline
            /// </summary>
            /// <param name="domainStep"></param>
            /// <param name="batchSize"></param>
            /// <returns></returns>
            public Builder AddStep(Func<IList<Message>, Task<IList<Message>>> domainStep, int batchSize = 10, int delay = 0)
            {
                if (_rootStep == null)
                {
                    _rootStep = new PipelineStep(domainStep, batchSize, delay);
                    _currentStep = _rootStep;
                    return this;
                }
                var nextStep = new PipelineStep(domainStep, batchSize, delay);
                _currentStep.AddNextStep(nextStep);
                _currentStep = nextStep;
                return this;
            }

            /// <summary>
            /// Adds name to pipeline
            /// </summary>
            /// <returns></returns>
            public Builder Name(string pipelineName)
            {
                _pipelineName = pipelineName;
                return this;
            }

            /// <summary>
            /// Builds pipeline object
            /// </summary>
            /// <returns></returns>
            public Pipeline Build()
            {
                var pipeline = new Pipeline(_pipelineName);
                pipeline.AddRoot(_rootStep);
                return pipeline;
            }
        }
    }
}
