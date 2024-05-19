using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Practica_SchimbValutar.Classes;

namespace Practica_SchimbValutar.MVVM.Views
{
    /// <summary>
    /// Interaction logic for UpdateView.xaml
    /// </summary>
    public partial class UpdateView : UserControl
    {
        readonly string conString = "Data Source=DESKTOP-N5UB5V3\\SQLEXPRESS;Initial Catalog=Schimb_Valutar;Integrated Security=True;Encrypt=False";
        public UpdateView()
        {
            InitializeComponent();
            CreateBox();
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
                sdr.Close();

                query = "select ID from Tranzactie";
                cmd = new SqlCommand(query, con);
                sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    BoxTransaction.Items.Add(sdr.GetValue(0));
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
            if (TxtName.Text != string.Empty || TxtPhone.Text != string.Empty || TxtSum.Text != string.Empty)
            {
                if (CheckString.CheckText(TxtName.Text) || CheckString.CheckInt(TxtPhone.Text) || CheckString.CheckInt(TxtSum.Text)) return;
            }

            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string query = $"exec updateTranzactii '{BoxTransaction.Text}', '{GetID("Clienti",TxtName.Text, TxtPhone.Text, con)}', '{GetID("Schimb", BoxCurrencyConv.Text, BoxCurrency.Text, con)}', {Convert.ToDouble(TxtSum.Text)}";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Tranzactia a fost modificata");
                con.Close();

                MainWindow main = new MainWindow();
                main.LoadTransactionGrid();
            }
            catch (Exception ex)
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
            else if (table == "Clienti")
            {
                query = $"select ID from Clienti where Nume + ' ' + Prenume = '{condition1}' and Telefon = {Convert.ToInt64(condition2)}";
            }

            SqlCommand cmd = new SqlCommand(query, con);
            return cmd.ExecuteScalar().ToString();
        }
    }
}
