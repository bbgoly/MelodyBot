namespace MelodyBot
{
    class Program
    {
        private static void Main(string[] args) => new Melody().RunAsync().GetAwaiter().GetResult();
    }
}
