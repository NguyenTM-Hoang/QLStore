using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLStore.BS_layer
{
    public class Bill_Show
    {
        public string ID_output { get; set; }
        public string Name_Cus { get; set; }
        public string Namepro { get; set; }
        public DateTime DateCreate { get; set; }
        public string ID_PRO { get; set; }
        public int Amount { get; set; }
        public string status { get; set; }
        public int Sale_price { get; set; }
        public int Initial_price { get; set; }
        public string Online { get; set; }
        public string Phone { get; set; }
        public int Ship { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public DateTime birthday { get; set; }
    } 
}
