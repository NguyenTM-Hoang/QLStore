using System.Windows;
using  QLStore.BackGroundWD;
using System.Windows.Controls;

namespace QLStore
{
    /// <summary>
    /// Interaction logic for ExpanderControl.xaml
    /// </summary>
    public partial class ExpanderControl : UserControl
    {
        public ExpanderControl(ItemMenu itemMenu)
        {
            InitializeComponent();

            if (itemMenu.listSub == null)
                ExpanderMenu.Visibility = Visibility.Collapsed;
            else ExpanderMenu.Visibility = Visibility.Visible;
            if (itemMenu.listSub == null)
                ListViewItemMenu.Visibility = Visibility.Visible;
            else ListViewItemMenu.Visibility = Visibility.Collapsed;

            this.DataContext = itemMenu;
        }
    }
}
