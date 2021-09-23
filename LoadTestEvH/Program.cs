using System;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LoadTestEvH
{
    class Program
    {
        static EventHubProducerClient _producerClient;

        static async Task Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Incorrect parameters, correct parameters: LoadTestEvH <jsonFileLocation> <evhName> <evhConnString>");
                    return;
                }

                var jsonFileLocation = args[0];
                var evhName = args[1];
                var connString = args[2];

                var batchSize = 300;
                if (args.Length == 4)
                {
                    var tmpBatchSize = Int32.Parse(args[3]);
                    if (tmpBatchSize > 0 && tmpBatchSize <= batchSize)
                    {
                        batchSize = tmpBatchSize;
                    }
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();
                await PublishData(jsonFileLocation, evhName, connString, batchSize);
                Console.WriteLine($"Process done in {sw.ElapsedMilliseconds} ms");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task PublishData(string jsonFileLocation, string evhName, string connString, int batchSize)
        {

            _producerClient = new EventHubProducerClient(connString, evhName,
                new EventHubProducerClientOptions()
                {
                    RetryOptions = { MaximumRetries = 10, Mode = EventHubsRetryMode.Exponential }
                });

            var dataBatch = await _producerClient.CreateBatchAsync();

            using var reader = new StreamReader(jsonFileLocation);

            var totalSent = 0;
            var i = 0;
            while (reader.Peek() > 0)
            {
                ++i;

                var str = reader.ReadLine();
                var obj = JObject.Parse(str);
                var data = obj.SelectToken("message");

                if (!dataBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"{data}"))))
                {
                    // if it is too large for the batch
                    throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
                }

                if (i % batchSize == 0)
                {
                    totalSent += batchSize;
                    // Use the producer client to send the batch of events to the event hub
                    await _producerClient.SendAsync(dataBatch);
                    Console.WriteLine($"A batch of {totalSent} events has been published.");
                    dataBatch = await _producerClient.CreateBatchAsync();
                }
            }

            if (dataBatch.Count > 0)
            {
                totalSent += dataBatch.Count;
                // Use the producer client to send the batch of events to the event hub
                await _producerClient.SendAsync(dataBatch);
                Console.WriteLine($"A batch of {totalSent} events has been published.");
            }

            await _producerClient.DisposeAsync();
        }

        private static async Task<IJEnumerable<JToken>> LoadJsonFile(string jsonFileLocation)
        {
            using StreamReader reader = new StreamReader(jsonFileLocation);
            var readFileResult = await reader.ReadToEndAsync();
            var jArray = JArray.Parse(readFileResult);
            return jArray.AsJEnumerable();
        }
    }
}
