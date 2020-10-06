using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace QLStore.BS_layer
{
    public class ImportProduct
    {
        public string Name { get; set; }
         public string ID { get; set; }
        public int Curr_amount { get; set; }
        public int Orig_price { get; set; }
        public string Supplier { get; set; }
        public string Type { get; set; }
        public string Descrip { get; set; }
        public string ID_Type { get; set; }
        public string ID_Supp { get; set; }
        public string Image_path { get; set; }
        public DateTime input_time { get; set; }
        public int capital { get; set; }
        public int Initial_amount { get; set; }
    }
}
