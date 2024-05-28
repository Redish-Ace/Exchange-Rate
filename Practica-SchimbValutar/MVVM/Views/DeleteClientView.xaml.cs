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

                string query = $"select * from getClient({Convert.ToInt64(TxtIDNP.Text)}, '{arr[0]}', '{arr[1]}', '{TxtAdress.Text}', {phone} ,'{TxtEmail.Text}')";

                SqlCommand cmd = new SqlCommand(query, con);
                if (cmd.ExecuteScalar() == null)
                {
                    MessageBox.Show("Nu s-a gasit clientul");
                    return;
                }
                string id = cmd.ExecuteScalar().ToString();

                query = $"delete from Clienti where ID = '{id}'";

                cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Clientul a fost sters");
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
                TxtName.Text = string.Empty;
                TxtPhone.Text = string.Empty;
            }
        }
    }
}
