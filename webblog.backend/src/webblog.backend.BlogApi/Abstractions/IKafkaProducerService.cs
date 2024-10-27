namespace webblog.backend.BlogApi.Abstractions
{
    public interface IKafkaProducerService
    {
        Task ProduceAsync(string topic, object message);
    }
}
