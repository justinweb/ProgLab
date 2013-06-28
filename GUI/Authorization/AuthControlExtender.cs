using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace ProgLab.GUI.Authorization
{
    /// <summary>
    /// 繼承自Component，才能於Design Time時拖拉一個元件出來使用
    /// </summary>
    /// <remarks>
    /// 用ProvideProperty來宣告在什麼型別上可以支援什麼屬性
    /// 至於屬性的型別就提供相對的Get/Set函式來處理
    /// ProvideProperty只能有一種型別，如果有多種型別的話，只能宣告共同的基底類別，再於CanExtend()中處理是否真正支援了
    /// ProvideProperty可以有多個屬性
    /// </remarks>
    [
        ProvideProperty("ResourceID", typeof(ToolStripMenuItem))    
    ]
    public class AuthControlExtender : Component, IExtenderProvider
    {
        private Dictionary<ToolStripMenuItem, string> dicResourceID = new Dictionary<ToolStripMenuItem, string>();

        #region IExtenderProvider 成員    
        /// <inheritdoc/>
        public bool CanExtend(object extendee)
        {
            if (extendee is ToolStripMenuItem || extendee is TextBox)
                return true;
            else
                return false;
        }

        #endregion

        /// <summary>
        /// ResourceID Setter
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="rid"></param>
        public void SetResourceID(ToolStripMenuItem ctrl, string rid)
        {
            dicResourceID[ctrl] = rid;
        }

        /// <summary>
        /// ResourceID Getter
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        [DefaultValue("")]
        public string GetResourceID(ToolStripMenuItem ctrl)
        {
            if (dicResourceID.ContainsKey(ctrl))
                return dicResourceID[ctrl];
            else
                return "";
        }
    }
}
