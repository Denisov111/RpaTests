using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace RpaServiceLibrary
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IRpaServiceCallback))]
    public interface IRpaService
    {
        [OperationContract]
        string SendMessage(string message);

        [OperationContract]
        List<string> ReciveMessage();

        [OperationContract]
        void ClearQueue();

        [OperationContract]
        string GetActiveTabFilePath();

        [OperationContract]
        void InitExtensionChannel();
    }

    public interface IRpaServiceCallback
    {
        /// <summary>
        /// GetActiveTabFilePath
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string GetPath();
    }
}
