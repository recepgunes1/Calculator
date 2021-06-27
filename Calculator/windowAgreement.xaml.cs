using System;
using System.Windows;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for windowAgreement.xaml
    /// </summary>
    public partial class windowAgreement : Window
    {
        public windowAgreement()
        {
            InitializeComponent();
        }
        private void wndwAgreement_Loaded(object sender, RoutedEventArgs e)
        {
            string strRules = $"   When I registered the application, I accepted these rules;{Environment.NewLine}" +
                $" 1. I never try to hack the app.{Environment.NewLine}" +
                $" 2. I will use everytime this.{Environment.NewLine}" +
                $" 3. I allow using my datas.{Environment.NewLine}" +
                $" 4. Some new rules will come.";
            txtblckRules.Text = strRules;
        }
    }
}
