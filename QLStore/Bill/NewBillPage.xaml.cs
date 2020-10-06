using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using QLStore.BS_layer;
using QLStore.Product;
using QLStore.Class_Converse;
namespace QLStore.Bill
{
    /// <summary>
    /// Interaction logic for ListBillPage.xaml
    /// </summary>
    public partial class NewBillPage : Page
    {
        //load listbill của trang trước
        public delegate void RefreshlistBill(bool data);
        public RefreshlistBill loadlistbill;

        Detail_Product1 product;
        Manage_Product manage = new Manage_Product();
        public NewBillPage()
        {
            InitializeComponent();
        }

        public NewBillPage(Detail_Product1 _product)
        {
            InitializeComponent();
            product = _product;
            btnProductName.Content = product.NameProduct;
            editNumberBuy.Text = "1";
            editSalePrice.Text = product.Original_Price.ToString();
            editNumberBuy.IsEnabled = true;
            BitmapImage soure = new BitmapImage(new Uri(product.Image_Path));
            imgProduct.Source = soure;
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            editCustomerName.Clear();
            editCustomerPhone.Clear();
            editNumberBuy.Clear();
            editTotalPrice.Clear();
            editSalePrice.Clear();
            editMoneyTaken.Clear();
            editMoneyExchange.Clear();
            editAddress.Clear();
            editDeposit.Clear();
            editShipCost.Clear();
            editMoneyWillGet.Clear();
            rdoGoToShop.IsChecked = true;
        }

        

        private void EditNumberBuy_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (product != null)
                if(editNumberBuy.Text!="")
                {
                    int numberbuy = Int32.Parse(editNumberBuy.Text);
                    int price =(int) product.Original_Price;
                    int total = numberbuy * price;
                    editTotalPrice.Text = total.ToString();

                    if (rdoGoToShop.IsChecked == true)
                    {

                        if (editMoneyTaken.Text.Length > 0)
                        {
                            double taken = 0;
                            double.TryParse(editMoneyTaken.Text, out taken);
                            editMoneyTaken.Text = taken.ToString("N0");
                            editMoneyTaken.CaretIndex = editMoneyTaken.Text.Length;

                            // Tính toán số tiền thối lại
                            double totalPrice = 0;
                            double.TryParse(editTotalPrice.Text, out totalPrice);
                            editMoneyExchange.Text = (taken - totalPrice).ToString("N0");
                        }
                    }
                    else
                    if (rdoShip.IsChecked == true)
                    {
                        if (editDeposit.Text.Length > 0 || editShipCost.Text.Length > 0)
                        {
                            double totalPrice = 0; // Giá bán
                            double.TryParse(editTotalPrice.Text, out totalPrice);

                            double deposit = 0; // Tiền cọc
                            double.TryParse(editDeposit.Text, out deposit);
                            editDeposit.CaretIndex = editDeposit.Text.Length;
                            editDeposit.Text = deposit.ToString();

                            double shipCost = 0; // Phí ship
                            double.TryParse(editShipCost.Text, out shipCost);
                            editShipCost.CaretIndex = editShipCost.Text.Length;
                            editShipCost.Text = shipCost.ToString();

                            // Tính số tiền cần thu khi đi giao
                            editMoneyWillGet.Text = (totalPrice - deposit + shipCost).ToString("N0");
                        }
                    }
                }
        }

        private void BtnProductId_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var chooseProduct = new ProductPage();
            chooseProduct.PickProductID = (x) =>
            {
                product = x;
                btnProductName.Content = product.NameProduct;

                // Cập nhật giá cả + lượng tặng (xóa rồi nhập lại để gọi hàm phát sinh)
                editNumberBuy.IsEnabled = true;
                editSalePrice.IsEnabled = true;
                editTotalPrice.IsEnabled = true;


                editNumberBuy.Text = "1";
                editSalePrice.Text = product.Original_Price.ToString();

            };
            NavigationService.Navigate(chooseProduct);
        }

        private void BtnProductName_Click(object sender, RoutedEventArgs e)
        {
            var chooseProduct = new ProductPage();
            chooseProduct.PickProductID = (x) =>
            {
                product = x;
                btnProductName.Content = product.NameProduct;

                // Cập nhật giá cả + lượng tặng (xóa rồi nhập lại để gọi hàm phát sinh)
                editNumberBuy.IsEnabled = true;
                editSalePrice.IsEnabled = true;
                editTotalPrice.IsEnabled = true;


                editNumberBuy.Text = "1";
                editSalePrice.Text = product.Original_Price.ToString();
                editTotalPrice.Text = editSalePrice.Text;
                if (product.Image_Path != null)
                {
                    BitmapImage source = new BitmapImage(new Uri(product.Image_Path));
                    imgProduct.Source = source;
                }
            };
            NavigationService.Navigate(chooseProduct);
        }

        private void Radio_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button == rdoGoToShop)
            {
                if (editAddress != null) editAddress.Clear();
                if (editDeposit != null) editDeposit.Clear();
                if (editShipCost != null) editShipCost.Clear();
                if (editMoneyWillGet != null) editMoneyWillGet.Clear();
            }
            else
            {
                if (editMoneyTaken != null) editMoneyTaken.Clear();
                if (editMoneyExchange != null) editMoneyExchange.Clear();
            }
        }

        private void EditMoneyTaken_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            if (editMoneyTaken.Text.Length > 0)
            {
                // Định dạng giá vừa nhập
                double taken = 0;
                double.TryParse(editMoneyTaken.Text, out taken);
                editMoneyTaken.Text = taken.ToString("N0");
                editMoneyTaken.CaretIndex = editMoneyTaken.Text.Length;

                // Tính toán số tiền thối lại
                double sellPrice = 0;
                double.TryParse(editSalePrice.Text, out sellPrice);
                editMoneyExchange.Text = (taken - sellPrice).ToString("N0");
            }
            else editMoneyExchange.Clear();
        }

        private void ShipTextchange_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            if (editDeposit.Text.Length > 0 || editShipCost.Text.Length > 0)
            {
                double totalPrice = 0; // Giá bán
                double.TryParse(editTotalPrice.Text, out totalPrice);

                double deposit = 0; // Tiền cọc
                double.TryParse(editDeposit.Text, out deposit);
                editDeposit.CaretIndex = editDeposit.Text.Length;
                editDeposit.Text = deposit.ToString();

                double shipCost = 0; // Phí ship
                double.TryParse(editShipCost.Text, out shipCost);
                editShipCost.CaretIndex = editShipCost.Text.Length;
                editShipCost.Text = shipCost.ToString();

                // Tính số tiền cần thu khi đi giao
                editMoneyWillGet.Text = (totalPrice - deposit + shipCost).ToString("N0");
            }
            else
            {
                editDeposit.Clear();
                editShipCost.Clear();
                editMoneyWillGet.Clear();
            }
        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ListBillPage());
        }

        bool CheckBill()
        {
            if (editCustomerName.Text.Length == 0 // Nếu tên khách hàng trống
                ||editCustomerEmail.Text.Length==0
                || editCustomerPhone.Text.Length==0
                || birthdate.Text.Length==0
                || imgProduct.Source==null  // Nếu sản phẩm mua trống
                || editNumberBuy.Text.Length == 0   // Nếu số lượng mua trống
                || (rdoGoToShop.IsChecked == true && editMoneyTaken.Text.Length == 0)   // Nếu thanh toán trực tiếp mà chưa đưa tiền
                || (rdoShip.IsChecked == true && editAddress.Text.Length == 0)          // Nếu thanh toán giao hàng mà không đưa địa chỉ
                || (rdoShip.IsChecked == true && editMoneyWillGet.Text.Length == 0))    // Nếu thanh toán giao hàng mà không biết số tiền sẽ thu
            {
                var dialog = new Dialog() { Message = "Vui lòng nhập đầy đủ thông tin" };
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
                return false;
            }

            // Kiểm tra còn đủ hàng hay không
          
            int number = 0;                                         // Số lượng mua
            int.TryParse(editNumberBuy.Text, out number);
            Manage_Product manage = new Manage_Product();
            if (manage.CheckCurrentAmount(product.ID_Product, number)==false)  // Nếu số lượng không đủ thì thông báo
            {
                var dialog = new Dialog() { Message = "Sản phẩm không đủ số lượng" };
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
                return false;
            }
            return true;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBill() == false) return;

            var bill = new Bill_Show();

            bill.DateCreate = DateTime.Now;
            bill.Name_Cus = editCustomerName.Text;
            bill.Phone = editCustomerPhone.Text.Length == 0 ? null : editCustomerPhone.Text;
            bill.ID_PRO = product.ID_Product;
            bill.Amount = (int.Parse(editNumberBuy.Text));
            bill.Initial_price = (int.Parse(editSalePrice.Text));
            bill.Sale_price = int.Parse(editTotalPrice.Text);
            bill.Online = (bool)rdoGoToShop.IsChecked ? "No" : "Yes";
            bill.Address = (bool)rdoShip.IsChecked ? editAddress.Text : null;
            bill.Ship = (bool)rdoShip.IsChecked ? Int32.Parse(editShipCost.Text) : 0;
            bill.status = (bool)rdoGoToShop.IsChecked ? "Complete" : "No complete";
            bill.Namepro = product.NameProduct;
            bill.birthday =DateTime.Parse( birthdate.Text);
            bill.Email = editCustomerEmail.Text == null ? "" : editCustomerEmail.Text;
            
            bill.ID_output = manage.Create_NewIdOutput_Auto();

            manage.AddnewOutput(bill);

            Dialog a = new Dialog()
            {
                Message = "Đã tạo Bill!"
            };
            a.Owner = Window.GetWindow(this);
            a.ShowDialog();
            if (loadlistbill != null)
                loadlistbill.Invoke(true);
                
        }
    }
}
