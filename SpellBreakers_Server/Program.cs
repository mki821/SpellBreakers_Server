namespace SpellBreakers_Server
{
    internal class Program
    {
        public static async Task Main()
        {
            Server server = new Server(5050);
            await server.StartAsync();
        }
    }
}
