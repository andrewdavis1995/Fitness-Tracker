using Andrew_2_0_Libraries.Models;
using System;
using System.Windows.Controls;

namespace MoneyMatters.Controls.Popups
{
    /// <summary>
    /// Interaction logic for Popup_CreateAccount.xaml
    /// </summary>
    public partial class Popup_CreateAccount : UserControl
    {
        Action _cancelCallback;
        Action<BankAccount> _confirmCallback;

        public Popup_CreateAccount(Action cancelCallback, Action<BankAccount> confirmCallback)
        {
            InitializeComponent();

            _cancelCallback = cancelCallback;
            _confirmCallback = confirmCallback;

            // configure buttons
            cmdCancel.Configure("Cancel");
            cmdConfirm.Configure("Confirm");
        }

        /// <summary>
        /// Caallback for the cancel button
        /// </summary>
        private void cmdCancel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _cancelCallback?.Invoke();
        }

        /// <summary>
        /// Callback for the confirm button
        /// </summary>
        private void cmdConfirm_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var account = new BankAccount(Guid.NewGuid(), "Fixed saver", "Bank name", "0102043", 241.77, 34.2, InterestType.None, DateTime.Now);
            _confirmCallback?.Invoke(account);
        }
    }
}
