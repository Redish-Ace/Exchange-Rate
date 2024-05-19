using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using Practica_SchimbValutar.Classes;
using System.Windows.Controls;
using System.Windows.Markup.Localizer;
using System.Linq;

namespace Practica_SchimbValutar
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        readonly MainWindow main = null;
        public LoginWindow(MainWindow window)
        {
            InitializeComponent();
            this.main = window;
        }

        int code;

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

        private void SendCode(string email)
        {
            Random rnd = new Random();
            code = rnd.Next(100000, 999999);

            try
            {
                //lilianginga86@gmail.com
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("MS_rYxDnR@trial-z3m5jgrq3xoldpyo.mlsender.net"),
                    Subject = "Cod de Verificare",
                    Body = code.ToString()
                };
                mailMessage.To.Add(email);

                SmtpClient smtpClient = new SmtpClient
                {
                    Host = "smtp.mailersend.net",
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("MS_rYxDnR@trial-z3m5jgrq3xoldpyo.mlsender.net", "wT8GhoQHpsMbQ8fl"),
                    EnableSsl = true
                };

                smtpClient.Send(mailMessage);
                Console.WriteLine("Email Sent Successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnSignSave_Click(object sender, EventArgs e)
        {
            if (TxtPass.Password != TxtPassConfirm.Password)
            {
                TxtPass.BorderBrush = Brushes.Red;
                TxtPassConfirm.BorderBrush = Brushes.Red;
                MessageBox.Show("Parola trebuie sa fie la fel");
                return;
            }
            if (TxtPass.Password == string.Empty || TxtPassConfirm.Password == string.Empty)
            {
                TxtPass.BorderBrush = Brushes.Red;
                TxtPassConfirm.BorderBrush = Brushes.Red;
                MessageBox.Show("Introdu parola");
                return;
            }
            TxtPass.BorderBrush = Brushes.LightGray;
            TxtPassConfirm.BorderBrush = Brushes.LightGray;

            if (TxtName.Text == string.Empty)
            {
                TxtName.BorderBrush = Brushes.Red;
                MessageBox.Show("Introduceti Numele");
                return;
            }
            TxtName.BorderBrush = Brushes.LightGray;

            if (TxtEmail.Text == string.Empty)
            {
                TxtEmail.BorderBrush = Brushes.Red;
                MessageBox.Show("Introduceti Emailul");
                return;
            }
            TxtEmail.BorderBrush = Brushes.LightGray;

            string xmlFilePath = @"F:\projects\Practica-SchimbValutar\Practica-SchimbValutar\LoginInfo.xml";
            List<Users> users = new List<Users>();

            if (File.Exists(xmlFilePath))
            {
                XmlSerializer serial = new XmlSerializer(typeof(List<Users>));
                using (FileStream fs = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read))
                {
                    users = (List<Users>)serial.Deserialize(fs);
                }
            }

            foreach (Users user in users)
            {
                if (user.Email.ToString() == TxtEmail.Text)
                {
                    TxtEmail.BorderBrush = Brushes.Red;
                    MessageBox.Show("Emailul este deja folosit");
                    break;
                }
                if(user.Name.ToString() == TxtName.Text)
                {
                    TxtName.BorderBrush = Brushes.Red;
                    MessageBox.Show("Numele este deja folosit, introduceti un nume diferit");
                    break;
                }
            }
            SendCode(TxtEmail.Text);
            MessageBox.Show("Verifica Emailul pentru codul de verificare");
        }

        private void TxtVerification_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(TxtVerification.Text == code.ToString())
            {
                TxtVerification.BorderBrush = Brushes.LightGray;

                string password = "bank";
                string encrypt = Encryption.Encrypt(TxtPass.Password, password);

                string xmlFilePath = @"F:\projects\Practica-SchimbValutar\Practica-SchimbValutar\LoginInfo.xml";
                List<Users> users;

                if (File.Exists(xmlFilePath))
                {
                    XmlSerializer serial = new XmlSerializer(typeof(List<Users>));
                    using (FileStream fs = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read))
                    {
                        users = (List<Users>)serial.Deserialize(fs);
                    }
                }
                else
                {
                    users = new List<Users>();
                }

                users.Add(new Users() { Name = TxtName.Text, Email = TxtEmail.Text, Password = encrypt });

                XmlSerializer serializer = new XmlSerializer(typeof(List<Users>));
                using (FileStream fs = new FileStream(xmlFilePath, FileMode.Create, FileAccess.Write))
                {
                    serializer.Serialize(fs, users);
                }

                MessageBox.Show("Utilizatorul a fost creat");

                TxtName.Text = string.Empty;
                TxtEmail.Text = string.Empty;
                TxtPass.Password = string.Empty;
                TxtPassConfirm.Password = string.Empty;
                TxtVerification.Text = string.Empty;
            }
            else
            {
                TxtVerification.BorderBrush = Brushes.Red;
            }
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string password = "bank";

                string xmlFilePath = @"F:\projects\Practica-SchimbValutar\Practica-SchimbValutar\LoginInfo.xml";
                List<Users> users = new List<Users>();

                if (File.Exists(xmlFilePath))
                {
                    XmlSerializer serial = new XmlSerializer(typeof(List<Users>));
                    using (FileStream fs = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read))
                    {
                        users = (List<Users>)serial.Deserialize(fs);
                    }
                }

                foreach( Users user in users )
                {
                    if(user.Name == TxtName.Text && Encryption.Decrypt(user.Password, password) == TxtPass.Password)
                    {
                        MessageBox.Show("Bine ati venit " + user.Name);
                        main.Login = user.Name;

                        main.LoginBtn.Visibility = Visibility.Hidden;
                        main.SignUpBtn.Visibility = Visibility.Hidden;
                        main.SignOutBtn.Visibility = Visibility.Visible;
                        main.DeleteBtn.Visibility = Visibility.Visible;

                        main.EnableButtons();
                        return;
                    }
                }

                MessageBox.Show("Utilizatorul nu a fost gasit");
            }
            catch(Exception ex)
            { 
                MessageBox.Show(ex.Message);
            }
        }
    }
}
