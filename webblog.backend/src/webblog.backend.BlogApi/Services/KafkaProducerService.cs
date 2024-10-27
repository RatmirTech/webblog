using Confluent.Kafka;
using System.Text.Json;
using webblog.backend.BlogApi.Abstractions;

namespace webblog.backend.BlogApi.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly string _bootstrapServers;

        public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
        {
            _logger = logger;
            _bootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers") ?? "localhost:9092";

            var config = new ProducerConfig
            {
                BootstrapServers = _bootstrapServers,
                Acks = Acks.All,
                LingerMs = 5,
                CompressionType = CompressionType.Snappy
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task ProduceAsync(string topic, object message)
        {
            try
            {
                var jsonMessage = JsonSerializer.Serialize(message);
                var kafkaMessage = new Message<Null, string> { Value = jsonMessage };

                var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);

                _logger.LogInformation($"Delivered message to '{deliveryResult.TopicPartitionOffset}'");
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError($"Failed to deliver message: {ex.Error.Reason}");
            }
        }
    }
}
