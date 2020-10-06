using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using QLStore.Supplier_;
using QLStore.BS_layer;
using QLStore.Class_Converse;
namespace QLStore.Product
{
    /// <summary>
    /// Interaction logic for ProductTypePage.xaml
    /// </summary>
    public partial class ProductTypePage : Page
    {

        public delegate void DelegateRefreshListTypeproduct(bool data);
        public DelegateRefreshListTypeproduct refreshCombobox;

        Manage_Product manage = new Manage_Product();

        public ProductTypePage()
        {
            InitializeComponent();
            TextboxSet(false);
            
            btnDelete.IsEnabled = false;
            btnEditProduct.IsEnabled = false;
            Thread thread = new Thread(delegate ()
            {
            var db = new Manage_Product();
            var producttype = new ObservableCollection<Type_product1>(db.Load_ProductType());
            

                Dispatcher.Invoke(() =>
                {
                    listProductType.ItemsSource = producttype;

                });

              });
            thread.Start();
            
        }
        // Thêm loại sản phẩm
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (listProductType.Items.Count > 0)
                listProductType.ScrollIntoView(listProductType.Items[0]);

            btnAdd.IsEnabled = false;
            btnDelete.IsEnabled = true;
            IconDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.ContentSave;
            btnDelete.ToolTip = "Save Product";
            btnEditProduct.IsEnabled = true;
            IconEdit.Kind = MaterialDesignThemes.Wpf.PackIconKind.Cancel;
            txbDescrip.Clear();
            txbIdtype.Clear();
            txbNameType.Clear();
            listProductType.IsEnabled = false;
            TextboxSet(true);

            txbIdtype.Focus();

            if(refreshCombobox!=null)
            {
                refreshCombobox.Invoke(true);
            }

        }

        void TextboxSet (bool t)
        {
            txbIdtype.IsEnabled = t;
            txbNameType.IsEnabled = t;
            txbDescrip.IsEnabled = t;
        }

        bool CheckInput()
        {
            if (txbDescrip.Text.Length == 0 || txbIdtype.Text.Length == 0 || txbNameType.Text.Length == 0)
            {
                Dialog a = new Dialog()
                {
                    Message = "You have to fill all textbox."
                };
                a.Owner = Window.GetWindow(this);
                return false;
            }
            return true;

        }
        //Save edit or delete new type
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (IconDelete.Kind == MaterialDesignThemes.Wpf.PackIconKind.DeleteCircle)
            {
                Dialog a = new Dialog()
                {
                    Message = "Are you sure to delete this Type product?"
                };
                a.Owner = Window.GetWindow(this);
                if (a.ShowDialog() == false) return;
                Type_product1 pro = listProductType.SelectedItem as Type_product1;
                manage.DeleteTypeProduct(pro.ID);
                listProductType.ItemsSource = new ObservableCollection<Type_product1>(manage.Load_ProductType());
                listProductType.SelectedIndex = -1;
                //xóa các textbox
                txbDescrip.Clear();
                txbIdtype.Clear();
                txbNameType.Clear();
                btnDelete.IsEnabled = false;
                btnEditProduct.IsEnabled = false;

                if (refreshCombobox != null)
                    refreshCombobox.Invoke(true);
            }
            
            if (IconDelete.Kind== MaterialDesignThemes.Wpf.PackIconKind.ContentSave)
            {
                if (CheckInput())
                {
                    Dialog b = new Dialog()
                    {
                        Message = "Are you sure to add this type product?"

                    };
                    b.Owner = Window.GetWindow(this);
                    if (b.ShowDialog() == true)
                    {
                        btnAdd.IsEnabled = true;
                        manage.AddNewTypeproduct(txbIdtype.Text, txbNameType.Text);
                        listProductType.ItemsSource = new ObservableCollection<Type_product1>(manage.Load_ProductType());
                        IconDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.DeleteCircle;
                        btnDelete.ToolTip = "Delete product";
                        IconEdit.Kind = MaterialDesignThemes.Wpf.PackIconKind.Edit;
                        listProductType.SelectedIndex = listProductType.Items.Count - 1;
                        listProductType.IsEnabled = true;
                        TextboxSet(false);

                    }

                }
            }
            
        }

        private void ListProductType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listProductType.SelectedItem == null) return;

            Type_product1 a = listProductType.SelectedItem as Type_product1;

            txbIdtype.Text = a.ID;
            txbNameType.Text = a.Type_Product_;
            txbDescrip.Text = "null";

            btnDelete.IsEnabled = true;
            btnEditProduct.IsEnabled = true;
                       
        }

        private void BtnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            if(IconEdit.Kind== MaterialDesignThemes.Wpf.PackIconKind.Edit)
            {
                txbDescrip.IsEnabled = txbNameType.IsEnabled = true;
                txbIdtype.IsEnabled = false;
                IconEdit.Kind = MaterialDesignThemes.Wpf.PackIconKind.ContentSave;
                btnEditProduct.ToolTip = "Save change";
                btnAdd.IsEnabled = false;
                listProductType.IsEnabled = false;

                IconDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.Cancel;
                btnDelete.ToolTip = "Cancel change";
            }
            else
            if(IconEdit.Kind == MaterialDesignThemes.Wpf.PackIconKind.ContentSave)
            {
                Dialog a = new Dialog()
                {
                    Message = "Are you sure to save ?"
                };
                a.Owner = Window.GetWindow(this);
                if (a.ShowDialog() == true)
                {
                    Type_product1 pro = listProductType.SelectedItem as Type_product1;
                    manage.EditProduct(pro.ID, txbNameType.Text);
                    listProductType.ItemsSource= new ObservableCollection<Type_product1>(manage.Load_ProductType());
                    IconDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.DeleteCircle;
                    btnDelete.ToolTip = "Delete product";
                    listProductType.IsEnabled = true;
                    IconEdit.Kind = MaterialDesignThemes.Wpf.PackIconKind.Edit;
                    btnEditProduct.ToolTip = "Edit product";
                    listProductType.SelectedIndex = listProductType.Items.Count - 1;
                    txbDescrip.IsEnabled = txbIdtype.IsEnabled = txbNameType.IsEnabled = false;
                    btnAdd.IsEnabled = true ;

                    if (refreshCombobox != null)
                    {
                        refreshCombobox.Invoke(true);
                    }
                }
            }
            else
            if(IconEdit.Kind == MaterialDesignThemes.Wpf.PackIconKind.Cancel)
            {
                Dialog a = new Dialog()
                {
                    Message = "Are you sure to cancel ?"
                };
                a.Owner = Window.GetWindow(this);
                if (a.ShowDialog()==true)
                {
                    txbDescrip.IsEnabled = txbIdtype.IsEnabled = txbNameType.IsEnabled = false;
                    listProductType.IsEnabled = true;
                    IconDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.DeleteCircle;
                    btnDelete.ToolTip = "Delete product";
                    btnDelete.IsEnabled = false;
                    btnAdd.IsEnabled = true;
                    IconEdit.Kind = MaterialDesignThemes.Wpf.PackIconKind.Edit;
                    btnEditProduct.ToolTip = "Edit product";
                    btnEditProduct.IsEnabled = false;
                    listProductType.SelectedIndex = -1;

                }
                else
                {
                    txbIdtype.Focus();
                }
            }
        }

      
    }
}
