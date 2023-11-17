using MoneyMatters.Helpers;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MoneyMatters.Controls
{
    /// <summary>
    /// Interaction logic for CustomComboBoxItem.xaml
    /// </summary>
    public partial class CustomComboBoxItem : ComboBoxItem
    {
        string _value;

        /// <summary>
        /// Destructor
        /// </summary>
        public CustomComboBoxItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Data to display</param>
        public CustomComboBoxItem(string data, bool setIcon)
        {
            InitializeComponent();
            lblContent.Content = data;
            _value = data;

            // set icon
            if (setIcon)
            {
                try
                {
                    brdIcon.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/"
                    + Assembly.GetExecutingAssembly().FullName +
                        ";component/Images/Icons/" + BankNameHelper.GetBankName(data).Replace(" ", "_") + ".png")));
                }
                catch (Exception)
                {
                    brdIcon.Background = null;
                }
            }
            else
            {
                colIcon.Width = new GridLength(0);
            }
        }

        /// <summary>
        /// Get the value as a string
        /// </summary>
        /// <returns>Value as a string</returns>
        public override string ToString() { return _value; }
    }
}
