using QLStore.Background;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLStore.Statistic
{
    public class InstanceStatistic : baseViewModel
    {

        private string nameType;
        private int amountType;
        private int totalProceed;


        public string NameType
        {
            get => nameType;
            set { nameType = value; OnPropertyChanged("NameType"); }
        }
        public int AmountType
        {
            get => amountType;
            set { amountType = value; OnPropertyChanged("AmountType"); }
        }
        public int TotalProceed
        {
            get => totalProceed;
            set { totalProceed = value; OnPropertyChanged("TotalProceed"); }
        }
    }
}
