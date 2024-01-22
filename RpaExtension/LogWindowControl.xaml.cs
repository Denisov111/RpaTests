using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RpaExtension
{
    /// <summary>
    /// Interaction logic for LogWindowControl.
    /// </summary>
    public partial class LogWindowControl : UserControl
    {
        public static DTE Dte {  get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogWindowControl"/> class.
        /// </summary>
        public LogWindowControl()
        {
            this.InitializeComponent();

            GetMessages();
        }

        private async void GetMessages()
        {
            await Task.Delay(30000);
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            object dte_ = await ServiceProvider.GetGlobalServiceAsync(typeof(DTE));
            DTE dte = (DTE)dte_;
            Dte = dte;
            object dte2_ = await ServiceProvider.GetGlobalServiceAsync(typeof(EnvDTE80.DTE2));
            EnvDTE80.DTE2 dte2 = (EnvDTE80.DTE2)dte2_;
            ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte);

            TextDocument objTextDoc = (TextDocument)dte.ActiveDocument.Object("TextDocument");
            var res2 = objTextDoc.Language;
            var res3 = dte.ActiveDocument.FullName;

            Uri baseAddress = new Uri("http://localhost:8000/RpaService/RpaService");
            //WSHttpBinding binding = new WSHttpBinding();
            WSDualHttpBinding binding = new WSDualHttpBinding();
            EndpointAddress address = new EndpointAddress(baseAddress);

            InstanceContext site = new InstanceContext(new CallbackHandler());

            RpaServiceClient client = new RpaServiceClient(site, binding, address);
            client.InitExtensionChannel();
            for (int i = 0; ; i++)
            {
                try
                {
                    var res = await client.ReciveMessageAsync();
                    List<string> result = res.ToList();
                    txtBox.Text = string.Join(Environment.NewLine, result);
                }
                catch { }
                
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "LogWindow");
        }
    }
}