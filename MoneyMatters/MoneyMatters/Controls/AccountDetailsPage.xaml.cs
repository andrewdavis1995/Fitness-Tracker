using Andrew_2_0_Libraries.Controllers;
using Andrew_2_0_Libraries.Models;
using MoneyMatters.Helpers;
using System;
using System.DirectoryServices.ActiveDirectory;
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

        public AccountDetailsPage()
        {
            InitializeComponent();
        }
        public void Display(BankAccount account, HomePage homePage)
        {
            InitializeComponent();
            _loaded = false;
            _account = account;
            _homePage = homePage;
            lblAccountName.Initialise(account.GetAccountName(), 30);
            lblBalance.Initialise(account.GetBalance(), 1000000, true);
            lblAccountNumber.Initialise(account.GetAccountNumber(), 20);
            lblInterest.Initialise(account.GetInterestRate(), 10, true);
            Visibility = Visibility.Visible;
            cmdClose.Initialise(() => { Visibility = System.Windows.Visibility.Collapsed; }, "cross");

            bool found = false;

            cmbBank.SelectedItem = null;
            cmbBank.Items.Clear();
            foreach (var bank in BankNameHelper.GetBankNames())
            {
                var cbi = new CustomComboBoxItem(bank, true);
                cmbBank.Items.Add(cbi);
                if (bank == account.GetBankName())
                {
                    cmbBank.SelectedItem = cbi;
                    found = true;
                }
            }

            cmbInterestType.SelectedItem = null;
            cmbInterestType.Items.Clear();
            foreach (var value in Enum.GetValues(typeof(InterestType)))
            {
                var cbi = new CustomComboBoxItem(value.ToString(), false);
                cbi.Tag = value;
                cmbInterestType.Items.Add(cbi);

                if (((InterestType)value) == account.GetInterestType())
                    cmbInterestType.SelectedItem = cbi;
            }

            (cmbBank.Items[cmbBank.Items.Count - 1] as CustomComboBoxItem).Style = FindResource("HoverFormatLast") as Style;
            (cmbInterestType.Items[cmbInterestType.Items.Count - 1] as CustomComboBoxItem).Style = FindResource("HoverFormatLast") as Style;

            if (!_initialised)
            {
                lblAccountName.Scale(1.5f);
                lblBalance.Scale(1.75f);
                AssignCallbacks_();
                _initialised = true;
            }

            _loaded = true;
        }

        private void AssignCallbacks_()
        {
            lblAccountName.AddCallback(() => SaveDetails_());
            lblAccountNumber.AddCallback(() => SaveDetails_());
            lblBalance.AddCallback(() => SaveDetails_());
            lblInterest.AddCallback(() => SaveDetails_());

            cmbBank.SelectionChanged += (e, o) => SaveDetails_();
            cmbInterestType.SelectionChanged += (e, o) => SaveDetails_();
        }

        private void SaveDetails_()
        {
            if (_account != null && _loaded)
            {
                // get data
                var interestTypeItem = cmbInterestType.SelectedItem as CustomComboBoxItem;
                var interestType = (InterestType)(interestTypeItem.Tag);

                // save data
                _account.UpdateAccount(lblAccountName.GetStringValue(), cmbBank.SelectedItem.ToString(),
                    lblAccountNumber.GetStringValue(), lblBalance.GetFloatValue(), lblInterest.GetFloatValue(),
                    interestType, DateTime.Now, /**/DateTime.Now);

                // save data
                _homePage.UpdateAccount(_account);
            }
        }
    }
}
