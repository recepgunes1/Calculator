using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.Net.Mail;
using Database;
using Security;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for windowMailSender.xaml
    /// </summary>
    public partial class windowMailSender : Window
    {
        public string[] acceptedMails = { "hotmail.com", "outlook.com", "gmail.com", "toros.edu.tr" };
        public SmtpClient sc;
        public MailMessage mail;
        public dbClass dbOperation;
        public dbClass.User _User;
        public windowMailSender(dbClass.User _user)
        {
            InitializeComponent();
            _User = _user;
            txtReceiverUsername.Text = _user.username;
            txtReceiverMail.Text = _user.email;
            txtReceiverKey.Text = new secClass(_User.username).key_generator(_User.ID);
            dbOperation = new dbClass("users.sqlite");
        }
        private void windowSender_Loaded(object sender, RoutedEventArgs e)
        {
            cmbSenderServer.Items.Add("SERVER"); //for placeholder
            cmbSenderServer.Items.Add("smtp.gmail.com"); //587 ssl+ start tls
            cmbSenderServer.Items.Add("smtp.office365.com"); //587 start tls
            cmbSenderServer.Items.Add("smtp.mail.yahoo.com"); //587 start tlss
            txtSenderPort.Text = "PORT";
            cmbSenderServer.SelectedIndex = 0;
            btnReceiverSend.IsEnabled = false;
        }
        private void cmbSenderServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbSenderServer.SelectedIndex == 0) txtSenderPort.Text = "PORT";
            else if (cmbSenderServer.SelectedIndex == 1) txtSenderPort.Text = "587";
            else if (cmbSenderServer.SelectedIndex == 2) txtSenderPort.Text = "587";
            else if (cmbSenderServer.SelectedIndex == 3) txtSenderPort.Text = "587";
        }
        private void btnSenderCheck_Click(object sender, RoutedEventArgs e)
        {
            bool _flag = false;
            try
            {
                string email = txtSenderMail.Text.ToLower().Trim();
                string password = pswdSenderMailPass.Password;
                if (new secClass(email).isEmail())
                {
                    if (txtSenderMail.Text.Length > 0 && pswdSenderMailPass.Password.Length > 0)
                    {
                        sc = new SmtpClient();
                        sc.Host = cmbSenderServer.SelectedItem.ToString();
                        sc.Port = Convert.ToInt32(txtSenderPort.Text);
                        sc.EnableSsl = true;
                        sc.Credentials = new NetworkCredential(txtSenderMail.Text, pswdSenderMailPass.Password);
                        sc.Send(new MailMessage(txtSenderMail.Text, "ug.recep.gunes@toros.edu.tr", "", ""));
                        _flag = true;
                        
                    }
                    else
                    {
                        MessageBox.Show("You have to full E-Mail and Passwords.", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show("Your E-Mail is not valid.", "E-Mail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            finally
            {
                if (_flag == true)
                {
                    btnReceiverSend.IsEnabled = true;
                    grdSender.IsEnabled = false;
                }
            }
        }
        private void btnReceiverSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mail = new MailMessage();
                mail.From = new MailAddress(txtSenderMail.Text, txtSenderMail.Text);
                mail.To.Add(txtReceiverMail.Text);
                mail.Subject = "Password Reset Key";
                mail.Body = $"Hello {txtReceiverUsername.Text};{Environment.NewLine}" +
                    $"Your key is {txtReceiverKey.Text}.{Environment.NewLine}";
                sc.Send(mail);
                _User.email = null;
                dbOperation.updateUser(_User);
                MessageBox.Show($"Mail was sended successfuly.{Environment.NewLine}" +
                    $"You'll exit in 3 second after clicking OK.","Mail",MessageBoxButton.OK,MessageBoxImage.Information);
                this.IsEnabled = false;
                System.Threading.Thread.Sleep(3000);
                this.Close();
                Owner.Focus();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"ERROR",MessageBoxButton.OK,MessageBoxImage.Exclamation);
            }
        }
    }
}
