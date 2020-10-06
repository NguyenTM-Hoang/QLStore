using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using QLStore.Supplier_;
using QLStore.BS_layer;
using QLStore.Class_Converse;
namespace QLStore.Product
{
    /// <summary>
    /// Interaction logic for NewProductPage.xaml
    /// </summary>
    public partial class NewProductPage : Page
    {

        public bool isEdit=false;
        public Manage_Product manage = new Manage_Product();
        //MANAGEMENT_STORE_Entities db = new MANAGEMENT_STORE_Entities();
        public delegate void DelegateRefeshProductList(bool Data);
        public DelegateRefeshProductList RefreshProductList;

        public NewProductPage()
        {
            InitializeComponent();

            Thread getTypes = new Thread(delegate () 
            {
                //MANAGEMENT_STORE_Entities db = new MANAGEMENT_STORE_Entities();
                var TypeProduct = manage.getListTypeName();
                var Supplier = manage.getListSupplierName();

                Dispatcher.Invoke(() =>
                 {
                     ProductId.Text= manage.Create_NewIdproduct_Auto();
                     comboProductTypes.ItemsSource = TypeProduct;
                     comboboxSupplier.ItemsSource = Supplier;
                     BitmapImage image = new BitmapImage();
                     image.BeginInit();
                     image.UriSource = new Uri("pack://application:,,/Images/Image.png");
                     image.EndInit();
                     imgProduct.Source = image;
                     imgProduct.Tag = null;
                 });

            });
            getTypes.Start();
        }

        public NewProductPage(Detail_Product1 product)
        {
            InitializeComponent();
            isEdit = true;
            imgProduct.Tag = product.Image_Path;
            Title.Content = "Edit product";
            ProductName.Text = product.NameProduct;
            ProductId.Text = product.ID_Product;
            ProductDescription.Text = product.Description_Pro;
            ProductPrice.Text = product.Original_Price.ToString();
            ProductCapital.Text = "0";           
            ProductDate.Text = manage.GetDateImport(product).ToString();
            ProductInitialAmount.Text = "0";
            if (product.Image_Path != null)
            {
                BitmapImage source = new BitmapImage(new Uri(product.Image_Path));
                imgProduct.Source = source;
            }
            Thread getTypes = new Thread(delegate ()
            {
                var Type = manage.getListTypeName();
                var Supplier = manage.getListSupplierName();
                Dispatcher.Invoke(() =>
                {
                    comboProductTypes.ItemsSource = Type;
                    comboboxSupplier.ItemsSource = Supplier;
                });
               
                for (int i = 0; i < Type.Count; i++)
                {
                    if (Type[i]== manage.GetType(product))
                    {
                        Dispatcher.Invoke(() =>
                        {

                            comboProductTypes.SelectedIndex = i;

                        });
                        break;
                    }
                }             
                    
                for (int i = 0; i < Supplier.Count; i++)
                {
                    if (Supplier[i] == manage.GetSupplier(product)) 
                    {
                        Dispatcher.Invoke(() =>
                        {
                            comboboxSupplier.SelectedIndex = i;

                        });
                        break;
                    }
                }
            });
            getTypes.Start();

        }

        private void BtnAddImageProduct_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (true == openFileDialog.ShowDialog())
            {
                string filename = openFileDialog.FileName;
                try
                {
                    BitmapImage source = new BitmapImage(new Uri(filename));
                    imgProduct.Source = source;
                    imgProduct.Tag = filename;
                }
                catch 
                {
                    var dialogError = new Dialog() { Message = "Tập tin ảnh không hợp lệ" };
                    dialogError.Owner = Window.GetWindow(this);
                    dialogError.ShowDialog();
                    return;
                }
            }
        }

        private void BtnRefesh_Click(object sender, RoutedEventArgs e)
        {
            ProductName.Clear();
            ProductPrice.Clear();
            ProductDescription.Clear();
            ProductDate.Text = null;
            ProductInitialAmount.Clear();
            ProductCapital.Clear();
            comboProductTypes.SelectedIndex = -1;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,/Images/Image.png");
            image.EndInit();
            imgProduct.Source = image;
            imgProduct.Tag = null;
        }

        //kiểm tra thôn tin trước khi save
        int checkTextbox()
        {
            if (ProductId.Text=="" || ProductName.Text=="" || ProductInitialAmount.Text==""||
                ProductPrice.Text==""|| ProductDate.Text==""|| ProductDescription.Text==""|| ProductCapital.Text==""||
                comboProductTypes.SelectedIndex==-1)
            {
                var dialogError1 = new Dialog() { Message = "Vui lòng nhập đầy đủ các thông tin" };
                dialogError1.Owner = Window.GetWindow(this);
                dialogError1.ShowDialog();
                return 1;
            }

            return 0;
        }

        

        private void BtnAddProductSave_Click(object sender, RoutedEventArgs e)
        {
            if (checkTextbox() == 0)
            {
                var dialogError = new Dialog() { Message = isEdit ? "Xác nhận sửa sản phẩm?" : "Thêm mới sản phẩm đã nhập?" };
                dialogError.Owner = Window.GetWindow(this);
                if (dialogError.ShowDialog() == true)
                {
                    try
                    {
                        manage.AddProduct(isEdit, ProductId.Text, comboProductTypes.Text,comboboxSupplier.Text, DateTime.Parse(ProductDate.Text), ProductName.Text, int.Parse( ProductPrice.Text),int.Parse(ProductInitialAmount.Text), ProductDescription.Text, imgProduct.Source.ToString());
                        if (RefreshProductList != null)
                        {
                            RefreshProductList.Invoke(true);
                        }
                        
                    }                    
                    catch (Exception r)
                    {
                        MessageBox.Show(r.Message);
                    }

                    Detail_Product1 a = new Detail_Product1();
                    a.ID_Product = ProductId.Text;
                    a.NameProduct = ProductName.Text;
                    a.ID_Supplier = manage.GetIDSupplier(comboboxSupplier.Text);
                    a.Description_Pro=  ProductDescription.Text;
                    a.Original_Price = Int32.Parse(ProductPrice.Text);
                    a.ID_TypeProduct = manage.GetIDType(comboProductTypes.Text);

                    if (imgProduct.Source != null)
                    {
                        BitmapImage source = new BitmapImage(new Uri(imgProduct.Source.ToString()));
                        a.Image_Path = source.ToString();
                    }
                    else a.Image_Path = null;

                    var page = new DetailProductPage(a);
                    page.refresh(true);
                    page.RefreshProductList = refreshCombo;
                    NavigationService.Navigate(page);

                }
            }

        }

        private void BtnAddProductType_Click(object sender, RoutedEventArgs e)
        {
            var typeW = new ProductTypePage();
            typeW.refreshCombobox = refreshCombo;
            NavigationService.Navigate(typeW);
        }


        public void refreshCombo(bool Data)
        {

            if (Data) // Nếu có chỉnh sửa danh sách loại sản phẩm thì refresh combo
            {
                int oldIndexType = comboProductTypes.SelectedIndex;
                int oldIndexSup = comboboxSupplier.SelectedIndex;
                // Get và hiển thị danh sách loại sản phẩm
                Thread getPTypes = new Thread(delegate ()
                {
                    //var db = new MANAGEMENT_STORE_Entities();
                    var productTypes = manage.getListTypeName();
                    var sup = manage.getListSupplierName();
                    Dispatcher.Invoke(() =>
                    {
                        comboProductTypes.ItemsSource = productTypes; // Tác động lên UI
                        comboboxSupplier.ItemsSource = sup;
                        if (oldIndexType > 0) comboProductTypes.SelectedIndex = oldIndexType;
                        if (oldIndexSup > 0) comboboxSupplier.SelectedIndex = oldIndexSup;
                        // Cập nhật tiếp trang ở trước
                        if (RefreshProductList != null)
                        {
                            RefreshProductList.Invoke(true);
                        }
                    });
                });
                getPTypes.Start();
            }
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void BtnAddSupplier_Click(object sender, RoutedEventArgs e)
        {
            SupplierPage a = new SupplierPage();
            a.refreshComboboxSup = refreshCombo;
            NavigationService.Navigate(a);
        }
    }
}
