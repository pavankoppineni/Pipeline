using Microsoft.Extensions.Hosting;
using Pipeline.Framework;

namespace Pipeline.Shell.Pipelines
{
    public class LeaveRequestBackgroundService : BackgroundService
    {
        private const string Name = "Leave.Request.Pipeline";
        private readonly IPipeline _pipeline;

        public LeaveRequestBackgroundService()
        {
            _pipeline = BuildPipeline();
        }

        private IPipeline BuildPipeline()
        {
            return new Framework.Pipeline.Builder()
                .Name(Name)
                .AddStep(DomainStepOne, 100, 1000)
                .AddStep(DomainStepTwo, 10, 2000)
                .AddStep(DomainStepThree, 200, 3000)
                .AddStep(DomainStepFour, 60, 4000)
                .Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                var messages = new List<Message>();
                var count = 100_00_00;
                while (count > 0)
                {
                    var message = new Message();
                    messages.Add(message);
                    count--;
                }
                await _pipeline.ProcessMessagesAsync(messages);
            }
        }

        private Task<IList<Message>> DomainStepOne(IList<Message> messages)
        {
            Console.WriteLine(nameof(DomainStepOne));
            return Task.FromResult(messages);
        }

        private Task<IList<Message>> DomainStepTwo(IList<Message> messages)
        {
            Console.WriteLine(nameof(DomainStepTwo));
            return Task.FromResult(messages);
        }

        private Task<IList<Message>> DomainStepThree(IList<Message> messages)
        {
            Console.WriteLine(nameof(DomainStepThree));
            return Task.FromResult(messages);
        }

        private Task<IList<Message>> DomainStepFour(IList<Message> messages)
        {
            Console.WriteLine(nameof(DomainStepFour));
            return Task.FromResult(messages);
        }
    }
}
