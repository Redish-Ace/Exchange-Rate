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

namespace Practica_SchimbValutar.MVVM.Views
{
    /// <summary>
    /// Interaction logic for InsertClientView.xaml
    /// </summary>
    public partial class InsertClientView : UserControl
    {
        readonly string conString = "Data Source=DESKTOP-N5UB5V3\\SQLEXPRESS;Initial Catalog=Schimb_Valutar;Integrated Security=True;Encrypt=False";
        public InsertClientView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CheckText.CheckInt(TxtIDNP.Text))
            {
                TxtIDNP.BorderBrush = Brushes.Red;
                MessageBox.Show("Introdu IDNP-ul clientului");
            }
            if (CheckText.CheckString(TxtName.Text))
            {
                TxtName.BorderBrush = Brushes.Red;
                MessageBox.Show("Introdu numele clientului");
            }
            if (CheckText.CheckString(TxtAdress.Text))
            {
                TxtAdress.BorderBrush = Brushes.Red;
                MessageBox.Show("Introdu o adresa");
            }
            if (CheckText.CheckInt(TxtPhone.Text))
            {
                TxtPhone.BorderBrush = Brushes.Red;
                MessageBox.Show("Introduceti un numer de telefon");
            }
            if (CheckText.CheckString(TxtEmail.Text))
            {
                TxtEmail.BorderBrush = Brushes.Red;
                MessageBox.Show("Alege valuta convertita");
            }

            if (CheckText.CheckInt(TxtIDNP.Text) || CheckText.CheckString(TxtName.Text) || CheckText.CheckString(TxtAdress.Text) || CheckText.CheckInt(TxtPhone.Text) || CheckText.CheckString(TxtEmail.Text)) return;

            TxtIDNP.BorderBrush = Brushes.LightGray;
            TxtName.BorderBrush = Brushes.LightGray;
            TxtAdress.BorderBrush = Brushes.LightGray;
            TxtPhone.BorderBrush = Brushes.LightGray;
            TxtEmail.BorderBrush = Brushes.LightGray;

            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string[] arr = TxtName.Text.Split(' ');

                string query = $"exec insertClienti '{CreateID(con)}', {Convert.ToInt64(TxtIDNP.Text)}, '{arr[0]}', '{arr[1]}', '{TxtAdress.Text}', {Convert.ToInt64(TxtPhone.Text)}, '{TxtEmail.Text}'";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Clientul a fost inregistrat");

                con.Close();

                MainWindow main = new MainWindow();
                ((MainWindow)System.Windows.Application.Current.MainWindow).LoadGrid("Client");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                TxtIDNP.Text = string.Empty;
                TxtName.Text = string.Empty;
                TxtAdress.Text = string.Empty;
                TxtPhone.Text = string.Empty;
                TxtEmail.Text = string.Empty;
            }
        }

        private string CreateID(SqlConnection con)
        {
            string query = "select count(*) from Clienti";
            SqlCommand cmd = new SqlCommand(query, con);
            int total = Convert.ToInt32(cmd.ExecuteScalar()) + 1;

            string id = "c";
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
    }
}
