using Hazelcast;

namespace Lab2Hazelcast;

public static class Queue
{
    public static async Task Run()
    {
        var options = new HazelcastOptionsBuilder().Build();
        options.ClusterName = "cluster";

        var client = await HazelcastClientFactory.StartNewClientAsync(options);

        var queue = await client.GetQueueAsync<string>("queue");


        var thread1 = new Thread(async () => await Produce());
        var thread2 = new Thread(async () => await Consume("Consumer1", $"Consumer 1 finished"));
        var thread3 = new Thread(async () => await Consume("Consumer2", $"Consumer 2 finished"));
        thread1.Start();
        thread2.Start();
        thread3.Start();

        Thread.Sleep(TimeSpan.FromSeconds(10));
    }

    private static async Task Consume(string consumer, string finish)
    {
        var options = new HazelcastOptionsBuilder().Build();
        options.ClusterName = "cluster";

        var client = await HazelcastClientFactory.StartNewClientAsync(options);

        var queue = await client.GetQueueAsync<string>("queue");

        var consumedCount = 0;

        while (consumedCount < 50)
        {
            var head = await queue.TakeAsync();
            Console.WriteLine(consumer + " consuming " + head);
            consumedCount++;
        }
        Console.WriteLine(finish);
    }

    private static async Task Produce()
    {
        var options = new HazelcastOptionsBuilder().Build();
        options.ClusterName = "cluster";

        var client = await HazelcastClientFactory.StartNewClientAsync(options);

        var queue = await client.GetQueueAsync<string>("queue");

        for (int i = 0; i < 100; i++)
        {
            await queue.OfferAsync(i.ToString());
            Console.WriteLine("Producing " + i);
        }

        Console.WriteLine("Producing finished");
    }
}