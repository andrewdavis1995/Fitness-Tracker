using Andrew_2_0_Libraries.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Net;
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

        // TODO: DEPRECATE THIS

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

            if (_bankAccount != null)
                LoadExistingData_();
        }

        /// <summary>
        /// Loads the bank details into the displays
        /// </summary>
        private void LoadExistingData_()
        {
            txtAccountName.Text = _bankAccount.GetAccountName();
            txtBankName.Text = _bankAccount.GetBankName();
            txtAccountNumber.Text = _bankAccount.GetAccountNumber();
            inpInterest.SetInputValue((float)_bankAccount.GetInterestRate());

            cmbInterestType.SelectedIndex = (int)_bankAccount.GetInterestType();
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

            cmbInterestType.SelectionChanged += (s, e) => 
            {
                var selected = cmbInterestType.SelectedItem as ComboBoxItem;
                var interestType = Enum.Parse(typeof(InterestType), selected?.Content.ToString());
                switch(interestType)
                {
                    case InterestType.None:
                    case InterestType.Varying:
                        inpInterest.Visibility = System.Windows.Visibility.Collapsed;
                        lblRate.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case InterestType.Yearly:
                    case InterestType.Monthly:
                        inpInterest.Visibility = System.Windows.Visibility.Visible;
                        lblRate.Visibility = System.Windows.Visibility.Visible;
                        break;
                }
            };

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
            if (valid) valid = txtAccountNumber.Text.Length > 2 || string.IsNullOrEmpty(txtAccountNumber.Text);
            if (valid) valid = txtBankName.Text.Length > 2;

            if (valid)
            {
                // use existing details if there are any
                var balance = _bankAccount != null ? (double)_bankAccount.GetBalance() : 0d;
                Guid id = _bankAccount != null ? _bankAccount.GetAccountId() : Guid.NewGuid();
                var interestType = Enum.Parse<InterestType>((cmbInterestType.SelectedItem as ComboBoxItem)?.Tag?.ToString());

                // return the account
                var account = new BankAccount(id, txtAccountName.Text, txtBankName.Text, txtAccountNumber.Text,
                    balance, inpInterest.GetValue(), interestType, DateTime.Now, new DateTime(), AccountType.Savings);
                _confirmCallback?.Invoke(account);

                // TODO: input for maturing date
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