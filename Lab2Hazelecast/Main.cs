using Hazelcast;

namespace Lab2Hazelcast;

public static class Main
{
    public static async Task Run()
    {
        var options = new HazelcastOptionsBuilder().Build();
        options.ClusterName = "cluster";

        var client = await HazelcastClientFactory.StartNewClientAsync(options);

        var map = await client.GetMapAsync<string, string>("my-distributed-map");

        for (var i = 0; i < 1000; i++)
        {
            await map.PutAsync($"Key {i}", i.ToString());
            Console.WriteLine(i);
        }
    }
}