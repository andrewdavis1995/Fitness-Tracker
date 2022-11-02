using Andrew_2_0_Libraries.Models;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace MoneyMatters.Controls.Popups
{
    /// <summary>
    /// Interaction logic for Popup_CreateAccount.xaml
    /// </summary>
    public partial class Popup_CreateAccount : UserControl
    {
        Action _cancelCallback;
        Action<BankAccount> _confirmCallback;
        BankAccount? _bankAccount;

        public Popup_CreateAccount(Action cancelCallback, Action<BankAccount> confirmCallback, BankAccount? account = null)
        {
            InitializeComponent();

            _bankAccount = account;
            _cancelCallback = cancelCallback;
            _confirmCallback = confirmCallback;

            LoadTypes_();

            // configure buttons
            cmdCancel.Configure("Cancel");
            cmdConfirm.Configure("Confirm");

            // configure numeric inputs
            inpInterest.Initialise(true, 20, 4);
        }

        /// <summary>
        /// Loads the types of measurements that can be used into the combobox
        /// </summary>
        private void LoadTypes_()
        {
            cmbInterestType.Items.Clear();

            var types = Enum.GetValues(typeof(InterestType));
            foreach (var t in types)
            {
                // add an item for each type
                cmbInterestType.Items.Add(new ComboBoxItem() { Content = Enum.GetName(typeof(InterestType), t), Tag = t });
            }
            cmbInterestType.SelectedIndex = 0;
        }

        /// <summary>
        /// Caallback for the cancel button
        /// </summary>
        private void cmdCancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _cancelCallback?.Invoke();
        }

        /// <summary>
        /// Callback for the confirm button
        /// </summary>
        private void cmdConfirm_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // validate input
            bool valid = txtAccountName.Text.Length > 2;
            if (valid) valid = txtAccountNumber.Text.Length > 2;
            if (valid) valid = txtBankName.Text.Length > 2;

            if (valid)
            {
                // use existing details if there are any
                var balance = _bankAccount != null ? (double)_bankAccount.GetBalance() : 0d;
                Guid id = _bankAccount != null ? _bankAccount.GetAccountId() : Guid.NewGuid();
                var interestType = Enum.Parse<InterestType>((cmbInterestType.SelectedItem as ComboBoxItem)?.Tag?.ToString());

                // return the account
                var account = new BankAccount(id, txtAccountName.Text, txtBankName.Text, txtAccountNumber.Text,
                    balance, inpInterest.GetValue(), interestType, DateTime.Now);
                _confirmCallback?.Invoke(account);
            }
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
    }
}