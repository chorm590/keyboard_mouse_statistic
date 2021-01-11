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

        internal static bool CanThreadRun;

        private static EventQueue.KMEvent[] events;
        private static int eventAmount;

        private const byte MAX_KEY_CHAIN_COUNT = 3;
        private static byte keyChainCount;
        private static NormalKey[] keyChain = new NormalKey[MAX_KEY_CHAIN_COUNT]; //最多支持三连普通按键。
        private static byte comboKeyCount; //记录最多按下的按键数

        private static short msWheelCounter; //同一方向的滚轮滚动聚合成一条记录以节约性能。负值表示向后滚动，正值表示向前滚动。
        private static DateTime msWheelTime; //与 msWheelCounter 配合使用，记录最后一次滚动事件的时间。

        private static FunKey lctrl;
        private static FunKey rctrl;
        private static FunKey lshift;
        private static FunKey rshift;
        private static FunKey lalt;
        private static FunKey ralt;
        private static FunKey lwin;
        private static FunKey rwin;

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
            eventAmount = 0; //Must do.
            EventQueue.migrate(ref events, ref eventAmount);
            Logger.v(TAG, "event amount:" + eventAmount);

            for (int idx = 0; idx < eventAmount; idx++)
            {
                if (events[idx].type == Constants.HookEvent.KEYBOARD_EVENT)
                {
                    parseKeyboardEvent(events[idx].eventCode, events[idx].keyCode, events[idx].time);
                }
                else if (events[idx].type == Constants.HookEvent.MOUSE_EVENT)
                {
                    parseMouseEvent(events[idx].eventCode, events[idx].keyCode, events[idx].x, events[idx].y, events[idx].time);
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

        private static void parseMouseEvent(short eventCode, short mouseData, short x, short y, DateTime time)
        {
            if (eventCode != Constants.MouseEvent.WM_MOUSEWHEEL && msWheelCounter != 0)
            {
                if (msWheelCounter < 0)
                {
                    StatisticManager.GetInstance.MouseEventHappen(Constants.MouseKey.MOUSE_WHEEL_BACKWARD, msWheelCounter * -1, 0, 0, msWheelTime);
                }
                else
                {
                    StatisticManager.GetInstance.MouseEventHappen(Constants.MouseKey.MOUSE_WHEEL_FORWARD, msWheelCounter, 0, 0, msWheelTime);
                }
                msWheelCounter = 0;
            }

            switch (eventCode)
            {
                case Constants.MouseEvent.WM_LBUTTONDOWN:
                    StatisticManager.GetInstance.MouseEventHappen(Constants.MouseKey.MOUSE_LEFT_BTN, 0, x, y, time);
                    break;
                case Constants.MouseEvent.WM_RBUTTONDOWN:
                    StatisticManager.GetInstance.MouseEventHappen(Constants.MouseKey.MOUSE_RIGHT_BTN, 0, x, y, time);
                    break;
                case Constants.MouseEvent.WM_MOUSEWHEEL:
                    if (mouseData == -120)
                    {
                        if (msWheelCounter > 0)
                        {
                            //上一次仍是前向滚动，本次突然变成后向滚动，需要将前面的前向滚动事件记录下来。
                            StatisticManager.GetInstance.MouseEventHappen(Constants.MouseKey.MOUSE_WHEEL_FORWARD, msWheelCounter, 0, 0, msWheelTime);
                            msWheelCounter = -1;
                        }
                        else
                        {
                            msWheelCounter--;
                            msWheelTime = time;
                        }
                    }
                    else if (mouseData == 120)
                    {
                        if (msWheelCounter < 0)
                        {
                            //上一次仍是后向滚动，本次突然变成前向滚动，需要将前面的后向滚动事件记录下来。
                            StatisticManager.GetInstance.MouseEventHappen(Constants.MouseKey.MOUSE_WHEEL_BACKWARD, msWheelCounter * -1, 0, 0, msWheelTime);
                            msWheelCounter = 1;
                        }
                        else
                        {
                            msWheelCounter++;
                            msWheelTime = time;
                        }
                    }
                    break;
                case Constants.MouseEvent.WM_MOUSESIDEDOWN:
                    if (mouseData == Constants.MouseDataHighOrder.SIDE_BACKWARD)
                    {
                        StatisticManager.GetInstance.MouseEventHappen(Constants.MouseKey.MOUSE_SIDE_KEY_BACKWARD, 0, 0, 0, time);
                    }
                    else if (mouseData == Constants.MouseDataHighOrder.SIDE_FORWARD)
                    {
                        StatisticManager.GetInstance.MouseEventHappen(Constants.MouseKey.MOUSE_SIDE_KEY_FORWARD, 0, 0, 0, time);
                    }
                    break;
            }
        }

        private static void keyDownProcess(short keycode, DateTime time)
        {
            //清除普通按键过期事件记录。
            if (keyChainCount > 0)
            {
                NormalKey[] tmp = new NormalKey[MAX_KEY_CHAIN_COUNT]; //记录需要保留的事件。
                byte count = 0;
                for (byte i = 0; i < keyChainCount; i++)
                {
                    if (!isTimeout(keyChain[i].time, time))
                    {
                        tmp[count].keycode = keyChain[i].keycode;
                        tmp[count].time = keyChain[i].time;

                        count++;
                    }
                }

                Logger.v(TAG, "event not expired count:" + count);
                for (byte i = 0; i < count; i++)
                {
                    keyChain[i].keycode = tmp[i].keycode;
                    keyChain[i].time = tmp[i].time;
                }
                keyChainCount = count;
            }

            //清除功能按键过期事件记录。
            if (lctrl.IsPress && isTimeout(lctrl.time, time))
            {
                lctrl.IsPress = false;
            }

            if (rctrl.IsPress && isTimeout(rctrl.time, time))
            {
                rctrl.IsPress = false;
            }

            if (lshift.IsPress && isTimeout(lshift.time, time))
            {
                lshift.IsPress = false;
            }

            if (rshift.IsPress && isTimeout(rshift.time, time))
            {
                rshift.IsPress = false;
            }

            if (lalt.IsPress && isTimeout(lalt.time, time))
            {
                lalt.IsPress = false;
            }

            if (ralt.IsPress && isTimeout(ralt.time, time))
            {
                ralt.IsPress = false;
            }

            if (lwin.IsPress && isTimeout(lwin.time, time))
            {
                lwin.IsPress = false;
            }

            if (rwin.IsPress && isTimeout(rwin.time, time))
            {
                rwin.IsPress = false;
            }


            if (keyChainCount >= MAX_KEY_CHAIN_COUNT)
                return;//忽略事件，不予记录。

            switch (keycode)
            {
                default:
                    //重复的按键不入链
                    if (keyChainCount > 0)
                    {
                        for (byte i = 0; i < keyChainCount; i++)
                        {
                            if (keyChain[i].keycode == keycode)
                            {
                                keyChain[i].time = time; //just upgrade event time.
                                return;
                            }
                        }
                    }

                    keyChain[keyChainCount].keycode = keycode;
                    keyChain[keyChainCount].time = time;
                    keyChainCount++;
                    break;
            }
            
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
                        StatisticManager.GetInstance.KeyboardEventHappen(doubleComboDetect(), time);
                    }
                    break;
                case 3:
                    if (comboKeyCount == 3)
                    {
                        //三键组合键事件。
                        StatisticManager.GetInstance.KeyboardEventHappen(tripleComboDetect(), time);
                    }
                    break;
                case 4:
                    if (comboKeyCount == 4)
                    {
                        //四键组合键事件。
                        StatisticManager.GetInstance.KeyboardEventHappen(quadraComboDetect(), time);
                    }
                    break;
                default:
                    keyChainCount = 0;
                    comboKeyCount = 0;
                    return;
            }

            StatisticManager.GetInstance.KeyboardEventHappen(keycode, time);
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
        /// 超时时长限制：5秒。
        /// </summary>
        private static bool isTimeout(DateTime oldTime, DateTime curTime)
        {
            double ts = (curTime - oldTime).TotalSeconds;
            return ts < -5 || ts > 5;
        }

        /// <summary>
        /// 用于记录功能键按下事件。
        /// </summary>
        private struct FunKey
        {
            internal bool IsPress;
            internal DateTime time;
        }

        private struct NormalKey
        {
            internal short keycode;
            internal DateTime time;
        }
    }
}
