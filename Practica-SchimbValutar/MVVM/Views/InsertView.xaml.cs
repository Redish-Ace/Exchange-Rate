using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Data.SqlClient;
using Practica_SchimbValutar;
using Practica_SchimbValutar.Classes;
using System.Security.Cryptography;
using Microsoft.Office.Interop.Excel;

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

        private string ClientId = "";

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CheckText.CheckString(TxtName.Text))
            {
                TxtName.BorderBrush = Brushes.Red;
                MessageBox.Show("Introdu numele clientului");
            }
            if (CheckText.CheckInt(TxtSum.Text))
            {
                TxtSum.BorderBrush = Brushes.Red;
                MessageBox.Show("Suma schimbata");
            }
            if (CheckText.CheckInt(TxtPhone.Text))
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

            if (CheckText.CheckString(TxtName.Text) || CheckText.CheckInt(TxtPhone.Text) || CheckText.CheckInt(TxtSum.Text) || BoxCurrencyConv.Text == string.Empty || BoxCurrency.Text == string.Empty) return;

            TxtName.BorderBrush = Brushes.LightGray;
            TxtSum.BorderBrush = Brushes.LightGray;
            TxtPhone.BorderBrush = Brushes.LightGray;
            BoxCurrencyConv.BorderBrush = Brushes.LightGray;
            BoxCurrency.BorderBrush = Brushes.LightGray;

            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

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
                else
                {
                    string query = $"select ID from Clienti where Telefon = {Convert.ToInt64(TxtPhone.Text)} and Nume + ' ' + Prenume = '{TxtName.Text}'";
                    SqlCommand cmd = new SqlCommand(query, con);
                    if (cmd.ExecuteScalar() == null)
                    {
                        MessageBox.Show("Clientul nu a fost gasit");
                    }
                    else
                    {
                        ClientId = cmd.ExecuteScalar().ToString();
                        InsertTransaction(ClientId);
                    }
                }
                con.Close();

                ((MainWindow)System.Windows.Application.Current.MainWindow).LoadGrid("Transaction");
            }
            catch (Exception ex)    
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string CreateID(string table , SqlConnection con)
        {
            string query = "";

            string id = "";

            switch (table)
            {
                case "Transaction":
                    {
                        query = "select count(*) from Tranzactie";
                        id = "t";
                    }
                    break;
                case "Rate":
                    {
                        query = "select count(*) from SchimbVechi";
                        id = "s";
                    }
                    break;
            }
            SqlCommand cmd = new SqlCommand(query, con);
            int total = Convert.ToInt32(cmd.ExecuteScalar()) + 1;

            if (total >= 1 && total <= 9)
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
                id += "0" + total;
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
            else if (table == "Valuta")
            {
                query = $"select ID from Valuta where Cod + ': ' + Denumire = '{condition1}'";
            }

            SqlCommand cmd = new SqlCommand(query, con);
            return cmd.ExecuteScalar().ToString();
        }

        public void InsertTransaction (string clientID)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string sid = GetID("Schimb", BoxCurrencyConv.Text, BoxCurrency.Text, con);
                string svid = CreateID("Rate", con);
                double sum = Convert.ToDouble(TxtSum.Text);

                string query = $"exec insertTranzactii '{CreateID("Transaction", con)}', '{clientID}', '{sid}', '{svid}', {sum}, null";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Tranzactia a fost inregistrat");
                con.Close();

                ((MainWindow)System.Windows.Application.Current.MainWindow).LoadGrid("Transaction");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("Nu s-a inregistrat tranzactia");
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
    }
}
