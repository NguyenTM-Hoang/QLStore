using QLStore.Background;
using QLStore.BS_layer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QLStore.Class_Converse;

namespace QLStore.Statistic
{
    public class StatisticViewModel :baseViewModel
    {
        // Binding with circle chart
        private ObservableCollection<Type_product1> listTypeProduct;
        //Binding with column chart- number of product in period
        private ObservableCollection<InstanceStatistic> listInstanceStatistic;
        //Binding with column chart- number product in 
        #region craft- StoreStock
        
        #endregion

        Manage_Product manage=new Manage_Product();

        #region Variable For Binding Model
        // Date to get period for Statistic
        DateTime startDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        // DateTime startDay = new DateTime(2020, 5, 1);
        DateTime endDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        // Date Selected In DateTimePicker
        private DateTime startTimePicker = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        private DateTime endTimePicker = new DateTime(DateTime.Now.Day);

        #region Command
        public ICommand UpdateStatitis { get; set; }
        //public ICommand 
        #endregion

        // private ObservableCollection<InstanceStatistic> listProductInRequire = new ObservableCollection<InstanceStatistic>();

        private bool isChooseTemplateSelected = true;// Here, use to get user choose template Day or selected Day

        private int itemIndexOption = 0;  // Statistic Group by: 1:year, 2: precious, 3: one month
        private string yearFrom = "2019";
        private int itemIndexYear = 0;      // Index of value (instead of use yearFrom, you can you itemIndexYear

        private string timeTo = "0";     // The end of period time for statistic

        private int itemIndexSpecific = 0;  // Specific like month 1, month 2,....

        private ObservableCollection<string> listSpecificYear = new ObservableCollection<string>() { "2019", "2020", "2021", "2022", "2023" };
        private ObservableCollection<string> listSpecificPrecious = new ObservableCollection<string>() { "I", "II", "III", "IV" };
        private ObservableCollection<string> listSpecificMonth = new ObservableCollection<string>() { "Month 1", "Month 2", "Month 3", "Month 4", "Month 5", "Month 6", "Month 7", "Month 8", "Month 9", "Month 10", "Month 11", "Month 12" };

        private ObservableCollection<string> listOption = new ObservableCollection<string>() { "Year", "Precious", "Month" };
        private ObservableCollection<string> listYear = new ObservableCollection<string>() { "2019", "2020", "2021", "2022", "2023" };
        private ObservableCollection<string> listSpecificOptionView;

        // private ObservableCollection<ProductTypeStatistic> listTmpStatitis = new ObservableCollection<ProductTypeStatistic>();
        void LoadInstanceStatistic()
        {
            #region craft
            //listInstanceStatistic = new ObservableCollection<InstanceStatistic>();
            //listInstanceStatistic = new ObservableCollection<InstanceStatistic>();
            //var listOutPut = DataProvider.Ins.DB.Output_Form.Where(
            //    x => x.Output_Date >= startTimePicker
            //    && x.Output_Date <= endDay);
            #endregion
            try
            {
                //  var listProductTmp = DataProvider.Ins.DB.usp_GetProductInDay(startDay, endDay);
                // var listProductTmp= 
                ListInstanceStatistic = manage.getProductPeriod(startDay, endDay);
                ListTypeProduct = manage.getTypeList();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            #endregion

        }


        public StatisticViewModel()
        {
            manage = new Manage_Product();
            ListInstanceStatistic = new ObservableCollection<InstanceStatistic>();
            ListTypeProduct = new ObservableCollection<Type_product1>();
            //listInstanceStatistic = new ObservableCollection<InstanceStatistic>();
            //LoadInstanceStatistic();
            //LoadInstanceStatistic();                                                                                                                                                                                                                                                                  
            #region GetData forItemStock
            // ItemStock = new ObservableCollection<StoreStock>();
            //LoadStoreStock();
            #endregion


            listSpecificOptionView = new ObservableCollection<string>();
            listSpecificOptionView = listSpecificYear;  //List Binding to View

            // listTypeProduct = new ObservableCollection<Type_product>(DataProvider.Ins.DB.Type_product);

            Thread threadTmp = new Thread(delegate ()
            {
                LoadInstanceStatistic();
            });
            threadTmp.Start();
            UpdateStatitis = new RelayCommand<UserControl>((p) => { return true; }, (p) =>
            {
                getDuration();

                // Thread thread = new Thread(delegate ()
                // {
                LoadInstanceStatistic();
                //  });

                #region GetCombobox
                //  thread.Start();

                #endregion
              //  MessageBox.Show(ListInstanceStatistic.Count + "   " + ListTypeProduct.Count);
            });

        }

        public ObservableCollection<string> ListOption
        {
            get
            {
                return listOption;
            }
            set { listOption = value; }
        }
        public ObservableCollection<string> ListYear { get => listYear; set => listYear = value; }
        public ObservableCollection<string> ListSpecificOptionView
        {
            get
            {
                return listSpecificOptionView;
            }
            set
            {
                listSpecificOptionView = value;
                OnPropertyChanged();
            }
        }
        public int ItemIndexOption
        {
            get => itemIndexOption;
            set
            {

                itemIndexOption = value;
                if (itemIndexOption == 2)
                {
                    listSpecificOptionView = listSpecificMonth;
                    OnPropertyChanged("ItemIndexOption");
                    OnPropertyChanged("ListSpecificOptionView");
                }
                else if (itemIndexOption == 1)
                {
                    listSpecificOptionView = listSpecificPrecious;
                    OnPropertyChanged("ItemIndexOption");
                    OnPropertyChanged("ListSpecificOptionView");
                }
                else
                {
                    listSpecificOptionView = listSpecificYear;
                    OnPropertyChanged("ItemIndexOption");
                    OnPropertyChanged("ListSpecificOptionView");
                    //    MessageBox.Show(StartTimePicker.ToString());
                }
            }
        }

        public string YearFrom { get => yearFrom; set => yearFrom = value; }

        public bool IsChooseTemplateSelected { get => isChooseTemplateSelected; set => isChooseTemplateSelected = value; }
        public DateTime StartTimePicker { get => startTimePicker; set => startTimePicker = value; }
        public DateTime EndTimePicker { get => endTimePicker; set => endTimePicker = value; }
        public int ItemIndexSpecific { get => itemIndexSpecific; set => itemIndexSpecific = value; }
        public int ItemIndexYear { get => itemIndexYear; set => itemIndexYear = value; }
        public string TimeTo { get => timeTo; set => timeTo = value; }
        public ObservableCollection<Type_product1> ListTypeProduct
        {
            get => listTypeProduct;
            set
            {
                listTypeProduct = value;
                OnPropertyChanged("ListTypeProduct");
            }
        }
        public ObservableCollection<InstanceStatistic> ListInstanceStatistic
        {
            get => listInstanceStatistic;
            set
            {
                listInstanceStatistic = value;
                OnPropertyChanged("ListInstanceStatistic");

            }
        }

        //public ObservableCollection<ProductTypeStatistic> ListTmpStatitis
        //{
        //    //get => listTmpStatitis;
        //    //set
        //    //{
        //    //    listTmpStatitis = value;
        //    //    OnPropertyChanged("listTmpStatistic");
        //    //}
        //}

        //   public ObservableCollection<InstanceStatistic> ListProductInRequire { get => listProductInRequire; set { listProductInRequire = value; OnPropertyChanged(); } }


        #region Calculate Time for Statistic
        void getDuration()
        {

            if (isChooseTemplateSelected) // Selected Template Period
            {
                switch (itemIndexOption)
                {
                    case 0: // Year
                        {
                            if (string.Compare(YearFrom, TimeTo) > 0)
                            {
                                MessageBox.Show("Input Valid Value");
                                return;
                            }

                            else
                            {
                                startDay = new DateTime(Convert.ToInt32(YearFrom), 1, 1);
                                endDay = new DateTime(Convert.ToInt32(TimeTo), 12, 31);
                            }
                            break;
                        }
                    case 1: //Precious
                        {
                            if (ItemIndexSpecific < 0)
                            {
                                MessageBox.Show("Choose item Now !!!");
                                return;
                            }
                            else
                            {
                                switch (ItemIndexSpecific)
                                {
                                    case 0: // Precious I
                                        {
                                            startDay = new DateTime(Convert.ToInt32(YearFrom), 1, 1);
                                            endDay = new DateTime(Convert.ToInt32(YearFrom), 3, 31);
                                            break;
                                        }
                                    case 1: // Precious II
                                        {
                                            startDay = new DateTime(Convert.ToInt32(YearFrom), 4, 1);
                                            endDay = new DateTime(Convert.ToInt32(YearFrom), 6, 30);
                                            break;
                                        }
                                    case 2: // Precious III
                                        {
                                            startDay = new DateTime(Convert.ToInt32(YearFrom), 7, 1);
                                            endDay = new DateTime(Convert.ToInt32(YearFrom), 9, 30);
                                            break;
                                        }
                                    default: // Precious IV
                                        {
                                            startDay = new DateTime(Convert.ToInt32(YearFrom), 10, 1);
                                            endDay = new DateTime(Convert.ToInt32(YearFrom), 12, 31);
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            int numOfDay = DateTime.DaysInMonth(Convert.ToInt32(YearFrom), ItemIndexSpecific + 1);
                            startDay = new DateTime(Convert.ToInt32(YearFrom), ItemIndexSpecific + 1, 1);
                            endDay = new DateTime(Convert.ToInt32(YearFrom), ItemIndexSpecific + 1, numOfDay);

                            break;
                        }
                }

            }
            else // Selected specific Date by DateTimePicker
            {
                if (startTimePicker == null || endTimePicker == null)
                {
                    MessageBox.Show("Input the Validate Information !!!");
                    return;
                }
                else if (DateTime.Compare(startTimePicker, endTimePicker) > 0) // Invalid Time
                {
                    MessageBox.Show("Input the Validate Information !!!");
                    return;
                }

                startDay = startTimePicker;
                endDay = endTimePicker;

            }
         //   MessageBox.Show(startDay.ToString() + "   " + endDay.ToString());
        }
        #endregion
    }
}