
using System;

namespace KMS.src.db
{
    class EventDetail
    {
        private const short DEF_YEAR = 0;
        private const byte DEF_MONTH = 0;
        private const byte DEF_DAY = 0;
        private const byte DEF_HOUR = 0;
        private const byte DEF_MINUTE = 0;
        private const byte DEF_SECOND = 0;

        private short year;
        private byte month;
        private byte day;
        private byte hour;
        private byte minute;
        private byte second;
        private ushort type;
        private ushort value;

        internal short Year
        {
            get { return year; }
            set
            {
                if (value < 1970 || value > 2099)
                {
                    year = DEF_YEAR;
                }
                else
                {
                    year = value;
                }
            }
        }

        internal byte Month
        {
            get { return month; }
            set
            {
                if (value < 1 || value > 12)
                {
                    year = DEF_YEAR;
                    month = DEF_MONTH;
                }
                else
                {
                    month = value;
                }
            }
        }

        internal byte Day
        {
            get { return day; }
            set
            {
                if (value < 1 || value > 31)
                {
                    year = DEF_YEAR;
                    month = DEF_MONTH;
                    day = DEF_DAY;
                }
                else
                {
                    day = value;
                }
            }
        }
        
        internal byte Hour
        {
            get { return hour; }
            set
            {
                if (value > 23)
                {
                    year = DEF_YEAR;
                    month = DEF_MONTH;
                    day = DEF_DAY;
                    hour = DEF_HOUR;
                }
                else
                {
                    hour = value;
                }
            }
        }
        
        internal byte Minute
        {
            get { return minute; }
            set
            {
                if (value > 59)
                {
                    year = DEF_YEAR;
                    month = DEF_MONTH;
                    day = DEF_DAY;
                    hour = DEF_HOUR;
                    minute = DEF_MINUTE;
                }
                else
                {
                    minute = value;
                }
            }
        }
        
        internal byte Second
        {
            get { return second; }
            set
            {
                if (value > 59)
                {
                    year = DEF_YEAR;
                    month = DEF_MONTH;
                    day = DEF_DAY;
                    hour = DEF_HOUR;
                    minute = DEF_MINUTE;
                    second = DEF_SECOND;
                }
                else
                {
                    second = value;
                }
            }
        }

        internal ushort Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        internal ushort Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
            }
        }

        internal void setTime(DateTime time)
        {
            Year = (short)time.Year;
            Month = (byte)time.Month;
            Day = (byte)time.Day;
            Hour = (byte)time.Hour;
            Minute = (byte)time.Minute;
            Second = (byte)time.Second;
        }
    }
}