using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MoneyMatters.Controls
{
    /// <summary>
    /// Interaction logic for EditableLabel.xaml
    /// </summary>
    public partial class EditableLabel : UserControl
    {
        bool _isNumeric = false;
        bool _allowDecimal = false;
        double _maxValue;
        bool _empty = true;

        Action? _saveCallback;

        /// <summary>
        /// Constructor
        /// </summary>
        public EditableLabel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialises the control for text/string input
        /// </summary>
        /// <param name="existingData">Value to start off with</param>
        /// <param name="maxLength">Maximum length of data to allow</param>
        public void Initialise(string existingData, int maxLength)
        {
            txtContent.Text = existingData;
            txtContent.MaxLength = maxLength;
            InputComplete_();
        }

        /// <summary>
        /// Initialises the control for numeric input
        /// </summary>
        /// <param name="existingData">Value to start off with</param>
        /// <param name="maxValue">Maximum permitted value</param>
        /// <param name="allowDecimal">Whether decimal values are allowed</param>
        public void Initialise(double existingData, double maxValue, bool allowDecimal)
        {
            _isNumeric = true;
            _allowDecimal = allowDecimal;
            _maxValue = maxValue;

            // display data
            string format = allowDecimal ? "0.00" : "0";
            lblContent.Text = existingData.ToString(format);
            txtContent.Text = existingData.ToString(format);
            _empty = false;
        }

        /// <summary>
        /// When the user has finished typing in this field (lost focus, or enter pressed)
        /// </summary>
        private void InputComplete_()
        {
            // remove spaces
            txtContent.Text = txtContent.Text.Trim();

            // add trailing decimal places and zeroes
            if (_isNumeric && _allowDecimal && txtContent.Text.Length > 0)
            {
                // find decimal point
                var index = txtContent.Text.IndexOf(".");

                // add one to the end if there isn't one
                if (index < 0)
                {
                    txtContent.Text += ".";
                    index = txtContent.Text.Length - 1;
                }

                // add trailing zeroes
                while (index >= 0 && txtContent.Text.Trim().Length < index + 3)
                {
                    txtContent.Text += "0";
                }
            }

            // if empty, display blank data 
            if (txtContent.Text.Length == 0)
            {
                // show a dummy value
                lblContent.Text = _isNumeric ? "0" : "No Data";
                lblContent.Opacity = 0.6f;

                // record that it was empty so we don't load the dummy value into the text box next time
                _empty = true;
            }
            else
            {
                // display value
                lblContent.Text = txtContent.Text;
                lblContent.Opacity = 1f;

                // no longer empty - free to load this value into the text box next time
                _empty = false;
            }

            // hide text box
            txtContent.Visibility = Visibility.Collapsed;
            lblContent.Visibility = Visibility.Visible;

            // hide the edit border
            brdBackground.BorderBrush = new SolidColorBrush(Colors.Transparent);

            // inform the parent that the data has been updated
            _saveCallback?.Invoke();
        }

        /// <summary>
        /// Scales the control up or down by the specified amount
        /// </summary>
        /// <param name="scale">How much to scale by</param>
        public void Scale(float scale)
        {
            // scale the controls
            Height *= scale;
            txtContent.FontSize *= scale;
            lblContent.FontSize *= scale;
        }

        /// <summary>
        /// Adds a callback function to call when the value is confirmed/input is complete
        /// </summary>
        /// <param name="callback">The action to call</param>
        public void AddCallback(Action callback)
        {
            _saveCallback = callback;
        }

        #region Accessors
        /// <summary>
        /// Returns the value entered into this input
        /// </summary>
        /// <returns>The user-entered value</returns>
        public string GetStringValue()
        {
            return txtContent.Text;
        }

        /// <summary>
        /// Returns the value entered into this input
        /// </summary>
        /// <returns>The user-entered value</returns>
        public float GetFloatValue()
        {
            if (_isNumeric)
                return float.Parse(txtContent.Text);

            return 0;
        }

        /// <summary>
        /// Returns the value entered into this input
        /// </summary>
        /// <returns>The user-entered value</returns>
        public float GetIntValue()
        {
            if (_isNumeric)
                return (int)(float.Parse(txtContent.Text));

            return 0;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event handler for when text is changed on the textbox
        /// </summary>
        private void txtContent_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // if this is a numeric field, don't allow letters
            if (_isNumeric)
            {
                // check that the value is a number (decimals only allowed if specified)
                var regexString = _allowDecimal ? "[^0-9.]" : "[^0-9]";
                Regex regex = new Regex(regexString);
                bool handled = (regex.IsMatch(e.Text));

                if (!handled)
                {
                    // check that the new value is not more than the maximum value
                    var preSelection = txtContent.Text.Substring(0, txtContent.SelectionStart); // before highlighted text
                    var postSelection = txtContent.Text.Substring(txtContent.SelectionStart + txtContent.SelectionLength);  // after highlighted text

                    // check decimal points
                    if (!handled && (preSelection + postSelection).Contains('.') && e.Text.Contains('.'))
                    {
                        // can't have multiple decimals
                        handled = true;
                    }
                    else
                    {
                        string newValue = (preSelection + postSelection).Insert(txtContent.SelectionStart, e.Text); // put new value in the middle

                        // if just a decimal point, the casting will fail
                        if (newValue == ".")
                        {
                            // set to "0."
                            handled = true;
                            txtContent.Text = "0.";
                            // move to the end of the text
                            txtContent.CaretIndex = 2;
                        }
                        else
                        {
                            // check data value is less than maximum
                            handled = (float.Parse(newValue) > _maxValue);
                        }
                        // don't allow more than 2 places after decimal
                        if (newValue.IndexOf(".") >= 0 && newValue.IndexOf(".") < newValue.Length - 3)
                        {
                            handled = true;
                        }
                    }
                }

                // if Handled = true, the input is ignored
                e.Handled = handled;
            }
        }

        /// <summary>
        /// Event handler for the mouse entering the control
        /// </summary>
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            brdBackground.Background = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255));
        }

        /// <summary>
        /// Event handler for when the mouse leaves the control
        /// </summary>
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            brdBackground.Background = new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Event handler for when the control is clicked - show the text input
        /// </summary>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // show textbox
            txtContent.Visibility = Visibility.Visible;
            lblContent.Visibility = Visibility.Collapsed;
            brdBackground.BorderBrush = new SolidColorBrush(Color.FromArgb(80, 255, 255, 255));

            // if no data entered, display blank string
            if (_empty)
            {
                txtContent.Text = "";
            }
            // otherwise display the text
            else
                txtContent.Text = lblContent.Text;

            // give the text box focus and move caret to the end of the text box
            txtContent.Focus();
            txtContent.CaretIndex = 1000;
        }

        /// <summary>
        /// Event handler for when the 
        /// </summary>
        private void txtContent_LostFocus(object sender, RoutedEventArgs e)
        {
            InputComplete_();
        }

        /// <summary>
        /// Event handler for when a key is pressed on the text box
        /// </summary>
        private void txtContent_KeyDown(object sender, KeyEventArgs e)
        {
            // if enter, move off the control
            if ((e.Key == Key.Enter))
            {
                InputComplete_();
            }
        }
        #endregion
    }
}
