using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Andrew_2_0_Libraries.Controllers;
using Andrew_2_0_Libraries.Models;
using MoneyMatters.Controls;
using MoneyMatters.Controls.Popups;
using MoneyMatters.Helpers;

namespace MoneyMatters
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Window
    {
        float _columnCount = 1;
        FinanceController _controller = new();
        Popup_CreateAccount? _creationPopup;
        Popup_AccountInformation _infoPopup;

        public HomePage()
        {
            InitializeComponent();
            cmdNewAccount.Configure("New Account");
            lhsAccounts.Initialise(() => { DisplayAccounts_(); }, "List", true);
            lhsExit.Initialise(() => { Environment.Exit(0); }, "Exit");
            chkBlurValues.AddTitle("Blur Values");
            chkBlurValues.AddCallbacks(() => BlurValues_(), () => UnblurValues_());
        }

        /// <summary>
        /// Blurs the values on accounts
        /// </summary>
        private void BlurValues_()
        {
            foreach (var control in grdAccounts.Children)
            {
                var disp = control as Display_Account;
                disp?.BlurValue();
            }
        }

        /// <summary>
        /// Un-blurs the values on accounts
        /// </summary>
        private void UnblurValues_()
        {
            foreach (var control in grdAccounts.Children)
            {
                var disp = control as Display_Account;
                disp?.UnblurValue();
            }
        }

        /// <summary>
        /// Displays the supplied list of recipes
        /// </summary>
        /// <param name="recipes">The recipes to display</param>
        private void DisplayAccounts_()
        {
            HideWindows_();

            grdAccounts.Children.Clear();

            // find recipes that match the filters
            var accounts = _controller.GetAccounts();

            // set up the grid (add 1 for the Total display)
            SetupAccountGrid_(accounts.Count + 1);

            var col = 1;
            var row = 1;

            // display total price
            var totalControl = new Display_Account();
            Grid.SetRow(totalControl, row);
            Grid.SetColumn(totalControl, col++);
            totalControl.ConfigureAsTotal(_controller.GetAccounts().Sum(a => a.GetBalance()));
            grdAccounts.Children.Add(totalControl);

            // iterate through each recipe
            foreach (var ac in accounts)
            {
                // create display
                AddAccountDisplay_(col, row, ac);

                // move to next column
                col++;
                if (col > _columnCount)
                {
                    // next row
                    col = 1;
                    row++;
                }
            }

            // "No recipes" message
            lblNoAccounts.Visibility = accounts.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            totalControl.Visibility = accounts.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void HideWindows_()
        {
            // TODO: hide windows
        }

        /// <summary>
        /// Resets the columns and rows to the default, before adding more new items
        /// </summary>
        private void ResetSetup_()
        {
            // reset columns
            var index = 2;
            while (index < grdAccounts.ColumnDefinitions.Count - 1)
            {
                grdAccounts.ColumnDefinitions.RemoveAt(index);
            }

            // reset rows
            index = 2;
            while (index < grdAccounts.RowDefinitions.Count - 1)
            {
                grdAccounts.RowDefinitions.RemoveAt(index);
            }
        }

        /// <summary>
        /// Adds a display to the grid
        /// </summary>
        /// <param name="col">Column to add it in</param>
        /// <param name="row">Row to add it in</param>
        /// <param name="recipe">Recipe this represents</param>
        void AddAccountDisplay_(int col, int row, BankAccount account)
        {
            var display = new Display_Account(account);
            Grid.SetColumn(display, col);
            Grid.SetRow(display, row);
            display.MouseLeftButtonDown += (sender, e) =>
            {
                DetailsPage.Display(display.GetAccount(), this);

                // TEMPORARILY DISABLING THIS
                //_infoPopup = new Popup_AccountInformation((a) =>
                //{
                //    _controller.UpdateAccount(a);
                //    DisplayAccounts_();
                //    grdOverall.Children.Remove(_infoPopup);
                //}, (a) =>
                //{
                //    grdOverall.Children.Remove(_infoPopup);

                //    _creationPopup = new Popup_CreateAccount(
                //        () => { grdOverall?.Children.Remove(_creationPopup); },
                //        (b) =>
                //        {
                //            _controller.UpdateAccount(b);
                //            grdOverall?.Children.Remove(_creationPopup);
                //            DisplayAccounts_();
                //        }, a
                //    );

                //    // show popup
                //    PopupController.AboveAll(_creationPopup);
                //    grdOverall?.Children.Add(_creationPopup);
                //},
                //account);
                //grdOverall.Children.Add(_infoPopup);
            };

            grdAccounts.Children.Add(display);
        }

        /// <summary>
        /// Add the necessary columns and rows
        /// </summary>
        /// <param name="numAccountCount">How many recipes there are</param>
        private void SetupAccountGrid_(int numAccountCount)
        {
            ResetSetup_();

            // calculate what is needed
            var colWidth = grdAccounts.ColumnDefinitions[1].Width.Value;
            var colHeight = grdAccounts.RowDefinitions[1].Height.Value;
            _columnCount = (int)(grdAccounts.ActualWidth / colWidth);
            var numRows = (int)(numAccountCount / _columnCount);
            numRows += (numAccountCount % _columnCount > 0 ? 1 : 0);

            // add columns
            for (int i = 1; i < _columnCount; i++)
            {
                grdAccounts.ColumnDefinitions.Insert(1, new ColumnDefinition() { Width = new GridLength(colWidth) });
            }

            // add rows
            for (int i = 1; i < numRows; i++)
            {
                grdAccounts.RowDefinitions.Insert(1, new RowDefinition() { Height = new GridLength(colHeight) });
            }

            // central align if there are enough recipes to fill the screen
            if (numRows >= 2)
                grdAccounts.ColumnDefinitions.First().Width = new GridLength(1, GridUnitType.Star);
            else
                grdAccounts.ColumnDefinitions.First().Width = new GridLength(20, GridUnitType.Pixel);
        }

        /// <summary>
        /// Event handler for when the window loads
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayAccounts_();
        }

        /// <summary>
        /// Event handler for new account button
        /// </summary>
        private void cmdNewAccount_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _creationPopup = new Popup_CreateAccount(
                () => { grdOverall?.Children.Remove(_creationPopup); },
                (a) =>
                {
                    _controller.UpdateAccount(a);
                    grdOverall?.Children.Remove(_creationPopup);
                    DisplayAccounts_();
                }
            );

            // show popup
            PopupController.AboveAll(_creationPopup);
            grdOverall?.Children.Add(_creationPopup);
        }

        /// <summary>
        /// Update the specified account
        /// </summary>
        /// <param name="account">The account to update</param>
        public void UpdateAccount(BankAccount account)
        {
            _controller.UpdateAccount(account);
            DisplayAccounts_();
        }
    }
}
