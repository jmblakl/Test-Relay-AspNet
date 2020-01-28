using ClientWrapper.cs;
using System;
using System.Threading.Tasks;

namespace TestDriver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            TheClient client = new TheClient("[replace with namespace]", "[replace with connection name]", "[replace with key name]", "[replace with key]");

            await client.DoQueryForThings();

            Console.WriteLine(await client.PerformSignificantWork("hello world"));
            Console.ReadLine();
        }
    }
}
