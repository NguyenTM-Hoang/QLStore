using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
namespace QLStore.BackGroundWD
{
    public class SubItem
    {
        public string Name { get; set; }
        public UserControl control { get; set; }
        
        public SubItem(string Name)
        {
            this.Name = Name;
           
            
        }
    }
}
