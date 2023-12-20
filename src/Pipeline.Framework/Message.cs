namespace Pipeline.Framework
{
    public class Message
    {
        public Guid Id { get; init; }
        public IDictionary<string, object> Data { get; init; }

        public static Message Create()
        {
            return new Message
            {
                Id = Guid.NewGuid(),
                Data = new Dictionary<string, object>()
            };
        }
    }
}
