
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

            internal const byte EF_A = 0xBA;   //;
            internal const byte EF_B = 0xBB;  //=
            internal const byte EF_C = 0xBC;   //,
            internal const byte EF_D = 0xBD;  //-
            internal const byte EF_E = 0xBE;  //.
            internal const byte EF_F = 0xBF;   // /

            internal const byte EF_G = 0xC0;   //`

            internal const byte EF_H = 0xDB;   //[
            internal const byte EF_I = 0xDC;   // \
            internal const byte EF_J = 0xDD;   //]
            internal const byte EF_K = 0xDE;   //'


            internal static readonly Key[] Keys =
            {

            };
        }

        internal static Type[] MultiFunKey =
        {
            new Type(0x110,     "!"),
            new Type(0x111,     "@"),
            new Type(0x112,     "#"),
            new Type(0x113,     "$"),
            new Type(0x114,     "%"),
            new Type(0x115,     "^"),
            new Type(0x116,     "&"),
            new Type(0x117,     "*"),
            new Type(0x118,     "("),
            new Type(0x119,     ")"),
            new Type(0x11a,     ":"),
            new Type(0x11b,     "+"),
            new Type(0x11c,     "<"),
            new Type(0x11d,     "_"),
            new Type(0x11e,     ">"),
            new Type(0x11f,     "?"),
            new Type(0x120,     "~"),
            new Type(0x121,     "{"),
            new Type(0x122,     "|"),
            new Type(0x123,     "}"),
            new Type(0x124,     "\"")
        };

        internal static Type[] ComboKeyType =
        {
            new Type(0x170,     "LCTRL_LSHIFT"),
            new Type(0x171,     "RCTRL_RSHIFT"),
            new Type(0x172,     "LCTRL_RSHIFT"),
            new Type(0x173,     "RCTRL_LSHIFT"),
            new Type(0x174,     "LCTRL_ENTER"),
            new Type(0x175,     "RCTRL_ENTER"),
            new Type(0x176,     "LSHIFT_ENTER"),
            new Type(0x177,     "RSHIFT_ENTER"),
            new Type(0x178,     "LALT_ENTER"),
            new Type(0x179,     "RALT_ENTER"),
            new Type(0x17A,     "LALT_TAB"),
            new Type(0x17B,     "RALT_TAB"),
            new Type(0x17C,     "LCTRL_LSHIFT_ESC"),
            new Type(0x17D,     "RCTRL_RSHIFT_ESC"),
            new Type(0x17E,     "LSHIFT_HOME"),
            new Type(0x17F,     "RSHIFT_HOME"),
            new Type(0x180,     "LSHIFT_END"),
            new Type(0x181,     "RSHIFT_END"),
            new Type(0x182,     "LSHIFT_LEFT"),
            new Type(0x183,     "RSHIFT_LEFT"),
            new Type(0x184,     "LSHIFT_UP"),
            new Type(0x185,     "RSHIFT_UP"),
            new Type(0x186,     "LSHIFT_RIGHT"),
            new Type(0x187,     "RSHIFT_RIGHT"),
            new Type(0x188,     "LSHIFT_DOWN"),
            new Type(0x189,     "RSHIFT_DOWN"),
            new Type(0x18A,     "LCTRL_A"),
            new Type(0x18B,     "RCTRL_A"),
            new Type(0x18C,     "LCTRL_S"),
            new Type(0x18D,     "RCTRL_S"),
            new Type(0x18E,     "LCTRL_F"),
            new Type(0x18F,     "RCTRL_F"),
            new Type(0x190,     "LCTRL_C"),
            new Type(0x191,     "RCTRL_C"),
            new Type(0x192,     "LCTRL_V"),
            new Type(0x193,     "RCTRL_V"),
            new Type(0x1F0,     "OTHER_DOUBLE_COMBO"),
            new Type(0x1F1,     "OTHER_TRIPLE_COMBO"),
            new Type(0x1F2,     "OTHER_QUADRA_COMBO")
        };

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
