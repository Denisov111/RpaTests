using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingStartedClient
{
    public class CallbackHandler : IRpaServiceCallback
    {
        public string GetPath()
        {
            return "Hi!!!";
        }
    }
}
