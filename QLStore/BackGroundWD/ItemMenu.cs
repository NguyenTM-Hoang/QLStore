using System.Windows.Controls;
using System.Collections.Generic;
using MaterialDesignThemes.Wpf;

namespace QLStore.BackGroundWD
{
    public class ItemMenu
    {
        public string Header { get; set; }
        public UserControl UserControl { get; set; }
        public List<SubItem> listSub { get; set; }
        public PackIconKind Icon { get; set; }

        public ItemMenu(string Headername, List<SubItem> subItems,PackIconKind icon)
        {
            this.Header = Headername;
            this.listSub = subItems;
            this.Icon = icon;
        }

        public ItemMenu(string Headername, UserControl userControl, PackIconKind icon)
        {
            this.Header = Headername;
            this.UserControl = userControl;
            this.Icon = icon;
        }
    }
}
