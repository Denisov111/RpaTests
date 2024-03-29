﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;

namespace TodoList
{
    public class TodoItem
    {
        private TodoWindowControl parent;
        private string name;
        [Description("Name of the ToDo item")]
        [Category("ToDo Fields")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                parent.UpdateList(this);
            }
        }

        private DateTime dueDate;
        [Description("Due date of the ToDo item")]
        [Category("ToDo Fields")]
        public DateTime DueDate
        {
            get { return dueDate; }
            set
            {
                dueDate = value;
                parent.UpdateList(this);
                parent.CheckForErrors();
            }
        }

        //private TodoWindowControl parent;

        public TodoItem(TodoWindowControl control, string itemName)
        {
            parent = control;
            name = itemName;
            dueDate = DateTime.Now;

            double daysAhead = 0;
            IVsPackage package = parent.parent.Package as IVsPackage;
            if (package != null)
            {
                object obj;
                package.GetAutomationObject("ToDo.General", out obj);

                ToolsOptions options = obj as ToolsOptions;
                if (options != null)
                {
                    daysAhead = options.DaysAhead;
                }
            }

            dueDate = dueDate.AddDays(daysAhead);
        }

        public override string ToString()
        {
            return name + " Due: " + dueDate.ToShortDateString();
        }
    }
}
