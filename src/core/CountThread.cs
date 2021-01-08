using KMS.src.tool;
using System.Threading;
using System.ComponentModel;
using System.Windows;
using System;
using KMS.src.db;
using System.Windows.Input;

namespace KMS.src.core
{
    /// <summary>
    /// created at 2020-12-30 19:54
    /// </summary>
    static class CountThread
    {
        private const string TAG = "CountThread";

        private static EventQueue.KMEvent[] events;
        private static int eventAmount;
        private static int idx;

        internal static bool CanThreadRun;
        private const byte MAX_KEY_CHAIN_COUNT = 4;
        private static byte keyChainCount;
        private static byte comboKeyCount; //记录最多按下的按键数
        private static KeyEvent[] keyChain = new KeyEvent[MAX_KEY_CHAIN_COUNT];

        internal static void ThreadProc()
        {
            Logger.v(TAG, "ThreadProc() run");
            events = new EventQueue.KMEvent[EventQueue.MAX_EVENT_AMOUNT];
            CanThreadRun = true;
            while (CanThreadRun)
            {
                statistic();
                Thread.Sleep(2000);
            }
            Logger.v(TAG, "ThreadProc() end");
        }

        private static void statistic()
        {
            //Should only be call by sub-thread.
            eventAmount = 0;
            EventQueue.migrate(ref events, ref eventAmount);
            Logger.v(TAG, "event amount:" + eventAmount);

            for (idx = 0; idx < eventAmount; idx++)
            {
                if (events[idx].type == EventQueue.EVENT_TYPE_KEYBOARD)
                {
                    parseKeyboardEvent(events[idx].eventCode, events[idx].keyCode, events[idx].time);
                }
                else if (events[idx].type == EventQueue.EVENT_TYPE_MOUSE)
                {
                    parseMouseEvent(events[idx].eventCode, events[idx].keyCode, events[idx].x, events[idx].y);
                }
                else
                {
                    Logger.v(TAG, "Invalid event type");
                }
            }
        }

        private static void parseKeyboardEvent(short eventCode, short keyCode, DateTime time)
        {
            switch (eventCode)
            {
                case Constants.KeyEvent.WM_KEYDOWN:
                case Constants.KeyEvent.WM_SYSKEYDOWN:
                    keyDownProcess(keyCode, time);
                    break;
                case Constants.KeyEvent.WM_SYSKEYUP:
                case Constants.KeyEvent.WM_KEYUP:
                    keyUpProcess(keyCode, time);
                    break;
                default:
                    break;
            }
        }

        private static void parseMouseEvent(short eventCode, short keyCode, short x, short y)
        { 
            
        }

        private static void keyDownProcess(short keycode, DateTime time)
        {
            //清除旧事件
            if (keyChainCount > 0)
            {
                KeyEvent[] tmp = new KeyEvent[MAX_KEY_CHAIN_COUNT];
                byte count = 0;
                sbyte tmp2;
                for (byte i = 0; i < keyChainCount; i++)
                {
                    if (keycode == keyChain[i].keycode)
                    {
                        tmp[count].keycode = keyChain[i].keycode;
                        tmp[count].IsUp = keyChain[i].IsUp;
                        tmp[count].Time = keyChain[i].Time;
                        count++;
                        continue;
                    }

                    if (!keyChain[i].IsUp)
                    {
                        if (keyChain[i].Time.Year != time.Year ||
                            keyChain[i].Time.Month != time.Month ||
                            keyChain[i].Time.Day != time.Day ||
                            keyChain[i].Time.Hour != time.Hour)
                        {
                            continue;
                        }

                        tmp2 = (sbyte)(time.Minute - keyChain[i].Time.Minute);
                        if (tmp2 <= 0)
                        {
                            tmp2 = (sbyte)(time.Second - keyChain[i].Time.Second);
                            if ((tmp2 >= 0 && tmp2 < 6) || (tmp2 < -54 && tmp2 > -60))
                            {
                                tmp[count].keycode = keyChain[i].keycode;
                                tmp[count].IsUp = keyChain[i].IsUp;
                                tmp[count].Time = keyChain[i].Time;
                                count++;
                            }
                        }
                    }
                }

                for (byte i = 0; i < count; i++)
                {
                    keyChain[i].keycode = tmp[i].keycode;
                    keyChain[i].IsUp = tmp[i].IsUp;
                    keyChain[i].Time = tmp[i].Time;
                }
                keyChainCount = count;
            }

            if (keyChainCount >= MAX_KEY_CHAIN_COUNT)
                return;//忽略事件，不予记录。

            //重复的按键不入链
            if (keyChainCount > 0)
            {
                for (byte i = 0; i < keyChainCount; i++)
                {
                    if (keyChain[i].keycode == keycode)
                        return;
                }
            }
            
            keyChain[keyChainCount].keycode = keycode;
            keyChain[keyChainCount].IsUp = false;
            keyChain[keyChainCount].Time = time;
            keyChainCount++;
        }

        private static void keyUpProcess(short keycode, DateTime time)
        {
            if (comboKeyCount == 0)
                comboKeyCount = keyChainCount;

            switch (keyChainCount)
            {
                case 1:
                    comboKeyCount = 0; //reset it.
                    break;
                case 2:
                    //coomboKeyCount数值与case标签值相等表示此时刚开始松开组合键，此时统计键入的组合键类型是最适合的。
                    if (comboKeyCount == 2)
                    {
                        //双键组合键事件。
                        StatisticManager.GetInstance.EventHappen(doubleComboDetect(), time);
                    }
                    break;
                case 3:
                    if (comboKeyCount == 3)
                    {
                        //三键组合键事件。
                        StatisticManager.GetInstance.EventHappen(tripleComboDetect(), time);
                    }
                    break;
                case 4:
                    if (comboKeyCount == 4)
                    {
                        //四键组合键事件。
                        StatisticManager.GetInstance.EventHappen(quadraComboDetect(), time);
                    }
                    break;
                default:
                    keyChainCount = 0;
                    comboKeyCount = 0;
                    return;
            }

            StatisticManager.GetInstance.EventHappen(keycode, time);
            keyChainCount--;
        }

        private static ushort doubleComboDetect()
        {
            if (keyChainCount != 2)
            {
                return (ushort)Constants.DbType.INVALID;
            }

            short a = keyChain[0].keycode;
            short b = keyChain[1].keycode;
            ushort comboKeyType = Constants.ComboKey.DOUBLE;

            if (a == Constants.Keyboard.LEFT_CTRL)
            {
                switch (b)
                {
                    case Constants.Keyboard.LEFT_SHIFT:
                        comboKeyType = Constants.ComboKey.LC_LS;
                        break;
                    case Constants.Keyboard.RIGHT_SHIFT:
                        comboKeyType = Constants.ComboKey.LC_RS;
                        break;
                    case Constants.Keyboard.ENTER:
                        comboKeyType = Constants.ComboKey.LC_ENTER;
                        break;
                    case Constants.Keyboard.A:
                        comboKeyType = Constants.ComboKey.LC_A;
                        break;
                    case Constants.Keyboard.S:
                        comboKeyType = Constants.ComboKey.LC_S;
                        break;
                    case Constants.Keyboard.F:
                        comboKeyType = Constants.ComboKey.LC_F;
                        break;
                    case Constants.Keyboard.C:
                        comboKeyType = Constants.ComboKey.LC_C;
                        break;
                    case Constants.Keyboard.V:
                        comboKeyType = Constants.ComboKey.LC_V;
                        break;
                }
            }
            else if (a == Constants.Keyboard.RIGHT_CTRL)
            {
                switch (b)
                {
                    case Constants.Keyboard.LEFT_SHIFT:
                        comboKeyType = Constants.ComboKey.RC_LS;
                        break;
                    case Constants.Keyboard.RIGHT_SHIFT:
                        comboKeyType = Constants.ComboKey.RC_RS;
                        break;
                    case Constants.Keyboard.ENTER:
                        comboKeyType = Constants.ComboKey.RC_ENTER;
                        break;
                    case Constants.Keyboard.A:
                        comboKeyType = Constants.ComboKey.RC_A;
                        break;
                    case Constants.Keyboard.S:
                        comboKeyType = Constants.ComboKey.RC_S;
                        break;
                    case Constants.Keyboard.F:
                        comboKeyType = Constants.ComboKey.RC_F;
                        break;
                    case Constants.Keyboard.C:
                        comboKeyType = Constants.ComboKey.RC_C;
                        break;
                    case Constants.Keyboard.V:
                        comboKeyType = Constants.ComboKey.RC_V;
                        break;
                }
            }
            else if (a == Constants.Keyboard.LEFT_SHIFT)
            {
                switch (b)
                {
                    case Constants.Keyboard.LEFT_CTRL:
                        comboKeyType = Constants.ComboKey.LC_LS;
                        break;
                    case Constants.Keyboard.RIGHT_CTRL:
                        comboKeyType = Constants.ComboKey.RC_LS;
                        break;
                    case Constants.Keyboard.ENTER:
                        comboKeyType = Constants.ComboKey.LS_ENTER;
                        break;
                    case Constants.Keyboard.HOME:
                        comboKeyType = Constants.ComboKey.LS_HOME;
                        break;
                    case Constants.Keyboard.END:
                        comboKeyType = Constants.ComboKey.LS_END;
                        break;
                    case Constants.Keyboard.LEFT:
                        comboKeyType = Constants.ComboKey.LS_LEFT;
                        break;
                    case Constants.Keyboard.UP:
                        comboKeyType = Constants.ComboKey.LS_UP;
                        break;
                    case Constants.Keyboard.RIGHT:
                        comboKeyType = Constants.ComboKey.LS_RIGHT;
                        break;
                    case Constants.Keyboard.DOWN:
                        comboKeyType = Constants.ComboKey.LS_DOWN;
                        break;
                }
            }
            else if (a == Constants.Keyboard.RIGHT_SHIFT)
            {
                switch (b)
                {
                    case Constants.Keyboard.LEFT_CTRL:
                        comboKeyType = Constants.ComboKey.LC_RS;
                        break;
                    case Constants.Keyboard.RIGHT_CTRL:
                        comboKeyType = Constants.ComboKey.RC_RS;
                        break;
                    case Constants.Keyboard.ENTER:
                        comboKeyType = Constants.ComboKey.RS_ENTER;
                        break;
                    case Constants.Keyboard.HOME:
                        comboKeyType = Constants.ComboKey.RS_HOME;
                        break;
                    case Constants.Keyboard.END:
                        comboKeyType = Constants.ComboKey.RS_END;
                        break;
                    case Constants.Keyboard.LEFT:
                        comboKeyType = Constants.ComboKey.RS_LEFT;
                        break;
                    case Constants.Keyboard.UP:
                        comboKeyType = Constants.ComboKey.RS_UP;
                        break;
                    case Constants.Keyboard.RIGHT:
                        comboKeyType = Constants.ComboKey.RS_RIGHT;
                        break;
                    case Constants.Keyboard.DOWN:
                        comboKeyType = Constants.ComboKey.RS_DOWN;
                        break;
                }
            }
            else if (a == Constants.Keyboard.LEFT_ALT)
            {
                switch (b)
                {
                    case Constants.Keyboard.ENTER:
                        comboKeyType = Constants.ComboKey.LA_ENTER;
                        break;
                    case Constants.Keyboard.TAB:
                        comboKeyType = Constants.ComboKey.LA_TAB;
                        break;
                }
            }
            else if (a == Constants.Keyboard.RIGHT_ALT)
            {
                switch (b)
                {
                    case Constants.Keyboard.ENTER:
                        comboKeyType = Constants.ComboKey.RA_ENTER;
                        break;
                    case Constants.Keyboard.TAB:
                        comboKeyType = Constants.ComboKey.RA_TAB;
                        break;
                }
            }
            else
            {
                //Not a valid combo-key
                comboKeyType = (ushort)Constants.DbType.INVALID;
            }

            return comboKeyType;
        }

        private static ushort tripleComboDetect()
        {
            if (keyChainCount != 3)
            {
                return (ushort)Constants.DbType.INVALID;
            }

            switch (keyChain[0].keycode)
            {
                case Constants.Keyboard.LEFT_CTRL:
                case Constants.Keyboard.RIGHT_CTRL:
                case Constants.Keyboard.LEFT_SHIFT:
                case Constants.Keyboard.RIGHT_SHIFT:
                case Constants.Keyboard.LEFT_ALT:
                case Constants.Keyboard.RIGHT_ALT:
                case Constants.Keyboard.LEFT_WIN:
                case Constants.Keyboard.RIGHT_WIN:
                    break;
                default:
                    return (ushort)Constants.DbType.INVALID;
            }

            return Constants.ComboKey.TRIPLE;
        }

        private static ushort quadraComboDetect()
        {
            if (keyChainCount != 4)
            {
                return (ushort)Constants.DbType.INVALID;
            }

            switch (keyChain[0].keycode)
            {
                case Constants.Keyboard.LEFT_CTRL:
                case Constants.Keyboard.RIGHT_CTRL:
                case Constants.Keyboard.LEFT_SHIFT:
                case Constants.Keyboard.RIGHT_SHIFT:
                case Constants.Keyboard.LEFT_ALT:
                case Constants.Keyboard.RIGHT_ALT:
                case Constants.Keyboard.LEFT_WIN:
                case Constants.Keyboard.RIGHT_WIN:
                    break;
                default:
                    return (ushort)Constants.DbType.INVALID;
            }

            return Constants.ComboKey.QUADRA;
        }

        /// <summary>
        /// 按键链表，主要用于记录组合键。
        /// </summary>
        private struct KeyEvent
        {
            internal short keycode;
            internal bool IsUp;
            internal DateTime Time;
        }
    }
}
