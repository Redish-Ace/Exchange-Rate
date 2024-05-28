using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.SqlServer.Server;
using System.Globalization;

namespace Practica_SchimbValutar.MVVM.Views
{
    /// <summary>
    /// Interaction logic for SelectView.xaml
    /// </summary>
    public partial class SelectView : UserControl
    {
        readonly string conString = "Data Source=DESKTOP-N5UB5V3\\SQLEXPRESS;Initial Catalog=Schimb_Valutar;Integrated Security=True;Encrypt=False";

        public SelectView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string query = "";

                if (sender.ToString().Contains("Azi"))
                {
                    query = "select * from tranzactiiAzi";
                }
                else if (sender.ToString().Contains("Avantajoase"))
                {
                    query = "select top 1 * from tranzactieAvantaj order by Suma_finala_lei desc";
                }
                else if (sender.ToString().Contains("Valuta"))
                {
                    query = "select * from valutaSolicitata";
                }

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                da.Fill(ds);

                ((MainWindow)System.Windows.Application.Current.MainWindow).DataSource = ds.Tables[0].DefaultView;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Comision_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string query = "select * from comisionTotal";

                SqlCommand cmd = new SqlCommand(query, con);
                MessageBox.Show("Comisionul total: " + cmd.ExecuteScalar().ToString() + " lei");

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string dateTime = Calendar.SelectedDate.Value.ToShortTimeString();

                string query = $"select * from tranzactiiData('{Calendar.SelectedDate.Value.ToString("yyyy-MM-dd")}') order by Suma_finala_lei desc";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                da.Fill(ds);

                ((MainWindow)System.Windows.Application.Current.MainWindow).DataSource = ds.Tables[0].DefaultView;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
