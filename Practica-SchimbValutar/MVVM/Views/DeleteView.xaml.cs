using Practica_SchimbValutar.Classes;
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
using System.Xml.Linq;
using static IronPython.Modules._ast;

namespace Practica_SchimbValutar.MVVM.Views
{
    /// <summary>
    /// Interaction logic for DeleteView.xaml
    /// </summary>
    public partial class DeleteView : UserControl
    {
        readonly string conString = "Data Source=DESKTOP-N5UB5V3\\SQLEXPRESS;Initial Catalog=Schimb_Valutar;Integrated Security=True;Encrypt=False";
        public DeleteView()
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
                if (CheckText.CheckString(TxtName.Text) || CheckText.CheckInt(TxtPhone.Text) || CheckText.CheckInt(TxtSum.Text)) return;
            }
            try
            {
                string name = "";
                if (TxtName.Text != string.Empty)
                {
                    name = TxtName.Text;
                }
                string phone = "";
                if(TxtPhone.Text != string.Empty)
                {
                    phone = TxtPhone.Text;
                }
                double sum = 0;
                if(TxtSum.Text != string.Empty)
                {
                    sum = Convert.ToDouble(TxtSum.Text);
                }

                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string query = $"select * from getTranzactii('{GetID("Clienti", name, phone, con)}', '{GetID("Schimb", BoxCurrencyConv.Text, BoxCurrency.Text, con)}', {sum}, null)";

                SqlCommand cmd = new SqlCommand(query, con);
                if (cmd.ExecuteScalar() == null)
                {
                    MessageBox.Show("Nu s-a gasit tranzactia");
                    return;
                }
                string id = cmd.ExecuteScalar().ToString();

                query = $"exec deleteSchimbV '{id}'";

                cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                query = $"delete from Tranzactie where ID = '{id}'";

                cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Tranzactia a fost stearsa");
                con.Close();

                ((MainWindow)System.Windows.Application.Current.MainWindow).LoadGrid("Transaction");
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
            if (condition1 == "") { return ""; }
            string query = "";
            if (table == "Schimb")
            {
                query = $"select ID from SchimbVechi where ID_Valuta_Convertita = '{GetID("Valuta", condition1, null, con)}' and ID_Valuta = '{GetID("Valuta", condition2, null, con)}'";
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
            if(cmd.ExecuteScalar() == null)
            {
                MessageBox.Show("Nu s-a gasit tranzactia");
                return "";
            }
            return cmd.ExecuteScalar().ToString();
        }
    }
}
