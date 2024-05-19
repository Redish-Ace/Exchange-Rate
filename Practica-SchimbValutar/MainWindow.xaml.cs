using Practica_SchimbValutar.Classes;
using Practica_SchimbValutar.MVVM.Views;
using System;
using System.Windows;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Windows.Input;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Practica_SchimbValutar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly string conString = "Data Source=DESKTOP-N5UB5V3\\SQLEXPRESS;Initial Catalog=Schimb_Valutar;Integrated Security=True;Encrypt=False";
        public MainWindow()
        {
            InitializeComponent();

            var currency = new CurrencyConversion();
            currency.InsertCurrency();
            currency.UpdateConversion();
            DeleteLastMonth();
            SaveExcel();
            EnableButtons();
            CreateBox();

            LoadGrid();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        public void EnableButtons()
        {
            if(BlockLogin.Text == "USER")
            {
                RadioInsert.IsEnabled = false;
                RadioUpdate.IsEnabled = false;
                RadioDelete.IsEnabled = false;
                RadioInsertClient.IsEnabled = false;
                RadioUpdateClient.IsEnabled = false;
                RadioDeleteClient.IsEnabled = false;
            }
            else
            {
                RadioInsert.IsEnabled = true;
                RadioUpdate.IsEnabled = true;
                RadioDelete.IsEnabled = true;
                RadioInsertClient.IsEnabled = true;
                RadioUpdateClient.IsEnabled = true;
                RadioDeleteClient.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow(this);
            loginWindow.Show();

            if (sender.ToString().Contains("Login"))
            {
                loginWindow.LoginBtn.Visibility = Visibility.Visible;
            
                loginWindow.BlockEmail.Visibility = Visibility.Hidden;
                loginWindow.TxtEmail.Visibility = Visibility.Hidden;
                loginWindow.BlockPassConfirm.Visibility = Visibility.Hidden;
                loginWindow.TxtPassConfirm.Visibility = Visibility.Hidden;
                loginWindow.BlockVerif.Visibility = Visibility.Hidden;
                loginWindow.TxtVerification.Visibility = Visibility.Hidden;
                loginWindow.SignBtn.Visibility = Visibility.Hidden;
                loginWindow.TxtPassConfirm.Visibility = Visibility.Hidden;
            }
            else if (sender.ToString().Contains("Sign"))
            {
                loginWindow.BlockEmail.Visibility = Visibility.Visible;
                loginWindow.TxtEmail.Visibility = Visibility.Visible;
                loginWindow.BlockPassConfirm.Visibility = Visibility.Visible;
                loginWindow.TxtPassConfirm.Visibility = Visibility.Visible;
                loginWindow.BlockVerif.Visibility = Visibility.Visible;
                loginWindow.TxtVerification.Visibility = Visibility.Visible;
                loginWindow.SignBtn.Visibility = Visibility.Visible;
                loginWindow.TxtPassConfirm.Visibility = Visibility.Visible;

                loginWindow.LoginBtn.Visibility = Visibility.Hidden;
            }
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            BlockLogin.Text = "USER";

            LoginBtn.Visibility = Visibility.Visible;
            SignUpBtn.Visibility = Visibility.Visible;
            SignOutBtn.Visibility = Visibility.Hidden;
            DeleteBtn.Visibility = Visibility.Hidden;

            EnableButtons();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Doresti sa stergi userul?";
            string caption = "Sterge user";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                string userName = BlockLogin.Text;
                BlockLogin.Text = "USER";

                LoginBtn.Visibility = Visibility.Visible;
                SignUpBtn.Visibility = Visibility.Visible;
                SignOutBtn.Visibility = Visibility.Hidden;
                DeleteBtn.Visibility = Visibility.Hidden;

                string xmlFilePath = @"F:\projects\c#\Practica-SchimbValutar\Practica-SchimbValutar\LoginInfo.xml";

                List<Users> users;

                if (File.Exists(xmlFilePath))
                {
                    XmlSerializer serial = new XmlSerializer(typeof(List<Users>));
                    using (FileStream fs = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read))
                    {
                        users = (List<Users>)serial.Deserialize(fs);
                    }
                }
                else
                {
                    users = new List<Users>();
                }

                List<Users> remaining = new List<Users>();
                foreach (Users user in users)
                {
                    if (user.Name != userName)
                    {
                        remaining.Add(user);
                    }
                }

                XmlSerializer serializer = new XmlSerializer(typeof(List<Users>));
                using (FileStream fs = new FileStream(xmlFilePath, FileMode.Create, FileAccess.Write))
                {
                    serializer.Serialize(fs, remaining);
                }

                MessageBox.Show($"Userul {userName} a fost sters");
            }

            EnableButtons();
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        public void LoadGrid()
        {
            try
            {
                SelectView selectView = new SelectView();

                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string query = "select * from getRate";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                OutputGrid.ItemsSource = ds.Tables[0].DefaultView;
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadTransactionGrid()
        {

            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string query = "select * from tranzactii";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                da.Fill(ds);

                OutputGrid.ItemsSource = ds.Tables[0].DefaultView;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadClientGrid()
        {

            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string query = "select * from Clienti";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                da.Fill(ds);

                OutputGrid.ItemsSource = ds.Tables[0].DefaultView;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveExcel()
        {
            if (File.Exists("F:\\projects\\Practica-SchimbValutar\\Practica-SchimbValutar\\Excel\\Schimb.xls"))
            {
                File.Delete("F:\\projects\\Practica-SchimbValutar\\Practica-SchimbValutar\\Excel\\Schimb.xls");
            }

            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet.Cells[1, 1] = "ID";
            xlWorkSheet.Cells[1, 2] = "Valuta_Convertita";
            xlWorkSheet.Cells[1, 3] = "Schimb";
            xlWorkSheet.Cells[1, 4] = "Valuta";

            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string query = "select ID, Valuta_Convertita, Schimb, Valuta from getRate";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader sdr = cmd.ExecuteReader();
                int i = 2;
                while (sdr.Read())
                {
                    xlWorkSheet.Cells[i, 1] = sdr.GetValue(0);
                    xlWorkSheet.Cells[i, 2] = sdr.GetValue(1);
                    xlWorkSheet.Cells[i, 3] = sdr.GetValue(2);
                    xlWorkSheet.Cells[i, 4] = sdr.GetValue(3);
                    i++;
                }

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            xlWorkBook.SaveAs("F:\\projects\\Practica-SchimbValutar\\Practica-SchimbValutar\\Excel\\Schimb.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);
        }
        public string Login
        {
            set { BlockLogin.Text = value; }
        }

        public IEnumerable DataSource
        {
            get { return OutputGrid.ItemsSource; }
            set { OutputGrid.ItemsSource = value; }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string query = "select * from getRate";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                OutputGrid.ItemsSource = ds.Tables[0].DefaultView;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string query = "select * from Clienti";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                OutputGrid.ItemsSource = ds.Tables[0].DefaultView;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string query = "select * from tranzactii";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                da.Fill(ds);

                ((MainWindow)System.Windows.Application.Current.MainWindow).DataSource = ds.Tables[0].DefaultView;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteLastMonth()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(today.Year, today.Month, 1);
                string query = $"exec deleteTranzactii null, null, null, 0,'{month}'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateBox()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string query = "select Cod + ': ' + Denumire from Valuta";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    BoxCurrencyConv.Items.Add(sdr.GetValue(0));
                    BoxCurrency.Items.Add(sdr.GetValue(0));
                }

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (CheckString.CheckInt(TxtSum.Text)) { 
                MessageBox.Show("Introdu o suma de bani, nu alte caractere");
                BoxCurrencyConv.Text = string.Empty;
                BoxCurrency.Text = string.Empty;
                TxtSum.Text = string.Empty;
                BlockResult.Text = string.Empty;
                return;
            }
            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(today.Year, today.Month, 1);
                string query = $"select Schimb from getRate where Valuta_Convertita = '{BoxCurrencyConv.Text}' and Valuta = '{BoxCurrency.Text}'";
                SqlCommand cmd = new SqlCommand(query, con);
                
                double schimb = Convert.ToDouble(cmd.ExecuteScalar());

                
                BlockResult.Text = (Convert.ToDouble(TxtSum.Text) * schimb).ToString();

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
