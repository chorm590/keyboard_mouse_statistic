using System;
using System.Collections.Generic;
using System.Text;

namespace KMS.src.db
{
    /// <summary>
    /// create at 2021-01-06 17:11
    /// </summary>
    class EventManager
    {
        private static EventManager instance;
        internal static EventManager GetInstance
        {
            get
            {
                if (instance == null)
                    instance = new EventManager();

                return instance;
            }
        }

        private EventManager()
        {
        
        }
    }
}
