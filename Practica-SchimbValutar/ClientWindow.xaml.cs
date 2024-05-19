using Practica_SchimbValutar.Classes;
using Practica_SchimbValutar.MVVM.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace Practica_SchimbValutar
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        readonly string conString = "Data Source=DESKTOP-N5UB5V3\\SQLEXPRESS;Initial Catalog=Schimb_Valutar;Integrated Security=True;Encrypt=False";
        readonly InsertView insertView = null;
        public ClientWindow(InsertView window)
        {
            InitializeComponent();
            this.insertView = window;

            if(insertView.TxtName.Text != string.Empty)
            {
                TxtName.Text = insertView.TxtName.Text;
            }
            if(insertView.TxtPhone.Text != string.Empty)
            {
                TxtPhone.Text = insertView.TxtPhone.Text;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void BtnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (CheckString.CheckInt(TxtIDNP.Text))
            {
                TxtIDNP.BorderBrush = Brushes.Red;
                MessageBox.Show("Introdu IDNP-ul clientului");
            }
            if (CheckString.CheckText(TxtName.Text))
            {
                TxtName.BorderBrush = Brushes.Red;
                MessageBox.Show("Introdu numele clientului");
            }
            if (CheckString.CheckText(TxtAdress.Text))
            {
                TxtAdress.BorderBrush = Brushes.Red;
                MessageBox.Show("Introdu o adresa");
            }
            if (CheckString.CheckInt(TxtPhone.Text))
            {
                TxtPhone.BorderBrush = Brushes.Red;
                MessageBox.Show("Introduceti un numer de telefon");
            }
            if (CheckString.CheckText(TxtEmail.Text))
            {
                TxtEmail.BorderBrush = Brushes.Red;
                MessageBox.Show("Alege valuta convertita");
            }

            if (CheckString.CheckInt(TxtIDNP.Text) || CheckString.CheckText(TxtName.Text) || CheckString.CheckText(TxtAdress.Text) || CheckString.CheckInt(TxtPhone.Text) || CheckString.CheckText(TxtEmail.Text)) return;

            TxtIDNP.BorderBrush = Brushes.LightGray;
            TxtName.BorderBrush = Brushes.LightGray;
            TxtAdress.BorderBrush = Brushes.LightGray;
            TxtPhone.BorderBrush = Brushes.LightGray;
            TxtEmail.BorderBrush = Brushes.LightGray;

            string id = "";

            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();
                id = CreateID(con);

                string[] arr = TxtName.Text.Split(' ');

                string query = $"exec insertClienti '{id}', {Convert.ToInt64(TxtIDNP.Text)}, '{arr[0]}', '{arr[1]}', '{TxtAdress.Text}', {Convert.ToInt64(TxtPhone.Text)}, '{TxtEmail.Text}'";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Clientul a fost inregistrat");

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                insertView.ClientID = id;

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
