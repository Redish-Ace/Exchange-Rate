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
using System.Runtime.InteropServices;

namespace Practica_SchimbValutar.MVVM.Views
{
    /// <summary>
    /// Interaction logic for UpdateClientView.xaml
    /// </summary>
    public partial class UpdateClientView : UserControl
    {
        readonly string conString = "Data Source=DESKTOP-N5UB5V3\\SQLEXPRESS;Initial Catalog=Schimb_Valutar;Integrated Security=True;Encrypt=False";
        public UpdateClientView()
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

                string query = "select IDNP from Clienti";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    BoxClient.Items.Add(sdr.GetValue(0));
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
            if (TxtName.Text != string.Empty || TxtPhone.Text != string.Empty || TxtAdress.Text != string.Empty || TxtEmail.Text != string.Empty)
            {
                if (CheckText.CheckString(TxtName.Text) || CheckText.CheckInt(TxtPhone.Text) || CheckText.CheckString(TxtAdress.Text) || CheckText.CheckString(TxtEmail.Text)) return;
            }
            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string[] arr = { "", "" };
                if (TxtName.Text != string.Empty) arr = TxtName.Text.Split(' ');

                long phone = 0;
                if (TxtPhone.Text != string.Empty)
                {
                    phone = Convert.ToInt64(TxtPhone.Text);
                }

                string query = $"exec updateClienti '{GetID(BoxClient.Text, con)}', '{arr[0]}' , '{arr[1]}', '{TxtAdress.Text}', {phone}, '{TxtEmail.Text}'";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Clientul a fost modificat");
                con.Close();

                ((MainWindow)System.Windows.Application.Current.MainWindow).LoadGrid("Client");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                TxtName.Text = string.Empty;
                TxtPhone.Text = string.Empty;
                TxtAdress.Text = string.Empty;
                TxtEmail.Text = string.Empty;
            }
        }

        private string GetID(string condition1, SqlConnection con)
        {
            string query = $"select ID from Clienti where IDNP = '{condition1}'";
            SqlCommand cmd = new SqlCommand(query, con);

            return cmd.ExecuteScalar().ToString();
        }
    }
}
