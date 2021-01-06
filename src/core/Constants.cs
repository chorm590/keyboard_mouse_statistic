
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

        internal static class KeyCode
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
            internal const byte DELETE = 0x2E;
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
        }

        internal static Type[] keys =
        {
            new Type(0x00,null),
            new Type(0x01,null),
            new Type(0x02,null),
            new Type(0x03,null),
            new Type(0x04,null),
            new Type(0x05,null),
            new Type(0x06,null),
            new Type(0x07,null),
            new Type(0x08,      "BACKSPACE"),
            new Type(0x09,      "TAB"),
            new Type(0x0a,null),
            new Type(0x0b,null),
            new Type(0x0c,      "CLEAR"),
            new Type(0x0d,      "ENTER"),
            new Type(0x0e,null),
            new Type(0x0f,null),

            new Type(0x10, null),
            new Type(0x11, null),
            new Type(0x12, null),
            new Type(0x13,      "PAUSE/BREAK"),
            new Type(0x14,      "CAPS LOCK"),
            new Type(0x15,null),
            new Type(0x16,null),
            new Type(0x17,null),
            new Type(0x18,null),
            new Type(0x19,null),
            new Type(0x1a,null),
            new Type(0x1b,      "ESC"),
            new Type(0x1c,null),
            new Type(0x1d,null),
            new Type(0x1e,null),
            new Type(0x1f,null),

            new Type(0x20,      "SPACE BAR"),
            new Type(0x21,      "PAGE UP"),
            new Type(0x22,      "PAGE DOWN"),
            new Type(0x23,      "END"),
            new Type(0x24,      "HOME"),
            new Type(0x25,      "LEFT ARROW"),
            new Type(0x26,      "UP ARROW"),
            new Type(0x27,      "RIGHT ARROW"),
            new Type(0x28,      "DOWN ARROW"),
            new Type(0x29,      "SELECT"),
            new Type(0x2a,      "PRINT"),
            new Type(0x2b,      "EXECUTE"),
            new Type(0x2c,      "PrtSc"),
            new Type(0x2d,      "Insert"),
            new Type(0x2e,      "Delete"),
            new Type(0x2f,      "HELP"),

            new Type(0x30,      "0"),
            new Type(0x31,      "1"),
            new Type(0x32,      "2"),
            new Type(0x33,      "3"),
            new Type(0x34,      "4"),
            new Type(0x35,      "5"),
            new Type(0x36,      "6"),
            new Type(0x37,      "7"),
            new Type(0x38,      "8"),
            new Type(0x39,      "9"),
            new Type(0x3a,null),
            new Type(0x3b,null),
            new Type(0x3c,null),
            new Type(0x3d,null),
            new Type(0x3e,null),
            new Type(0x3f,null),

            new Type(0x40,null),
            new Type(0x41,      "A"),
            new Type(0x42,      "B"),
            new Type(0x43,      "C"),
            new Type(0x44,      "D"),
            new Type(0x45,      "E"),
            new Type(0x46,      "F"),
            new Type(0x47,      "G"),
            new Type(0x48,      "H"),
            new Type(0x49,      "I"),
            new Type(0x4a,      "J"),
            new Type(0x4b,      "K"),
            new Type(0x4c,      "L"),
            new Type(0x4d,      "M"),
            new Type(0x4e,      "N"),
            new Type(0x4f,      "O"),

            new Type(0x50,      "P"),
            new Type(0x51,      "Q"),
            new Type(0x52,      "R"),
            new Type(0x53,      "S"),
            new Type(0x54,      "T"),
            new Type(0x55,      "U"),
            new Type(0x56,      "V"),
            new Type(0x57,      "W"),
            new Type(0x58,      "X"),
            new Type(0x59,      "Y"),
            new Type(0x5a,      "Z"),
            new Type(0x5b,      "left win key"),
            new Type(0x5c,      "right win key"),
            new Type(0x5d,      "app key"),
            new Type(0x5e,null),
            new Type(0x5f,      "sleep key"),

            new Type(0x60,      "NUM_0"),
            new Type(0x61,      "NUM_1"),
            new Type(0x62,      "NUM_2"),
            new Type(0x63,      "NUM_3"),
            new Type(0x64,      "NUM_4"),
            new Type(0x65,      "NUM_5"),
            new Type(0x66,      "NUM_6"),
            new Type(0x67,      "NUM_7"),
            new Type(0x68,      "NUM_8"),
            new Type(0x69,      "NUM_9"),
            new Type(0x6a,      "NUM_*"),
            new Type(0x6b,      "NUM_+"),
            new Type(0x6c,      "NUM_ENT"),
            new Type(0x6d,      "NUM_-"),
            new Type(0x6e,      "NUM_."),
            new Type(0x6f,      "NUM_/"),

            new Type(0x70,      "F1"),
            new Type(0x71,      "F2"),
            new Type(0x72,      "F3"),
            new Type(0x73,      "F4"),
            new Type(0x74,      "F5"),
            new Type(0x75,      "F6"),
            new Type(0x76,      "F7"),
            new Type(0x77,      "F8"),
            new Type(0x78,      "F9"),
            new Type(0x79,      "F10"),
            new Type(0x7a,      "F11"),
            new Type(0x7b,      "F12"),
            new Type(0x7c,null),
            new Type(0x7d,null),
            new Type(0x7e,null),
            new Type(0x7f,null),

            new Type(0x80,null),
            new Type(0x81,null),
            new Type(0x82,null),
            new Type(0x83,null),
            new Type(0x84,null),
            new Type(0x85,null),
            new Type(0x86,null),
            new Type(0x87,null),
            new Type(0x88,null),
            new Type(0x89,null),
            new Type(0x8a,null),
            new Type(0x8b,null),
            new Type(0x8c,null),
            new Type(0x8d,null),
            new Type(0x8e,null),
            new Type(0x8f,null),

            new Type(0x90,      "NUM LOCK"),
            new Type(0x91,      "SCROLL LOCK"),
            new Type(0x92,null),
            new Type(0x93,null),
            new Type(0x94,null),
            new Type(0x95,null),
            new Type(0x96,null),
            new Type(0x97,null),
            new Type(0x98,null),
            new Type(0x99,null),
            new Type(0x9a,null),
            new Type(0x9b,null),
            new Type(0x9c,null),
            new Type(0x9d,null),
            new Type(0x9e,null),
            new Type(0x9f,null),

            new Type(0xa0,      "LEFT SHIFT"),
            new Type(0xa1,      "RIGHT SHIFT"),
            new Type(0xa2,      "LEFT CTRL"),
            new Type(0xa3,      "RIGHT CTRL"),
            new Type(0xa4,      "LEFT ALT"),
            new Type(0xa5,      "RIGHT ALT"),
            new Type(0xa6,null),
            new Type(0xa7,null),
            new Type(0xa8,null),
            new Type(0xa9,null),
            new Type(0xaa,null),
            new Type(0xab,null),
            new Type(0xac,null),
            new Type(0xad,null),
            new Type(0xae,null),
            new Type(0xaf,null),

            new Type(0xb0,null),
            new Type(0xb1,null),
            new Type(0xb2,null),
            new Type(0xb3,null),
            new Type(0xb4,null),
            new Type(0xb5,null),
            new Type(0xb6,null),
            new Type(0xb7,null),
            new Type(0xb8,null),
            new Type(0xb9,null),
            new Type(0xba,      ";"),
            new Type(0xbb,      "="),
            new Type(0xbc,      ","),
            new Type(0xbd,      "-"),
            new Type(0xbe,      "."),
            new Type(0xbf,      "/"),

            new Type(0xc0,      "`"),
            new Type(0xc1,null),
            new Type(0xc2,null),
            new Type(0xc3,null),
            new Type(0xc4,null),
            new Type(0xc5,null),
            new Type(0xc6,null),
            new Type(0xc7,null),
            new Type(0xc8,null),
            new Type(0xc9,null),
            new Type(0xca,null),
            new Type(0xcb,null),
            new Type(0xcc,null),
            new Type(0xcd,null),
            new Type(0xce,null),
            new Type(0xcf,null),

            new Type(0xd0,null),
            new Type(0xd1,null),
            new Type(0xd2,null),
            new Type(0xd3,null),
            new Type(0xd4,null),
            new Type(0xd5,null),
            new Type(0xd6,null),
            new Type(0xd7,null),
            new Type(0xd8,null),
            new Type(0xd9,null),
            new Type(0xda,null),
            new Type(0xdb,      "["),
            new Type(0xdc,      @"\"),
            new Type(0xdd,      "]"),
            new Type(0xde,      "'"),
            new Type(0xdf,null),

            new Type(0xe0,null),
            new Type(0xe1,null),
            new Type(0xe2,null),
            new Type(0xe3,null),
            new Type(0xe4,null),
            new Type(0xe5,null),
            new Type(0xe6,null),
            new Type(0xe7,null),
            new Type(0xe8,null),
            new Type(0xe9,null),
            new Type(0xea,null),
            new Type(0xeb,null),
            new Type(0xec,null),
            new Type(0xed,null),
            new Type(0xee,null),
            new Type(0xef,null),

            new Type(0xf0,null),
            new Type(0xf1,null),
            new Type(0xf2,null),
            new Type(0xf3,null),
            new Type(0xf4,null),
            new Type(0xf5,null),
            new Type(0xf6,null),
            new Type(0xf7,null),
            new Type(0xf8,null),
            new Type(0xf9,null),
            new Type(0xfa,null),
            new Type(0xfb,null),
            new Type(0xfc,null),
            new Type(0xfd,null),
            new Type(0xfe,null),
            new Type(0xff,null)
        };

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

        internal static Keys[] keyset =
        {
            new Keys(0x00,"",""),
            new Keys(0x01,"",""),
            new Keys(0x02,"",""),
            new Keys(0x03,"",""),
            new Keys(0x04,"",""),
            new Keys(0x05,"",""),
            new Keys(0x06,"",""),
            new Keys(0x07,"",""),
            new Keys(0x08,     "BACKSPACE",                        "回退键"),
            new Keys(0x09,     "TAB",                              "TAB"),
            new Keys(0x0a,"",""),
            new Keys(0x0b,"",""),
            new Keys(0x0c,     "CLEAR",                             "清除键"),
            new Keys(0x0d,     "ENTER",                             "回车键"),
            new Keys(0x0e,"",""),
            new Keys(0x0f,"",""),

            new Keys(0x10, "", ""),
            new Keys(0x11, "", ""),
            new Keys(0x12, "", ""),
            new Keys(0x13,     "PAUSE/BREAK",                       "pause/break"),
            new Keys(0x14,     "CAPS LOCK",                         "Caps lock"),
            new Keys(0x15,"",""),
            new Keys(0x16,"",""),
            new Keys(0x17,"",""),
            new Keys(0x18,"",""),
            new Keys(0x19,"",""),
            new Keys(0x1a,"",""),
            new Keys(0x1b,     "ESC",                               "ESC键"),
            new Keys(0x1c,"",""),
            new Keys(0x1d,"",""),
            new Keys(0x1e,"",""),
            new Keys(0x1f,"",""),

            new Keys(0x20,     "SPACE BAR",                         "空格键"),
            new Keys(0x21,     "PAGE UP",                           "Page up"),
            new Keys(0x22,     "PAGE DOWN",                         "Page down"),
            new Keys(0x23,     "END",                               "END"),
            new Keys(0x24,     "HOME",                              "HOME"),
            new Keys(0x25,     "LEFT ARROW",                        "左方向"),
            new Keys(0x26,     "UP ARROW",                          "上方向"),
            new Keys(0x27,     "RIGHT ARROW",                       "右方向"),
            new Keys(0x28,     "DOWN ARROW",                        "下方向"),
            new Keys(0x29,     "SELECT",                            "select键"),
            new Keys(0x2a,     "PRINT",                             "print键"),
            new Keys(0x2b,     "EXECUTE",                           "EXECUTE键"),
            new Keys(0x2c,     "PrtSc",                             "PrtSc"),
            new Keys(0x2d,     "Insert",                            "Insert"),
            new Keys(0x2e,     "Delete",                            "Delete"),
            new Keys(0x2f,     "HELP",                              "帮助键"),

            new Keys(0x30,     "0",                                "数字0",           ")",                    ")"),
            new Keys(0x31,     "1",                                "数字1",           "!",                    "!"),
            new Keys(0x32,     "2",                                "数字2",           "@",                    "@"),
            new Keys(0x33,     "3",                                "数字3",           "#",                    "#"),
            new Keys(0x34,     "4",                                "数字4",           "$",                    "$"),
            new Keys(0x35,     "5",                                "数字5",           "%",                    "%"),
            new Keys(0x36,     "6",                                "数字6",           "^",                    "^"),
            new Keys(0x37,     "7",                                "数字7",           "&",                    "&"),
            new Keys(0x38,     "8",                                "数字8",           "*",                    "*"),
            new Keys(0x39,     "9",                                "数字9",           "(",                    "("),
            new Keys(0x3a,"",""),
            new Keys(0x3b,"",""),
            new Keys(0x3c,"",""),
            new Keys(0x3d,"",""),
            new Keys(0x3e,"",""),
            new Keys(0x3f,"",""),

            new Keys(0x40,"",""),
            new Keys(0x41,     "A",                                "A"),
            new Keys(0x42,     "B",                                "B"),
            new Keys(0x43,     "C",                                "C"),
            new Keys(0x44,     "D",                                "D"),
            new Keys(0x45,     "E",                                "E"),
            new Keys(0x46,     "F",                                "F"),
            new Keys(0x47,     "G",                                "G"),
            new Keys(0x48,     "H",                                "H"),
            new Keys(0x49,     "I",                                "I"),
            new Keys(0x4a,     "J",                                "J"),
            new Keys(0x4b,     "K",                                "K"),
            new Keys(0x4c,     "L",                                "L"),
            new Keys(0x4d,     "M",                                "M"),
            new Keys(0x4e,     "N",                                "N"),
            new Keys(0x4f,     "O",                                "O"),

            new Keys(0x50,     "P",                                "P"),
            new Keys(0x51,     "Q",                                "Q"),
            new Keys(0x52,     "R",                                "R"),
            new Keys(0x53,     "S",                                "S"),
            new Keys(0x54,     "T",                                "T"),
            new Keys(0x55,     "U",                                "U"),
            new Keys(0x56,     "V",                                "V"),
            new Keys(0x57,     "W",                                "W"),
            new Keys(0x58,     "X",                                "X"),
            new Keys(0x59,     "Y",                                "Y"),
            new Keys(0x5a,     "Z",                                "Z"),
            new Keys(0x5b,      "left win key",                     "左WIN"),
            new Keys(0x5c,      "right win key",                    "右WIN"),
            new Keys(0x5d,      "app key",                          "应用键"),
            new Keys(0x5e,"",""),
            new Keys(0x5f,      "sleep key",                        "睡眠键"),

            new Keys(0x60,      "NUM_0",                            "小键盘0"),
            new Keys(0x61,      "NUM_1",                            "小键盘1"),
            new Keys(0x62,      "NUM_2",                            "小键盘2"),
            new Keys(0x63,      "NUM_3",                            "小键盘3"),
            new Keys(0x64,      "NUM_4",                            "小键盘4"),
            new Keys(0x65,      "NUM_5",                            "小键盘5"),
            new Keys(0x66,      "NUM_6",                            "小键盘6"),
            new Keys(0x67,      "NUM_7",                            "小键盘7"),
            new Keys(0x68,      "NUM_8",                            "小键盘8"),
            new Keys(0x69,      "NUM_9",                            "小键盘9"),
            new Keys(0x6a,      "NUM_*",                            "小键盘*"),
            new Keys(0x6b,      "NUM_+",                            "小键盘+"),
            new Keys(0x6c,      "NUM_ENT",                          "小键盘ENT"),
            new Keys(0x6d,      "NUM_-",                            "小键盘-"),
            new Keys(0x6e,      "NUM_.",                            "小键盘."),
            new Keys(0x6f,      "NUM_/",                            "小键盘/"),

            new Keys(0x70,      "F1",                               "F1"),
            new Keys(0x71,      "F2",                               "F2"),
            new Keys(0x72,      "F3",                               "F3"),
            new Keys(0x73,      "F4",                               "F4"),
            new Keys(0x74,      "F5",                               "F5"),
            new Keys(0x75,      "F6",                               "F6"),
            new Keys(0x76,      "F7",                               "F7"),
            new Keys(0x77,      "F8",                               "F8"),
            new Keys(0x78,      "F9",                               "F9"),
            new Keys(0x79,      "F10",                              "F10"),
            new Keys(0x7a,      "F11",                              "F11"),
            new Keys(0x7b,      "F12",                              "F12"),
            new Keys(0x7c,"",""),
            new Keys(0x7d,"",""),
            new Keys(0x7e,"",""),
            new Keys(0x7f,"",""),

            new Keys(0x80,"",""),
            new Keys(0x81,"",""),
            new Keys(0x82,"",""),
            new Keys(0x83,"",""),
            new Keys(0x84,"",""),
            new Keys(0x85,"",""),
            new Keys(0x86,"",""),
            new Keys(0x87,"",""),
            new Keys(0x88,"",""),
            new Keys(0x89,"",""),
            new Keys(0x8a,"",""),
            new Keys(0x8b,"",""),
            new Keys(0x8c,"",""),
            new Keys(0x8d,"",""),
            new Keys(0x8e,"",""),
            new Keys(0x8f,"",""),

            new Keys(0x90,      "NUM LOCK",                         "NUM LOCK"),
            new Keys(0x91,      "SCROLL LOCK",                      "SCROLL LOCK"),
            new Keys(0x92,"",""),
            new Keys(0x93,"",""),
            new Keys(0x94,"",""),
            new Keys(0x95,"",""),
            new Keys(0x96,"",""),
            new Keys(0x97,"",""),
            new Keys(0x98,"",""),
            new Keys(0x99,"",""),
            new Keys(0x9a,"",""),
            new Keys(0x9b,"",""),
            new Keys(0x9c,"",""),
            new Keys(0x9d,"",""),
            new Keys(0x9e,"",""),
            new Keys(0x9f,"",""),

            new Keys(0xa0,      "LEFT SHIFT",                       "左SHIFT"),
            new Keys(0xa1,      "RIGHT SHIFT",                      "右SHIFT"),
            new Keys(0xa2,      "LEFT CTRL",                        "左CTRL"),
            new Keys(0xa3,      "RIGHT CTRL",                       "右CTRL"),
            new Keys(0xa4,      "LEFT ALT",                        "左ALT"),
            new Keys(0xa5,      "RIGHT ALT",                       "右ALT"),
            new Keys(0xa6,"",""),
            new Keys(0xa7,"",""),
            new Keys(0xa8,"",""),
            new Keys(0xa9,"",""),
            new Keys(0xaa,"",""),
            new Keys(0xab,"",""),
            new Keys(0xac,"",""),
            new Keys(0xad,"",""),
            new Keys(0xae,"",""),
            new Keys(0xaf,"",""),

            new Keys(0xb0,"",""),
            new Keys(0xb1,"",""),
            new Keys(0xb2,"",""),
            new Keys(0xb3,"",""),
            new Keys(0xb4,"",""),
            new Keys(0xb5,"",""),
            new Keys(0xb6,"",""),
            new Keys(0xb7,"",""),
            new Keys(0xb8,"",""),
            new Keys(0xb9,"",""),
            new Keys(0xba,      ";",                                ";",                ":",                ":"),
            new Keys(0xbb,      "=",                                "=",                "+",                "+"),
            new Keys(0xbc,      ",",                                ",",                "<",                "<"),
            new Keys(0xbd,      "-",                                "-",                "_",                "_"),
            new Keys(0xbe,      ".",                                ".",                ">",                ">"),
            new Keys(0xbf,      "/",                                "/",                "?",                "?"),

            new Keys(0xc0,      "`",                                "`",                "~",                "~"),
            new Keys(0xc1,"",""),
            new Keys(0xc2,"",""),
            new Keys(0xc3,"",""),
            new Keys(0xc4,"",""),
            new Keys(0xc5,"",""),
            new Keys(0xc6,"",""),
            new Keys(0xc7,"",""),
            new Keys(0xc8,"",""),
            new Keys(0xc9,"",""),
            new Keys(0xca,"",""),
            new Keys(0xcb,"",""),
            new Keys(0xcc,"",""),
            new Keys(0xcd,"",""),
            new Keys(0xce,"",""),
            new Keys(0xcf,"",""),

            new Keys(0xd0,"",""),
            new Keys(0xd1,"",""),
            new Keys(0xd2,"",""),
            new Keys(0xd3,"",""),
            new Keys(0xd4,"",""),
            new Keys(0xd5,"",""),
            new Keys(0xd6,"",""),
            new Keys(0xd7,"",""),
            new Keys(0xd8,"",""),
            new Keys(0xd9,"",""),
            new Keys(0xda,"",""),
            new Keys(0xdb,      "[",                                "[",                "{",                "{"),
            new Keys(0xdc,     @"\",                               @"\",                "|",                "|"),
            new Keys(0xdd,      "]",                                "]",                "}",                "}"),
            new Keys(0xde,      "'",                                "'",                "\"",                "\""),
            new Keys(0xdf,"",""),

            new Keys(0xe0,"",""),
            new Keys(0xe1,"",""),
            new Keys(0xe2,"",""),
            new Keys(0xe3,"",""),
            new Keys(0xe4,"",""),
            new Keys(0xe5,"",""),
            new Keys(0xe6,"",""),
            new Keys(0xe7,"",""),
            new Keys(0xe8,"",""),
            new Keys(0xe9,"",""),
            new Keys(0xea,"",""),
            new Keys(0xeb,"",""),
            new Keys(0xec,"",""),
            new Keys(0xed,"",""),
            new Keys(0xee,"",""),
            new Keys(0xef,"",""),

            new Keys(0xf0,"",""),
            new Keys(0xf1,"",""),
            new Keys(0xf2,"",""),
            new Keys(0xf3,"",""),
            new Keys(0xf4,"",""),
            new Keys(0xf5,"",""),
            new Keys(0xf6,"",""),
            new Keys(0xf7,"",""),
            new Keys(0xf8,"",""),
            new Keys(0xf9,"",""),
            new Keys(0xfa,"",""),
            new Keys(0xfb,"",""),
            new Keys(0xfc,"",""),
            new Keys(0xfd,"",""),
            new Keys(0xfe,"",""),
            new Keys(0xff,"",""),
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
    }
}
