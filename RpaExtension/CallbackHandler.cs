using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpaExtension
{
    public class CallbackHandler : IRpaServiceCallback
    {
        public string GetPath()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return MessagesWorker.Dte.ActiveDocument.FullName;
        }
    }
}
