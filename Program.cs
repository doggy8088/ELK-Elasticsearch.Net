using System;
using System.Threading.Tasks;
using Elasticsearch.Net;

namespace c1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // var lowlevelClient = new ElasticLowLevelClient();

            var settings = new ConnectionConfiguration(new Uri("http://localhost:9200")).RequestTimeout(TimeSpan.FromMinutes(2));
            var lowlevelClient = new ElasticLowLevelClient(settings);

            // var uris = new []
            // {
            //     new Uri("http://es01:9200"),
            //     new Uri("http://es02:9201"),
            //     new Uri("http://es03:9202"),
            // };

            // var connectionPool = new SniffingConnectionPool(uris);
            // var settings = new ConnectionConfiguration(connectionPool);

            // var lowlevelClient = new ElasticLowLevelClient(settings);

            var person = new Person
            {
                FirstName = "Will",
                LastName = "Laarman"
            };

            var ndexResponse = lowlevelClient.Index<BytesResponse>("people", "1", PostData.Serializable(person));
            byte[] responseBytes = ndexResponse.Body;

            Console.WriteLine();
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(responseBytes));
            Console.WriteLine();

            var asyncIndexResponse = await lowlevelClient.IndexAsync<StringResponse>("people", "1", PostData.Serializable(person));
            string responseString = asyncIndexResponse.Body;

            var searchResponse = lowlevelClient.Search<StringResponse>("people", PostData.Serializable(new
            {
                from = 0,
                size = 10,
                query = new
                {
                    match = new
                    {
                        firstName = new {
                            query = "Will"
                        }
                    }
                }
            }));

            var successful = searchResponse.Success;
            var successOrKnownError = searchResponse.SuccessOrKnownError;
            var exception = searchResponse.OriginalException;
            var responseJson = searchResponse.Body;
            Console.WriteLine(responseJson);
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}