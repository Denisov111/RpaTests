using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace RpaServiceLibrary
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
    public class RpaService : IRpaService
    {
        private Queue<string> messages = new Queue<string>();
        private IRpaServiceCallback callback = null;

        public RpaService() 
        {
        }

        public string SendMessage(string message)
        {
            try
            {
                messages.Enqueue(message);
                return $"message {message} was added";
            }
            catch (Exception ex)
            {
                return ex.StackTrace;
            }
        }

        public List<string> ReciveMessage()
        {
            List<string> messList = new List<string>();
            try
            {
                while (messages.Count > 0)
                {
                    messList.Add(messages.Dequeue());
                    if (messList.Count > 100)
                    {
                        break;
                    }
                }
            }
            catch { }

            return messList;
        }

        public void ClearQueue()
        {
            try
            {
                messages.Clear();
            }
            catch { }
        }

        public string GetActiveTabFilePath()
        {
            try
            {
                var path = callback.GetPath();
                return path;
            }
            catch(Exception ex)
            {
                return ex.StackTrace;
            }
        }

        public void InitExtensionChannel()
        {
            callback = OperationContext.Current.GetCallbackChannel<IRpaServiceCallback>();
        }
    }
}
