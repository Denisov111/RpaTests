﻿using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
//using System.Threading.Tasks;
namespace TodoList
{
    /// <summary>
    /// Interaction logic for TodoWindowControl.
    /// </summary>
    public partial class TodoWindowControl : UserControl
    {
        public TodoWindow parent;
        /// <summary>
        /// Initializes a new instance of the <see cref="TodoWindowControl"/> class.
        /// </summary>
        public TodoWindowControl(TodoWindow window)
        {
            this.InitializeComponent();
            parent = window;
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
            if (textBox.Text.Length > 0)
            {
                var item = new TodoItem(this, textBox.Text);
                listBox.Items.Add(item);
                TrackSelection();
                CheckForErrors();
            }
        }

        public void CheckForErrors()
        {
            foreach (TodoItem item in listBox.Items)
            {
                if (item.DueDate < DateTime.Now)
                {
                    ReportError("To Do Item is out of date: "
                        + item.ToString());
                }
            }
        }
        public void UpdateList(TodoItem item)
        {
            var index = listBox.SelectedIndex;
            listBox.Items.RemoveAt(index);
            listBox.Items.Insert(index, item);
            listBox.SelectedItem = index;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TrackSelection();
        }

        private SelectionContainer mySelContainer;
        private System.Collections.ArrayList mySelItems;
        private IVsWindowFrame frame = null;

        private void TrackSelection()
        {
            if (frame == null)
            {
                var shell = parent.GetVsService(typeof(SVsUIShell)) as IVsUIShell;
                if (shell != null)
                {
                    var guidPropertyBrowser = new
                    Guid(ToolWindowGuids.PropertyBrowser);
                    shell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate,
                    ref guidPropertyBrowser, out frame);
                }
            }
            if (frame != null)
            {
                frame.Show();
            }
            if (mySelContainer == null)
            {
                mySelContainer = new SelectionContainer();
            }

            mySelItems = new System.Collections.ArrayList();

            var selected = listBox.SelectedItem as TodoItem;
            if (selected != null)
            {
                mySelItems.Add(selected);
            }

            mySelContainer.SelectedObjects = mySelItems;

            ITrackSelection track = parent.GetVsService(typeof(STrackSelection))
                                    as ITrackSelection;
            if (track != null)
            {
                track.OnSelectChange(mySelContainer);
            }
        }

        [Guid("72de1eAD-a00c-4f57-bff7-57edb162d0be")]
        public class TodoWindowTaskProvider : TaskProvider
        {
            public TodoWindowTaskProvider(IServiceProvider sp)
                : base(sp)
            {
            }
        }

        private TodoWindowTaskProvider taskProvider;
        private void CreateProvider()
        {
            if (taskProvider == null)
            {
                taskProvider = new TodoWindowTaskProvider(parent);
                taskProvider.ProviderName = "To Do";
            }
        }

        private void ClearError()
        {
            CreateProvider();
            taskProvider.Tasks.Clear();
        }
        private void ReportError(string p)
        {
            CreateProvider();
            var errorTask = new Microsoft.VisualStudio.Shell.TaskListItem();
            errorTask.CanDelete = false;
            errorTask.Category = TaskCategory.Comments;
            errorTask.Text = p;

            taskProvider.Tasks.Add(errorTask);

            taskProvider.Show();

            var taskList = parent.GetVsService(typeof(SVsTaskList))
                as IVsTaskList2;
            if (taskList == null)
            {
                return;
            }

            var guidProvider = typeof(TodoWindowTaskProvider).GUID;
            taskList.SetActiveProvider(ref guidProvider);
        }

        //public class Task
        //{
        //    public bool CanDelete { get; set; }
        //}
    }
}