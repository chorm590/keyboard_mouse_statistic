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
            internal const byte PRTSC = 0x2c;
            internal const byte INSERT = 0x2d;
            internal const byte DELETE = 0x2e;

            internal const byte NUM0 = 0x30;
            internal const byte NUM1 = 0x31;
            internal const byte NUM2 = 0x32;
            internal const byte NUM3 = 0x33;
            internal const byte NUM4 = 0x34;
            internal const byte NUM5 = 0x35;
            internal const byte NUM6 = 0x36;
            internal const byte NUM7 = 0x37;
            internal const byte NUM8 = 0x38;
            internal const byte NUM9 = 0x39;

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

            internal const byte NUMPAD0 = 0x60;
            internal const byte NUMPAD1 = 0x61;
            internal const byte NUMPAD2 = 0x62;
            internal const byte NUMPAD3 = 0x63;
            internal const byte NUMPAD4 = 0x64;
            internal const byte NUMPAD5 = 0x65;
            internal const byte NUMPAD6 = 0x66;
            internal const byte NUMPAD7 = 0x67;
            internal const byte NUMPAD8 = 0x68;
            internal const byte NUMPAD9 = 0x69;
            internal const byte NUMPAD_MULTIPLY = 0x6a;
            internal const byte NUMPAD_ADD = 0x6b;
            internal const byte NUMPAD_ENTER = 0x6c;
            internal const byte NUMPAD_MINUS = 0x6d;
            internal const byte NUMPAD_DOT = 0x6e;
            internal const byte NUMPAD_DIVISION = 0x6f;

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

            internal const byte NUMLOCK = 0x90;
            internal const byte SCRLOCK = 0x91;

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
                {PRTSC,                new Key(PRTSC,                  "PrtSc",                    "PrtSc")},
                {INSERT,               new Key(INSERT,                 "Insert",                   "Ins")},
                {DELETE,               new Key(DELETE,                 "Delete",                   "Del")},
                {NUM0,                 new Key(NUM0,                   "0",                        "0")},
                {NUM1,                 new Key(NUM1,                   "1",                        "1")},
                {NUM2,                 new Key(NUM2,                   "2",                        "2")},
                {NUM3,                 new Key(NUM3,                   "3",                        "3")},
                {NUM4,                 new Key(NUM4,                   "4",                        "4")},
                {NUM5,                 new Key(NUM5,                   "5",                        "5")},
                {NUM6,                 new Key(NUM6,                   "6",                        "6")},
                {NUM7,                 new Key(NUM7,                   "7",                        "7")},
                {NUM8,                 new Key(NUM8,                   "8",                        "8")},
                {NUM9,                 new Key(NUM9,                   "9",                        "9")},
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
                {NUMPAD0,              new Key(NUMPAD0,                "Num0",                     "小键盘0")},
                {NUMPAD1,              new Key(NUMPAD1,                "Num1",                     "小键盘1")},
                {NUMPAD2,              new Key(NUMPAD2,                "Num2",                     "小键盘2")},
                {NUMPAD3,              new Key(NUMPAD3,                "Num3",                     "小键盘3")},
                {NUMPAD4,              new Key(NUMPAD4,                "Num4",                     "小键盘4")},
                {NUMPAD5,              new Key(NUMPAD5,                "Num5",                     "小键盘5")},
                {NUMPAD6,              new Key(NUMPAD6,                "Num6",                     "小键盘6")},
                {NUMPAD7,              new Key(NUMPAD7,                "Num7",                     "小键盘7")},
                {NUMPAD8,              new Key(NUMPAD8,                "Num8",                     "小键盘8")},
                {NUMPAD9,              new Key(NUMPAD9,                "Num9",                     "小键盘9")},
                {NUMPAD_MULTIPLY,      new Key(NUMPAD_MULTIPLY,        "Num_*",                    "小键盘*")},
                {NUMPAD_ADD,           new Key(NUMPAD_ADD,             "Num_+",                    "小键盘+")},
                {NUMPAD_ENTER,         new Key(NUMPAD_ENTER,           "Num_Enter",                "小键盘Enter")},
                {NUMPAD_MINUS,         new Key(NUMPAD_MINUS,           "Num_-",                    "小键盘-")},
                {NUMPAD_DOT,           new Key(NUMPAD_DOT,             "Num_.",                    "小键盘.")},
                {NUMPAD_DIVISION,      new Key(NUMPAD_DIVISION,        "Num_/",                    "小键盘/")},
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
                {NUMLOCK,              new Key(NUMLOCK,                "Num Lock",                 "Num Lock")},
                {SCRLOCK,              new Key(SCRLOCK,                "Scroll Lock",              "Scroll Lock")},
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
        {
            internal const ushort LC_LS = 0x170;
            internal const ushort RC_RS = 0x171;
            internal const ushort LC_RS = 0x172;
            internal const ushort RC_LS = 0x173;
            internal const ushort LC_ENTER = 0x174;
            internal const ushort RC_ENTER = 0x175;
            internal const ushort LS_ENTER = 0x176;
            internal const ushort RS_ENTER = 0x177;
            internal const ushort LA_ENTER = 0x178;
            internal const ushort RA_ENTER = 0x179;
            internal const ushort LA_TAB = 0x17A;
            internal const ushort RA_TAB = 0x17B;
            internal const ushort LC_LS_ESC = 0x17C;
            internal const ushort RC_RS_ESC = 0x17D;
            internal const ushort LS_HOME = 0x17E;
            internal const ushort RS_HOME = 0x17F;
            internal const ushort LS_END = 0x180;
            internal const ushort RS_END = 0x181;
            internal const ushort LS_LEFT = 0x182;
            internal const ushort RS_LEFT = 0x183;
            internal const ushort LS_UP = 0x184;
            internal const ushort RS_UP = 0x185;
            internal const ushort LS_RIGHT = 0x186;
            internal const ushort RS_RIGHT = 0x187;
            internal const ushort LS_DOWN = 0x188;
            internal const ushort RS_DOWN = 0x189;
            internal const ushort LC_A = 0x18A;
            internal const ushort RC_A = 0x18B;
            internal const ushort LC_S = 0x18C;
            internal const ushort RC_S = 0x18D;
            internal const ushort LC_F = 0x18E;
            internal const ushort RC_F = 0x18F;
            internal const ushort LC_C = 0x190;
            internal const ushort RC_C = 0x191;
            internal const ushort LC_V = 0x192;
            internal const ushort RC_V = 0x193;
            internal const ushort DOUBLE = 0x1F0;
            internal const ushort TRIPLE = 0x1F1;
            internal const ushort QUADRA = 0x1F2;

            internal static readonly Dictionary<ushort, Type> Keys = new Dictionary<ushort, Type>
            {
                {LC_LS    ,     new Type(0x170,     "LCTRL_LSHIFT")},
                {RC_RS    ,     new Type(0x171,     "RCTRL_RSHIFT")},
                {LC_RS    ,     new Type(0x172,     "LCTRL_RSHIFT")},
                {RC_LS    ,     new Type(0x173,     "RCTRL_LSHIFT")},
                {LC_ENTER ,     new Type(0x174,     "LCTRL_ENTER")},
                {RC_ENTER ,     new Type(0x175,     "RCTRL_ENTER")},
                {LS_ENTER ,     new Type(0x176,     "LSHIFT_ENTER")},
                {RS_ENTER ,     new Type(0x177,     "RSHIFT_ENTER")},
                {LA_ENTER ,     new Type(0x178,     "LALT_ENTER")},
                {RA_ENTER ,     new Type(0x179,     "RALT_ENTER")},
                {LA_TAB   ,     new Type(0x17A,     "LALT_TAB")},
                {RA_TAB   ,     new Type(0x17B,     "RALT_TAB")},
                {LC_LS_ESC,     new Type(0x17C,     "LCTRL_LSHIFT_ESC")},
                {RC_RS_ESC,     new Type(0x17D,     "RCTRL_RSHIFT_ESC")},
                {LS_HOME  ,     new Type(0x17E,     "LSHIFT_HOME")},
                {RS_HOME  ,     new Type(0x17F,     "RSHIFT_HOME")},
                {LS_END   ,     new Type(0x180,     "LSHIFT_END")},
                {RS_END   ,     new Type(0x181,     "RSHIFT_END")},
                {LS_LEFT  ,     new Type(0x182,     "LSHIFT_LEFT")},
                {RS_LEFT  ,     new Type(0x183,     "RSHIFT_LEFT")},
                {LS_UP    ,     new Type(0x184,     "LSHIFT_UP")},
                {RS_UP    ,     new Type(0x185,     "RSHIFT_UP")},
                {LS_RIGHT ,     new Type(0x186,     "LSHIFT_RIGHT")},
                {RS_RIGHT ,     new Type(0x187,     "RSHIFT_RIGHT")},
                {LS_DOWN  ,     new Type(0x188,     "LSHIFT_DOWN")},
                {RS_DOWN  ,     new Type(0x189,     "RSHIFT_DOWN")},
                {LC_A     ,     new Type(0x18A,     "LCTRL_A")},
                {RC_A     ,     new Type(0x18B,     "RCTRL_A")},
                {LC_S     ,     new Type(0x18C,     "LCTRL_S")},
                {RC_S     ,     new Type(0x18D,     "RCTRL_S")},
                {LC_F     ,     new Type(0x18E,     "LCTRL_F")},
                {RC_F     ,     new Type(0x18F,     "RCTRL_F")},
                {LC_C     ,     new Type(0x190,     "LCTRL_C")},
                {RC_C     ,     new Type(0x191,     "RCTRL_C")},
                {LC_V     ,     new Type(0x192,     "LCTRL_V")},
                {RC_V     ,     new Type(0x193,     "RCTRL_V")},
                {DOUBLE   ,     new Type(0x1F0,     "OTHER_DOUBLE_COMBO")},
                {TRIPLE   ,     new Type(0x1F1,     "OTHER_TRIPLE_COMBO")},
                {QUADRA   ,     new Type(0x1F2,     "OTHER_QUADRA_COMBO")}
            };
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

        internal static class MouseKey
        {
            internal const short MOUSE_LEFT_BTN = 0x201;
            internal const short MOUSE_RIGHT_BTN = 0x202;
            internal const short MOUSE_SIDE_KEY_FORWARD = 0x210;
            internal const short MOUSE_SIDE_KEY_BACKWARD = 0x211;
            internal const short MOUSE_WHEEL_FORWARD = 0x212;
            internal const short MOUSE_WHEEL_BACKWARD = 0x213;
            internal const short MOUSE_LEFT_BTN_AREA = 0x214;
            internal const short MOUSE_RIGHT_BTN_AREA = 0x215;

            internal static readonly Dictionary<short, Type> Keys = new Dictionary<short, Type>
            {
                {MOUSE_LEFT_BTN,                new Type(0x201,"MOUSE_LEFT_BTN") },
                {MOUSE_RIGHT_BTN,               new Type(0x202,"MOUSE_RIGHT_BTN")},
                {MOUSE_SIDE_KEY_FORWARD,        new Type(0x210,"MOUSE_SIDE_KEY_FORWARD")},
                {MOUSE_SIDE_KEY_BACKWARD,       new Type(0x211,"MOUSE_SIDE_KEY_BACKWARD")},
                {MOUSE_WHEEL_FORWARD,           new Type(0x212,"MOUSE_WHEEL_FORWARD")},
                {MOUSE_WHEEL_BACKWARD,          new Type(0x213,"MOUSE_WHEEL_BACKWARD")},
                {MOUSE_LEFT_BTN_AREA,           new Type(0x214,"MOUSE_LEFT_BTN_AREA")},
                {MOUSE_RIGHT_BTN_AREA,          new Type(0x215,"MOUSE_RIGHT_BTN_AREA")}
            };
        }

        internal static class MouseDataHighOrder
        {
            internal const int SIDE_FORWARD = 0x1; //鼠标侧键前标志。high-order in DWORD
            internal const int SIDE_BACKWARD = 0x10; //鼠标侧键后退标志。high-order in DWORD
        }
        
        internal enum DbType
        {
            INVALID = 0,
            //0 ~ 255 was reserved by keyboard code
            //0x100 ~ 0x10f was reserved by keyboard event
            //0x110 ~ 0x1ff was reserved, you can found the definition in others class in this file.
            //0x200 ~ 0x20f was reserved by mouse event
            //0x210 ~ 0x2ff was reserved by other mouse type.
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
            //0x1000 ~ 0xffff is free
        }

        internal static class Statistic
        {
            internal static Type KbAll = new Type((int)DbType.KB_ALL, "键盘按键");
            internal static Type KbCombo = new Type((int)DbType.KB_COMBO_ALL, "组合键总计");
            internal static Type KbSkTop1 = new Type(0x1000, "Single key top1");
            internal static Type KbSkTop2 = new Type(0x1001, "Single key top2");
            internal static Type KbSkTop3 = new Type(0x1002, "Single key top3");
            internal static Type KbCkTop1 = new Type(0x1003, "Combo key top3");
            internal static Type KbCkTop2 = new Type(0x1004, "Combo key top3");
            internal static Type KbCkTop3 = new Type(0x1005, "Combo key top3");
        }
    }
}
