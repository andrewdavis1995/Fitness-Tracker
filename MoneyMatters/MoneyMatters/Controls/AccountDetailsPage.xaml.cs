using Andrew_2_0_Libraries.Models;
using MoneyMatters.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MoneyMatters.Controls
{
    /// <summary>
    /// Interaction logic for AccountDetailsPage.xaml
    /// </summary>
    public partial class AccountDetailsPage : UserControl
    {
        BankAccount? _account = null;
        bool _initialised = false;
        bool _loaded = false;
        HomePage _homePage;
        bool _initialCreation = false;

        public AccountDetailsPage()
        {
            InitializeComponent();
            cmdSave.Configure("Save");
            cmdSave.MouseLeftButtonDown += CmdSave_MouseLeftButtonDown;
        }

        private void CmdSave_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SaveDetails_(true);
        }

        public void Display(BankAccount account, HomePage homePage)
        {
            _account = account;

            Display_(homePage);

            lblAccountName.Initialise(account.GetAccountName(), 30);
            lblBalance.Initialise(account.GetBalance(), 1000000, true);
            lblAccountNumber.Initialise(account.GetAccountNumber(), 20);
            lblInterest.Initialise(account.GetInterestRate(), 10, true);
            datePicker.SelectedDate = account.GetMaturing();
            _initialCreation = false;

            SetActiveComboBoxItems_(account);
            AccountTypeChanged_();  // ensure necessary controls are shown/hidden

            cmdSave.Visibility = Visibility.Collapsed;

            _loaded = true;
        }

        public void Display(HomePage homePage)
        {
            Display_(homePage);

            _account = null;

            lblAccountName.Initialise("Account Name", 30);
            lblBalance.Initialise(0, 1000000, true);
            lblAccountNumber.Initialise("0", 20);
            lblInterest.Initialise(0, 10, true);
            _initialCreation = true;
            AccountTypeChanged_();  // ensure necessary controls are shown/hidden
            cmdSave.Visibility = Visibility.Visible;

            _loaded = true;
        }

        /// <summary>
        /// Sets active items in the combo box items
        /// </summary>
        /// <param name="account"></param>
        private void SetActiveComboBoxItems_(BankAccount account)
        {
            foreach (CustomComboBoxItem item in cmbBank.Items)
            {
                if (item.ToString() == account.GetBankName())
                    cmbBank.SelectedItem = item;
            }

            foreach (CustomComboBoxItem item in cmbAccountType.Items)
            {
                var type = (AccountType)item.Tag;
                if (type == account.GetAccountType())
                    cmbAccountType.SelectedItem = item;
            }

            foreach (CustomComboBoxItem item in cmbInterestType.Items)
            {
                var type = (InterestType)item.Tag;
                if (type == account.GetInterestType())
                    cmbInterestType.SelectedItem = item;
            }
        }

        void Display_(HomePage homePage)
        {
            _loaded = false;
            _homePage = homePage;

            cmbBank.SelectedItem = null;
            cmbBank.Items.Clear();
            foreach (var bank in BankNameHelper.GetBankNames())
            {
                var cbi = new CustomComboBoxItem(bank, true);
                cmbBank.Items.Add(cbi);
            }
            cmbBank.SelectedIndex = 0;

            cmbInterestType.SelectedItem = null;
            cmbInterestType.Items.Clear();
            foreach (var value in Enum.GetValues(typeof(InterestType)))
            {
                var cbi = new CustomComboBoxItem(value.ToString(), false);
                cbi.Tag = value;
                cmbInterestType.Items.Add(cbi);
            }
            cmbInterestType.SelectedIndex = 0;

            cmbAccountType.SelectedItem = null;
            cmbAccountType.Items.Clear();
            foreach (var value in Enum.GetValues(typeof(AccountType)))
            {
                var cbi = new CustomComboBoxItem(value.ToString(), false);
                cbi.Tag = value;
                cmbAccountType.Items.Add(cbi);
            }
            cmbAccountType.SelectedIndex = 0;

            cmdClose.Initialise(() => { Visibility = Visibility.Collapsed; }, "cross");

            (cmbBank.Items[cmbBank.Items.Count - 1] as CustomComboBoxItem).Style = FindResource("HoverFormatLast") as Style;
            (cmbInterestType.Items[cmbInterestType.Items.Count - 1] as CustomComboBoxItem).Style = FindResource("HoverFormatLast") as Style;
            (cmbAccountType.Items[cmbAccountType.Items.Count - 1] as CustomComboBoxItem).Style = FindResource("HoverFormatLast") as Style;

            if (!_initialised)
            {
                lblAccountName.Scale(1.5f);
                lblBalance.Scale(1.75f);
                AssignCallbacks_();
                _initialised = true;
            }

            Visibility = Visibility.Visible;
        }

        private void AssignCallbacks_()
        {
            lblAccountName.AddCallback(() => SaveDetails_());
            lblAccountNumber.AddCallback(() => SaveDetails_());
            lblBalance.AddCallback(() => SaveDetails_());
            lblInterest.AddCallback(() => SaveDetails_());

            cmbBank.SelectionChanged += (e, o) => SaveDetails_();
            cmbInterestType.SelectionChanged += (e, o) => SaveDetails_();
            cmbAccountType.SelectionChanged += (e, o) => AccountTypeChanged_();
            cmbAccountType.SelectionChanged += (e, o) => SaveDetails_();
        }

        /// <summary>
        /// Called when the account type changed
        /// </summary>
        private void AccountTypeChanged_()
        {
            if (cmbAccountType.SelectedItem != null)
            {
                var accountTypeItem = cmbAccountType.SelectedItem as CustomComboBoxItem;
                var accountType = (AccountType)(accountTypeItem.Tag);

                switch (accountType)
                {
                    case AccountType.FixedRate:
                        cmbBank.Visibility = Visibility.Visible;
                        cmbInterestType.Visibility = Visibility.Visible;
                        lblAccountNumber.Visibility = Visibility.Visible;
                        lblInterest.Visibility = Visibility.Visible;
                        datePicker.Visibility = Visibility.Visible;
                        break;
                    case AccountType.Savings:
                        cmbBank.Visibility = Visibility.Visible;
                        cmbInterestType.Visibility = Visibility.Visible;
                        lblAccountNumber.Visibility = Visibility.Visible;
                        lblInterest.Visibility = Visibility.Visible;
                        datePicker.Visibility = Visibility.Collapsed;
                        break;
                    case AccountType.Crypto:
                        cmbBank.Visibility = Visibility.Collapsed;
                        cmbInterestType.Visibility = Visibility.Collapsed;
                        lblAccountNumber.Visibility = Visibility.Visible;
                        lblInterest.Visibility = Visibility.Collapsed;
                        datePicker.Visibility = Visibility.Collapsed;
                        break;
                    case AccountType.Material:
                        cmbBank.Visibility = Visibility.Collapsed;
                        cmbInterestType.Visibility = Visibility.Collapsed;
                        lblAccountNumber.Visibility = Visibility.Collapsed;
                        lblInterest.Visibility = Visibility.Collapsed;
                        datePicker.Visibility = Visibility.Collapsed;
                        break;
                    case AccountType.Stocks:
                        cmbBank.Visibility = Visibility.Visible;
                        cmbInterestType.Visibility = Visibility.Collapsed;
                        lblAccountNumber.Visibility = Visibility.Visible;
                        lblInterest.Visibility = Visibility.Collapsed;
                        datePicker.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        /// <summary>
        /// Save the details after updated
        /// </summary>
        private void SaveDetails_(bool overrideCreation = false)
        {
            if (_loaded && (!_initialCreation || overrideCreation))
            {
                if (cmbAccountType.SelectedItem != null)
                {
                    var accountTypeItem = cmbAccountType.SelectedItem as CustomComboBoxItem;
                    var accountType = (AccountType)(accountTypeItem.Tag);

                    // data specific to account type
                    var matureDate = GetMatureDate_(accountType);
                    var interestType = GetInterestType_(accountType);
                    var interestRate = GetInterestRate_(accountType);
                    var bankName = GetBankName_(accountType);

                    var accountName = lblAccountName.GetStringValue();
                    var accountNumber = lblAccountNumber.GetStringValue();

                    var balance = lblBalance.GetFloatValue();

                    var account = new BankAccount(_account == null ? Guid.NewGuid() : _account.GetAccountId(), accountName, bankName, accountNumber,
                        balance, interestRate, interestType, DateTime.Now, matureDate, accountType);

                    // validate data
                    if (ValidData_(accountName, interestRate, accountNumber, accountType))
                    {
                        // this will add a new account
                        _homePage.UpdateAccount(account);
                        _account = account;
                        cmdSave.Visibility = Visibility.Collapsed;
                        _initialCreation = false;
                    }
                }
                else
                {
                    _homePage.ErrorPopup.MoveOn("No account type specified");
                }
            }
        }

        /// <summary>
        /// Validates the data
        /// </summary>
        /// <param name="accountName">The name of the account</param>
        /// <param name="accountNumber">The number of the account</param>
        /// <param name="interestRate">The interest rate</param>
        /// <param name="accountType">Type of account</param>
        /// <returns>Whether the data is valid</returns>
        private bool ValidData_(string accountName, float interestRate, string accountNumber, AccountType accountType)
        {
            var valid = true;

            // account name
            if (accountName.Length < 3) { valid = false; _homePage.ErrorPopup.MoveOn("Account name is too short"); }

            // account number
            if (valid)
            {
                switch (accountType)
                {
                    case AccountType.Crypto:
                    case AccountType.Savings:
                    case AccountType.FixedRate:
                    {
                        if (accountNumber.Length < 3) { valid = false; _homePage.ErrorPopup.MoveOn("Account number is too short"); }
                    }
                    break;
                }
            }

            // interest rate
            if (valid)
            {
                switch (accountType)
                {
                    case AccountType.FixedRate:
                    {
                        if (interestRate < 1) { valid = false; _homePage.ErrorPopup.MoveOn("Interest rate cannot be less than 1% for fixed savers"); }
                    }
                    break;
                }
            }

            // mature date
            if (valid)
            {
                switch (accountType)
                {
                    case AccountType.FixedRate:
                    {
                        if (datePicker.SelectedDate != null)
                        {
                            var date = (DateTime)datePicker.SelectedDate;
                            if(date < DateTime.Today) { valid = false; _homePage.ErrorPopup.MoveOn("Maturity date cannot be in the past"); }
                        }
                    }
                    break;
                }
            }
            return valid;
        }

        /// <summary>
        /// Gets the name of the bank for this account
        /// </summary>
        /// <param name="accountType">The type of account</param>
        /// <returns>The name of the bank</returns>
        private string GetBankName_(AccountType accountType)
        {
            var bankName = string.Empty;
            switch (accountType)
            {
                // savings/fixed rate can be specified
                case AccountType.Savings:
                case AccountType.FixedRate:
                case AccountType.Stocks:
                {
                    bankName = (cmbBank.SelectedItem != null) ? cmbBank.SelectedItem.ToString() : "";
                }
                break;
            }

            return bankName;
        }

        /// <summary>
        /// Gets the name of the bank for this account
        /// </summary>
        /// <param name="accountType">The type of account</param>
        /// <returns>The name of the bank</returns>
        private DateTime GetMatureDate_(AccountType accountType)
        {
            var date = new DateTime();
            switch (accountType)
            {
                // savings/fixed rate can be specified
                case AccountType.FixedRate:
                {
                    if (datePicker.SelectedDate != null)
                        date = (DateTime)datePicker.SelectedDate;
                }
                break;
            }

            return date;
        }

        /// <summary>
        /// Gets the interest rate specified
        /// </summary>
        /// <param name="accountType">The type of account</param>
        /// <returns>The interest rate</returns>
        private float GetInterestRate_(AccountType accountType)
        {
            var interestRate = 0f;

            switch (accountType)
            {
                // savings/fixed rate can be specified
                case AccountType.Savings:
                case AccountType.FixedRate:
                {
                    interestRate = lblInterest.GetFloatValue();
                }
                break;
            }

            return interestRate;
        }

        /// <summary>
        /// Gets the interest type specified
        /// </summary>
        /// <param name="accountType">The type of account</param>
        /// <returns>The interest type</returns>
        private InterestType GetInterestType_(AccountType accountType)
        {
            var type = InterestType.None;

            switch (accountType)
            {
                // savings/fixed rate can be specified
                case AccountType.Savings:
                case AccountType.FixedRate:
                {
                    var interestTypeItem = cmbInterestType.SelectedItem as CustomComboBoxItem;
                    if (interestTypeItem != null) { type = (InterestType)(interestTypeItem.Tag); };
                }
                break;
                // crypto is varying
                case AccountType.Crypto:
                    type = InterestType.Varying;
                    break;
            }

            return type;
        }
    }
}
