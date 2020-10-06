using System;
using System.Collections.ObjectModel;
using System.Threading;
using Aspose.Cells;
using System.Windows;
using System.Windows.Controls;
using QLStore.BS_layer;
using System.Windows.Navigation;
using QLStore.Class_Converse;
namespace QLStore.Product
{
    /// <summary>
    /// Interaction logic for ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {

        ObservableCollection<Detail_Product1> products;
        Manage_Product dbProduct = new Manage_Product();
        
        public delegate void DelegatePickProductID(Detail_Product1 pro);
        public DelegatePickProductID PickProductID;
        
        public ProductPage()
        {
            InitializeComponent();
            Thread thread = new Thread(delegate ()
            {
                Manage_Product dbProduct = new Manage_Product();
                products = new ObservableCollection<Detail_Product1>(dbProduct.LoadData_Product());

                int Arrangeindex = 0;
                Dispatcher.Invoke(() => { Arrangeindex = comboProductArrange.SelectedIndex; });
                products = dbProduct.Arrange_Product(Arrangeindex);
               
                Dispatcher.Invoke(() =>
                {
                    listviewShowProduct.ItemsSource = products;
                    ProgressBar.IsEnabled = false;
                    ProgressBar.Visibility = Visibility.Hidden;
                });

            });
            thread.Start();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NewProductPage newProduct = new NewProductPage();
            newProduct.RefreshProductList = Refresh;
            NavigationService.Navigate(new NewProductPage());
        }
        public void Refresh( bool data)
        {
            if(data)
            {
                Thread threadt = new Thread(delegate ()
                {
                    Manage_Product dbProduct = new Manage_Product();
                    products = new ObservableCollection<Detail_Product1>(dbProduct.LoadData_Product());

                    int Arrangeindex = 0;
                    Dispatcher.Invoke(() => { Arrangeindex = comboProductArrange.SelectedIndex; });
                    products = dbProduct.Arrange_Product(Arrangeindex);

                    Dispatcher.Invoke(() =>
                    {
                        listviewShowProduct.ItemsSource = products;
                        ProgressBar.IsEnabled = false;
                        ProgressBar.Visibility = Visibility.Hidden;
                    });

                });
                threadt.Start();

            }
        }

        private void ComboProductArrange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Thread thread = new Thread(delegate ()
            {
                Manage_Product dbProduct = new Manage_Product();
                products = new ObservableCollection<Detail_Product1>(dbProduct.LoadData_Product());

                int Arrangeindex = 0;
                Dispatcher.Invoke(() => { Arrangeindex = comboProductArrange.SelectedIndex; });
                products = dbProduct.Arrange_Product(Arrangeindex);

                Dispatcher.Invoke(() =>
                {
                    listviewShowProduct.ItemsSource = products;
                    ProgressBar.IsEnabled = false;
                    ProgressBar.Visibility = Visibility.Hidden;
                });

            });
            thread.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (txbFindProduct.Text == "")
            {
                products = new ObservableCollection<Detail_Product1>(dbProduct.LoadData_Product());
                listviewShowProduct.ItemsSource = products;
            }
            else
            {
                ObservableCollection<Detail_Product1> searchproducts = new ObservableCollection<Detail_Product1>();

                searchproducts = dbProduct.SearchProduct(txbFindProduct.Text);
                if (searchproducts.Count > 0)
                    listviewShowProduct.ItemsSource = searchproducts;
                else
                {
                    var dialog = new Dialog() { Message = "Do not find any product!" };
                    dialog.Owner = Window.GetWindow(this);
                    dialog.ShowDialog();
                }
                txbFindProduct.Text = "";
            }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

       
        private void PackIcon_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".xlsx";
            openFileDialog.Filter = "Excel Workbook (.xlsx)|*.xlsx";

            if (false == openFileDialog.ShowDialog()) return;
            string filename = openFileDialog.FileName;

            var excel = new ImportFromExcel(filename);
            excel.Owner = Window.GetWindow(this);
            excel.SendProduct = Import;
            excel.Show();
        }

        public void Import(ObservableCollection<ImportProduct> Data)
        {
            if (Data != null)
            {                
                for (int i = 0; i < Data.Count; i++)
                {
                    try
                    {
                        dbProduct.AddProduct(false, Data[i].ID, Data[i].Type, Data[i].Supplier, Data[i].input_time, Data[i].Name, Data[i].Orig_price, Data[i].Initial_amount, Data[i].Descrip, Data[i].Image_path);
                        // Tăng số sản phẩm của loại sản phẩm
                        dbProduct.IncreaseTypeAmount(Data[i].ID_Type, Data[i].Curr_amount);
                    }
                    catch (Exception ex)
                    {
                        continue; // Không xảy ra lỗi trùng mã vì đã xử lý trước
                    }
                }
                Refresh(true);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ProgressBar.IsEnabled = true;
            ProgressBar.Visibility = Visibility.Visible;
            Thread thread2 = new Thread(delegate ()
            {
                Manage_Product dbProduct = new Manage_Product();
                products = new ObservableCollection<Detail_Product1>(dbProduct.LoadData_Product());

                int Arrangeindex = 0;
                Dispatcher.Invoke(() => { Arrangeindex = comboProductArrange.SelectedIndex; });
                products = dbProduct.Arrange_Product(Arrangeindex);

                Dispatcher.Invoke(() =>
                {
                    listviewShowProduct.ItemsSource = products;
                    ProgressBar.IsEnabled = false;
                    ProgressBar.Visibility = Visibility.Hidden;
                });

            });
            thread2.Start();
        }

        private void ListviewShowProduct_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Detail_Product1 detail = listviewShowProduct.SelectedItem as Detail_Product1;
            if (detail!=null)
            {
                if( PickProductID!=null)
                {
                    PickProductID.Invoke(detail);
                    if (NavigationService.CanGoBack) NavigationService.GoBack();
                }
                else
                {
                    DetailProductPage detailPage = new DetailProductPage(detail);
                    detailPage.RefreshProductList = Refresh;
                    NavigationService.Navigate(detailPage);
                }
                
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Dialog() { Message = "Xuất dữ liệu ra tập tin Excel?" };
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == false) return;

            // Mở hộp thoại lưu tập tin
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.DefaultExt = ".xlsx";
            saveFileDialog.Filter = "Excel Workbook (.xlsx)|*.xlsx";

            if (false == saveFileDialog.ShowDialog()) return;
            string filename = saveFileDialog.FileName;

            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];
            worksheet.Name = "Product";

            // Ghi các cột
            worksheet.Cells["A1"].PutValue("Product Name");
            worksheet.Cells["B1"].PutValue("Product ID");
            worksheet.Cells["C1"].PutValue("Original Price");
            worksheet.Cells["D1"].PutValue("Date Input");
            worksheet.Cells["E1"].PutValue("Initial Amount");
            worksheet.Cells["F1"].PutValue("Current Amount");
            worksheet.Cells["G1"].PutValue("Capital");
            worksheet.Cells["H1"].PutValue("Description");
            worksheet.Cells["I1"].PutValue("Supplier ID");
            worksheet.Cells["J1"].PutValue("Type ID");
            worksheet.Cells["K1"].PutValue("Image_Path");

            for (int i = 0; i < products.Count; i++)
            {
                worksheet.Cells[$"A{i + 2}"].PutValue(products[i].NameProduct);
                worksheet.Cells[$"B{i + 2}"].PutValue(products[i].ID_Product);
                worksheet.Cells[$"C{i + 2}"].PutValue(products[i].Original_Price);

                DateTime time = dbProduct.GetDateImport(products[i]);
                worksheet.Cells[$"D{i + 2}"].PutValue(time.ToString());
                int s = dbProduct.GetinitialAmount(products[i]);
                worksheet.Cells[$"E{i + 2}"].PutValue(s.ToString());
                worksheet.Cells[$"F{i + 2}"].PutValue(products[i].Amount_Current);
               
                worksheet.Cells[$"G{i + 2}"].PutValue("awr");
                worksheet.Cells[$"H{i + 2}"].PutValue(products[i].Description_Pro);
                worksheet.Cells[$"I{i + 2}"].PutValue(products[i].ID_Supplier);
                worksheet.Cells[$"J{i + 2}"].PutValue(products[i].ID_TypeProduct);
                worksheet.Cells[$"H{i + 2}"].PutValue(products[i].Image_Path);
            }

            // Lưu lại
            worksheet.AutoFitColumns();
            workbook.Save(filename);
        }
    }

   
}
