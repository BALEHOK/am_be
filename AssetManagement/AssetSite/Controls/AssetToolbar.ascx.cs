using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core;
using IToolbarButton = AppFramework.Core.PL.IToolbarButton;

namespace AssetSite.Controls
{

    /// <summary>
    /// Toolbar
    /// </summary>
    public partial class AssetToolbar : System.Web.UI.UserControl
    {
        public ICollection<Enumerators.ToolbarButtonType> ButtonCollection { get; private set; } 

        public Dictionary<string, string> Options { get; set; }

        public string ExternalScript { get; set; }

        private readonly List<IToolbarButton> _controls = new List<IToolbarButton>();

        public IToolbarButton this[Enumerators.ToolbarButtonType buttonType]
        {
            get { return _controls.SingleOrDefault(c => c.ButtonType == buttonType); }
        }

        public AssetToolbar()
        {
            this.Options = new Dictionary<string, string>();
            ButtonCollection = new ObservableSortedSet<Enumerators.ToolbarButtonType>();
            (ButtonCollection as ObservableSortedSet<Enumerators.ToolbarButtonType>).CollectionChanged += AssetToolbar_CollectionChanged;
        }

        void AssetToolbar_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Enumerators.ToolbarButtonType t;
                IToolbarButton button = null;
                var btnType = (Enumerators.ToolbarButtonType)e.NewItems[0];

                switch (btnType)
                {
                    case Enumerators.ToolbarButtonType.Documents:
                        this.Options.Add("ExternalScript", this.ExternalScript);
                        button
                            = LoadControl("~/Controls/AssetToolbarButton.ascx",
                                          btnType,
                                          Options);
                        break;
                    default:
                        button
                            = LoadControl("~/Controls/AssetToolbarButton.ascx",
                                          btnType,
                                          Options);
                        break;
                }

                if (button == null)
                    throw new Exception(string.Format("Unable to load button of type {0}", btnType.ToString()));

                _controls.Add(button);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (var button in _controls)
            {
                phButtons.Controls.Add(button as Control);
            }
        }

        private IToolbarButton LoadControl(string userControlPath, params object[] constructorParameters)
        {
            List<Type> constParamTypes = new List<Type>();

            foreach (object constParam in constructorParameters)
            {
                constParamTypes.Add(constParam.GetType());
            }

            IToolbarButton ctl = Page.LoadControl(userControlPath) as IToolbarButton;

            // Find the relevant constructor
            ConstructorInfo constructor = ctl.GetType().BaseType.GetConstructor(constParamTypes.ToArray());

            //And then call the relevant constructor
            if (constructor == null)
            {
                throw new MemberAccessException("The requested constructor was not found on : " + ctl.GetType().BaseType.ToString());
            }
            else
            {
                constructor.Invoke(ctl, constructorParameters);
            }

            // Finally return the fully initialized UC
            return ctl;
        }
    }
}
