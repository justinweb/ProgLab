using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ProgLab.Util.ConsoleLib
{
    public class SimulateKey
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public const uint WM_KEYDOWN = 0x0100;

        /// <summary>
        /// 
        /// </summary>
        public const uint VK_SHIFT = 0x10;

        #region StructLayout

        [StructLayout(LayoutKind.Explicit)]
        internal struct INPUT
        {
            [FieldOffset(0)]
            internal int type;//0:mouse event;1:keyboard event;2:hardware event
            [FieldOffset(4)]
            internal MOUSEINPUT mi;
            [FieldOffset(4)]
            internal KEYBDINPUT ki;
            [FieldOffset(4)]
            internal HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            internal int uMsg;
            internal short wParamL;
            internal short wParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            internal ushort wVk;
            internal ushort wScan;
            internal uint dwFlags;
            internal uint time;
            internal IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal int mouseData;
            internal int dwFlags;
            internal int time;
            internal IntPtr dwExtraInfo;
        }

        #endregion

        #endregion

        #region Static Functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpKeyState"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern bool SetKeyboardState(byte[] lpKeyState);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint SendInput(uint nInput, ref INPUT pInput, int cbSize);

        /// <summary>
        /// 輸入字串
        /// </summary>
        /// <param name="text">字串</param>
        /// <param name="isPressEnterKeyword">是否自動輸入Enter(0x0D)Keyword</param>
        public static void PressWord(string text, bool isPressEnterKeyword)
        {

            /*2013/08/01 By Aaron 模擬在命令視窗鍵入字元行為*/
            foreach (char c in text.ToCharArray())
            {
                short sendData = VkKeyScan(c);
                bool needShift = (sendData & 0x0100) > 0;

                if (needShift)
                { SimulateKey.PressShfit(true); }

                SimulateKey.PostMessage(Process.GetCurrentProcess().MainWindowHandle, WM_KEYDOWN, sendData, 0);

                if (needShift)
                { PressShfit(false); }
            }
            if (isPressEnterKeyword)
            { PostMessage(Process.GetCurrentProcess().MainWindowHandle, WM_KEYDOWN, 0x0D, 0); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isOn"></param>
        public static void PressShfit(bool isOn)
        {
            //按住SHIFT
            INPUT Input = new INPUT();
            if (isOn)
            {
                Input.type = 1; //keyboard_input
                //Input.ki.wVk = 0x14; //CAPS_Lock
                Input.ki.wVk = 0x10; //VK_SHIFT
                Input.ki.dwFlags = 0;
                SendInput(1, ref Input, Marshal.SizeOf(Input));
            }
            else
            {
                //放開Caps Lock                
                Input.type = 1;
                Input.ki.wVk = 0x10;
                Input.ki.dwFlags = 2;//key_up
                SendInput(1, ref Input, Marshal.SizeOf(Input));
            }
        }

        #endregion
    }
}
