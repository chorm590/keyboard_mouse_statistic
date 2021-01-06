using KMS.src.tool;
using System.Threading;
using System.ComponentModel;
using System.Windows;
using System;
using KMS.src.db;

namespace KMS.src.core
{
    /// <summary>
    /// created at 2020-12-30 19:54
    /// </summary>
    static class StatisticThread
    {
        private const string TAG = "StatisticThread";

        private static EventQueue.KMEvent[] events = new EventQueue.KMEvent[EventQueue.MAX_EVENT_AMOUNT];
        private static int eventAmount;
        private static int idx;

        internal static bool CanThreadRun;
        internal static SpecifyKeyDown skd;


        internal static void ThreadProc()
        {
            Logger.v(TAG, "ThreadProc() run");
            CanThreadRun = true;
            while (CanThreadRun)
            {
                statistic();
                Thread.Sleep(2000); //Check every 2s.
            }
            Logger.v(TAG, "ThreadProc() end");
        }

        private static void statistic()
        {
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
            switch (keycode)
            {
                case Constants.KeyCode.LEFT_SHIFT:
                    skd.isLShiftDown = true;
                    skd.lShiftDownTime = time;
                    break;
                case Constants.KeyCode.RIGHT_SHIFT:
                    skd.isRShiftDown = true;
                    skd.rShiftDownTime = time;
                    break;
                case Constants.KeyCode.LEFT_CTRL:
                    skd.isLCtrlDown = true;
                    skd.lCtrlDownTime = time;
                    break;
                case Constants.KeyCode.RIGHT_CTRL:
                    skd.isRCtrlDown = true;
                    skd.rCtrlDownTime = time;
                    break;
                case Constants.KeyCode.LEFT_ALT:
                    skd.isLAltDown = true;
                    skd.lAltDownTime = time;
                    break;
                case Constants.KeyCode.RIGHT_ALT:
                    skd.isRAltDown = true;
                    skd.rAltDownTime = time;
                    break;
                case Constants.KeyCode.LEFT_WIN:
                    skd.isLWinDown = true;
                    skd.lWinDownTime = time;
                    break;
                case Constants.KeyCode.RIGHT_WIN:
                    skd.isRWinDown = true;
                    skd.rWinDownTime = time;
                    break;
            }
        }

        private static void keyUpProcess(short keycode, DateTime time)
        {
            EventDetail ed = null;

            switch (keycode)
            {
                case Constants.KeyCode.LEFT_SHIFT:
                    skd.isLShiftDown = false;
                    break;
                case Constants.KeyCode.RIGHT_SHIFT:
                    skd.isRShiftDown = false;
                    break;
                case Constants.KeyCode.LEFT_CTRL:
                    skd.isLCtrlDown = false;
                    break;
                case Constants.KeyCode.RIGHT_CTRL:
                    skd.isRCtrlDown = false;
                    break;
                case Constants.KeyCode.LEFT_ALT:
                    skd.isLAltDown = false;
                    break;
                case Constants.KeyCode.RIGHT_ALT:
                    skd.isRAltDown = false;
                    break;
                case Constants.KeyCode.LEFT_WIN:
                    skd.isLWinDown = false;
                    break;
                case Constants.KeyCode.RIGHT_WIN:
                    skd.isRWinDown = false;
                    break;
                // combo key detect... 2021-01-06 16:35
                case Constants.KeyCode.NUM0:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_0;
                    }
                    break;
                case Constants.KeyCode.NUM1:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_1;
                    }
                    break;
                case Constants.KeyCode.NUM2:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_2;
                    }
                    break;
                case Constants.KeyCode.NUM3:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_3;
                    }
                    break;
                case Constants.KeyCode.NUM4:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_4;
                    }
                    break;
                case Constants.KeyCode.NUM5:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_5;
                    }
                    break;
                case Constants.KeyCode.NUM6:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_6;
                    }
                    break;
                case Constants.KeyCode.NUM7:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_7;
                    }
                    break;
                case Constants.KeyCode.NUM8:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_8;
                    }
                    break;
                case Constants.KeyCode.NUM9:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_9;
                    }
                    break;
                case Constants.KeyCode.EF_A:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_A;
                    }
                    break;
                case Constants.KeyCode.EF_B:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_B;
                    }
                    break;
                case Constants.KeyCode.EF_C:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_C;
                    }
                    break;
                case Constants.KeyCode.EF_D:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_D;
                    }
                    break;
                case Constants.KeyCode.EF_E:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_E;
                    }
                    break;
                case Constants.KeyCode.EF_F:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_F;
                    }
                    break;
                case Constants.KeyCode.EF_G:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_G;
                    }
                    break;
                case Constants.KeyCode.EF_H:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_H;
                    }
                    break;
                case Constants.KeyCode.EF_I:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_I;
                    }
                    break;
                case Constants.KeyCode.EF_J:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_J;
                    }
                    break;
                case Constants.KeyCode.EF_K:
                    if (skd.isLShiftDown || skd.isRShiftDown)
                    {
                        ed = new EventDetail();
                        ed.setTime(time);
                        ed.Type = (ushort)Constants.DbType.EF_K;
                    }
                    break;
            }
        }

        /// <summary>
        /// 常见组合键被按下时要特别记录。2021-01-06 15:17
        /// </summary>
        internal struct SpecifyKeyDown
        {
            internal bool isLCtrlDown;
            internal DateTime lCtrlDownTime;

            internal bool isRCtrlDown;
            internal DateTime rCtrlDownTime;

            internal bool isLShiftDown;
            internal DateTime lShiftDownTime;

            internal bool isRShiftDown;
            internal DateTime rShiftDownTime;

            internal bool isLAltDown;
            internal DateTime lAltDownTime;

            internal bool isRAltDown;
            internal DateTime rAltDownTime;

            internal bool isLWinDown;
            internal DateTime lWinDownTime;

            internal bool isRWinDown;
            internal DateTime rWinDownTime;
        }
    }
}
