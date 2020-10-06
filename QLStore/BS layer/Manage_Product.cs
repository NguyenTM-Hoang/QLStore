using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Windows;
using QLStore.Database_layer;
using QLStore.Statistic;
using QLStore.Class_Converse;
namespace QLStore.BS_layer
{
    public class Manage_Product
    {
        DBMain DBMain = new DBMain();
       // MANAGEMENT_STORE_Entities db = new MANAGEMENT_STORE_Entities();
        //đã chuyển
        public ObservableCollection<Detail_Product1> LoadData_Product()
        {
            DataTable data = DBMain.ExecuteQueryDataSet("Select * from Detail_Product", CommandType.Text);
            ObservableCollection<Detail_Product1> list = new ObservableCollection<Detail_Product1>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                Detail_Product1 a = new Detail_Product1();
                a.ID_Product = data.Rows[i][0].ToString();
                a.ID_TypeProduct = data.Rows[i][1].ToString();
                a.ID_Supplier = data.Rows[i][2].ToString();
                a.Original_Price = (int)data.Rows[i][3];
                a.NameProduct = data.Rows[i][4].ToString();
                a.Description_Pro = data.Rows[i][5].ToString();
                a.Image_Path = data.Rows[i][6].ToString();
                a.Amount_Current = (int)data.Rows[i][7];
                list.Add(a);
            }
            return list;

        }
        //ok
        public string Create_NewIdproduct_Auto()
        {
            ObservableCollection<Detail_Product1> product = LoadData_Product();
            int count = product.Count();
            string s1 = product[count - 1].ID_Product;
            int s2 = Convert.ToInt32(s1.Remove(0, 4));
            string Id_product;
            if (s2 + 1 < 10)
                Id_product = "Item00" + (s2 + 1).ToString();
            else
                Id_product = "Item0" + (s2 + 1).ToString();
            return Id_product;
        }
        //đc
        public ObservableCollection<Detail_Product1> Arrange_Product(int Arrangeindex)
        {
            ObservableCollection<Detail_Product1> products = LoadData_Product();
            for (int i = 0; i < products.Count - 1; i++)
                for (int j = i + 1; j < products.Count; j++)
                {
                    if (Compare_Product(products[i], products[j], Arrangeindex))
                    {
                        var temp = products[i];
                        products[i] = products[j];
                        products[j] = temp;
                    }
                }
            return products;
        }
        //Đã chuyển
        public bool Compare_Product(Detail_Product1 a, Detail_Product1 b, int i)
        {
            //Nhập kho lâu nhất 
            //timeinput của a
            DataTable t = DBMain.ExecuteQueryDataSet("SELECT *FROM Input_Form Where ID_Product='" + a.ID_Product + "'", CommandType.Text);
            DateTime timea = DateTime.Parse(t.Rows[0][3].ToString());
            int amounta = int.Parse(t.Rows[0][4].ToString());

            DataTable r = DBMain.ExecuteQueryDataSet("SELECT *FROM Input_Form Where ID_Product='" + b.ID_Product + "'", CommandType.Text);
            DateTime timeb = DateTime.Parse(r.Rows[0][3].ToString());
            int amountb = int.Parse(r.Rows[0][4].ToString());

            switch (i)
            {

                case 0: //nhập kho lâu nhất
                    return timea > timeb;

                case 1: //nhập kho gần đây

                    return timea < timeb;
                case 2: //giá tăng dần return true

                    return a.Original_Price > b.Original_Price;

                case 3:// giá giảm dần
                    return a.Original_Price < b.Original_Price;
                case 4: //tồn kho nhiều nhất
                    return a.Amount_Current < b.Amount_Current;
                case 5: //tồn kho ít nhất
                    return a.Amount_Current > b.Amount_Current;
                case 6: //bán chạy nhất                  
                    return (amounta - a.Amount_Current) < (amountb - b.Amount_Current);
                case 7:  //bán ế nhất
                    return (amounta - a.Amount_Current) > (amountb - b.Amount_Current);
                default:
                    return true;
            }

        }

        #region Product add //Đã chuyển
        public void AddProduct(bool isEdit, string ID, string typeName, string Name_sup, DateTime dateTime, string Name, int Price, int amount_init, string descr, string img_Path)
        {

            SqlParameter id = new SqlParameter("@ID", ID);
            string query = "FindIDType";
            DataTable dataTable;
            SqlParameter ID_type = new SqlParameter("@ID_type", DBMain.FindOneValue("FindIDType", CommandType.StoredProcedure, new SqlParameter("@NameType", typeName)));
            //Find id sup
            query = "Select * from Supplier where Name_Sup='" + Name_sup + "'";
            dataTable = DBMain.ExecuteQueryDataSet(query, CommandType.Text);
            SqlParameter ID_sup = new SqlParameter("@ID_sup", dataTable.Rows[0][0].ToString());
            SqlParameter namepro = new SqlParameter("@name", Name);
            SqlParameter price = new SqlParameter("@Org_price", Price);
            SqlParameter descrip = new SqlParameter("@Descrp", descr);
            SqlParameter Image = new SqlParameter("@Image_path", img_Path);
            SqlParameter amount = new SqlParameter("@Amount", amount_init);

            List<SqlParameter> parameters_product = new List<SqlParameter>();
            parameters_product.Add(id);
            parameters_product.Add(ID_sup);
            parameters_product.Add(ID_type);
            parameters_product.Add(namepro);
            parameters_product.Add(price);
            parameters_product.Add(descrip);
            parameters_product.Add(Image);

            // Nếu sửa
            #region Edit
            if (isEdit)
            {

                SqlParameter Input_date = new SqlParameter("@Date", dateTime);
                query = "FindInputID";
                SqlParameter ID_in = new SqlParameter("@ID", DBMain.FindOneValue("FindInputID", CommandType.StoredProcedure, new SqlParameter("@ID_Pro", ID)));
                string ID_input = DBMain.FindOneValue(query, CommandType.StoredProcedure, new SqlParameter("@ID_Pro", ID));
                query = "select * from Input_Form where ID_Input= '" + ID_input + "'";
                //tăng số lượng sp lên
                DataTable b = DBMain.ExecuteQueryDataSet(query, CommandType.Text);
                int num = Int32.Parse(b.Rows[0][4].ToString());
                num += amount_init;
                amount = new SqlParameter("@Amount", num);
                //Update vào input_form
                List<SqlParameter> parameters_Input = new List<SqlParameter>();
                parameters_Input.Add(ID_in);
                parameters_Input.Add(ID_sup);
                parameters_Input.Add(new SqlParameter("@ID_PRO", ID));
                parameters_Input.Add(amount);
                parameters_Input.Add(Input_date);

                DBMain.MyExecuteNonQuery("Update_Input_Form", CommandType.StoredProcedure, parameters_Input);
                //update Sản phẩm
                parameters_product.Add(amount);
                DBMain.MyExecuteNonQuery("Update_Detail_Product", CommandType.StoredProcedure, parameters_product);

                DataTable oldProduct = DBMain.ExecuteQueryDataSet("Select * form Detail_Product where ID='" + ID + "'", CommandType.Text);
                DataTable oldType = DBMain.ExecuteQueryDataSet("Select * form Type_Product where Type_Product='" + typeName + "'", CommandType.Text);
                if (oldProduct.Rows[0][1].ToString() != oldType.Rows[0][0].ToString()) // Nếu có thay đổi mã sản phẩm
                {
                    // Tăng mã mới
                    int t = Int32.Parse(oldType.Rows[0][2].ToString()) + amount_init;
                    SqlParameter NumofType = new SqlParameter("@Number", t);
                    List<SqlParameter> parameters_type = new List<SqlParameter>();
                    parameters_type.Add(ID_type);
                    parameters_type.Add(new SqlParameter("@Name", typeName));
                    parameters_type.Add(NumofType);
                    DBMain.MyExecuteNonQuery("Update_Type_Product", CommandType.StoredProcedure, parameters_type);

                    int oldAmount = int.Parse(oldType.Rows[0][2].ToString()) + amount_init; // Giảm mã cũ

                    NumofType = new SqlParameter("@Number", oldAmount);
                    SqlParameter oldtype_id = new SqlParameter("@ID", oldProduct.Rows[0][1].ToString());
                    List<SqlParameter> parameters_oldtype = new List<SqlParameter>();
                    parameters_oldtype.Add(oldtype_id);
                    parameters_oldtype.Add(new SqlParameter("@Name", oldType.Rows[0][1].ToString()));
                    parameters_oldtype.Add(NumofType);
                    DBMain.MyExecuteNonQuery("Update_Type_Product", CommandType.StoredProcedure, parameters_type);
                }
            }
            #endregion

            // Nếu thêm
            #region Add
            else // Nếu thêm
            {
                //THÊM SẢN PHẨM VÀO BẢNG Detail_product
                query = "AddnewProduct";
                parameters_product.Add(amount);
                DBMain.MyExecuteNonQuery(query, CommandType.StoredProcedure, parameters_product);
                //tự động tạo ID cho Input_form

                DataTable Input_f = DBMain.ExecuteQueryDataSet("Select * from Input_Form", CommandType.Text);
                string id_previous = Input_f.Rows[Input_f.Rows.Count - 1][0].ToString();
                string s1 = id_previous;
                int s2 = Convert.ToInt32(s1.Remove(0, 2));
                string newID;
                if (s2 + 1 < 10)
                    newID = "Ip00" + (s2 + 1).ToString();
                else
                    newID = "Ip0" + (s2 + 1).ToString();
                SqlParameter ID_in = new SqlParameter("@ID", newID);
                SqlParameter Input_date = new SqlParameter("@Input_date", dateTime);


                //Thêm vào input_form
                query = "AddnewInput_Form";
                List<SqlParameter> parameters_Input = new List<SqlParameter>();
                parameters_Input.Add(ID_in);
                parameters_Input.Add(new SqlParameter("@ID_PRO", ID));
                parameters_Input.Add(ID_sup);
                // parameters_Input.Add(ID_type);
                parameters_Input.Add(amount);
                parameters_Input.Add(Input_date);
                DBMain.MyExecuteNonQuery(query, CommandType.StoredProcedure, parameters_Input);

                //Type_product
                string t = DBMain.FindOneValue("FindIDType", CommandType.StoredProcedure, new SqlParameter("@NameType", typeName));
                query = "select * from Type_Product where ID= '" + t + "'";
                DataTable a = DBMain.ExecuteQueryDataSet(query, CommandType.Text);
                int number = Int32.Parse(a.Rows[0][2].ToString()) + amount_init;
                SqlParameter Number = new SqlParameter("@Number", number);

                List<SqlParameter> parameters_Type = new List<SqlParameter>();

                parameters_Type.Add(new SqlParameter("@ID", a.Rows[0][0]));
                parameters_Type.Add(new SqlParameter("@Name", typeName));
                parameters_Type.Add(Number);
                DBMain.MyExecuteNonQuery("Update_Type_Product", CommandType.StoredProcedure, parameters_Type);

            }
            #endregion
            //db.SaveChanges();
        }
        #endregion


        //dùng cho hàm Import từ excel đc
        public void IncreaseTypeAmount(string idType, int amount)
        {
            DataTable type = DBMain.ExecuteQueryDataSet("Select Num_Of_Product where ID='" + idType + "'", CommandType.Text);

            SqlParameter id = new SqlParameter("@ID", idType);
            int k = int.Parse(type.Rows[0][2].ToString()) + amount;
            SqlParameter num = new SqlParameter("@Amount", k);
            List<SqlParameter> t = new List<SqlParameter>();
            t.Add(id);
            t.Add(num);
            DBMain.MyExecuteNonQuery("UpdateAmount_Type", CommandType.StoredProcedure, t);
        }

        //đã chuyển
        public ObservableCollection<Detail_Product1> SearchProduct(string text)
        {
            ObservableCollection<Detail_Product1> products = LoadData_Product();
            ObservableCollection<Detail_Product1> searchproducts = new ObservableCollection<Detail_Product1>();

            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].NameProduct.Contains(text))
                    searchproducts.Add(products[i]);
            }
            return searchproducts;
        }

        //Type product
        //Đã chuyển
        public ObservableCollection<Type_product1> Load_ProductType()
        {
            DataTable data = DBMain.ExecuteQueryDataSet("Select * from Type_product", CommandType.Text);
            ObservableCollection<Type_product1> list = new ObservableCollection<Type_product1>();
            for (int i = 0; i < data.Rows.Count; i++)
            {

                Type_product1 a = new Type_product1();
                a.ID = data.Rows[i][0].ToString();
                a.Type_Product_ = data.Rows[i][1].ToString();
                a.Num_Of_Product = int.Parse(data.Rows[i][2].ToString());

                list.Add(a);
            }
            return list;
        }

        //đã chuyển
        public bool AddNewTypeproduct(string ID, string name)
        {

            SqlParameter Id = new SqlParameter("@ID", ID);
            SqlParameter Name = new SqlParameter("@Typename", name);
            int n = 0;
            SqlParameter Num = new SqlParameter("@Number", n);
            List<SqlParameter> type = new List<SqlParameter>();
            type.Add(Id);
            type.Add(Name);
            type.Add(Num);
            try
            {
                DBMain.MyExecuteNonQuery("AddnewType_Product", CommandType.StoredProcedure, type);


            }
            catch
            {

            }
            return true;
        }
        //Đã chuyển
        public bool DeleteTypeProduct(string id)
        {
            SqlParameter ID = new SqlParameter("@ID", id);
            List<SqlParameter> t = new List<SqlParameter>();
            t.Add(ID);
            try
            {
                DBMain.MyExecuteNonQuery("DeleteType_Product", CommandType.StoredProcedure, t);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

            //Xóa những sản phẩm có 
            DataTable product = DBMain.ExecuteQueryDataSet("Select * from Detail_Product", CommandType.Text);
            if (product.Rows.Count > 0)
            {
                Dialog a = new Dialog()
                {
                    Message = "If you delete, all product of this type will be deleted!"
                };

                if (a.ShowDialog() == false) return false;

                for (int i = 0; i < product.Rows.Count; i++)
                {
                    if (product.Rows[i][1].ToString() == id)
                        DBMain.MyExecuteQuery("DeleteInput_Form", CommandType.StoredProcedure, new SqlParameter("@ID_pro", product.Rows[i][0].ToString()));
                }
                DBMain.MyExecuteQuery("DeleteDetail_Product_Byidtype", CommandType.StoredProcedure, new SqlParameter("@ID_type", id));
            }
            return true;
        }
        //đã chuyển
        public void EditProduct(string IdType, string NameType)
        {
            DataTable a = DBMain.ExecuteQueryDataSet("Select * from Type_product where ID='" + IdType + "'", CommandType.Text);

            SqlParameter ID = new SqlParameter("@ID", IdType);
            SqlParameter Name = new SqlParameter("@Name", IdType);
            SqlParameter num = new SqlParameter("@Number", (int)a.Rows[0][2]);
            List<SqlParameter> t = new List<SqlParameter>();
            t.Add(ID);
            t.Add(Name);
            t.Add(num);
            DBMain.MyExecuteNonQuery("Update_Type_Product", CommandType.StoredProcedure, t);
        
        }

        // Các hàm lấy dữ liệu cho Detail_page  đã chuyển
        #region Các hàm get
        public Detail_Product1 GetProduct(string ID)
        {
            ObservableCollection<Detail_Product1> a = LoadData_Product();
            for (int i = 0; i < a.Count; i++)
                if (a[i].ID_Product == ID) return a[i];
            return null;
        }


        public DateTime GetDateImport(Detail_Product1 product)
        {
            DataTable a = DBMain.ExecuteQueryDataSet("SELECT * FROM Input_Form WHERE ID_Product='" + product.ID_Product + "'", CommandType.Text);
            return DateTime.Parse(a.Rows[0][3].ToString());


        }

        public int GetinitialAmount(Detail_Product1 product)
        {

            DataTable a = DBMain.ExecuteQueryDataSet("SELECT * FROM Input_Form WHERE ID_Product='" + product.ID_Product + "'", CommandType.Text);
            return int.Parse(a.Rows[0][4].ToString());
        }

        //lấy ID type
        public string GetIDType(string nametype)
        {
            //return name type
            DataTable a = DBMain.ExecuteQueryDataSet("SELECT * FROM Type_product WHERE Type_Product='" + nametype+ "'", CommandType.Text);
            return a.Rows[0][0].ToString();
        }

        //Lấy tên type
        public string GetType(Detail_Product1 product)
        {
            //return name type
            DataTable a = DBMain.ExecuteQueryDataSet("SELECT * FROM Type_product WHERE ID='" + product.ID_TypeProduct + "'", CommandType.Text);
            return a.Rows[0][1].ToString();
        }

        public Type_product1 getType(string idProduct)
        {
            ObservableCollection<Type_product1> a = Load_ProductType();
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i].ID == idProduct) return a[i];
            }
            return null;
        }

        public ObservableCollection<string> getListSupplierName()
        {
            ObservableCollection<string> t = new ObservableCollection<string>();
            ObservableCollection<Supplier1> a = LoadData_Supplier();
            for (int i = 0; i < a.Count; i++)
            {
                t.Add(a[i].Name_Sup);
            }
            return t;
        }

        public ObservableCollection<string> getListTypeName()
        {
            ObservableCollection<string> t = new ObservableCollection<string>();
            ObservableCollection<Type_product1> a = Load_ProductType();
            for (int i = 0; i < a.Count; i++)
            {
                t.Add(a[i].Type_Product_);
            }
            return t;
        }
        //đã chuyển
        public ObservableCollection<Type_product1> getTypeList()
        {
            //var temp = db.Type_product;
            ObservableCollection<Type_product1> listTmp = Load_ProductType();
            return listTmp;
        }

        public string GetIDSupplier(string NameSup)
        {

            DataTable a = DBMain.ExecuteQueryDataSet("SELECT * FROM Supplier WHERE Name_Sup='" + NameSup + "'", CommandType.Text);
            return (a.Rows[0][0].ToString());

        }
        //lấy tên sup
        public string GetSupplier(Detail_Product1 product)
        {

            DataTable a = DBMain.ExecuteQueryDataSet("SELECT * FROM Supplier WHERE ID_sup='" + product.ID_Supplier + "'", CommandType.Text);
            return (a.Rows[0][1].ToString());

        }

        public string GetSupplier(string id)
        {

            DataTable a = DBMain.ExecuteQueryDataSet("SELECT * FROM Supplier WHERE ID_sup='" + id + "'", CommandType.Text);
            return (a.Rows[0][1].ToString());

        }

        // mới thêm
        //Bill đã chuyển
        public ObservableCollection<Bills> Load_Bill(string ID_product)
        {
            ObservableCollection<Bills> listbill = new ObservableCollection<Bills>();
            try
            {

                DataTable a = DBMain.ExecuteQueryDataSet("SELECT * FROM Output_Form WHERE ID_Product='" + ID_product + "'", CommandType.Text);
                if (a != null)
                    for (int i = 0; i < a.Rows.Count; i++)
                    {
                        Bills bill = new Bills();
                        bill.CustName = DBMain.FindOneValue("FindNameCustomer", CommandType.StoredProcedure, new SqlParameter("@ID_cus", a.Rows[0][2].ToString()));
                        bill.Date = a.Rows[0][4].ToString();
                        bill.Price_sale = (a.Rows[0][6].ToString());
                        bill.Amountsale = a.Rows[0][3].ToString();
                        listbill.Add(bill);
                        // MessageBox.Show(listbill[count].CustName + "," + listbill[count].Date);
                    }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return listbill;
        }
        //đã chuyển
        public void DeleteProduct(Detail_Product1 product)
        {

            DBMain.MyExecuteQuery("DeleteDetail_Product", CommandType.StoredProcedure, new SqlParameter("@ID", product.ID_Product));
            DBMain.MyExecuteQuery("DeleteInput_Form", CommandType.StoredProcedure, new SqlParameter("@ID_pro", product.ID_Product));

        }
        #endregion

        //Supplier chuyển r
        public ObservableCollection<Supplier1> LoadData_Supplier()
        {
            DataTable data = DBMain.ExecuteQueryDataSet("Select * from Supplier", CommandType.Text);
            ObservableCollection<Supplier1> list = new ObservableCollection<Supplier1>();
            if (data != null)
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    Supplier1 a = new Supplier1();
                    a.ID_sup = data.Rows[i][0].ToString();
                    a.Name_Sup = data.Rows[i][1].ToString();
                    a.Address_sup = data.Rows[i][2].ToString();
                    a.Phone = data.Rows[i][3].ToString();
                    a.Email = data.Rows[i][4].ToString();
                    a.MoreInfo = data.Rows[i][5].ToString();

                    list.Add(a);
                }
                return list;
            }
            return list;

        }
        //DA CHUYEN
        public void AddnewSupplier(string id, string name, string addr, string phone, string email, string more)
        {
            SqlParameter ID = new SqlParameter("@ID", name);
            SqlParameter NAME = new SqlParameter("@Name", name);
            SqlParameter add = new SqlParameter("@Addr", addr);
            SqlParameter phon = new SqlParameter("@Phone", phone);
            SqlParameter mail = new SqlParameter("@Email", email);
            SqlParameter mre = new SqlParameter("@MoreIn", more);
            List<SqlParameter> t = new List<SqlParameter>();
            t.Add(ID);
            t.Add(NAME);
            t.Add(add);
            t.Add(phon);
            t.Add(mail);
            t.Add(mre);
            DBMain.MyExecuteNonQuery("AddnewSupplier", CommandType.StoredProcedure, t);


        }
        //đã chuyển
        public string Create_NewIdSupplier_Auto()
        {
            ObservableCollection<Supplier1> sup = LoadData_Supplier();
            int count = sup.Count();
            string s1 = sup[count - 1].ID_sup;
            int s2 = Convert.ToInt32(s1.Remove(0, 4));
            string Id_sup;
            if (s2 + 1 < 10)
                Id_sup = "SUPP00" + (s2 + 1).ToString();
            else
                Id_sup = "SUPP0" + (s2 + 1).ToString();
            return Id_sup;
        }
        //DA CHUYEN

        public void DeleteSupplier(Supplier1 a)
        {

            DBMain.MyExecuteQuery("DeleteSupplier", CommandType.StoredProcedure, new SqlParameter("@ID", a.ID_sup));
            DBMain.MyExecuteQuery("DeleteDetail_Product_byIdsup", CommandType.StoredProcedure, new SqlParameter("@ID_sup", a.ID_sup));
            DBMain.MyExecuteQuery("DeleteInput_Form_byIdsup", CommandType.StoredProcedure, new SqlParameter("@ID_sup", a.ID_sup));


        }
        //chua chuyen
        public void EditSupplier(string id, string name, string addr, string phone, string email, string more)
        {
            List<SqlParameter> t = new List<SqlParameter>();
            t.Add(new SqlParameter("@id", id));
            t.Add(new SqlParameter("@Name_Sup", name));
            t.Add(new SqlParameter("@Add", addr));
            t.Add(new SqlParameter("@Phone", phone));
            t.Add(new SqlParameter("@Email", email));
            t.Add(new SqlParameter("@More", more));

            DBMain.MyExecuteNonQuery("EditSupplier", CommandType.StoredProcedure, t);

            //mới thêm          
        }
        //da chuuyen

        public ObservableCollection<Output_Form1> Load_Output_Form()
        {
            try
            {
                DataTable data = DBMain.ExecuteQueryDataSet("Select * from Output_Form", CommandType.Text);
                ObservableCollection<Output_Form1> list = new ObservableCollection<Output_Form1>();
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    Output_Form1 a = new Output_Form1();
                    a.ID_Output = data.Rows[i][0].ToString();
                    a.ID_Product = data.Rows[i][1].ToString();
                    a.ID_Customer = data.Rows[i][2].ToString();
                    a.Amount = int.Parse(data.Rows[i][3].ToString());
                    a.Output_Date = DateTime.Parse(data.Rows[i][4].ToString());
                    a.Price_Sale = int.Parse(data.Rows[i][6].ToString());
                    a.Note = data.Rows[i][7].ToString();
                    a.Status = data.Rows[i][8].ToString();
                    a.BuyOnline = data.Rows[i][9].ToString();
                    a.Ship = int.Parse(data.Rows[i][10].ToString());
                    list.Add(a);
                }

                return list;
            }
            catch { return null; }

        }

        //đã chuyển
        public ObservableCollection<Bill_Show> Load_ListBill()
        {
            ObservableCollection<Output_Form1> out_form = Load_Output_Form();
            ObservableCollection<Bill_Show> a = new ObservableCollection<Bill_Show>();
            for (int i = 0; i < out_form.Count; i++)
            {

                Bill_Show n = new Bill_Show();
                n.ID_output = out_form[i].ID_Output;
                n.Namepro = DBMain.FindOneValue("FindNameProduct", CommandType.StoredProcedure, new SqlParameter("@ID_pro", out_form[i].ID_Product));
                n.Initial_price = n.Sale_price = Int32.Parse(out_form[i].Price_Sale.ToString());
                n.Name_Cus = DBMain.FindOneValue("FindNameCustomer", CommandType.StoredProcedure, new SqlParameter("@ID_cus", out_form[i].ID_Customer));
                n.status = out_form[i].Status;
                n.ID_PRO = out_form[i].ID_Product;
                n.Online = out_form[i].BuyOnline;
                n.Amount = (Int32)out_form[i].Amount;
                n.DateCreate = (DateTime)out_form[i].Output_Date;
                n.Phone = DBMain.FindOneValue("FindPhoneCustomer", CommandType.StoredProcedure, new SqlParameter("@ID_cus", out_form[i].ID_Customer));
                n.Ship = (int)out_form[i].Ship;
                n.Address = DBMain.FindOneValue("FindAddressCus", CommandType.StoredProcedure, new SqlParameter("@ID_cus", out_form[i].ID_Customer));
                string t = DBMain.FindOneValue("FindEmailCustomer", CommandType.StoredProcedure, new SqlParameter("@ID_cus", out_form[i].ID_Customer));
                n.Email = t == "" ? "" : t;
                n.birthday = DateTime.Parse(DBMain.FindOneValue("FindBirCustomer", CommandType.StoredProcedure, new SqlParameter("@ID_cus", out_form[i].ID_Customer)));
                //lấy danh sách các Bill để show lên listbill
                a.Add(n);
            }

            return a;
        }

        //Sắp xếp Bill đã chuyển
        public ObservableCollection<Bill_Show> Arrange_ListBill(int Arrangeindex)
        {
            ObservableCollection<Bill_Show> Bills = Load_ListBill();
            ObservableCollection<Bill_Show> returnBill = new ObservableCollection<Bill_Show>();
            if (Arrangeindex == 0)
                return Bills;
            if (Arrangeindex == 1)
            {
                for (int i = 0; i < Bills.Count - 1; i++)
                    if (Bills[i].status == "Complete")
                        returnBill.Add(Bills[i]);
                return returnBill;
            }

            if (Arrangeindex == 2)
            {
                for (int i = 0; i < Bills.Count - 1; i++)
                    if (Bills[i].status == "Not Complete")
                        returnBill.Add(Bills[i]);
                return returnBill;
            }


            if (Arrangeindex == 3)
            {
                for (int i = 0; i < Bills.Count - 1; i++)
                    for (int j = i + 1; j < Bills.Count; j++)
                    {
                        if (Bills[i].DateCreate < Bills[j].DateCreate)
                        {
                            var temp = Bills[i];
                            Bills[i] = Bills[j];
                            Bills[j] = temp;
                        }
                    }
                return Bills;
            }
            if (Arrangeindex == 4)
            {
                for (int i = 0; i < Bills.Count - 1; i++)
                    for (int j = i + 1; j < Bills.Count; j++)
                    {
                        if (Bills[i].DateCreate > Bills[j].DateCreate)
                        {
                            var temp = Bills[i];
                            Bills[i] = Bills[j];
                            Bills[j] = temp;
                        }
                    }
                return Bills;
            }

            if (Arrangeindex == 5)
            {
                for (int i = 0; i < Bills.Count - 1; i++)
                    if (Bills[i].status == "Not Complete")
                        returnBill.Add(Bills[i]);
                return returnBill;
            }


            if (Arrangeindex == 6)
            {
                for (int i = 0; i < Bills.Count - 1; i++)
                    if (Bills[i].Online.Trim() == "Yes")
                        returnBill.Add(Bills[i]);
                return returnBill;
            }
            if (Arrangeindex == 7)
            {
                for (int i = 0; i < Bills.Count - 1; i++)
                    if (Bills[i].Online.Trim() == "No")
                        returnBill.Add(Bills[i]);
                return returnBill;
            }

            return Bills;
        }
        //đã chuyển
        public void UpdateStatusBill(Bill_Show bill_)
        {
            SqlParameter id = new SqlParameter("@ID_OUT", bill_.ID_output);

            SqlParameter stat = new SqlParameter("@status", "Complete");
            List<SqlParameter> a = new List<SqlParameter>();
            a.Add(id);
            a.Add(stat);
            DBMain.MyExecuteNonQuery("UpdateStatusBill", CommandType.StoredProcedure, a);

            //var t=db.Output_Form.Find(bill_.ID_output);
            //t.Status = "Complete";
            //db.SaveChanges();

        }
        //đã chuyển
        public void CancelBill(Bill_Show bill)
        {
            SqlParameter id = new SqlParameter("@ID_OUT", bill.ID_output);

            SqlParameter stat = new SqlParameter("@status", "Cancel");
            List<SqlParameter> a = new List<SqlParameter>();
            a.Add(id);
            a.Add(stat);
            DBMain.MyExecuteNonQuery("UpdateStatusBill", CommandType.StoredProcedure, a);

            DataTable currPro = DBMain.ExecuteQueryDataSet("Select * from Detail_Product where ID_Product='" + bill.ID_PRO + "'", CommandType.Text);
            SqlParameter id_pro = new SqlParameter("@ID_pro", bill.ID_output);

            SqlParameter amount = new SqlParameter("@Amount", bill.Amount + int.Parse(currPro.Rows[0][7].ToString()));
            List<SqlParameter> b = new List<SqlParameter>();
            b.Add(id_pro);
            b.Add(amount);

            DBMain.MyExecuteNonQuery("UpdateAmountProduct", CommandType.StoredProcedure, b);

            
        }
        //đã chuyển
        public bool CheckCurrentAmount(string id_product, int number)
        {
            DataTable t = DBMain.ExecuteQueryDataSet("Select Amount_Current from Detail_Product where ID_Product='" + id_product + "'", CommandType.Text);

            if (t != null)
            {
                if (int.Parse(t.Rows[0][0].ToString()) < number) return false;
                else
                    return true;
            }
            return false;
        }
        //đã chuyển
        public string Create_NewIdOutput_Auto()
        {
            ObservableCollection<Output_Form1> output_Forms = new ObservableCollection<Output_Form1>();
            output_Forms = Load_Output_Form();

            int count = output_Forms.Count();
            string s1 = output_Forms[count - 1].ID_Output;
            int s2 = Convert.ToInt32(s1.Remove(0, 3));
            string Id_out;
            if (s2 + 1 < 10)
                Id_out = "Out00" + (s2 + 1).ToString();
            else
                Id_out = "Out0" + (s2 + 1).ToString();
            return Id_out;
        }
        //đã chuyen
        public string Create_NewIdCustomer_Auto()
        {
            ObservableCollection<Customer1> customers = Load_Customer();

            int count = customers.Count();
            string s1 = customers[count - 1].ID_Customer;
            int s2 = Convert.ToInt32(s1.Remove(0, 3));
            string Id;
            if (s2 + 1 < 10)
                Id = "CUS00" + (s2 + 1).ToString();
            else
                Id = "CUS0" + (s2 + 1).ToString();
            return Id;
        }

        public void AddnewOutput(Bill_Show bill_)
        {

            List<SqlParameter> t = new List<SqlParameter>();
            t.Add(new SqlParameter("@ID", bill_.ID_output));
            t.Add(new SqlParameter("@ID_pro", bill_.ID_PRO));
            t.Add(new SqlParameter("@Amount", bill_.Amount));
            t.Add(new SqlParameter("@Date", bill_.DateCreate));
            t.Add(new SqlParameter("@Event",""));
            t.Add(new SqlParameter("@Price", bill_.Sale_price));
            t.Add(new SqlParameter("@Note", ""));
            t.Add(new SqlParameter("@Status", bill_.status));
            t.Add(new SqlParameter("@BuyOn", bill_.Online));
            t.Add(new SqlParameter("@Ship", bill_.Ship));
            //nếu khách hàng đã tồn tại thì ko cần thêm vào
            string idcus = Create_NewIdCustomer_Auto();
            if (DBMain.ExecuteQueryDataSet("Select ID_Customer from Customer where  Phone='"+bill_.Phone+"'",CommandType.Text) == null) //chưa tồn tại
            {
                
                bool lt = AddNewCustomer(idcus, bill_.Name_Cus, bill_.Address, bill_.Phone, bill_.Email, bill_.DateCreate);
                

            }
            t.Add(new SqlParameter("@ID_cus", idcus));
            DBMain.MyExecuteNonQuery("AddnewOutput_Form", CommandType.StoredProcedure, t);

        }
        //Statistic đã chuyển
        public ObservableCollection<InstanceStatistic> getProductPeriod(DateTime start, DateTime finish)
        {

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@StartTime", start));
            list.Add(new SqlParameter("@EndTime", finish));


            DataTable table = DBMain.ExecuteQueryData("GetProductPeriod", CommandType.StoredProcedure, list);

            ObservableCollection<InstanceStatistic> listSta = new ObservableCollection<InstanceStatistic>();

            for (int i = 0; i < table.Rows.Count; i++)
            {

                InstanceStatistic sta = new InstanceStatistic();
                sta.NameType = table.Rows[i].ItemArray[0].ToString();
                sta.AmountType = Convert.ToInt32(table.Rows[i].ItemArray[1]);
                sta.TotalProceed = Convert.ToInt32(table.Rows[i].ItemArray[2]);

                listSta.Add(sta);
            }
            return listSta;
        }

        
        //đã chuyển
        #region Customer

        //Đã chuyển

        public ObservableCollection<Customer1> Load_Customer()
        {
            ObservableCollection<Customer1> listCus1 = new ObservableCollection<Customer1>();

            DataTable table = DBMain.ExecuteQueryDataSet("LoadCustomer", CommandType.StoredProcedure);
            for (int i = 0; i < table.Rows.Count; i++)
            {

                Customer1 cus1 = new Customer1();
                cus1.ID_Customer = table.Rows[i].ItemArray[0].ToString();
                cus1.Name_Cus = table.Rows[i].ItemArray[1].ToString();
                cus1.Address_Cus = table.Rows[i].ItemArray[2].ToString();
                cus1.Phone = table.Rows[i].ItemArray[3].ToString();
                cus1.Birthday = Convert.ToDateTime(table.Rows[i].ItemArray[4]);
                cus1.Email = table.Rows[i].ItemArray[5].ToString();

                listCus1.Add(cus1);
            }
            return listCus1;

        }

        public bool DeleteCustomer(string id)
        {
            try
            {

                List<SqlParameter> parameter = new List<SqlParameter>();
                parameter.Add(new SqlParameter("@ID_Cus", id));

                DBMain.MyExecuteNonQuery("DeleteCustomer", CommandType.StoredProcedure, parameter);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }
        //đã chuyển
        public bool AddNewCustomer(string ID, string name, string address, string phone, string gmail, DateTime birthday)
        {           
           string hasValue = DBMain.FindOneValue("GetCustomer", CommandType.StoredProcedure, new SqlParameter("@ID_Cus", ID));
            string connStr = "";
            if (hasValue == null)
            {
                connStr = "AddNewCustomer";

                {
                    connStr = "UpdateCustomer";

                }

                List<SqlParameter> list = new List<SqlParameter>();
                list.Add(new SqlParameter("@ID_Cus", ID));
                list.Add(new SqlParameter("@Name_Cus", name));
                list.Add(new SqlParameter("@Address_Cus", address));
                list.Add(new SqlParameter("@Phone_Cus", phone));
                list.Add(new SqlParameter("@Birthday_Cus", birthday));
                list.Add(new SqlParameter("@Email_Cus", gmail));
                DBMain.MyExecuteNonQuery(connStr, CommandType.StoredProcedure, list);
               

            }
            return true;
            #endregion
        }
    }
}
