namespace CBS.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessage2(object message, string topic_queue_Name, string connectionString);
    }
}
