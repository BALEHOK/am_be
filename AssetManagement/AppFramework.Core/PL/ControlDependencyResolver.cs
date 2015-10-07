using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AppFramework.Core.PL
{
    public class ContolDependencyResolver
    {
        private readonly IUnityContainer _container;
        private readonly HashSet<Control> _alreadyInitializedControls = new HashSet<Control>();

        public ContolDependencyResolver(IUnityContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            _container = container;
        }

        public void InitializeControlTree(Control control)
        {
            InitializeControl(control);
            foreach (Control ctl in GetControlTree(control))
            {
                InitializeControl(ctl);
            }
        }

        private void InitializeControl(Control control)
        {
            //var typeFullName = control.GetType().FullName ?? string.Empty;
            //if (typeFullName.StartsWith("System.Web"))
            //    return;

            BuildUp(control);
            if (control as DataBoundControl != null)
            {
                var dataBoundControl = control as DataBoundControl;
                dataBoundControl.DataBound += this.InitializeDataBoundControl;
            }
            else if (control as Repeater != null)
            {
                ((Repeater)control).ItemDataBound += OnItemDataBound;
            }
        }

        // Get the controls in the page's control tree excluding the page itself
        private IEnumerable<Control> GetControlTree(Control root)
        {
            foreach (Control child in root.Controls)
            {
                yield return child;
                foreach (Control c in GetControlTree(child))
                {
                    yield return c;
                }
            }
        }

        private void OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                foreach (Control control in GetControlTree(e.Item))
                {
                    InitializeControl(control);
                }
            }
        }

        private void InitializeDataBoundControl(object sender, EventArgs e)
        {
            var control = (DataBoundControl)sender;
            if (control != null)
            {
                control.DataBound -= InitializeDataBoundControl;
                BuildUp(control);
            }
        }

        private void BuildUp(Control instance)
        {
            if (!_alreadyInitializedControls.Contains(instance))
            {
                _container.BuildUp(instance.GetType(), instance);
                // Ensure every user control is only initialized once.
                _alreadyInitializedControls.Add(instance);
            }
        }
    }
}
