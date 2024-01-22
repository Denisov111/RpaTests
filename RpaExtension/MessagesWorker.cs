using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO.Packaging;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RpaExtension
{
    internal class MessagesWorker
    {
        public static DTE Dte { get; set; }

        private static AsyncPackage _package;
        private static IVsOutputWindowPane _pane;

        #region Singleton

        private static readonly MessagesWorker instance = new MessagesWorker();

        public static MessagesWorker GetInstance(AsyncPackage package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            return instance;
        }

        private MessagesWorker() { }

        #endregion Singleton

        public async Task WorkAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            // получение DTE
            object dte_ = await ServiceProvider.GetGlobalServiceAsync(typeof(DTE));
            DTE dte = (DTE)dte_;
            Dte = dte;

            //ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte);
            //TextDocument objTextDoc = (TextDocument)dte.ActiveDocument.Object("TextDocument");
            //var res2 = objTextDoc.Language;
            //var res3 = dte.ActiveDocument.FullName;

            // настройка клиента службы
            Uri baseAddress = new Uri("http://localhost:8000/RpaService/RpaService");
            WSDualHttpBinding binding = new WSDualHttpBinding();
            EndpointAddress address = new EndpointAddress(baseAddress);
            InstanceContext site = new InstanceContext(new CallbackHandler());
            RpaServiceClient client = new RpaServiceClient(site, binding, address);

            // создание канала обратной связи для службы
            client.InitExtensionChannel();

            // TODO: Создание панели для вывода сообщений RPA 
            await CreatePane(new Guid(), "Created Pane", true, false);

            // Опрос службы на предмет новых сообщений от RPA
            for (int i = 0; ; i++)
            {
                try
                {
                    var res = await client.ReciveMessageAsync();
                    List<string> result = res.ToList();

                    // Вывод в панель вывода RPA
                    await WriteToRpaLogPane(string.Join(Environment.NewLine, result) + Environment.NewLine);
                }
                catch { }

                await Task.Delay(1000);
            }
        }

        private async Task WriteToRpaLogPane(string message)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            _pane.OutputString(message);
        }

        private static async Task CreatePane(Guid paneGuid, string title, bool visible, bool clearWithSolution)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var outp = await _package.GetServiceAsync(typeof(SVsOutputWindow));
            IVsOutputWindow output = (IVsOutputWindow)outp;
            //IVsOutputWindowPane pane;

            // Create a new pane.
            output.CreatePane(
                ref paneGuid,
                title,
                Convert.ToInt32(visible),
                Convert.ToInt32(clearWithSolution));

            // Retrieve the new pane.
            output.GetPane(ref paneGuid, out _pane);
        }
    }
}
