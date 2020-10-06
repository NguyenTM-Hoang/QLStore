using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;
using Aspose.Cells;
using System.Windows;
using System.Windows.Controls;
using QLStore.BS_layer;
using System.Windows.Navigation;
using QLStore.Product;
namespace QLStore.Bill
{
    /// <summary>
    /// Interaction logic for NewBillPage.xaml
    /// </summary>
    public partial class ListBillPage : Page
    {
     
        Manage_Product manage = new Manage_Product();
        ObservableCollection<Bill_Show> Bills;
        public ListBillPage()
        {
            
            List<string> list_Filter = new List<string>()
            {
                "All Bill here",
                "Completely Bill",
                "Not completely Bill",
                "Recently Bill",
                "Longest Bill",
                "Canceled Bill",
                "Buy online",
                "Buy at shop"
            };
            
            InitializeComponent();


            EnableTextbox(false);
            Thread loadBill = new Thread(delegate ()
               {

                   int Arrangeindex = 0;
                   Dispatcher.Invoke(() => { Arrangeindex = comboFilter.SelectedIndex; });
                   Bills = manage.Arrange_ListBill(Arrangeindex);

                   Dispatcher.Invoke(() =>
                   {

                       comboFilter.ItemsSource = list_Filter;
                       listBill.ItemsSource = Bills;
                       ProgressBar.IsEnabled = false;
                       ProgressBar.Visibility = Visibility.Hidden;
                   });
               });
            loadBill.Start();
    
        }

        private void EditAddress_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listBill.ItemsSource = null;
            ProgressBar.IsEnabled = true;
            ProgressBar.Visibility = Visibility.Visible;
            Thread thread = new Thread(delegate ()
            {
                Manage_Product dbProduct = new Manage_Product();
                
                int Arrangeindex = 0;
                Dispatcher.Invoke(() => { Arrangeindex = comboFilter.SelectedIndex; });
                Bills = dbProduct.Arrange_ListBill(Arrangeindex);

                Dispatcher.Invoke(() =>
                {
                    listBill.ItemsSource = Bills;
                    ProgressBar.IsEnabled = false;
                    ProgressBar.Visibility = Visibility.Hidden;
                });

            });
            thread.Start();
        }

        private void ListBill_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Bill_Show bill = new Bill_Show();
            if (listBill.SelectedItem == null) return;
            bill = listBill.SelectedItem as Bill_Show;
            editAddress.Text = bill.Address == "" ? "No address" : bill.Address;
            Editnameproduct.Text = bill.Namepro;
            editNumber.Text = bill.Amount.ToString();
            editOriginalPrice.Text = bill.Initial_price.ToString();
            editPhone.Text = bill.Phone;
            editSellPrice.Text = bill.Sale_price.ToString();
            editShipCost.Text = bill.Ship.ToString();

            if (bill.status == "Complete" || bill.status=="Canceled")
            {
                btnComplete.IsEnabled = false;
                btnIgnore.IsEnabled = false;
            }
            else
            {
                btnComplete.IsEnabled = true;
                btnIgnore.IsEnabled = true;
            }


        }

        void EnableTextbox(bool t)
        {
            Editnameproduct.IsEnabled =
                editShipCost.IsEnabled =
                editSellPrice.IsEnabled =
                editPhone.IsEnabled =
                editOriginalPrice.IsEnabled =
                editNumber.IsEnabled =
                editMoneyWillGet.IsEnabled =
                editAddress.IsEnabled = t;

        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            var confirm = new Dialog() { Message = "Giao hàng đã thành công?" };
            confirm.Owner = Window.GetWindow(this);
            confirm.ShowDialog();
            if (true == confirm.DialogResult)
            {
                // Lấy đối tượng Bill đang chọn trong List View
                if (listBill.SelectedItem == null) return;
                Bill_Show bill = listBill.SelectedItem as Bill_Show;

                try
                {
                    manage.UpdateStatusBill(bill);
                    // Cập nhật lên List View
                    int curIndex = listBill.SelectedIndex;
                    Bills = manage.Load_ListBill();
                    listBill.ItemsSource = Bills;
                    listBill.SelectedIndex = curIndex;
                    // Tắt 2 button
                    btnComplete.IsEnabled = btnIgnore.IsEnabled = false;
                }
                catch (Exception)
                {

                }
            }
        }

        private void BtnIgnore_Click(object sender, RoutedEventArgs e)
        {
            var confirm = new Dialog() { Message = "Hủy đơn hàng đang giao?" };
            confirm.Owner = Window.GetWindow(this);
            confirm.ShowDialog();
            if (true == confirm.DialogResult)
            {
                // Lấy đối tượng Bill đang chọn trong List View
                if (listBill.SelectedItem == null) return;
                Bill_Show bill = listBill.SelectedItem as Bill_Show;
                
                try
                {
                    manage.CancelBill(bill);
                    // Cập nhật lên List View
                    int curIndex = listBill.SelectedIndex;
                    Bills = manage.Load_ListBill();
                    listBill.ItemsSource = Bills;
                    listBill.SelectedIndex = curIndex;
                    // Tắt 2 button
                    btnComplete.IsEnabled = btnIgnore.IsEnabled = false;
                }
                catch (Exception)
                {

                }
            }

        }

        private void BtnSeeProduct_Click(object sender, RoutedEventArgs e)
        {
            if (listBill.SelectedItem == null) return;
            Bill_Show bill_ = listBill.SelectedItem as Bill_Show;
            try
            {
                var target = manage.GetProduct(bill_.ID_PRO);
                NavigationService.Navigate(new DetailProductPage(target));
            }
            catch
            {

            }
            
        }

        void RefreshListBill(bool data)
        {
            if(data)
            {
                Thread loadBill = new Thread(delegate ()
                {

                    int Arrangeindex = 0;
                    Dispatcher.Invoke(() => { Arrangeindex = comboFilter.SelectedIndex; });
                    Bills = manage.Arrange_ListBill(Arrangeindex);

                    Dispatcher.Invoke(() =>
                    {

                        listBill.ItemsSource = Bills;
                        ProgressBar.IsEnabled = false;
                        ProgressBar.Visibility = Visibility.Hidden;
                    });
                });
                loadBill.Start();
            }
        }

        private void BtnNewBill_Click(object sender, RoutedEventArgs e)
        {
            NewBillPage a = new NewBillPage();
            a.loadlistbill = RefreshListBill;
            NavigationService.Navigate(a);
        }
    }
}
