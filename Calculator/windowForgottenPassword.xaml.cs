using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Security;
using Database;


namespace Calculator
{
    /// <summary>
    /// Interaction logic for windowForgottenPassword.xaml
    /// </summary>
    public partial class windowForgottenPassword : Window
    {
        public secClass secUsername;
        public secClass secPassword;
        public dbClass dbOperation;
        public dbClass.User changedUser;
        public windowForgottenPassword()
        {
            dbOperation = new dbClass("users.sqlite");
            InitializeComponent();
        }
        private void wndwForgottenPassword_Loaded(object sender, RoutedEventArgs e)
        {
            grpPasswordReset.IsEnabled = false;
        }

        private void btnKeyGet_Click(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            string username = txtbxKeyGetUsername.Text.ToLower().Trim();
            string email = txtbxKeyGetMail.Text.ToLower().Trim();
            secClass secUsername = new secClass(username);
            List<dbClass.User> lstUsers = new List<dbClass.User>();
            if(secUsername.isUsername().All<bool>(value => value == false))
            {
                if (new secClass(email).isEmail())
                {
                    lstUsers = dbOperation.getAllUser();
                    foreach (dbClass.User _temp in lstUsers)
                    {
                        if (_temp.username == username)
                        {
                            flag = true;
                            changedUser = _temp;
                            txtbxKeyCheckUsername.Text = username;
                            txtbxKeyGetUsername.Clear();
                            txtbxKeyGetMail.Clear();
                        }
                    }
                    if(flag == true)
                    {
                        changedUser.email = email;
                        dbOperation.updateUser(changedUser);
                    }
                    else
                    {
                        MessageBox.Show("User wasn't found in database!", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        txtbxKeyGetUsername.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("Your E-Mail is not valid.", "E-Mail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtbxKeyGetMail.Clear();
                }
            }
            else
            {
                string strMessage = string.Empty;
                string strHeader = "Username Error";
                if (secUsername.isUsername()[0] == true)
                    strMessage += $"Your username doesn't contain {secUsername._rChar}.{Environment.NewLine}";
                if (secUsername.isUsername()[1] == true)
                    strMessage += $"Your username length must be equal or higher than 3(Three).{Environment.NewLine}";
                if (secUsername.isUsername()[2] == true)
                    strMessage += $"Your username doesn't begin with number.{Environment.NewLine}";
                MessageBox.Show(strMessage, strHeader, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                txtbxKeyGetUsername.Clear();
            }
        }
        private void btnCheckKey_Click(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            string username = txtbxKeyCheckUsername.Text.Trim().ToLower();
            string key = txtbxKeyCheckKey.Text.Trim().ToLower();
            secUsername = new secClass(username);
            if(secUsername.isUsername().All<bool>(value => value == false))
            {
                if (key.Length == 8 && new secClass(key).badCharChecker() == false)
                {
                    List<dbClass.User> allUsers = dbOperation.getAllUser();
                    foreach (dbClass.User temp in allUsers)
                    {
                        if (temp.username == username)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag == true)
                    {
                        changedUser = dbOperation.getUser(username);
                        if (key == secUsername.key_generator(changedUser.ID))
                        {
                            txtbxKeyCheckUsername.Clear();
                            txtbxKeyCheckKey.Clear();
                            grpKeyCheck.IsEnabled = false;
                            grpPasswordReset.IsEnabled = true;
                            txtblckResetUsername.Text = changedUser.username;
                            txtbxKeyGetUsername.Clear();
                            txtbxKeyGetMail.Clear();
                            txtbxKeyCheckUsername.Clear();
                            txtbxKeyCheckKey.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Your key is wrong.","Key",MessageBoxButton.OK, MessageBoxImage.Warning);
                            txtbxKeyCheckKey.Clear();
                        }
                    }
                    else
                    {
                        MessageBox.Show("The user doesn't find.", "User", MessageBoxButton.OK, MessageBoxImage.Warning);
                        txtbxKeyCheckUsername.Clear();
                        txtbxKeyCheckKey.Clear();
                    }
                }
                else
                {
                    secClass _secKey = new secClass(txtbxKeyCheckKey.Text);
                    string _message = txtbxKeyCheckKey.Text.Length != 8 ? $"Key Lengths must be 8(Eight).{Environment.NewLine}" : string.Empty;
                    _message += _secKey.badCharChecker() ? $"Key doesn't contain the char which is {_secKey._rChar}." : string.Empty;
                    string _header = "Key";
                    MessageBox.Show(_message, _header, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                string strMessage = string.Empty;
                string strHeader = "Username Error";
                if (secUsername.isUsername()[0] == true)
                    strMessage += $"Your username doesn't contain {secUsername._rChar}.{Environment.NewLine}";
                if (secUsername.isUsername()[1] == true)
                    strMessage += $"Your username length must be equal or higher than 3(Three).{Environment.NewLine}";
                if (secUsername.isUsername()[2] == true)
                    strMessage += $"Your username doesn't begin with number.{Environment.NewLine}";
                MessageBox.Show(strMessage, strHeader, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                txtbxKeyCheckUsername.Clear();
            }
        }
        private void btnResetSaveIt_Click(object sender, RoutedEventArgs e)
        {
            string password1 = pswdResetPassword1.Password.Trim();
            string password2 = pswdResetPassword2.Password.Trim();
            if(password1 == password2)
            {
                secPassword = new secClass(password1);
                if(secPassword.isPassword().All<bool>(value => value == false))
                {
                    changedUser.password = secPassword.md5Generator();
                    changedUser.email = null;
                    dbOperation.updateUser(changedUser);
                    MessageBox.Show("You password was updated successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    Owner.Visibility = Visibility.Visible;
                    this.Visibility = Visibility.Collapsed;
                }
                else
                {
                    string strMessage = string.Empty;
                    string strHeader = "Password Error";
                    if (secPassword.isPassword()[0] == true)
                        strMessage += $"Your password doesn't contain {secPassword._rChar}.{Environment.NewLine}";
                    if (secPassword.isPassword()[1] == true)
                        strMessage += $"Your password length must be equal or higher than 8(Eight).{Environment.NewLine}";
                    if (secPassword.isPassword()[2] == true)
                        strMessage += $"Lower case char should exist in your password at least 1(One).{Environment.NewLine}";
                    if (secPassword.isPassword()[3] == true)
                        strMessage += $"Upper case char should exist in your password at least 1(One).{Environment.NewLine}";
                    if (secPassword.isPassword()[4] == true)
                        strMessage += $"Number should exist in your password at least 1(One).{Environment.NewLine}";
                    MessageBox.Show(strMessage, strHeader, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Both of passwords are not equal.", "Password", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void wndwForgottenPassword_Closed(object sender, EventArgs e)
        {
            if (this.Visibility != Visibility.Hidden && Owner.Visibility != Visibility.Visible)
            {
                Owner.Visibility = Visibility.Visible;
                this.Visibility = Visibility.Hidden;
            }
        }

        private void wndwForgottenPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                if (this.Visibility != Visibility.Hidden && Owner.Visibility != Visibility.Visible)
                {
                    Owner.Visibility = Visibility.Visible;
                    this.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
