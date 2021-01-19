using KMS.src.db;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KMS
{
    /// <summary>
    /// Interaction logic for GlobalStatisticDetail.xaml
    /// </summary>
    public partial class GlobalStatisticDetail : Window
    {
        private const string TAG = "GlobalStatisticDetail";

        private List<Record> records;

        public GlobalStatisticDetail()
        {
            InitializeComponent();
        }

        internal void SetStatistic(Record msSideForward, Record msSideBackward, Record msWheelClick, List<Record> kbKeys)
        {
            if (msSideBackward is null || msSideForward is null || msWheelClick is null || kbKeys is null)
                return;

            records = new List<Record>(kbKeys.Count + 3);
            records.AddRange(kbKeys);
            records.Add(msSideBackward);
            records.Add(msSideForward);
            records.Add(msWheelClick);
        }
    }
}
