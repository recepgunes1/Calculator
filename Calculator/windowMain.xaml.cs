using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.IO;
using Security;
using Database;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public dbClass dbOperation;
        public dbClass.User loggedIn;
        public dbClass.User selectedUser;
        public windowAgreement agreement;
        public windowForgottenPassword forgottenPassword;
        public windowMailSender mailSender;
        public int intTempID;
        public List<Window> lstwMailSender = new List<Window>();
        public MainWindow()
        {
            InitializeComponent();
            dbOperation = new dbClass("users.sqlite");
            agreement = new windowAgreement();
        }
        //Other Methods
        private void afterLogin(dbClass.User _user)
        {
            txtRegisterUsername.Clear();
            pswdRegisterPassword.Clear();
            pswdRegisterPassword2.Clear();
            txtLoginUsername.Clear();
            pswdLoginPassword.Clear();
            chckAgreemnt.IsChecked = false;
            tbCalculator.IsEnabled = true;
            if (dbOperation.isAdmin(_user.username) == true)
            {
                tbAdmin.IsEnabled = true;
                chckRemoveLogs.IsChecked = false;
            }
            txtblckUsername.Text = _user.username;
            if (_user.logs != null)
            {
                for (int i = 0; i < _user.logs.Count; i++)
                {
                    lstCalculatorLogs.Items.Add(_user.logs[i]);
                }
            }
            mainTab.SelectedIndex = 1;
            tbLoginRegister.IsEnabled = false;
        }
        private void userListRefresh()
        {
            lstvwInfoUsers.Items.Clear();
            List<dbClass.User> allUsers = dbOperation.getAllUser();
            for (int i = 0; i < allUsers.Count; i++)
            {
                ListViewItem tempItem = new ListViewItem
                {
                    Content = allUsers[i].username,
                    Foreground = allUsers[i].situation == 1 ? Brushes.DarkGreen : Brushes.DarkRed
                };
                tempItem.Foreground = allUsers[i].email != null ? Brushes.Purple : tempItem.Foreground; 
                lstvwInfoUsers.Items.Add(tempItem);
            }
        }
        private void Login()
        {
            bool blFlag = false;
            List<bool> blSecResult = new List<bool>();
            string strUsername = txtLoginUsername.Text.ToLower().Trim();
            string pswdPassword = pswdLoginPassword.Password.Trim();
            string strMessage = string.Empty, strHeader = string.Empty;
            secClass secUsername = new secClass(strUsername);
            secClass secPassword = new secClass(pswdPassword);
            List<dbClass.User> lstAllUser = dbOperation.getAllUser();
            dbClass.User usrTempUser;
            foreach (dbClass.User usrItem in lstAllUser)
                if (usrItem.username == strUsername)
                    blFlag = true;
            if(secUsername.isUsername().All<bool>(value => value == false))
            {
                if (blFlag == true) 
                {
                    if (secPassword.isPassword().All<bool>(value => value == false)) 
                    {
                        usrTempUser = dbOperation.getUser(strUsername);
                        if(secPassword.md5Generator() == usrTempUser.password) 
                        {
                            if(usrTempUser.situation == 1) 
                            {
                                loggedIn = usrTempUser;
                                loggedIn.last_login = DateTime.Now.ToString();
                                intTempID = loggedIn.ID;
                                dbOperation.updateUser(loggedIn);
                                afterLogin(loggedIn);
                            }
                            else
                            {
                                MessageBox.Show("Your account was disabled.", "Account", MessageBoxButton.OK, MessageBoxImage.Information);
                                loginClear();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Your password was wrong.", "Password", MessageBoxButton.OK, MessageBoxImage.Warning);
                            pswdLoginPassword.Clear();
                        }
                    }
                    else
                    {
                        blSecResult.AddRange(secPassword.isPassword());
                        strHeader = "Password Error";
                        if (blSecResult[0] == true)
                            strMessage += $"Your password doesn't contain {secPassword._rChar}.{Environment.NewLine}";
                        if (blSecResult[1] == true)
                            strMessage += $"Your password length must be equal or higher than 8(Eight).{Environment.NewLine}";
                        if (blSecResult[2] == true)
                            strMessage += $"Lower case char should exist in your password at least 1(One).{Environment.NewLine}";
                        if (blSecResult[3] == true)
                            strMessage += $"Upper case char should exist in your password at least 1(One).{Environment.NewLine}";
                        if (blSecResult[4] == true)
                            strMessage += $"Number should exist in your password at least 1(One).{Environment.NewLine}";
                        MessageBox.Show(strMessage, strHeader, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        loginClear();


                    }
                }
                else
                {
                    MessageBox.Show("Your account wasn't found.", "Account", MessageBoxButton.OK, MessageBoxImage.Information);
                    loginClear();
                }
            }
            else
            {
                blSecResult.AddRange(secUsername.isUsername());
                strHeader = "Username Error";
                if (blSecResult[0] == true)
                    strMessage += $"Your username doesn't contain {secUsername._rChar}.{Environment.NewLine}";
                if (blSecResult[1] == true)
                    strMessage += $"Your username length must be equal or higher than 3(Three).{Environment.NewLine}";
                if (blSecResult[2] == true)
                    strMessage += $"Your username doesn't begin with number.{Environment.NewLine}";
                MessageBox.Show(strMessage, strHeader, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                loginClear();
            }
        }
        private void Register()
        {
            bool blFlag = false;
            List<bool> blSecResult = new List<bool>();
            string strUsername = txtRegisterUsername.Text.ToLower().Trim();
            string pswdPassword = pswdRegisterPassword.Password.Trim();
            string pswdPassword2 = pswdRegisterPassword2.Password.Trim();
            string strMessage = string.Empty, strHeader = string.Empty;
            secClass secUsername = new secClass(strUsername);
            secClass secPassword = new secClass(pswdPassword);
            List<dbClass.User> lstAllUser = dbOperation.getAllUser();
            dbClass.User usrRegisteredUser = new dbClass.User();
            foreach (dbClass.User usrItem in lstAllUser)
                if (usrItem.username == strUsername)
                    blFlag = true;
            if (chckAgreemnt.IsChecked == true)
            {
                if (secUsername.isUsername().All<bool>(value => value == false))
                {
                    if (pswdPassword == pswdPassword2)
                    {
                        if (secPassword.isPassword().All<bool>(value => value == false))
                        {
                            if (blFlag != true)
                            {
                                usrRegisteredUser.username = strUsername;
                                usrRegisteredUser.password = secPassword.md5Generator();
                                usrRegisteredUser.last_login = DateTime.Now.ToString();
                                dbOperation.registerUser(usrRegisteredUser);
                                loggedIn = dbOperation.getUser(strUsername);
                                intTempID = loggedIn.ID;
                                afterLogin(loggedIn);
                            }
                            else
                            {
                                MessageBox.Show("Your account already registered.", "Account", MessageBoxButton.OK, MessageBoxImage.Information);
                                registerClear();
                            }
                        }
                        else
                        {
                            blSecResult.AddRange(secPassword.isPassword());
                            strHeader = "Password Error";
                            if (blSecResult[0] == true)
                                strMessage += $"Your password doesn't contain {secPassword._rChar}.{Environment.NewLine}";
                            if (blSecResult[1] == true)
                                strMessage += $"Your password length must be equal or higher than 8(Eight).{Environment.NewLine}";
                            if (blSecResult[2] == true)
                                strMessage += $"Lower case char should exist in your password at least 1(One).{Environment.NewLine}";
                            if (blSecResult[3] == true)
                                strMessage += $"Upper case char should exist in your password at least 1(One).{Environment.NewLine}";
                            if (blSecResult[4] == true)
                                strMessage += $"Number should exist in your password at least 1(One).{Environment.NewLine}";
                            MessageBox.Show(strMessage, strHeader, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            registerClear();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Both of passwords aren't equal.", "Password", MessageBoxButton.OK, MessageBoxImage.Information);
                        pswdRegisterPassword.Clear();
                        pswdRegisterPassword2.Clear();
                    }
                }
                else
                {
                    blSecResult.AddRange(secUsername.isUsername());
                    strHeader = "Username Error";
                    if (blSecResult[0] == true)
                        strMessage += $"Your username doesn't contain {secUsername._rChar}.{Environment.NewLine}";
                    if (blSecResult[1] == true)
                        strMessage += $"Your username length must be equal or higher than 3(Three).{Environment.NewLine}";
                    if (blSecResult[2] == true)
                        strMessage += $"Your username doesn't begin with number.{Environment.NewLine}";
                    MessageBox.Show(strMessage, strHeader, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtRegisterUsername.Clear();
                    pswdRegisterPassword.Clear();
                    pswdRegisterPassword2.Clear();
                }
            }
            else
            {
                MessageBox.Show("You have to accept agreement.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void ExitSession()
        {
            lstCalculatorLogs.Items.Clear();
            txtCalculatorOperation.Clear();
            lstvwInfoUsers.Items.Clear();
            txtInfoUsername.Clear();
            txtInfoPassword.Clear();
            chckRemoveLogs.IsChecked = false;
            tbAdmin.IsEnabled = false;
            tbCalculator.IsEnabled = false;
            tbLoginRegister.IsEnabled = true;
            mainTab.SelectedIndex = 0;
            if (lstwMailSender != null)
            {
                foreach (var vrItem in lstwMailSender)
                {
                    vrItem.Close();
                }
            }
            loggedIn = new dbClass.User();
        }
        private void ExitProgram()
        {
            MessageBoxResult boxResult = MessageBox.Show("Do you want to exit application?", "Exit Application",
                                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (boxResult == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
        }
        private void operationWriter(object sender, RoutedEventArgs e)
        {
            try
            {
                txtCalculatorOperation.Focus();
                Button myButton = (Button)sender;
                string strChar = myButton.Content.ToString();
                if(strChar == "=")
                {
                    calculate();
                }
                else if (strChar == "C")
                {
                    txtCalculatorOperation.Clear();
                }
                else if(strChar == "CE")
                {
                    if (txtCalculatorOperation.Text.Length > 0)
                    {
                        txtCalculatorOperation.Text = txtCalculatorOperation.Text.Substring(0, txtCalculatorOperation.Text.Length - 1);
                    }
                }
                else
                {
                    strChar = strChar == "X" ? "*" : strChar;
                    txtCalculatorOperation.Text += strChar;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                txtCalculatorOperation.SelectionStart = txtCalculatorOperation.Text.Length;
            }
        }
        private void calculate()
        {
            try
            {
                bool _flag = true;
                char[] arrayOfOperators = new char[4] { '+', '-', '*', '/' };
                DataTable dataTable = new DataTable();
                double dbResult;
                string strInput = txtCalculatorOperation.Text.Trim().ToLower();
                string strTemp = string.Empty;
                List<string> logs = new List<string>();
                List<string> vs = new List<string>(strInput.Split(arrayOfOperators));
                if (strInput.Contains(",")) strInput = strInput.Replace(",", string.Empty);
                dbResult = Convert.ToDouble(dataTable.Compute(strInput, string.Empty));
                if (vs.Count < 2 && Convert.ToDouble(vs[0]) == dbResult)
                    _flag = false;
                if (_flag && !strInput.Contains("/0"))
                {
                    strTemp = strInput + " = " + dbResult.ToString("N2");
                    txtCalculatorOperation.Text = dbResult.ToString("N2");
                    lstCalculatorLogs.Items.Insert(0, strTemp);
                    if (loggedIn.logs == null) 
                        loggedIn.logs = new List<string>();
                    loggedIn.logs.Insert(0, strTemp);
                    dbOperation.updateUser(loggedIn);
                    txtCalculatorOperation.Text = dbResult.ToString("N2");
                }
                else
                {
                    MessageBox.Show("You can't calculate the operation.", "Unexcepted Situation", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCalculatorOperation.Clear();
                }
            }
            catch(Exception ex)
            {
                StreamWriter streamWriter = new StreamWriter("debug.txt");
                streamWriter.WriteLine(ex.Message+Environment.NewLine);
                streamWriter.Close();
                MessageBox.Show("An unexpected error has occurred.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                txtCalculatorOperation.Clear();
            }
            finally
            {
                txtCalculatorOperation.SelectionStart = txtCalculatorOperation.Text.Length;
            }
        }
        private void loginClear()
        {
            txtLoginUsername.Clear();
            pswdLoginPassword.Clear();
        }
        private void registerClear()
        {
            txtRegisterUsername.Clear();
            pswdRegisterPassword.Clear();
            pswdRegisterPassword2.Clear();
            if (chckAgreemnt.IsChecked == true)
                chckAgreemnt.IsChecked = false;
        }
        private void infoSaveClear()
        {
            lstvwInfoUsers.SelectedIndex = -1;
            txtInfoUsername.Clear();
            txtInfoPassword.Clear();
            txtInfoLastLogin.Clear();
            lstInfoLogs.Items.Clear();
            chckRemoveLogs.IsChecked = false;
            rdInfoSituationE.IsChecked = false;
            rdInfoSituationD.IsChecked = false;
            grdUserInfo.IsEnabled = false;
        }
        //Window Events
        private void wndwMain_Loaded(object sender, RoutedEventArgs e)
        {
            agreement.Owner = this;
            tbCalculator.IsEnabled = false;
            tbAdmin.IsEnabled = false;
            chckAgreemnt.IsChecked = false;
            chckRemoveLogs.IsChecked = false;
            grdUserInfo.IsEnabled = false;
        }
        private void wndwMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (loggedIn.username != null)
                {
                    MessageBoxResult boxResult = MessageBox.Show("Do you want to exit current session?", "Exit Session",
                                MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (boxResult == MessageBoxResult.Yes)
                    {
                        ExitSession();
                    }
                }
                else
                {
                    ExitProgram();
                }
            }
            else if (e.Key == Key.Enter && (txtLoginUsername.IsSelectionActive == true || pswdLoginPassword.IsSelectionActive == true))
            {
                Login();
            }
            else if(e.Key == Key.Enter && (txtRegisterUsername.IsSelectionActive == true || pswdRegisterPassword.IsSelectionActive == true || pswdRegisterPassword2.IsSelectionActive == true))
            {
                Register();
            }
            else if (e.Key == Key.F5 && loggedIn.username != null)
            {
                loggedIn = dbOperation.getUser(intTempID);
                if (loggedIn.situation == 1)
                {
                    lstCalculatorLogs.Items.Clear();
                    if(loggedIn.logs != null)
                        loggedIn.logs.ForEach(item => lstCalculatorLogs.Items.Add(item));
                    txtblckUsername.Text = loggedIn.username;
                    if(loggedIn.ID == 1)
                    {
                        txtUserSearch.Clear();
                        txtInfoUsername.Clear();
                        txtInfoPassword.Clear();
                        lstInfoLogs.Items.Clear();
                        lstvwInfoUsers.Items.Clear();
                        txtInfoLastLogin.Clear();
                        chckRemoveLogs.IsChecked = false;
                        rdInfoSituationE.IsChecked = false;
                        rdInfoSituationD.IsChecked = false;
                        userListRefresh();
                    }
                }
                else
                {
                    MessageBox.Show("Your accoun't was disabled.", "User", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ExitSession();
                }
            }
        }
        //Login Screen Events
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Register();
        }
        private void chckAgreemnt_MouseEnter(object sender, MouseEventArgs e)
        {
            agreement.Left = this.Left + 310;
            agreement.Top = this.Top + 290;;
            agreement.Visibility = Visibility.Visible;
        }
        private void chckAgreemnt_MouseLeave(object sender, MouseEventArgs e)
        {
            agreement.Visibility = Visibility.Collapsed;
        }
        private void lblLoginForgetPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            forgottenPassword = new windowForgottenPassword();
            forgottenPassword.Owner = this;
            forgottenPassword.Visibility = Visibility.Visible;
            this.Visibility = Visibility.Hidden;
        }
        //Calculate Screen Events
        private void btnExitSession_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult boxResult = MessageBox.Show("Do you want to exit from current session?", "Exit Session",
                              MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (boxResult == MessageBoxResult.Yes)
            {
                ExitSession();
            }
        }
        private void btnExitProgram_Click(object sender, RoutedEventArgs e)
        {
            ExitProgram();
        }

        private void btnClearLogs_Click(object sender, RoutedEventArgs e)
        {
            loggedIn.logs = null;
            dbOperation.updateUser(loggedIn);
            lstCalculatorLogs.Items.Clear();
        }
        private void txtCalculatorOperation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                calculate();
            }
        }
        private void txtCalculatorOperation_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtCalculatorOperation.Text.Length > 1)
            {
                string _isEqual = txtCalculatorOperation.Text.Substring(txtCalculatorOperation.Text.Length - 1);
                if (_isEqual == "=")
                {
                    txtCalculatorOperation.Text = txtCalculatorOperation.Text.Replace(_isEqual, string.Empty);
                    calculate();
                }
            }
        }
        //Admin Screen Events
        private void btnInfoSave_Click(object sender, RoutedEventArgs e)
        {
            List<bool> blSecResult = new List<bool>();
            DateTime dtTempDate;
            string strUsername = txtInfoUsername.Text.ToLower().Trim();
            string strPassword = txtInfoPassword.Text.Trim().ToLower();
            List<string> lstLogs = chckRemoveLogs.IsChecked == true ? null : lstInfoLogs.Items.Cast<string>().ToList();
            int intSituation = rdInfoSituationE.IsChecked == true ? 1 : 0;
            string strMessage = string.Empty, strHeader = string.Empty;
            secClass secUsername = new secClass(strUsername);
            secClass secPassword = new secClass(strPassword);
            dbClass.User usrTempUser = dbOperation.getUser(lstvwInfoUsers.SelectedIndex+1);
            if (secUsername.isUsername().All<bool>(value => value == false))
            {
                if (secPassword.isMD5())
                {
                    usrTempUser.username = strUsername;
                    usrTempUser.password = strPassword;
                    usrTempUser.logs = lstLogs;
                    usrTempUser.situation = intSituation;
                    usrTempUser.last_login = DateTime.TryParse(txtInfoLastLogin.Text, out dtTempDate) ? txtInfoLastLogin.Text : usrTempUser.last_login;
                    dbOperation.updateUser(usrTempUser);
                    userListRefresh();
                    if(intTempID == 1)
                    {
                        loggedIn = dbOperation.getUser(intTempID);
                        txtblckUsername.Text = loggedIn.username;
                    }
                }
                else
                {
                    MessageBox.Show("Your password doesn't look like MD5.", "Passwowrd", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                blSecResult.AddRange(secUsername.isUsername());
                strHeader = "Username Error";
                if (blSecResult[0] == true)
                    strMessage += $"Your username doesn't contain {secUsername._rChar}.{Environment.NewLine}";
                if (blSecResult[1] == true)
                    strMessage += $"Your username length must be equal or higher than 3(Three).{Environment.NewLine}";
                if (blSecResult[2] == true)
                    strMessage += $"Your username doesn't begin with number.{Environment.NewLine}";
                MessageBox.Show(strMessage, strHeader, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            infoSaveClear();
        }
        private void lstvwInfoUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstvwInfoUsers.SelectedIndex != -1)
            {
                grdUserInfo.IsEnabled = true;
                lstInfoLogs.Items.Clear();
                string _username = lstvwInfoUsers.SelectedItem.ToString().Split(':')[1].Trim();
                selectedUser = dbOperation.getUser(_username);
                secClass _key = new secClass(selectedUser.username);
                txtInfoUsername.Text = selectedUser.username;
                txtInfoPassword.Text = selectedUser.password;
                if (selectedUser.logs != null)
                {
                    for (int i = 0; i < selectedUser.logs.Count; i++)
                    {
                        lstInfoLogs.Items.Add(selectedUser.logs[i]);
                    }
                }
                rdInfoSituationE.IsChecked = selectedUser.situation == 1 ? true : false;
                rdInfoSituationD.IsChecked = selectedUser.situation == 0 ? true : false;
                txtInfoLastLogin.Text = Convert.ToDateTime(selectedUser.last_login).ToString();
            }
        }
        private void txtUserSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string pattern = txtUserSearch.Text;
            List<dbClass.User> searchList = dbOperation.getAllUser();
            List<dbClass.User> result = new List<dbClass.User>();
            foreach (dbClass.User item in searchList)
            {
                if (item.username.Contains(pattern))
                {
                    result.Add(item);
                }
            }
            lstvwInfoUsers.Items.Clear();
            foreach (dbClass.User item in result)
            {
                ListViewItem tempItem = new ListViewItem { Content = item.username,
                    Foreground = item.situation == 1 ? Brushes.DarkGreen : Brushes.DarkRed };
                tempItem.Foreground = item.email == null ? tempItem.Foreground : Brushes.Purple;
                lstvwInfoUsers.Items.Add(tempItem);
            }
            infoSaveClear();
        }
        private void grdAdmin_Loaded(object sender, RoutedEventArgs e)
        {
            userListRefresh();
        }
        private void lstvwInfoUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstvwInfoUsers.SelectedIndex != -1)
            {
                mailSender = new windowMailSender(selectedUser);
                mailSender.Owner = this;
                mailSender.Show();
                lstwMailSender.Add(mailSender);
            }
        }
    }
}
