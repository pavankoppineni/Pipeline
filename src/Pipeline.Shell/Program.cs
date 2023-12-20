// See https://aka.ms/new-console-template for more information
using Pipeline.Framework;

Console.WriteLine("Hello, World!");

Task<IList<Message>> DomainStepOne(IList<Message> messages)
{
    Console.WriteLine("Domain Step One");
    return Task.FromResult(messages);
}

Task<IList<Message>> DomainStepTwo(IList<Message> messages)
{
    Console.WriteLine("Domain Step Two");
    return Task.FromResult(messages);
}

var pipeline = new Pipeline.Framework.Pipeline.Builder()
    .Name("Employee Pipeline")
    .AddStep(DomainStepOne, 20, 5000)
    .AddStep(DomainStepTwo, 30, 10000)
    .Build();

var messages = new List<Message>();
var count = 100000;
while (count > 0)
{
    messages.Add(new Message());
    count--;
}
await pipeline.ProcessMessagesAsync(messages);
Console.WriteLine("Complete");