using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Data.SqlClient;
using Practica_SchimbValutar;
using Practica_SchimbValutar.Classes;

namespace Practica_SchimbValutar.MVVM.Views
{
    /// <summary>
    /// Interaction logic for InsertView.xaml
    /// </summary>
    public partial class InsertView : UserControl
    {
        readonly string conString = "Data Source=DESKTOP-N5UB5V3\\SQLEXPRESS;Initial Catalog=Schimb_Valutar;Integrated Security=True;Encrypt=False";
        public InsertView()
        {
            InitializeComponent();
            CreateBox();
        }

        private string ClientId;

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
                sdr.Close();

                con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CheckString.CheckText(TxtName.Text))
            {
                TxtName.BorderBrush = Brushes.Red;
                MessageBox.Show("Introdu numele clientului");
            }
            if (CheckString.CheckInt(TxtSum.Text))
            {
                TxtSum.BorderBrush = Brushes.Red;
                MessageBox.Show("Suma schimbata");
            }
            if (CheckString.CheckInt(TxtPhone.Text))
            {
                TxtPhone.BorderBrush = Brushes.Red;
                MessageBox.Show("Introduceti un numer de telefon");
            }
            if (BoxCurrencyConv.Text == string.Empty)
            {
                BoxCurrencyConv.BorderBrush = Brushes.Red;
                MessageBox.Show("Alege valuta convertita"); 
            }
            if (BoxCurrency.Text == string.Empty)
            {
                BoxCurrency.BorderBrush = Brushes.Red;
                MessageBox.Show("Alege valuta");
            }

            if (CheckString.CheckText(TxtName.Text) || CheckString.CheckInt(TxtPhone.Text) || CheckString.CheckInt(TxtSum.Text) || BoxCurrencyConv.Text == string.Empty || BoxCurrency.Text == string.Empty) return;

            TxtName.BorderBrush = Brushes.LightGray;
            TxtSum.BorderBrush = Brushes.LightGray;
            TxtPhone.BorderBrush = Brushes.LightGray;
            BoxCurrencyConv.BorderBrush = Brushes.LightGray;
            BoxCurrency.BorderBrush = Brushes.LightGray;

            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                //make it so it doesnt work until insertClient stops
                string query = $"exec insertTranzactii '{CreateID(con)}', '{InsertClient(TxtName.Text, Convert.ToInt32(TxtPhone.Text), con)}', '{GetID("Schimb", BoxCurrencyConv.Text, BoxCurrency.Text, con)}', {Convert.ToDouble(TxtSum.Text)}";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Tranzactia a fost inregistrat");
                con.Close();

                MainWindow main = new MainWindow();
                main.LoadTransactionGrid();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                TxtName.Text = string.Empty;
                TxtPhone.Text = string.Empty;
                TxtSum.Text = string.Empty;
                BoxCurrencyConv.Text = string.Empty;
                BoxCurrency.Text = string.Empty;
            }
        }

        private string CreateID(SqlConnection con)
        {
            string query = "select count(*) from Tranzactie";
            SqlCommand cmd = new SqlCommand(query, con);
            int total = Convert.ToInt32(cmd.ExecuteScalar()) + 1;

            string id = "t";
            if(total >= 1 && total <= 9)
            {
                id += "00000" + total;
            }
            else if (total >= 10 && total <= 99)
            {
                id += "0000" + total;
            }
            else if (total >= 100 && total <= 999)
            {
                id += "000" + total;
            }
            else if (total >= 1000 && total <= 9999)
            {
                id += "00" + total;
            }
            else if (total >= 10000 && total <= 99999)
            {
                id += "0" + total ;
            }
            else
            {
                id += total;
            }

            return id;
        }

        private string GetID(string table, string condition1, string condition2, SqlConnection con)
        {
            string query = "";
            if (table == "Schimb")
            {
                query = $"select ID from Schimb where ID_Valuta_Convertita = '{GetID("Valuta", condition1, null, con)}' and ID_Valuta = '{GetID("Valuta", condition2, null, con)}'";
            }
            else if(table == "Valuta")
            {
                query = $"select ID from Valuta where Cod + ': ' + Denumire = '{condition1}'";
            }

            SqlCommand cmd = new SqlCommand(query, con);
            return cmd.ExecuteScalar().ToString();
        }

        public string ClientID
        {
            get { return ClientId; }
            set { ClientId = value; }
        }

        private string InsertClient(string condition, long phone, SqlConnection con)
        {
            //create a new window to insert new client

            string messageBoxText = "Este un client nou?";
            string caption = "Client Nou";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                ClientWindow client = new ClientWindow(this);
                client.Show();
            }
            else {
                string query = $"select ID from Clienti where Telefon = {phone} and Nume + ' ' + Prenume = '{condition}'";
                SqlCommand cmd = new SqlCommand(query, con);
                return cmd.ExecuteScalar().ToString();
            }

            while(ClientId == string.Empty) { }

            return ClientId;
        }
    }
}
