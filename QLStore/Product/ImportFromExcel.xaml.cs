using System;
using System.Collections.ObjectModel;
using Aspose.Cells;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QLStore.Class_Converse;
using QLStore.BS_layer;
namespace QLStore.Product
{
    /// <summary>
    /// Interaction logic for ImportFromExcel.xaml
    /// </summary>
    public partial class ImportFromExcel : Window
    {
       // MANAGEMENT_STORE_Entities db = new MANAGEMENT_STORE_Entities();
        ObservableCollection<Type_product1> productTypes;
        ObservableCollection<ImportProduct> import;
        public string filename;
        Manage_Product manage = new Manage_Product();

        // Delegate nhận dữ liệu từ cửa sổ Import
        public delegate void DelegateSendProductType(ObservableCollection<Type_product1> Data);
        public DelegateSendProductType SendProductType;
        public delegate void DelegateSendProduct(ObservableCollection<ImportProduct> Data);
        public DelegateSendProduct SendProduct;

        public ImportFromExcel(string _filename)
        {
            filename = _filename;
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Left = Owner.Left + Owner.Width / 2 - Width / 2;
            Top = this.Owner.Top + Owner.Height / 2 - Height / 2;

            Thread thread = new Thread(delegate ()
            {
                // Mở Excel và đọc
                Workbook workbook = new Workbook(filename);
                Worksheet worksheet = workbook.Worksheets[0];
                // Nếu import loại sản phẩm
                if (worksheet.Name.Equals("Product Type"))
                {
                    productTypes = new ObservableCollection<Type_product1>();

                    // Bắt đầu từ hàng thứ 2
                    int i = 2;
                    while (worksheet.Cells[$"B{i}"].Value != null)
                    {
                        // Nếu dữ liệu đã tồn tại thì thôi
                        if (manage.GetIDType(worksheet.Cells[$"B{i}"].Value.ToString()) != null)
                        {
                            i++;
                            continue;
                        }

                        // Kiểm tra tên, ngày có trống không
                        if (worksheet.Cells[$"A{i}"].Value == null
                            || worksheet.Cells[$"C{i}"].Value == null)
                        {
                            i++;
                            continue;
                        }

                        // Tới đây được tức có dữ liệu đã đúng
                        Type_product1 type = new Type_product1()
                        {
                           Type_Product_ = worksheet.Cells[$"A{i}"].Value.ToString(),
                            ID = worksheet.Cells[$"B{i}"].Value.ToString(),
                            Num_Of_Product = Int32.Parse(worksheet.Cells[$"B{i}"].Value.ToString())
                            
                        };
                        productTypes.Add(type);
                        i++;
                    }

                    // Cập nhật UI
                    Dispatcher.Invoke(() =>
                    {
                        listData.ItemsSource = productTypes;
                    });
                }
                // Nếu import sản phẩm
                else if (worksheet.Name.Equals("Product"))
                {
                    import = new ObservableCollection<ImportProduct>();

                    // Bắt đầu từ hàng thứ 2
                    int i = 2;
                    while (worksheet.Cells[$"B{i}"].Value != null)
                    {
                        // Nếu dữ liệu đã tồn tại thì thôi
                        string t = worksheet.Cells[$"B{i}"].Value.ToString();
                        if (manage.GetProduct(t) != null)
                        {
                            i++;
                            continue;
                        }

                        // Nếu loại sản phẩm không tồn tại thì thôi
                        t = worksheet.Cells[$"J{i}"].Value.ToString();
                        if (manage.getType(t) == null)
                        {
                            i++;
                            continue;
                        }

                        t = worksheet.Cells[$"I{i}"].Value.ToString();
                    if (manage.GetSupplier(t) == null)
                        {
                            i++;
                            continue;
                        }

                        // Kiểm tra các cột khác có trống không (trừ MÔ TẢ, LOẠI SP và ẢNH SP)
                        if (worksheet.Cells[$"A{i}"].Value == null
                            || worksheet.Cells[$"C{i}"].Value == null
                            || worksheet.Cells[$"D{i}"].Value == null
                            || worksheet.Cells[$"E{i}"].Value == null
                            || worksheet.Cells[$"F{i}"].Value == null
                            || worksheet.Cells[$"G{i}"].Value == null
                            || worksheet.Cells[$"K{i}"].Value == null)
                        {
                            i++;
                            continue;
                        }

                        // Kiểm tra ngày có đúng định dạng không
                        string date = worksheet.Cells[$"D{i}"].Value.ToString();
                        DateTime dateTime = new DateTime();
                        try
                        {
                            dateTime = DateTime.Parse(date);
                        }
                        catch (Exception ex)
                        {
                            i++;
                            continue;
                        }

                        // Tới đây được tức có dữ liệu đã đúng
                        try
                        {
                            Detail_Product1 product = new Detail_Product1()
                            {
                                NameProduct = worksheet.Cells[$"A{i}"].Value.ToString(),
                                ID_Product = worksheet.Cells[$"B{i}"].Value.ToString(),
                                Original_Price = Int32.Parse(worksheet.Cells[$"C{i}"].Value.ToString()),                               
                                Amount_Current = int.Parse(worksheet.Cells[$"F{i}"].Value.ToString()),
                                Description_Pro = worksheet.Cells[$"H{i}"].Value == null ? null : worksheet.Cells[$"H{i}"].Value.ToString(),
                                ID_TypeProduct = worksheet.Cells[$"J{i}"].Value.ToString(),
                                Image_Path = worksheet.Cells[$"K{i}"].Value == null ? null : worksheet.Cells[$"K{i}"].Value.ToString(),
                                ID_Supplier= worksheet.Cells[$"I{i}"].Value.ToString()


                            };
                            DateTime time = DateTime.Parse(worksheet.Cells[$"D{i}"].Value.ToString());
                            int Capital = Int32.Parse(worksheet.Cells[$"G{i}"].Value.ToString());
                            int Amount_Initial = int.Parse(worksheet.Cells[$"F{i}"].Value.ToString());
                            ImportProduct a = new ImportProduct()
                            {
                                Name = product.NameProduct.ToString(),
                                ID = product.ID_Product,
                                Curr_amount = Int32.Parse(product.Amount_Current.ToString()),
                                Orig_price = Int32.Parse(product.Original_Price.ToString()),
                                Supplier = manage.GetSupplier(product.ID_Supplier),
                                Type = manage.GetType(product),
                                Descrip = product.Description_Pro,
                                ID_Type = product.ID_TypeProduct,
                                ID_Supp = product.ID_Supplier,
                                Image_path = product.Image_Path,
                                input_time = time,
                                capital = Capital,
                                Initial_amount = Amount_Initial
                                
                            };
                            import.Add(a);
                            
                        }
                        catch (Exception) { }
                        i++;
                        continue;
                    }

                    // Cập nhật UI
                    Dispatcher.Invoke(() =>
                    {
                        listData.ItemsSource =import;
                    });
                }

                // Nếu không có dữ liệu nào có thể import thì thông báo
                Dispatcher.Invoke(() =>
                {
                    if (listData.Items.Count == 0) emptyAnnounce.Visibility = Visibility.Visible;
                    ProgressBar.IsEnabled = false;
                    ProgressBar.Visibility = Visibility.Hidden;
                });
            });
            thread.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (SendProductType != null)
            {
                SendProductType.Invoke(productTypes);
            }
            if (SendProduct!= null)
            {
                
                SendProduct.Invoke(import);
            }
            this.Close();
        }
    }
}
