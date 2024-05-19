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
                if (CheckString.CheckText(TxtName.Text) || CheckString.CheckInt(TxtPhone.Text) || CheckString.CheckText(TxtAdress.Text) || CheckString.CheckText(TxtEmail.Text)) return;
            }
            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string[] arr = TxtName.Text.Split(' ');

                string query = $"exec updateClient '{GetID(BoxClient.Text, con)}', '{arr[0]}' , '{arr[1]}', '{TxtAdress.Text}', {Convert.ToInt64(TxtPhone.Text)}, '{TxtEmail.Text}'";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Clientul a fost modificat");
                con.Close();

                MainWindow main = new MainWindow();
                main.LoadClientGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                TxtName.Text = string.Empty;
                TxtPhone.Text = string.Empty;
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
