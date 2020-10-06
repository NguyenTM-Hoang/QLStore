using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using QLStore.BS_layer;
using System.Collections.ObjectModel;
using QLStore.Bill;
using QLStore.Class_Converse;
namespace QLStore.Product
{
    /// <summary>
    /// Interaction logic for DetailProductPage.xaml
    /// </summary>
    public partial class DetailProductPage : Page
    {
        Manage_Product manage = new Manage_Product();
        Detail_Product1 product;
        public delegate void DelegateRefeshProductList(bool Data);
        public DelegateRefeshProductList RefreshProductList;

        public DetailProductPage(Detail_Product1 product)
        {
            InitializeComponent();
            this.product = product;
            refresh(false);
           
        }

        private void ListBill_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void refresh(bool Data)
        {
            if (Data) // Nếu vừa sửa xong
            {
               
            }
            // Đưa thông tin lên UI
            txbProductName.Text = product.NameProduct;
            txbProductId.Text = product.ID_Product;
            txbOriginalPrice.Text = product.Original_Price.ToString();
            txbSupplier.Text = manage.GetSupplier(product);
            txbDateImport.Text = manage.GetDateImport(product).ToString();
            txbCurrAmount.Text = product.Amount_Current.ToString() +"/"+ manage.GetinitialAmount(product).ToString();           
            // editProductType.Text = product.ProductType;
            if (product.Description_Pro != null) txbDescri.Text = product.Description_Pro;
            if (product.Image_Path != null)
            {
                BitmapImage source = new BitmapImage(new Uri(product.Image_Path));
                imgProduct.Source =source;
            }

            // Lấy tên loại sản phẩm và các hóa đơn liên quan
            Thread thread = new Thread(delegate ()
            {
                try
                {
                    string productTypeName = manage.GetType(product);              // Lấy tên loại sản phẩm
                    ObservableCollection<Bills> bills = manage.Load_Bill(product.ID_Product); // Lấy danh sách hóa đơn
                     
                    // Đưa lên UI
                    Dispatcher.Invoke(() => {
                        txbProductType.Text = productTypeName;
                        listBill.ItemsSource = bills;                       
                        // Hiện thông báo nếu không có Bill nào
                        if (bills.Count == 0) noBillAnnounce.Visibility = Visibility.Visible;
                    });
                }
           catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            
            });
            thread.Start();
        }

        private void BtnEditProduct_Click(object sender, RoutedEventArgs e)
        {

            if (RefreshProductList != null)
                RefreshProductList.Invoke(true);

            NewProductPage a = new NewProductPage(this.product);
            a.RefreshProductList = refresh;
            NavigationService.Navigate(a);
        }
        //mới thêm
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Dialog a = new Dialog()
            {
                Message = "Are you sure to delete this product?"
            };

            a.Owner = Window.GetWindow(this);
            if (a.ShowDialog() == false) return;
            manage.DeleteProduct(this.product);

            Dialog b = new Dialog()
            {
                Message = "Delete completely"
            };
            b.Owner = Window.GetWindow(this);
            ProductPage page = new ProductPage();
            page.Refresh(true);
            NavigationService.Navigate(page);
        }

        private void BtnAddbill_Click(object sender, RoutedEventArgs e)
        {
            var addBill = new NewBillPage(this.product);
            NavigationService.Navigate(addBill);
        }
    }
}
