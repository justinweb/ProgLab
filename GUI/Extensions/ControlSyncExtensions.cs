using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace ProgLab.Utility.GUI.Extensions
{
    /// <summary>
    /// 多執行緒模式中更新介面元件的擴充函式
    /// </summary>
    public static class ControlSyncExtensions
    {
        /// <summary>
        /// 更新Control的文字
        /// </summary>
        /// <param name="ctrl">要更新的Control</param>
        /// <param name="txt">要顯示於Text屬性中的字串</param>
        public static void UpdateControlText(this Control ctrl, string txt)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(new Action<Control, string>(UpdateControlText), new object[] { ctrl, txt });
            }
            else
                ctrl.Text = txt;
        }

        /// <summary>
        /// 更新ToolStripItem的文字
        /// </summary>
        /// <param name="ctrl">要更新的ToolStripItem</param>
        /// <param name="txt">要顯示於ToolStripItem的Text屬性中的字串</param>
        public static void UpdateControlText(this ToolStripItem ctrl, string txt)
        {
            if (ctrl.Owner.InvokeRequired)
            {
                ctrl.Owner.Invoke(new Action<ToolStripItem, string>(UpdateControlText), new object[] { ctrl, txt });
            }
            else
                ctrl.Text = txt;
        }

        /// <summary>
        /// 更新Control的Enable狀態
        /// </summary>
        /// <param name="ctrl">要設定的Control</param>
        /// <param name="isEnable">要設定的Enable值</param>
        public static void UpdateControlStatus(this Control ctrl, bool isEnable)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(new Action<Control, bool>(UpdateControlStatus), new object[] { ctrl, isEnable });
            }
            else
                ctrl.Enabled = isEnable;
        }

        /// <summary>
        /// 更新Contorl的前、背景色
        /// </summary>
        /// <param name="ctrl">要設定的Control</param>
        /// <param name="foreColor">要設定的前景色</param>
        /// <param name="backColor">要設定的背景色</param>
        public static void UpdateControlColor(this Control ctrl, Color? foreColor, Color? backColor)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(new Action<Control, Color?, Color?>(UpdateControlColor),
                    new object[] { ctrl, foreColor, backColor });
            }
            else
            {
                if (foreColor != null) ctrl.ForeColor = foreColor.Value;
                if (backColor != null) ctrl.BackColor = backColor.Value;
            }
        }
        /// <summary>
        /// 更新ToolStripItem的前、背景色
        /// </summary>
        /// <param name="ctrl">要設定的ToolStripItem</param>
        /// <param name="foreColor">要設定的前景色</param>
        /// <param name="backColor">要設定的背景色</param>
        public static void UpdateControlColor(this ToolStripItem ctrl, Color? foreColor, Color? backColor)
        {
            if (ctrl.Owner.InvokeRequired)
            {
                ctrl.Owner.Invoke(new Action<ToolStripItem, Color?, Color?>(UpdateControlColor),
                    new object[] { ctrl, foreColor, backColor });
            }
            else
            {
                if (foreColor != null) ctrl.ForeColor = foreColor.Value;
                if (backColor != null) ctrl.BackColor = backColor.Value;
            }
        }
        /// <summary>
        /// 加入ListViewItem到ListView中
        /// </summary>
        /// <param name="lv">要被加入的ListView</param>
        /// <param name="lvItem">要加入的ListViewItem</param>
        public static void SyncAddItem(this ListView lv, ListViewItem lvItem)
        {
            if (lv.InvokeRequired)
            {
                lv.Invoke(new Action<ListView, ListViewItem>(SyncAddItem), new object[] { lv, lvItem });
            }
            else
            {
                lv.Items.Add(lvItem);
            }
        }
        /// <summary>
        /// 清除ComboBox中的資料
        /// </summary>
        /// <param name="cmb">要被清除的ComboBox</param>
        public static void SyncClear(this ComboBox cmb)
        {
            if (cmb.InvokeRequired)
            {
                cmb.Invoke(new Action<ComboBox>(SyncClear), new object[] { cmb });
            }
            else
            {
                cmb.Items.Clear();
            }
        }

        /// <summary>
        /// 宣告一個可被Invoke()呼叫的Delegate型別
        /// </summary>
        public delegate void SyncInvokeFunc();
        /// <summary>
        /// 可以在多執行緒模式下對UI物件進行任何方式的更新
        /// </summary>
        /// <remarks>
        /// 可以自行提供任何要更新的程式碼
        /// </remarks>
        /// <param name="syncObj">支援可以Invoke/InvokeRequired的物件</param>
        /// <param name="syncFunc">要執行的函式</param>
        /// <example>
        /// <code language="C#">
        /// this.btnConnect.SafeInvoke(() => { this.btnConnect.Text = "Disconnect"; });
        /// // 上面就跟下面的結果一樣
        /// this.btnConnect.UpdateControlText( "Disconnect" ); 
        /// </code>
        /// </example>
        public static void SyncInvoke(this ISynchronizeInvoke syncObj, SyncInvokeFunc syncFunc)
        {
            if (syncObj.InvokeRequired)
            {
                syncObj.Invoke(new Action<ISynchronizeInvoke, SyncInvokeFunc>(SyncInvoke), new object[] { syncObj, syncFunc });
            }
            else
            {
                syncFunc();
            }
        }
    }
}
