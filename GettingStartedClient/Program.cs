using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GettingStartedClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            InstanceContext site = new InstanceContext(new CallbackHandler());
            RpaServiceClient client = new RpaServiceClient(site, "WSDualHttpBinding_IRpaService");
            //var result = client.GetActiveTabFilePath();
            for (int i = 0; i<100; i++)
            {
                string mess= $"{string.Format("[{0:H:mm:ss:fff}] ", DateTime.Now)}{i}";
                var result = client.SendMessage(i.ToString());
                Console.WriteLine("result {0}", result);
                Thread.Sleep(50);
            }

            //for (int i = 0; ; i++)
            //{
            //    var result = client.GetActiveTabFilePath();
            //    Console.WriteLine("result {0}", result);
            //    Thread.Sleep(100);
            //}

            //Console.WriteLine(result);
            Console.ReadLine();
            //client.Close();
        }
    }
}
