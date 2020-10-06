using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLStore.Class_Converse
{
    public class Output_Form1
    {
        public string ID_Output { get; set; }
        public string ID_Product { get; set; }
        public string ID_Customer { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<System.DateTime> Output_Date { get; set; }
        public Nullable<int> Price_Sale { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public string BuyOnline { get; set; }
        public Nullable<int> Ship { get; set; }
    }
}
