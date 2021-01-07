
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace KMS.src.core
{
    static class Constants
    {

        internal static class KeyEvent
        {
            internal const short WM_KEYDOWN = 0x100;
            internal const short WM_KEYUP = 0x101;
            internal const short WM_SYSKEYDOWN = 0x0104;
            internal const short WM_SYSKEYUP = 0x105;
        }

        internal static class Keyboard
        {
            internal const byte BACKSPACE = 0x8;
            internal const byte TAB = 0x9;
            internal const byte ENTER = 0xd;

            internal const byte PAUSE_BREAK = 0x13;
            internal const byte CAPS_LOCK = 0x14;
            internal const byte ESC = 0x1B;

            internal const byte SPACE_BAR = 0x20;
            internal const byte PAGE_UP = 0x21;
            internal const byte PAGE_DOWN = 0x22;
            internal const byte END = 0x23;
            internal const byte HOME = 0x24;
            internal const byte LEFT = 0x25;
            internal const byte UP = 0x26;
            internal const byte RIGHT = 0x27;
            internal const byte DOWN = 0x28;
            internal const byte PrtSc = 0x2c;
            internal const byte Insert = 0x2d;
            internal const byte DELETE = 0x2e;

            internal const byte Num0 = 0x30;
            internal const byte Num1 = 0x31;
            internal const byte Num2 = 0x32;
            internal const byte Num3 = 0x33;
            internal const byte Num4 = 0x34;
            internal const byte Num5 = 0x35;
            internal const byte Num6 = 0x36;
            internal const byte Num7 = 0x37;
            internal const byte Num8 = 0x38;
            internal const byte Num9 = 0x39;

            internal const byte A = 0x41;
            internal const byte B = 0x42;
            internal const byte C = 0x43;
            internal const byte D = 0x44;
            internal const byte E = 0x45;
            internal const byte F = 0x46;
            internal const byte G = 0x47;
            internal const byte H = 0x48;
            internal const byte I = 0x49;
            internal const byte J = 0x4A;
            internal const byte K = 0x4B;
            internal const byte L = 0x4C;
            internal const byte M = 0x4D;
            internal const byte N = 0x4E;
            internal const byte O = 0x4F;

            internal const byte P = 0x50;
            internal const byte Q = 0x51;
            internal const byte R = 0x52;
            internal const byte S = 0x53;
            internal const byte T = 0x54;
            internal const byte U = 0x55;
            internal const byte V = 0x56;
            internal const byte W = 0x57;
            internal const byte X = 0x58;
            internal const byte Y = 0x59;
            internal const byte Z = 0x5A;
            internal const byte LEFT_WIN = 0x5B;
            internal const byte RIGHT_WIN = 0x5C;

            internal const byte NumPad0 = 0x60;
            internal const byte NumPad1 = 0x61;
            internal const byte NumPad2 = 0x62;
            internal const byte NumPad3 = 0x63;
            internal const byte NumPad4 = 0x64;
            internal const byte NumPad5 = 0x65;
            internal const byte NumPad6 = 0x66;
            internal const byte NumPad7 = 0x67;
            internal const byte NumPad8 = 0x68;
            internal const byte NumPad9 = 0x69;
            internal const byte NumPad_Multiply = 0x6a;
            internal const byte NumPad_Add = 0x6b;
            internal const byte NumPad_Enter = 0x6c;
            internal const byte NumPad_Minus = 0x6d;
            internal const byte NumPad_Dot = 0x6e;
            internal const byte NumPad_Division = 0x6f;

            internal const byte F1 = 0x70;
            internal const byte F2 = 0x71;
            internal const byte F3 = 0x72;
            internal const byte F4 = 0x73;
            internal const byte F5 = 0x74;
            internal const byte F6 = 0x75;
            internal const byte F7 = 0x76;
            internal const byte F8 = 0x77;
            internal const byte F9 = 0x78;
            internal const byte F10 = 0x79;
            internal const byte F11 = 0x7A;
            internal const byte F12 = 0x7B;

            internal const byte NumLock = 0x90;
            internal const byte ScrLock = 0x91;

            internal const byte LEFT_SHIFT = 0xA0;
            internal const byte RIGHT_SHIFT = 0xA1;
            internal const byte LEFT_CTRL = 0xA2;
            internal const byte RIGHT_CTRL = 0xA3;
            internal const byte LEFT_ALT = 0xA4;
            internal const byte RIGHT_ALT = 0xA5;

            internal const byte PUNCTUATION_L1 = 0xba;   //;
            internal const byte PUNCTUATION_02 = 0xbb;  //=
            internal const byte PUNCTUATION_M1 = 0xbc;   //,
            internal const byte PUNCTUATION_01 = 0xbd;  //-
            internal const byte PUNCTUATION_M2 = 0xbe;  //.
            internal const byte PUNCTUATION_M3 = 0xbf;   // /

            internal const byte PUNCTUATION_11 = 0xc0;   //`

            internal const byte PUNCTUATION_P1 = 0xdb;   //[
            internal const byte PUNCTUATION_P3 = 0xdc;   // \
            internal const byte PUNCTUATION_P2 = 0xdd;   //]
            internal const byte PUNCTUATION_L2 = 0xde;   //'

            internal static readonly Dictionary<byte, Key> Keys = new Dictionary<byte, Key>
            {
                {BACKSPACE,            new Key(BACKSPACE,              "Backspace",                "Backspace")},
                {TAB,                  new Key(TAB,                    "Tab",                      "Tab")},
                {ENTER,                new Key(ENTER,                  "Enter",                    "回车键")},
                {PAUSE_BREAK,          new Key(PAUSE_BREAK,            "Pause/Break",              "Pause/Break")},
                {CAPS_LOCK,            new Key(CAPS_LOCK,              "Caps Lock",                "Caps Lock")},
                {ESC,                  new Key(ESC,                    "Esc",                      "Esc")},
                {SPACE_BAR,            new Key(SPACE_BAR,              "Space bar",                "空格键")},
                {PAGE_UP,              new Key(PAGE_UP,                "Page up",                  "Page up")},
                {PAGE_DOWN,            new Key(PAGE_DOWN,              "Page down",                "Page down")},
                {END,                  new Key(END,                    "End",                      "End")},
                {HOME,                 new Key(HOME,                   "Home",                     "Home")},
                {LEFT,                 new Key(LEFT,                   "Left",                     "左方向键")},
                {UP,                   new Key(UP,                     "Up",                       "上方向键")},
                {RIGHT,                new Key(RIGHT,                  "Right",                    "右方向键")},
                {DOWN,                 new Key(DOWN,                   "Down",                     "下方向键")},
                {PrtSc,                new Key(PrtSc,                  "PrtSc",                    "PrtSc")},
                {Insert,               new Key(Insert,                 "Insert",                   "Ins")},
                {DELETE,               new Key(DELETE,                 "Delete",                   "Del")},
                {Num0,                 new Key(Num0,                   "0",                        "0")},
                {Num1,                 new Key(Num1,                   "1",                        "1")},
                {Num2,                 new Key(Num2,                   "2",                        "2")},
                {Num3,                 new Key(Num3,                   "3",                        "3")},
                {Num4,                 new Key(Num4,                   "4",                        "4")},
                {Num5,                 new Key(Num5,                   "5",                        "5")},
                {Num6,                 new Key(Num6,                   "6",                        "6")},
                {Num7,                 new Key(Num7,                   "7",                        "7")},
                {Num8,                 new Key(Num8,                   "8",                        "8")},
                {Num9,                 new Key(Num9,                   "9",                        "9")},
                {A,                    new Key(A,                      "A",                        "A")},
                {B,                    new Key(B,                      "B",                        "B")},
                {C,                    new Key(C,                      "C",                        "C")},
                {D,                    new Key(D,                      "D",                        "D")},
                {E,                    new Key(E,                      "E",                        "E")},
                {F,                    new Key(F,                      "F",                        "F")},
                {G,                    new Key(G,                      "G",                        "G")},
                {H,                    new Key(H,                      "H",                        "H")},
                {I,                    new Key(I,                      "I",                        "I")},
                {J,                    new Key(J,                      "J",                        "J")},
                {K,                    new Key(K,                      "K",                        "K")},
                {L,                    new Key(L,                      "L",                        "L")},
                {M,                    new Key(M,                      "M",                        "M")},
                {N,                    new Key(N,                      "N",                        "N")},
                {O,                    new Key(O,                      "O",                        "O")},
                {P,                    new Key(P,                      "P",                        "P")},
                {Q,                    new Key(Q,                      "Q",                        "Q")},
                {R,                    new Key(R,                      "R",                        "R")},
                {S,                    new Key(S,                      "S",                        "S")},
                {T,                    new Key(T,                      "T",                        "T")},
                {U,                    new Key(U,                      "U",                        "U")},
                {V,                    new Key(V,                      "V",                        "V")},
                {W,                    new Key(W,                      "W",                        "W")},
                {X,                    new Key(X,                      "X",                        "X")},
                {Y,                    new Key(Y,                      "Y",                        "Y")},
                {Z,                    new Key(Z,                      "Z",                        "Z")},
                {LEFT_WIN,             new Key(LEFT_WIN,               "Left win",                 "左Win")},
                {RIGHT_WIN,            new Key(RIGHT_WIN,              "Right win",                "右Win")},
                {NumPad0,              new Key(NumPad0,                "Num0",                     "小键盘0")},
                {NumPad1,              new Key(NumPad1,                "Num1",                     "小键盘1")},
                {NumPad2,              new Key(NumPad2,                "Num2",                     "小键盘2")},
                {NumPad3,              new Key(NumPad3,                "Num3",                     "小键盘3")},
                {NumPad4,              new Key(NumPad4,                "Num4",                     "小键盘4")},
                {NumPad5,              new Key(NumPad5,                "Num5",                     "小键盘5")},
                {NumPad6,              new Key(NumPad6,                "Num6",                     "小键盘6")},
                {NumPad7,              new Key(NumPad7,                "Num7",                     "小键盘7")},
                {NumPad8,              new Key(NumPad8,                "Num8",                     "小键盘8")},
                {NumPad9,              new Key(NumPad9,                "Num9",                     "小键盘9")},
                {NumPad_Multiply,      new Key(NumPad_Multiply,        "Num_*",                    "小键盘*")},
                {NumPad_Add,           new Key(NumPad_Add,             "Num_+",                    "小键盘+")},
                {NumPad_Enter,         new Key(NumPad_Enter,           "Num_Enter",                "小键盘Enter")},
                {NumPad_Minus,         new Key(NumPad_Minus,           "Num_-",                    "小键盘-")},
                {NumPad_Dot,           new Key(NumPad_Dot,             "Num_.",                    "小键盘.")},
                {NumPad_Division,      new Key(NumPad_Division,        "Num_/",                    "小键盘/")},
                {F1,                   new Key(F1,                     "F1",                       "F1")},
                {F2,                   new Key(F2,                     "F2",                       "F2")},
                {F3,                   new Key(F3,                     "F3",                       "F3")},
                {F4,                   new Key(F4,                     "F4",                       "F4")},
                {F5,                   new Key(F5,                     "F5",                       "F5")},
                {F6,                   new Key(F6,                     "F6",                       "F6")},
                {F7,                   new Key(F7,                     "F7",                       "F7")},
                {F8,                   new Key(F8,                     "F8",                       "F8")},
                {F9,                   new Key(F9,                     "F9",                       "F9")},
                {F10,                  new Key(F10,                    "F10",                      "F10")},
                {F11,                  new Key(F11,                    "F11",                      "F11")},
                {F12,                  new Key(F12,                    "F12",                      "F12")},
                {NumLock,              new Key(NumLock,                "Num Lock",                 "Num Lock")},
                {ScrLock,              new Key(ScrLock,                "Scroll Lock",              "Scroll Lock")},
                {LEFT_SHIFT,           new Key(LEFT_SHIFT,             "Left Shift",               "左Shift")},
                {RIGHT_SHIFT,          new Key(RIGHT_SHIFT,            "Right Shift",              "右Shift")},
                {LEFT_CTRL,            new Key(LEFT_CTRL,              "Left Ctrl",                "左Ctrl")},
                {RIGHT_CTRL,           new Key(RIGHT_CTRL,             "Right Ctrl",               "右Ctrl")},
                {LEFT_ALT,             new Key(LEFT_ALT,               "Left Alt",                 "左Alt")},
                {RIGHT_ALT,            new Key(RIGHT_ALT,              "Right Alt",                "右Alt")},
                {PUNCTUATION_L1,       new Key(PUNCTUATION_L1,         ";",                        ";")},
                {PUNCTUATION_02,       new Key(PUNCTUATION_02,         "=",                        "=")},
                {PUNCTUATION_M1,       new Key(PUNCTUATION_M1,         ",",                        ",")},
                {PUNCTUATION_01,       new Key(PUNCTUATION_01,         "-",                        "-")},
                {PUNCTUATION_M2,       new Key(PUNCTUATION_M2,         ".",                        ".")},
                {PUNCTUATION_M3,       new Key(PUNCTUATION_M3,         "/",                        "/")},
                {PUNCTUATION_11,       new Key(PUNCTUATION_11,         "`",                        "`")},
                {PUNCTUATION_P1,       new Key(PUNCTUATION_P1,         "[",                        "[")},
                {PUNCTUATION_P3,       new Key(PUNCTUATION_P3,         "\\",                       "\\")},
                {PUNCTUATION_P2,       new Key(PUNCTUATION_P2,         "]",                        "]")},
                {PUNCTUATION_L2,       new Key(PUNCTUATION_L2,         "'",                        "'")}
            };
        }

        internal static class MultiFunctionKey
        {
            internal const int MF_1 = 0x110;
            internal const int MF_2 = 0x111;
            internal const int MF_3 = 0x112;
            internal const int MF_4 = 0x113;
            internal const int MF_5 = 0x114;
            internal const int MF_6 = 0x115;
            internal const int MF_7 = 0x116;
            internal const int MF_8 = 0x117;
            internal const int MF_9 = 0x118;
            internal const int MF_0 = 0x119;

            internal const int PUN_11 = 0x120;
            internal const int PUN_01 = 0x11d;
            internal const int PUN_02 = 0x11b;
            internal const int PUN_P1 = 0x121;
            internal const int PUN_P2 = 0x123;
            internal const int PUN_P3 = 0x122;
            internal const int PUN_L1 = 0x11a;
            internal const int PUN_L2 = 0x124;
            internal const int PUN_M1 = 0x11c;
            internal const int PUN_M2 = 0x11e;
            internal const int PUN_M3 = 0x11f;

            internal static readonly Dictionary<int, Type> Keys = new Dictionary<int, Type>
            {
                {MF_1,      new Type(0x110, "!")},
                {MF_2,      new Type(0x111, "@")},
                {MF_3,      new Type(0x112, "#")},
                {MF_4,      new Type(0x113, "$")},
                {MF_5,      new Type(0x114, "%")},
                {MF_6,      new Type(0x115, "^")},
                {MF_7,      new Type(0x116, "&")},
                {MF_8,      new Type(0x117, "*")},
                {MF_9,      new Type(0x118, "(")},
                {MF_0,      new Type(0x119, ")")},

                {PUN_11,    new Type(0x120, "~")},
                {PUN_01,    new Type(0x11d, "_")},
                {PUN_02,    new Type(0x11b, "+")},
                {PUN_P1,    new Type(0x121, "{")},
                {PUN_P2,    new Type(0x123, "}")},
                {PUN_P3,    new Type(0x122, "|")},
                {PUN_L1,    new Type(0x11a, ":")},
                {PUN_L2,    new Type(0x124, "\"")},
                {PUN_M1,    new Type(0x11c, "<")},
                {PUN_M2,    new Type(0x11e, ">")},
                {PUN_M3,    new Type(0x11f, "?")}
            };
        }

        internal static class ComboKey
        {Dictionary instead~
            internal static Type LC_LS              = new Type(0x170,     "LCTRL_LSHIFT");
            internal static Type RC_RS              = new Type(0x171,     "RCTRL_RSHIFT");
            internal static Type LC_RS              = new Type(0x172,     "LCTRL_RSHIFT");
            internal static Type RC_LS              = new Type(0x173,     "RCTRL_LSHIFT");
            internal static Type LC_ENTER           = new Type(0x174,     "LCTRL_ENTER");
            internal static Type RC_ENTER           = new Type(0x175,     "RCTRL_ENTER");
            internal static Type LS_ENTER           = new Type(0x176,     "LSHIFT_ENTER");
            internal static Type RS_ENTER           = new Type(0x177,     "RSHIFT_ENTER");
            internal static Type LA_ENTER           = new Type(0x178,     "LALT_ENTER");
            internal static Type RA_ENTER           = new Type(0x179,     "RALT_ENTER");
            internal static Type LA_TAB             = new Type(0x17A,     "LALT_TAB");
            internal static Type RA_TAB             = new Type(0x17B,     "RALT_TAB");
            internal static Type LC_LS_ESC          = new Type(0x17C,     "LCTRL_LSHIFT_ESC");
            internal static Type RC_RS_ESC          = new Type(0x17D,     "RCTRL_RSHIFT_ESC");
            internal static Type LS_HOME            = new Type(0x17E,     "LSHIFT_HOME");
            internal static Type RS_HOME            = new Type(0x17F,     "RSHIFT_HOME");
            internal static Type LS_END             = new Type(0x180,     "LSHIFT_END");
            internal static Type RS_END             = new Type(0x181,     "RSHIFT_END");
            internal static Type LS_LEFT            = new Type(0x182,     "LSHIFT_LEFT");
            internal static Type RS_LEFT            = new Type(0x183,     "RSHIFT_LEFT");
            internal static Type LS_UP              = new Type(0x184,     "LSHIFT_UP");
            internal static Type RS_UP              = new Type(0x185,     "RSHIFT_UP");
            internal static Type LS_RIGHT           = new Type(0x186,     "LSHIFT_RIGHT");
            internal static Type RS_RIGHT           = new Type(0x187,     "RSHIFT_RIGHT");
            internal static Type LS_DOWN            = new Type(0x188,     "LSHIFT_DOWN");
            internal static Type RS_DOWN            = new Type(0x189,     "RSHIFT_DOWN");
            internal static Type LC_A               = new Type(0x18A,     "LCTRL_A");
            internal static Type RC_A               = new Type(0x18B,     "RCTRL_A");
            internal static Type LC_S               = new Type(0x18C,     "LCTRL_S");
            internal static Type RC_S               = new Type(0x18D,     "RCTRL_S");
            internal static Type LC_F               = new Type(0x18E,     "LCTRL_F");
            internal static Type RC_F               = new Type(0x18F,     "RCTRL_F");
            internal static Type LC_C               = new Type(0x190,     "LCTRL_C");
            internal static Type RC_C               = new Type(0x191,     "RCTRL_C");
            internal static Type LC_V               = new Type(0x192,     "LCTRL_V");
            internal static Type RC_V               = new Type(0x193,     "RCTRL_V");
            internal static Type DOUBLE             = new Type(0x1F0,     "OTHER_DOUBLE_COMBO");
            internal static Type TRIPLE             = new Type(0x1F1,     "OTHER_TRIPLE_COMBO");
            internal static Type QUADRA             = new Type(0x1F2,     "OTHER_QUADRA_COMBO");
        }

        internal static class MouseEvent
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

        internal static Type[] MouseKey =
        {
            new Type(0x201,"MOUSE_LEFT_BTN"),
            new Type(0x202,"MOUSE_RIGHT_BTN"),
            new Type(0x210,"MOUSE_SIDE_KEY_FORWARD"),
            new Type(0x211,"MOUSE_SIDE_KEY_BACKWARD"),
            new Type(0x212,"MOUSE_WHEEL_FORWARD"),
            new Type(0x213,"MOUSE_WHEEL_BACKWARD"),
            new Type(0x214,"MOUSE_LEFT_BTN_AREA"),
            new Type(0x215,"MOUSE_RIGHT_BTN_AREA"),
        };

        internal static class MouseDataHighOrder
        {
            internal const int SIDE_FORWARD = 0x1; //鼠标侧键前标志。high-order in DWORD
            internal const int SIDE_BACKWARD = 0x10; //鼠标侧键后退标志。high-order in DWORD
        }

        internal enum DbType
        {
            //0 ~ 255 was reserved by keyboard code
            //0x100 ~ 0x10f was reserved by keyboard event
            EF_1 = 0x110, //数字1键的额外功能，即感叹号。
            EF_2,
            EF_3,
            EF_4,
            EF_5,
            EF_6,
            EF_7,
            EF_8,
            EF_9,
            EF_0,
            EF_A, //:
            EF_B, //+
            EF_C, //<
            EF_D, //_
            EF_E, //>
            EF_F, //?
            EF_G, //~
            EF_H, //{
            EF_I, //|
            EF_J, //}
            EF_K, //"
            //combo key begin...
            LCTRL_LSHIFT = 0x170,
            RCTRL_RSHIFT,
            LCTRL_RSHIFT,
            RCTRL_LSHIFT,
            LCTRL_ENTER,
            RCTRL_ENTER,
            LSHIFT_ENTER,
            RSHIFT_ENTER,
            LALT_ENTER,
            RALT_ENTER,
            LALT_TAB,
            RALT_TAB,
            LCTRL_LSHIFT_ESC,
            RCTRL_RSHIFT_ESC,
            LSHIFT_HOME,
            RSHIFT_HOME,
            LSHIFT_END,
            RSHIFT_END,
            LSHIFT_LEFT,
            RSHIFT_LEFT,
            LSHIFT_UP,
            RSHIFT_UP,
            LSHIFT_RIGHT,
            RSHIFT_RIGHT,
            LSHIFT_DOWN,
            RSHIFT_DOWN,
            LCTRL_A,
            RCTRL_A,
            LCTRL_S,
            RCTRL_S,
            LCTRL_F,
            RCTRL_F,
            LCTRL_C,
            RCTRL_C,
            LCTRL_V,
            RCTRL_V,
            OTHERS_DOUBLE_COMBO = 0x1f0,
            OTHERS_TRIPLE_COMBO,
            OTHERS_QUADRA_COMBO,
            //0x200 ~ 0x20f was reserved by mouse event
            MOUSE_FORWARD_SK = 0x210,
            MOUSE_BACKWARD_SK,
            MOUSE_WHEEL_FORWARD,
            MOUSE_WHEEL_BACKWARD,
            MOUSE_LBTN_AREA,
            MOUSE_RBTN_AREA,
            KB_ALL = 0x300,
            KB_COMBO_ALL,
            MOUSE_ALL,
            HOUR_KB_FIRST = 0x310,
            HOUR_KB_SECOND,
            HOUR_KB_THIRD,
            HOUR_KB_COMBO_FIRST,
            HOUR_KB_COMBO_SECOND,
            HOUR_KB_COMBO_THIRD,
            HOUR_MS_LB_FIRST,
            HOUR_MS_LB_SECOND,
            HOUR_MS_LB_THIRD,
            HOUR_MS_RB_FIRST,
            HOUR_MS_RB_SECOND,
            HOUR_MS_RB_THIRD,
            MONTH_KB_FIRST = 0x330,
            MONTH_KB_SECOND,
            MONTH_KB_THIRD,
            MONTH_KB_COMBO_FIRST,
            MONTH_KB_COMBO_SECOND,
            MONTH_KB_COMBO_THIRD,
            MONTH_MS_LB_FIRST,
            MONTH_MS_LB_SECOND,
            MONTH_MS_LB_THIRD,
            MONTH_MS_RB_FIRST,
            MONTH_MS_RB_SECOND,
            MONTH_MS_RB_THIRD,
            YEAR_KB_FIRST = 0x350,
            YEAR_KB_SECOND,
            YEAR_KB_THIRD,
            YEAR_KB_COMBO_FIRST,
            YEAR_KB_COMBO_SECOND,
            YEAR_KB_COMBO_THIRD,
            YEAR_MS_LB_FIRST,
            YEAR_MS_LB_SECOND,
            YEAR_MS_LB_THIRD,
            YEAR_MS_RB_FIRST,
            YEAR_MS_RB_SECOND,
            YEAR_MS_RB_THIRD,
        }

        internal static Type KbAll = new Type((int)DbType.KB_ALL, "键盘按键");
    }
}
