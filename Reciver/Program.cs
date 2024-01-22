
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reciver
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RpaServiceClient client = new RpaServiceClient();

            for (int i = 0; ; i++)
            {
                List<string> result = client.ReciveMessage().ToList();
                Console.WriteLine("result {0}", string.Join(Environment.NewLine, result));
                Thread.Sleep(1000);
            }

            Console.WriteLine("\nPress <Enter> to terminate the client.");
            Console.ReadLine();
            client.Close();
        }
    }
}
