using System;
using System.Collections.Generic;
using System.Text;

namespace KMS.src.core
{
    static class Constants
    {

        internal static class KeyEventCode
        {
            internal const int WM_KEYDOWN = 0x100;
            internal const int WM_KEYUP = 0x101;
            internal const int WM_SYSKEYDOWN = 0x0104;
            internal const int WM_SYSKEYUP = 0x105;
        }

        internal static Keys[] keyset =
        {
            new Keys(0x01, "VK_LBUTTON",        "Left mouse button",                "鼠标左键"),
            new Keys(0x02, "VK_RBUTTON",        "Right mouse button",               "鼠标右键"),
            new Keys(0x03, "VK_CANCEL",         "Control-break processing",         "vk_cancel"),
            new Keys(0x04, "VK_MBUTTON",        "Middle mouse button",              "鼠标中键"),
            new Keys(0x05, "VK_XBUTTON1",       "X1 mouse button",                  "x1_key"),
            new Keys(0x06, "VK_XBUTTON2",       "X2 mouse button",                  "x2_key"),
            new Keys(0x08, "VK_BACK",           "BACKSPACE key",                    "回退键"),
            new Keys(0x09, "VK_TAB",            "TAB key",                          "TAB"),
            new Keys(0x0c, "VK_CLEAR",          "CLEAR key",                        "清除键"),
            new Keys(0x0d, "VK_RETURN",         "ENTER key",                        "回车键"),
            new Keys(0x10, "VK_SHIFT",          "SHIFT key",                        "shift键"),
            new Keys(0x11, "VK_CONTROL",        "CTRL key",                         "ctrl键"),
            new Keys(0x12, "VK_ALT",            "ALT key",                          "ALT键"),
            new Keys(0x13, "VK_PAUSE",          "PAUSE key",                        "pause"),
            new Keys(0x14, "VK_CAPITAL",        "CAPS LOCK key",                    "Caps"),
            new Keys(0x1b, "VK_ESCAPE",         "ESC key",                          "ESC键"),
            new Keys(0x20, "VK_SPACE",          "SPACEBAR",                         "空格键"),
            new Keys(0x21, "VK_PRIOR",          "PAGE UP key",                      "Page up"),
            new Keys(0x22, "VK_NEXT",           "PAGE DOWN key",                    "Page down"),
            new Keys(0x23, "VK_END",            "END key",                          "END键"),
            new Keys(0x24, "VK_HOME",           "HOME key",                         "HOME键"),
            new Keys(0x25, "VK_LEFT",           "LEFT ARROW key",                   "左方向键"),
            new Keys(0x26, "VK_UP",             "UP ARROW key",                     "上方向键"),
            new Keys(0x27, "VK_RIGHT",          "RIGHT ARROW key",                  "右方向键"),
            new Keys(0x28, "VK_DOWN",           "DOWN ARROW key",                   "下方向键"),
            new Keys(0x29, "VK_SELECT",         "SELECT key",                       "select键"),
            new Keys(0x2a, "VK_PRINT",          "PRINT key",                        "print键"),
            new Keys(0x2b, "VK_EXECUTE",        "EXECUTE key",                      "EXECUTE键"),
            new Keys(0x2c, "VK_SNAPSHOT",       "PRINT SCREEN key",                 "PrtSc键"),
            new Keys(0x2d, "VK_INSERT",         "INS key",                          "Insert键"),
            new Keys(0x2e, "VK_DELETE",         "DEL key",                          "Delete键"),
            new Keys(0x2f, "VK_HELP",           "HELP key",                         "帮助键"),
        };

        internal enum KeyCode
        {
            KEY_0 = 0x30,
            KEY_1 = 0x31,
            KEY_2 = 0x32,
            KEY_3 = 0x33,
            KEY_4 = 0x34,
            KEY_5 = 0x35,
            KEY_6 = 0x36,
            KEY_7 = 0x37,
            KEY_8 = 0x38,
            KEY_9 = 0x39,
            KEY_A = 0x41,
            KEY_B = 0x42,
            KEY_C = 0x43,
            KEY_D = 0x44,
            KEY_E = 0x45,
            KEY_F = 0x46,
            KEY_G = 0x47,
            KEY_H = 0x48,
            KEY_I = 0x49,
            KEY_J = 0x4A,
            KEY_L = 0x4C,
            KEY_K = 0x4B,
            KEY_M = 0x4D,
            KEY_N = 0x4E,
            KEY_O = 0x4F,
            KEY_P = 0x50,
            KEY_Q = 0x51,
            KEY_R = 0x52,
            KEY_S = 0x53,
            KEY_T = 0x54,
            KEY_U = 0x55,
            KEY_V = 0x56,
            KEY_W = 0x57,
            KEY_X = 0x58,
            KEY_Y = 0x59,
            KEY_Z = 0x5A,

            VK_LWIN = 0x5b, //left windows key(natural keyboard)
            VK_RWIN = 0x5c, //right windows key(natural keyboard)
            VK_APPS = 0x5d, //applications key(natural keyboard)
            VK_SLEEP = 0x5f, //computer sleep key

            KEY_NUMPAD0 = 0x60,
            KEY_NUMPAD1 = 0x61,
            KEY_NUMPAD2 = 0x62,
            KEY_NUMPAD3 = 0x63,
            KEY_NUMPAD4 = 0x64,
            KEY_NUMPAD5 = 0x65,
            KEY_NUMPAD6 = 0x66,
            KEY_NUMPAD7 = 0x67,
            KEY_NUMPAD8 = 0x68,
            KEY_NUMPAD9 = 0x69,
            VK_MULTIPLY = 0x6a, //
            VK_ADD = 0x6b, //
            KEY_SEPARATOR = 0x6C, //Separator key
            KEY_SUBTRACT = 0x6D, //Subtract key
            KEY_DECIMAL = 0x6E, //Decimal key
            KEY_DIVIDE = 0x6F, //Divide key
            KEY_F1 = 0x70, //F1 key
            KEY_F2 = 0x71, //F2 key
            KEY_F3 = 0x72, //F3 key
            KEY_F4 = 0x73, //F4 key
            KEY_F5 = 0x74, //F5 key
            KEY_F6 = 0x75, //F6 key
            KEY_F7 = 0x76, //F7 key
            KEY_F8 = 0x77, //F8 key
            KEY_F9 = 0x78, //F9 key
            KEY_F10 = 0x79, //F10 key
            KEY_F11 = 0x7A, //F11 key
            KEY_F12 = 0x7B, //F12 key

            VK_NUMLOCK = 0x90, //NUM LOCK key
            KEY_SCROLL = 0x91, //SCROLL LOCK key

            KEY_LSHIFT = 0xA0, //Left SHIFT key
            KEY_RSHIFT = 0xA1, //Right SHIFT key
            KEY_LCONTROL = 0xA2, //Left CONTROL key
            KEY_RCONTROL = 0xA3, //Right CONTROL key
            KEY_LMENU = 0xA4, //Left MENU key
            KEY_RMENU = 0xA5, //Right MENU key
            KEY_MINUS = 0xBD, // - key
            KEY_PLUS = 0xBB, // + key
            KEY_COMMA = 0xBC, //, key
            KEY_PERIOD = 0xBE, //. key
            KEY_PLAY = 0xFA, //Play key
            KEY_ZOOM = 0xFB, //Zoom key
        }

        internal static class MouseEventCode
        {
            internal const int WM_MOUSEMOVE = 0x200;
            internal const int WM_LBUTTONDOWN = 0x201;
            internal const int WM_LBUTTONUP = 0x202;
            internal const int WM_RBUTTONDOWN = 0x204;
            internal const int WM_RBUTTONUP = 0x205;
            internal const int WM_MOUSEWHEEL = 0x20a;
            internal const int WM_MOUSESIDEDOWN = 0x20b; //鼠标侧键按下事件（猜测）。
            internal const int WM_MOUSESIDEUP = 0x20c; //鼠标侧键抬起事件（猜测）。
            internal const int WM_MOUSEHWHEEL = 0x20e; //水平方向的滚动，一般鼠标没有这个事件。
        }

        internal static class MouseDataHighOrder
        {
            internal const int SIDE_FORWARD = 0x1; //鼠标侧键前标志。high-order in DWORD
            internal const int SIDE_BACKWARD = 0x10; //鼠标侧键后退标志。high-order in DWORD
        }
    }
}
