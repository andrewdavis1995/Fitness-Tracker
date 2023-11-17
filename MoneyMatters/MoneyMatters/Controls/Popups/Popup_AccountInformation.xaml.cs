using Andrew_2_0_Libraries.Models;
using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MoneyMatters.Controls.Popups
{
    /// <summary>
    /// Interaction logic for Popup_CreateAccount.xaml
    /// </summary>
    public partial class Popup_AccountInformation : UserControl
    {
        Action<BankAccount> _confirmCallback;
        Action<BankAccount> _editCallback;
        BankAccount _bankAccount;

        public Popup_AccountInformation(Action<BankAccount> confirmCallback, Action<BankAccount> editCallback, BankAccount account)
        {
            InitializeComponent();

            _bankAccount = account;
            _confirmCallback = confirmCallback;
            _editCallback = editCallback;

            inpBalance.Initialise(true, 100000, 8, "0.00");

            txtAccountName.Text = account.GetAccountName();
            txtAccountNumber.Text = account.GetAccountNumber();
            inpBalance.txtContent.Text = account.GetBalance().ToString("0.00");
            txtBankName.Text = account.GetBankName();
            txtInterestRate.Text = account.GetInterestRate().ToString("0.00") + "%";

            inpBalance.SetColour(Color.FromRgb(255, 255, 255));

            switch (account.GetInterestType())
            {
                case InterestType.None:
                    txtInterestRate.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case InterestType.Monthly:
                    txtInterestRate.Text += " paid monthly";
                    break;
                case InterestType.Yearly:
                    txtInterestRate.Text += " paid yearly";
                    break;
            }

            // configure buttons
            cmdConfirm.Configure("Save and Close");
            cmdEdit.Configure("Edit");
        }

        /// <summary>
        /// Callback for the confirm button
        /// </summary>
        private void cmdConfirm_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _bankAccount.SetBalance(inpBalance.GetValue());
            _confirmCallback?.Invoke(_bankAccount);
        }

        /// <summary>
        /// Event handler for account number text input
        /// </summary>
        private void txtAccountNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // check that the value is a number
            var regexString = "[^0-9]";
            Regex regex = new Regex(regexString);
            e.Handled = (regex.IsMatch(e.Text));
        }

        /// <summary>
        /// Event handler for edit button
        /// </summary>
        private void cmdEdit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _editCallback.Invoke(_bankAccount);
        }
    }
}