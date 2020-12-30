using KMS.src.tool;
using System;
using System.Runtime.InteropServices;

namespace KMS.src.core
{
    internal static class KMEventHook
    {
        private const string TAG = "GlobalEventListener";

        private const int WH_KEYBOARD_LL = 13; //low-level keyboard event symbol.
        private const int WH_MOUSE_LL = 14; //mouse event as above.

        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x105;

        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MOUSEWHEEL = 0x20a;
        private const int WM_MOUSESIDEDOWN = 0x20b; //鼠标侧键按下事件（猜测）。
        private const int WM_MOUSESIDEUP = 0x20c; //鼠标侧键抬起事件（猜测）。
        private const int WM_MOUSEHWHEEL = 0x20e; //水平方向的滚动，一般鼠标没有这个事件。

        private const int WM_MOUSEDATA_HO_FORWARD = 0x1; //鼠标侧键前标志。high-order in DWORD
        private const int WM_MOUSEDATA_HO_BACKWARD = 0x10; //鼠标侧键后退标志。high-order in DWORD

        internal struct Keyboard_LL_Hook_Data
        {
            public UInt32 vkCode;
            public UInt32 scanCode;
            public UInt32 flags;
            public UInt32 time;
            public IntPtr extraInfo;
        }

        internal struct Mouse_LL_Hook_Data
        {
            internal long yx; //coordination of event with little endian.
            internal readonly int mouseData;
            internal readonly uint flags;
            internal readonly uint time;
            internal readonly IntPtr dwExtraInfo;
        }

        private static IntPtr pKeyboardHook = IntPtr.Zero; //键盘钩子句柄，通过句柄值来判断是否已注册钩子监听。
        private static IntPtr pMouseHook = IntPtr.Zero; //The hook reference of global mouse event.
        //钩子委托声明
        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);
        private static HookProc keyboardHookProc;
        private static HookProc mouseHookProc;

        //安装钩子
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr pInstance, int threadID);
        //卸载钩子
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(IntPtr pHookHandle);
        //使用WINDOWS API函数代替获取当前实例的函数,防止钩子失效
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam); //parameter 'hhk' is ignored.


        private static int KeyboardHookCallback(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
            {
                //TODO 把异常事件记录下来。写到数据库中。
                return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }
            else
            {
                Keyboard_LL_Hook_Data khd = (Keyboard_LL_Hook_Data)Marshal.PtrToStructure(lParam, typeof(Keyboard_LL_Hook_Data));
                if (wParam.ToInt32() == WM_KEYDOWN || wParam.ToInt32() == WM_SYSKEYDOWN)
                {
                    Logger.v(TAG, $"key down:{khd.vkCode}");
                }
                else if (wParam.ToInt32() == WM_KEYUP || wParam.ToInt32() == WM_SYSKEYUP)
                {
                    Logger.v(TAG, $"key up:{khd.vkCode}, keys:{khd.vkCode}, scanCode:{khd.scanCode}, flags:{khd.flags}, time:{khd.time}");
                }
                else
                {
                    //TODO 记录下来。
                }
            }

            return 0;
        }

        private static int MouseHookCallback(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
            {
                //TODO record it.
                return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }
            else
            {
                Mouse_LL_Hook_Data mhd = (Mouse_LL_Hook_Data)Marshal.PtrToStructure(lParam, typeof(Mouse_LL_Hook_Data));
                Console.WriteLine();
                int x = (int)(mhd.yx & 0xffffffff);
                Logger.v(TAG, $"({x},{mhd.yx >> 32}), key event:{wParam}");
                if (wParam.ToInt32() == WM_MOUSEWHEEL)
                {
                    short delta = (short)(mhd.mouseData >> 16);
                    Logger.v(TAG, $"wheel delta:{delta}");
                }
                else
                {
                    Logger.v(TAG, $"mouse data:{mhd.mouseData}");
                }
                Logger.v(TAG, $"time:{mhd.time}, flags:{mhd.flags}");
            }

            return 0;
        }

        internal static bool InsertHook()
        {
            bool iRet;
            iRet = InsertKeyboardHook();
            if (!iRet)
            {
                return false;
            }

            iRet = InsertMouseHook();
            if (!iRet)
            {
                removeKeyboardHook();
                return false;
            }

            return true;
        }

        //安装钩子方法
        private static bool InsertKeyboardHook()
        {
            Logger.v(TAG, "InsertKeyboardHook()");
            if (pKeyboardHook == IntPtr.Zero)//不存在钩子时
            {
                //创建钩子
                keyboardHookProc = KeyboardHookCallback;
                pKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL,
                    keyboardHookProc,
                    /*GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName)*/(IntPtr)0,
                    0);

                if (pKeyboardHook == IntPtr.Zero)//如果安装钩子失败
                {
                    Logger.v(TAG, "hook insert failed");
                    removeKeyboardHook();
                    return false;
                }
            }
            else
            {
                Logger.v(TAG, "Hook already working");
            }

            return true;
        }

        private static bool InsertMouseHook()
        {
            Logger.v(TAG, "InsertMouseHook()");
            if (pMouseHook == IntPtr.Zero)
            {
                mouseHookProc = MouseHookCallback;
                pMouseHook = SetWindowsHookEx(WH_MOUSE_LL,
                    mouseHookProc,
                    (IntPtr)0,
                    0);

                if (pMouseHook == IntPtr.Zero)
                {
                    Logger.v(TAG, "Mouse hook insert failed");
                    removeMouseHook();
                    return false;
                }
            }
            else
            {
                Logger.v(TAG, "The mouse hook already working");
            }

            return true;
        }

        internal static bool RemoveHook()
        {
            bool iRet;
            iRet = removeKeyboardHook();
            if (iRet)
            {
                iRet = removeMouseHook();
            }

            return iRet;
        }

        private static bool removeKeyboardHook()
        {
            Logger.v(TAG, "RemoveKeyboardHook()");
            if (pKeyboardHook != IntPtr.Zero)
            {
                if (UnhookWindowsHookEx(pKeyboardHook))
                {
                    pKeyboardHook = IntPtr.Zero;
                }
                else
                {
                    Logger.v(TAG, "keyboard hook remove failed");
                    return false;
                }
            }

            return true;
        }

        private static bool removeMouseHook()
        {
            Logger.v(TAG, "RemoveMouseHook()");
            if (pMouseHook != IntPtr.Zero)
            {
                if (UnhookWindowsHookEx(pMouseHook))
                {
                    pMouseHook = IntPtr.Zero;
                }
                else
                {
                    Logger.v(TAG, "mouse hook remove failed");
                    return false;
                }
            }

            return true;
        }
    }
}
