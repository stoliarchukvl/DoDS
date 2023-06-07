using Hazelcast;

namespace Lab2Hazelcast;

public static class OptimisticLocking
{
    public static async Task Run()
    {
        var key = "1";
        var thread1 = new Thread(async () => await Insert(key, $"Thread 1 finished.Result = "));
        var thread2 = new Thread(async () => await Insert(key, $"Thread 2 finished.Result = "));
        var thread3 = new Thread(async () => await Insert(key, $"Thread 3 finished.Result = "));
        thread1.Start();
        thread2.Start();
        thread3.Start();

        Thread.Sleep(TimeSpan.FromSeconds(60));

        var options = new HazelcastOptionsBuilder().Build();
        options.ClusterName = "cluster";

        var client = await HazelcastClientFactory.StartNewClientAsync(options);

        var map = await client.GetMapAsync<string, string>("my-distributed-map");

        Console.WriteLine("Finished! Result = " + await map.GetAsync(key));
    }

    private static async Task Insert(string key, string lastMessage)
    {
        var options = new HazelcastOptionsBuilder().Build();
        options.ClusterName = "cluster";

        var client = await HazelcastClientFactory.StartNewClientAsync(options);

        var map = await client.GetMapAsync<string, string>("my-distributed-map");

        await map.PutAsync(key, 0.ToString());
        Console.WriteLine("Start");

        for (var i = 0; i < 1000; i++)
        {
            while (true)
            {
                var oldValue = int.Parse(await map.GetAsync(key));
                var newValue = oldValue + 1;
                if (await map.ReplaceAsync(key, newValue.ToString(), oldValue.ToString()))
                {
                    break;
                }
            }
        }

        Console.WriteLine(lastMessage + await map.GetAsync(key));
    }
}