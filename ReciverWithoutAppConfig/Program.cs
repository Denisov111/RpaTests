using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReciverWithoutAppConfig
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:8000/RpaService/RpaService");
            WSHttpBinding binding = new WSHttpBinding();
            EndpointAddress address = new EndpointAddress(baseAddress);

            RpaServiceClient client = new RpaServiceClient(binding, address);

            for (int i = 0; ; i++)
            {
                List<string> result = client.ReciveMessage().ToList();
                Console.WriteLine("result {0}", string.Join(Environment.NewLine, result));
                Thread.Sleep(1000);
            }
        }
    }
}
