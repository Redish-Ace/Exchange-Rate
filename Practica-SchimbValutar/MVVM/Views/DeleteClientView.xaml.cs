using Practica_SchimbValutar.Classes;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static IronPython.Modules._ast;

namespace Practica_SchimbValutar.MVVM.Views
{
    /// <summary>
    /// Interaction logic for DeleteClientView.xaml
    /// </summary>
    public partial class DeleteClientView : UserControl
    {
        readonly string conString = "Data Source=DESKTOP-N5UB5V3\\SQLEXPRESS;Initial Catalog=Schimb_Valutar;Integrated Security=True;Encrypt=False";
        public DeleteClientView()
        {
            InitializeComponent();
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

                string query = $"select * from getClient({Convert.ToInt64(TxtIDNP.Text)}, '{arr[0]}', '{arr[1]}', '{TxtAdress.Text}', {Convert.ToInt64(TxtPhone.Text)} ,'{TxtEmail.Text}')";

                SqlCommand cmd = new SqlCommand(query, con);
                string id = cmd.ExecuteScalar().ToString();

                query = $"delete from Clienti where ID = '{id}'";

                cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Clientul a fost sters");
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
    }
}
