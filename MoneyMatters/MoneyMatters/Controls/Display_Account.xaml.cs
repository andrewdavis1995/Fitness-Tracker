using Andrew_2_0_Libraries.Models;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MoneyMatters.Controls
{
    /// <summary>
    /// Interaction logic for Display_Workout.xaml
    /// </summary>
    public partial class Display_Account : UserControl
    {
        BankAccount _account;
    
        /// <summary>
        /// Constructor
        /// </summary>
        public Display_Account(BankAccount account)
        {
            InitializeComponent();
            _account = account;

            // setup display with correct data
            txtAccountName.Text = account.GetAccountName();
            txtBalance.Text = "£" + account.GetBalance().ToString("0.00");
            txtBankName.Text = account.GetBankName();
            txtAccountNumber.Text = account.GetAccountNumber().ToString();
            txtUpdated.Text = "Last Updated: " + account.GetUpdated().ToString("dd/MM/yyyy");

            try
            {
                imgIcon.Source = new BitmapImage(new Uri("pack://application:,,,/"
                    + Assembly.GetExecutingAssembly().FullName +
                    ";component/Images/Icons/" + account.GetBankName() + ".png"));
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Event handler for when the mouse enters this control
        /// </summary>
        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Highlight.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Event handler for when the mouse leaves this control
        /// </summary>
        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Highlight.Visibility = System.Windows.Visibility.Collapsed;
        }
        
        
        /// <summary>
        /// Accessor for the account linked to this display
        /// </summary>
        /// <returns>The linked account</returns>
        public BankAccount GetAccount()
        {
            return _account;
        }
    }
}
