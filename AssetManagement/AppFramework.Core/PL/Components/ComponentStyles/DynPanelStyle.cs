using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace AppFramework.Core.PL.Components
{
    #region Constants Class

    /// <summary>
    /// Collection of Constants for DynPanel Component
    /// </summary>
    public static class DynPanelConst
    {
        public const string PropHeaderCssStyle = "HeaderCssStyle";
        public const string PropContentCssStyle = "ContentCssStyle";
        public const string PropFloat = "Float";
        public const string PropClear = "Clear";

        public const string FloatLeft = "left";
        public const string FloatRight = "right";
        public const string FloatNone = "none";

        public const string ClearRight = "right";
        public const string ClearLeft = "left";
        public const string ClearBoth = "both";
        public const string ClearNone = "none";
    }

    #endregion

    #region Enumerations Class

    /// <summary>
    /// 
    /// </summary>
    public enum Clear
    {
        Left,
        Right,
        Both,
        None
    }

    /// <summary>
    /// 
    /// </summary>
    public enum Float
    {
        Left,
        Right,
        None
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public class DynPanelStyle: System.Web.UI.WebControls.PanelStyle
    {

        public DynPanelStyle(StateBag viewState) : base(viewState) { }

        #region Properties

        public string HeaderCssStyle
        {
            get 
            { 
                return ViewState[DynPanelConst.PropHeaderCssStyle] != null ? 
                    (string)ViewState[DynPanelConst.PropHeaderCssStyle] : ""; 
            }
            set { ViewState[DynPanelConst.PropHeaderCssStyle] = value; }
        }

        public string ContentCssStyle
        {
            get
            {
                return ViewState[DynPanelConst.PropContentCssStyle] != null ?
                    (string)ViewState[DynPanelConst.PropContentCssStyle] : "";
            }
            set { ViewState[DynPanelConst.PropContentCssStyle] = value; }
        }

        public Float Float
        {
            get { 
                    return ViewState[DynPanelConst.PropFloat] != null ? 
                        (Float)ViewState[DynPanelConst.PropFloat] : Float.None; 
                }
            set { ViewState[DynPanelConst.PropFloat] = value; }
        }

        public Clear Clear
        {
            get 
            { 
                return ViewState[DynPanelConst.PropClear] != null ? 
                    (Clear)ViewState[DynPanelConst.PropClear] : Clear.None; 
            }
            set { ViewState[DynPanelConst.PropClear] = value; }
        }

        #endregion

        #region Helper Methods

        private String GetFloat(Float _float)
        {
            string result = "";

            switch (_float)
            {
                case Float.Left:
                    result = DynPanelConst.FloatLeft;
                    break;
                case Float.Right:
                    result = DynPanelConst.FloatRight;
                    break;
                case Float.None:
                    result = DynPanelConst.FloatNone;
                    break;
                default:
                    result = DynPanelConst.FloatNone;
                    break;
            }

            return result;
        }

        private String GetClear(Clear _clear)
        {
            string result = "";

            switch (_clear)
            {
                case Clear.Left:
                    result = DynPanelConst.ClearLeft;
                    break;
                case Clear.Right:
                    result = DynPanelConst.ClearRight;
                    break;
                case Clear.Both:
                    result = DynPanelConst.ClearBoth;
                    break;
                case Clear.None:
                    result = DynPanelConst.ClearNone;
                    break;
                default:
                    result = DynPanelConst.ClearNone;
                    break;
            }

            return result;
        }

        #endregion

        #region Override Style methods (IsSet, IsEmpty, Rest, FillStyleAttributes, CopyFrom, MergeWith

        /// <summary>
        /// Check whether the collection contains an item with the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <remarks>Each Custom style class should implement this method.</remarks>
        internal bool IsSet(string key)
        {
            return ViewState[key] != null;
        }

        /// <summary>
        /// Custom Style class must override the IsEpty property with !IsSet(key) for all custom style properties.
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return base.IsEmpty && !IsSet("");
            }
        }

        /// <summary>
        /// Because the setter of each property adds a new item to the ViewState collection,
        /// method named Reset must be overridden to remove the items that its properties have 
        /// added to the ViewState collection.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (IsEmpty) { return; }

            if (IsSet(DynPanelConst.PropFloat)) { ViewState.Remove(DynPanelConst.PropFloat); }
            if (IsSet(DynPanelConst.PropClear)) { ViewState.Remove(DynPanelConst.PropClear); }
        }

        /// <summary>
        /// Custom style class must override this method to add the CSS style attributes that it supports to
        /// the CssStyleCollection (attributes)
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="urlResolver"></param>
        protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
        {
            base.FillStyleAttributes(attributes, urlResolver);

            if (IsSet(DynPanelConst.PropClear)) { attributes.Add("clear", GetClear(this.Clear)); }
            if (IsSet(DynPanelConst.PropFloat)) { attributes.Add("float", GetFloat(this.Float)); }
        }

        /// <summary>
        /// Custom style class must override the CopyFrom method of its base class to copy the property values 
        /// of the style object passed in as argument to its associated style properties
        /// </summary>
        /// <param name="s"></param>
        public override void CopyFrom(System.Web.UI.WebControls.Style s)
        {
            if (s == null) { return; }

            base.CopyFrom(s);

            DynPanelStyle dps = s as DynPanelStyle;
            if (dps == null || dps.IsEmpty) { return; }

            if (dps.IsSet(DynPanelConst.PropFloat)) this.Float = dps.Float;
            if (dps.IsSet(DynPanelConst.PropClear)) this.Clear = dps.Clear;
        }

        public override void MergeWith(System.Web.UI.WebControls.Style s)
        {
            if (s == null) { return; }

            if (IsEmpty)
            {
                CopyFrom(s);
                return;
            }

            DynPanelStyle dps = s as DynPanelStyle;
            if (dps == null || dps.IsEmpty) { return; }

            if (dps.IsSet(DynPanelConst.PropClear) && !IsSet(DynPanelConst.PropClear)) this.Clear = dps.Clear;
            if (dps.IsSet(DynPanelConst.PropFloat) && !IsSet(DynPanelConst.PropFloat)) this.Float = dps.Float;
        }

        #endregion
    }
}
