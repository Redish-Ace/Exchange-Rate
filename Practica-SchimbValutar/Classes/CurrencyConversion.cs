using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Windows;
using System.Collections.Generic;
using System.Threading;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Diagnostics;
using System.Data;

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

        private dynamic GetRates()
        {
            int retryCount = 0;
            const int maxRetries = 10;

            while (retryCount < maxRetries)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = client.GetAsync(url).Result;
                        response.EnsureSuccessStatusCode();
                        string responseBody = response.Content.ReadAsStringAsync().Result;

                        dynamic data = JsonConvert.DeserializeObject(responseBody);

                        dynamic exchangeRates = data.rates;
                        return exchangeRates;
                    }
                }
                catch (HttpRequestException e)
                {
                    retryCount++;
                    if (retryCount < maxRetries)
                    {
                        Console.WriteLine($"Request failed with status code {e.Message}. Retrying in {Math.Pow(2, retryCount)} seconds...");
                        Thread.Sleep(Convert.ToInt32(Math.Pow(2, retryCount) * 1000));
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return "";
        }

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
                        query = $"insert into Valuta (ID, Cod, Denumire) values('{CreateID("Valuta", con)}','{currency.Key}', '{currency.Value}')";
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

        private string CreateID(string table ,SqlConnection conn)
        {
            try
            {
                string query = "";
                if (table == "Valuta")
                {
                    query = "select count(ID) from Valuta";
                }
                else if (table == "Schimb")
                {
                    query = "select count(ID) from Schimb";
                }
                SqlCommand cmd = new SqlCommand(query, conn);
                string id;

                int count = Convert.ToInt32(cmd.ExecuteScalar()) + 1;

                string letter = "";
                if (table == "Valuta") letter = "v";
                else if (table == "Schimb") letter = "s";
                
                if (count >= 1 && count <= 9)
                {
                    id = letter + "00" + count;
                }
                else if (count >= 10 && count <= 99)
                {
                    id = letter + "0" + count;
                }
                else
                {
                    id = letter + count;
                }

                return id;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return "";
        }

        private string GetID(string table, SqlConnection conn, string condition1, string condition2)
        {
            try
            {
                string query = "";
                if (table == "Valuta")
                {
                    query = $"select ID from Valuta where Cod = '{condition1}'";
                }
                else if (table == "Schimb")
                {
                    query = $"select ID from Schimb where ID_Valuta_Convertita = '{condition1}' AND ID_Valuta = '{condition2}'";
                }
                SqlCommand cmd = new SqlCommand(query, conn);
                string id = "";

                id = cmd.ExecuteScalar().ToString();

                return id;
            }
            catch (Exception ex)
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

        private double GetConversion(string code, string code2, dynamic rates)
        {
            float originalAmount = 1 / (float)rates[code];
            float convertedAmount = originalAmount * (float)rates[code2];

            return convertedAmount;
        }
    }
}

