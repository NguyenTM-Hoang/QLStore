using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using QLStore.BackGroundWD;
using QLStore.Product;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using QLStore.Bill;
using QLStore.Statistic;
using QLStore.CustomerMVVM;

namespace QLStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }

        private void BtnSetting_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAccount_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {

        }
        
             
        private void ListViewItemMenu_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //showFrame.Navigate(new ProductPage());
        }

        private void A_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            showFrame.Navigate(new ProductPage());
        }

        private void ListViewItemMenu_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void Listbillitem_PreviewMouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            showFrame.Navigate(new ListBillPage());
        }

        private void Grid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            showFrame.Navigate(new StatisticPage());
        }

        private void Grid_PreviewMouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            showFrame.Navigate(new CustomerPage());
        }
    }
}
