using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Windows;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace Practica_SchimbValutar.Classes
{
    internal class CurrencyConversion
    {
        readonly string conString = "Data Source=DESKTOP-N5UB5V3\\SQLEXPRESS;Initial Catalog=Schimb_Valutar;Integrated Security=True;Encrypt=False";

        readonly Dictionary<string, string> currencyIso = new Dictionary<string, string>() {
            {"CHF", "Swiss Franc"}, {"EUR", "Euro"}, {"GBP", "Pound Sterling"}, {"MDL", "Moldovan Leu"}, {"RON", "Romanian Leu"},
            {"RUB", "Russian Ruble"},{"UAH", "Ukrainian Hryvnia"}, {"USD", "United States Dollar"}};

        //static readonly string apiKey = "998861d13c2d44ceb37998ac8f558491";

        //readonly string  url = $"https://Openexchangerates.org/api/latest.json?app_id={apiKey}";

        static readonly string apiKey = "22b75d7898d14e70824ef3eee0e246bb";

        readonly string  url = $"https://api.currencyfreaks.com/v2.0/rates/latest?apikey={apiKey}";


        public void InsertCurrency()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string query = "select count(ID) from Valuta";
                SqlCommand command = new SqlCommand(query, con);
                if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                {
                    foreach (var currency in currencyIso)
                    {
                        query = $"insert into Valuta (ID, Cod, Denumire) values('{CreateID(con)}','{currency.Key}', '{currency.Value}')";
                        command = new SqlCommand(query, con);
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Insert Completed");
                }

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string CreateID( SqlConnection conn)
        {
            try
            {
                string query  = "select count(ID) from Valuta";
                
                SqlCommand cmd = new SqlCommand(query, conn);
                string id;

                int count = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                
                if (count >= 1 && count <= 9)
                {
                    id = "v00" + count;
                }
                else if (count >= 10 && count <= 99)
                {
                    id = "v0" + count;
                }
                else
                {
                    id = "v" + count;
                }

                return id;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return "";
        }

        public void UpdateConversion()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = @"F:\projects\Practica-SchimbValutar\Practica-SchimbValutar\Python\AddExchangeRate.py",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process { StartInfo = startInfo };
            process.Start();
            process.WaitForExit();
        }
    }
}

