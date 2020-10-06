using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
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
using QLStore.Class_Converse;
using System.Collections.ObjectModel;

namespace QLStore.Supplier_
{
    /// <summary>
    /// Interaction logic for SupplierPage.xaml
    /// </summary>
    public partial class SupplierPage : Page
    {
        public delegate void DelegateRefreshListSuplier(bool data);
        public DelegateRefreshListSuplier refreshComboboxSup;

        Manage_Product manage = new Manage_Product();
        public SupplierPage()
        {
            InitializeComponent();
            setTextBox(false);
            btnDelete.IsEnabled = false;
            btnEdit.IsEnabled = false;
            listSupplier.IsEnabled = true;
            btnAdd.IsEnabled = true;
            Thread thread = new Thread(delegate ()
            {
                var db = new Manage_Product();
                var suplier = new ObservableCollection<Supplier1>(db.LoadData_Supplier());

                Dispatcher.Invoke(() =>
                {
                    listSupplier.ItemsSource = suplier;

                });

            });
            thread.Start();

        }

        void setTextBox( bool t)
        {
            txbAddress.IsEnabled = txbEmail.IsEnabled = 
                txbIdSup.IsEnabled = txbNameSup.IsEnabled = 
                txbPhone.IsEnabled = txbMoreInfo.IsEnabled =t;
        }

        bool CheckInput()
        {
            if (txbAddress.Text.Length == 0 || txbEmail.Text.Length == 0
                || txbNameSup.Text.Length == 0 || txbMoreInfo.Text.Length == 0
                 ||txbPhone.Text.Length == 0 )
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

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (listSupplier.Items.Count > 0)
                listSupplier.ScrollIntoView(listSupplier.Items[0]);
            txbAddress.Clear();
            txbIdSup.Clear();
            txbEmail.Clear();
            txbMoreInfo.Clear();
            txbNameSup.Clear();
            txbPhone.Clear();
            btnAdd.IsEnabled = false;
            btnDelete.IsEnabled = true;
            IconDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.ContentSave;
            btnDelete.ToolTip = "Save";
            btnEdit.IsEnabled = true;
            IconEdit.Kind = MaterialDesignThemes.Wpf.PackIconKind.Cancel;
            setTextBox(true);
            txbIdSup.IsEnabled = false;
            txbIdSup.Text = manage.Create_NewIdSupplier_Auto();
            listSupplier.IsEnabled = false;

            txbIdSup.Focus();

            if (refreshComboboxSup != null)
            {
                refreshComboboxSup.Invoke(true);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (IconDelete.Kind == MaterialDesignThemes.Wpf.PackIconKind.DeleteCircle)
            {
                Dialog a = new Dialog()
                {
                    Message = "Are you sure to save ?"
                };
                a.Owner = Window.GetWindow(this);
                if (a.ShowDialog() == false) return;
                Dialog b = new Dialog()
                {
                    Message = "If you delete this supplier, you will delete \n all product of this supplier.  Are you sure?"
                };
                b.Owner = Window.GetWindow(this);
                if (b.ShowDialog() == false) return;
                Supplier1 sup = listSupplier.SelectedItem as Supplier1;
                manage.DeleteSupplier(sup);
                listSupplier.ItemsSource = new ObservableCollection<Supplier1>(manage.LoadData_Supplier());
                listSupplier.SelectedIndex = -1;
                //xóa các textbox
                txbAddress.Clear();
                txbIdSup.Clear();
                txbEmail.Clear();
                txbMoreInfo.Clear();
                txbNameSup.Clear();
                txbPhone.Clear();
                btnDelete.IsEnabled = false;
                btnEdit.IsEnabled = false;

                if (refreshComboboxSup != null)
                    refreshComboboxSup.Invoke(true);
            }            
            else
            if (IconDelete.Kind == MaterialDesignThemes.Wpf.PackIconKind.ContentSave)
            {
                if (CheckInput())
                {
                    Dialog b = new Dialog()
                    {
                        Message = "Are you sure to add this Supplier?"

                    };
                    b.Owner = Window.GetWindow(this);
                    if (b.ShowDialog() == true)
                    {
                        btnAdd.IsEnabled = true;
                        manage.AddnewSupplier(txbIdSup.Text, txbNameSup.Text, txbAddress.Text, txbPhone.Text, txbEmail.Text, txbMoreInfo.Text);
                        listSupplier.ItemsSource = new ObservableCollection<Supplier1>(manage.LoadData_Supplier());
                        IconDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.DeleteCircle;
                        btnDelete.ToolTip = "Delete Supplier";
                        IconEdit.Kind = MaterialDesignThemes.Wpf.PackIconKind.Edit;
                        listSupplier.SelectedIndex = listSupplier.Items.Count - 1;
                        txbIdSup.Clear();
                        txbAddress.Clear();
                        txbIdSup.Clear();
                        txbEmail.Clear();
                        txbMoreInfo.Clear();
                        txbNameSup.Clear();
                        txbPhone.Clear();
                        listSupplier.IsEnabled = true;
                        setTextBox(false);

                    }

                }
            }

        }

        private void ListSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listSupplier.SelectedItem == null) return;

            Supplier1 a = listSupplier.SelectedItem as Supplier1;

            txbIdSup.Text =a.ID_sup;
            txbNameSup.Text = a.Name_Sup;
            txbAddress.Text = a.Address_sup;
            txbEmail.Text = a.Email;
            txbMoreInfo.Text = a.MoreInfo;
            txbPhone.Text = a.Phone;

            btnDelete.IsEnabled = true;
            btnEdit.IsEnabled = true;

        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (IconEdit.Kind == MaterialDesignThemes.Wpf.PackIconKind.Edit)
            {
                setTextBox(true);
                txbIdSup.IsEnabled = false;
                IconEdit.Kind = MaterialDesignThemes.Wpf.PackIconKind.ContentSave;
                btnEdit.ToolTip = "Save change";
                btnAdd.IsEnabled = false;
                listSupplier.IsEnabled = false;

                IconDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.Cancel;
                btnDelete.ToolTip = "Cancel change";
            }
            else
            if (IconEdit.Kind == MaterialDesignThemes.Wpf.PackIconKind.ContentSave)
            {
                Dialog a = new Dialog()
                {
                    Message = "Are you sure to save ?"
                };
                a.Owner = Window.GetWindow(this);
                if (a.ShowDialog() == true)
                {
                    Supplier1 pro = listSupplier.SelectedItem as Supplier1;
                    manage.EditSupplier(txbIdSup.Text, txbNameSup.Text, txbAddress.Text, txbPhone.Text, txbEmail.Text, txbMoreInfo.Text);
                    listSupplier.ItemsSource = new ObservableCollection<Supplier1>(manage.LoadData_Supplier());
                    IconDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.DeleteCircle;
                    btnDelete.ToolTip = "Delete";
                    listSupplier.IsEnabled = true;
                    IconEdit.Kind = MaterialDesignThemes.Wpf.PackIconKind.Edit;
                    btnEdit.ToolTip = "Edit";
                    listSupplier.SelectedIndex = listSupplier.Items.Count - 1;
                    setTextBox(false);
                    btnAdd.IsEnabled = true;

                    if (refreshComboboxSup != null)
                    {
                        refreshComboboxSup.Invoke(true);
                    }
                }
            }
            else
            if (IconEdit.Kind == MaterialDesignThemes.Wpf.PackIconKind.Cancel)
            {
                Dialog a = new Dialog()
                {
                    Message = "Are you sure to cancel ?"
                };
                a.Owner = Window.GetWindow(this);
                if (a.ShowDialog() == true)
                {
                    setTextBox(false);
                    listSupplier.IsEnabled = true;
                    IconDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.DeleteCircle;
                    btnDelete.ToolTip = "Delete";
                    btnDelete.IsEnabled = false;
                    btnAdd.IsEnabled = true;
                    IconEdit.Kind = MaterialDesignThemes.Wpf.PackIconKind.Edit;
                    btnEdit.ToolTip = "Edit";
                    btnEdit.IsEnabled = false;
                    listSupplier.SelectedIndex = -1;

                }
                else
                {
                    txbNameSup.Focus();
                }


            }
        }
    }
}
