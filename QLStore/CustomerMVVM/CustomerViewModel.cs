using QLStore.Background;
using QLStore.BS_layer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using QLStore.Class_Converse;

namespace QLStore.CustomerMVVM
{
   public  class CustomerViewModel:baseViewModel
    {
        private ObservableCollection<Customer1> listCustomers;
        private Customer1 customerItem = new Customer1();

       

        public ObservableCollection<Customer1> ListCustomers
        {
            get
            {
                // var db = new Manage_Product();
                // productTypes = new ObservableCollection<Type_product>(db.Load_ProductType());
                Manage_Product manage = new Manage_Product();
                listCustomers = new ObservableCollection<Customer1>(manage.Load_Customer());
                return listCustomers;
            }
            set
            {
                listCustomers = value;
                OnPropertyChanged("ListCustomers");
            }
        }
        public Customer1 CustomerItem
        {
            get => customerItem;
            set
            {
                customerItem = (Customer1)value;
                // typetItems = (Type_product)value;
                if (customerItem != null)
                {
                    IDItem = customerItem.ID_Customer;
                    NameItem = customerItem.Name_Cus;
                    AddressItem = customerItem.Address_Cus;
                    GmailItem = customerItem.Email;
                    PhoneItem = customerItem.Phone;
                    BirthDay = customerItem.Birthday.Value;

                }
                OnPropertyChanged("CustomerItem");
            }
        }

        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand CancelCommand { get; set; }


        private bool isEnable = false; //Enable for textBox and Add button, because Button Add just enable or able
        private bool isNotAddnew = false;   // this parameter just for txtIDType
        private bool isSelectedItem = true;  // For AddBtn and ListView Selected

        private int selectedIndex = -1;

        private string iDItem = "";
        private string nameItem = "";
        private string addressItem = "";
        private string phoneItem = "";
        private string gmailItem = "";
        private DateTime birthDay = DateTime.Now;

        private int amountItem = 0;
        public ICommand AddICommand { get; set; }
        public ICommand SaveICommand { get; set; }
        public ICommand EditICommand { get; set; }
        public ICommand DeleteICommand { get; set; }
        public ICommand CancelICommand { get; set; }



        public CustomerViewModel()
        {
            ListCustomers = new ObservableCollection<Customer1>();
            //CustomerItem = new CustomerMVVM();
            reset();

            Thread thread = new Thread(delegate ()
            {
                //  var db = new Manage_Product();
                Manage_Product manage = new Manage_Product();
                listCustomers = new ObservableCollection<Customer1>(manage.Load_Customer());
            });
            thread.Start();

            AddICommand = new RelayCommand<UserControl>((p) =>  // Execute and Can Execute
            {
                if (IsEnable) return false;
                return true;
            }, (p) =>
            {

                IsEnable = true;
                IsNotAddnew = true;  // Add new Product, can access IDtxtbox
                IsSelectedItem = false;
                SelectedIndex = -1;

                // Data
                IDItem = "";
                NameItem = "";
                AddressItem = "";
                GmailItem = "";
                PhoneItem = "";
                BirthDay = DateTime.Now;




            });


            DeleteICommand = new RelayCommand<UserControl>((p) =>  // Execute and Can Execute
            {
                if (customerItem == null) return false;
                return true;
            }, (p) =>
            {
                Dialog a = new Dialog()
                {
                    Message = "Are you sure to delete this Type product?"
                };
                if (a.ShowDialog() == true)
                {
                    Customer1 cus = (Customer1)CustomerItem;

                    //  manage.DeleteTypeProduct(pro.ID);
                    Manage_Product manage = new Manage_Product();
                    manage.DeleteCustomer(cus.ID_Customer);

                    ListCustomers = new ObservableCollection<Customer1>(manage.Load_Customer());

                    SelectedIndex = -1; // Reset listView
                    //resetGUI
                    reset();

                }



            });
            EditICommand = new RelayCommand<UserControl>((p) =>  // Execute and Can Execute
            {
                if (customerItem == null) return false;
                return true;
            }, (p) =>
            {

                IsEnable = true;

            });
            CancelICommand = new RelayCommand<UserControl>((p) =>  // Execute and Can Execute
            {

                return true;
            }, (p) =>
            {
                Dialog a = new Dialog()
                {
                    Message = "Are you sure to cancel ?"
                };

                if (a.ShowDialog() == true)
                {

                    reset();
                    SelectedIndex = -1;

                }
            });
            SaveICommand = new RelayCommand<UserControl>((p) =>  // Execute and Can Execute
            {

                return true;
            }, (p) =>
            {
                if (CheckInput())
                {
                    Dialog b = new Dialog()
                    {
                        Message = "Are you sure to add this type product?"
                    };

                    if (b.ShowDialog() == true)
                    {

                        Manage_Product manage = new Manage_Product();
                        manage.AddNewCustomer(IDItem, NameItem, AddressItem, PhoneItem, GmailItem, BirthDay);


                        ListCustomers = new ObservableCollection<Customer1>(manage.Load_Customer());

                        reset();
                        SelectedIndex = -1;


                    }

                }


            });

        }
        #region Validate Function
        void reset()
        {
            //Display
            IsEnable = false;
            IsNotAddnew = false;
            IsSelectedItem = true;

            // Data
            IDItem = "";
            NameItem = "";
            AddressItem = "";
            GmailItem = "";
            PhoneItem = "";
            BirthDay = DateTime.Now;

        }
        bool CheckInput()
        {
            if (NameItem == "" || IDItem == "" || PhoneItem == "")
            {
                Dialog a = new Dialog()
                {
                    Message = "You have to fill all textbox. "
                };
                a.ShowDialog();
                return false;
            }
            return true;

        }
        #endregion

        #region enable- visible GUI

        public bool IsEnable
        {
            get => isEnable;
            set
            {
                isEnable = value;
                OnPropertyChanged("IsEnable");
            }
        }


        public int AmountItem
        {
            get => amountItem;
            set
            {
                amountItem = value;
                OnPropertyChanged("AmountItem");
            }
        }

        public bool IsNotAddnew
        {
            get => isNotAddnew;
            set
            {
                isNotAddnew = value;

                OnPropertyChanged("IsNotAddnew");
                 OnPropertyChanged("VisibleAdd");
            }
        }



        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        public bool IsSelectedItem
        {
            get => isSelectedItem;
            set
            {
                isSelectedItem = value;
                OnPropertyChanged("IsSelectedItem");
            }
        }
        #endregion 
        #region Binding parameter
        public string IDItem
        {
            get => iDItem;
            set
            {
                iDItem = value;
                OnPropertyChanged("IDItem");
            }
        }
        public string NameItem
        {
            get => nameItem;
            set
            {
                nameItem = value;
                OnPropertyChanged("NameItem");
            }
        }

        public string AddressItem
        {
            get => addressItem;
            set
            {
                addressItem = value;
                OnPropertyChanged("AddressItem");
            }
        }
        public string PhoneItem
        {
            get => phoneItem;
            set
            {
                phoneItem = value;
                OnPropertyChanged("PhoneItem");
            }
        }
        public string GmailItem
        {
            get => gmailItem;
            set
            {
                gmailItem = value;
                OnPropertyChanged("GmailItem");
            }

        }

        public DateTime BirthDay
        {
            get => birthDay;
            set
            {
                birthDay = value;
                OnPropertyChanged("BirthDay");
            }
        }
        #endregion
    }
    public class IConverterOppositeAddButton : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isHidden = (bool)value;
            if (isHidden)  //HasEdit => change it to savechange and cancel
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IConverterWithAddButton : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isHidden = (bool)value;
            if (isHidden)  //HasEdit => change it to savechange and cancel
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
