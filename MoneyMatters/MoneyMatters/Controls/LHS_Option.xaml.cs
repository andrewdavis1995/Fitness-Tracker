using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MoneyMatters.Controls
{
    /// <summary>
    /// Interaction logic for CustomButton.xaml
    /// </summary>
    public partial class LHS_Option : UserControl
    {
        Action? _callback;
        bool _setColour = true;

        /// <summary>
        /// Constructor
        /// </summary>
        public LHS_Option()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialises the button with the message and callbacks provided
        /// </summary>
        /// <param name="callback">The function to call when the button is clicked</param>
        /// <param name="imgIcon">The image to use as the icon</param>
        /// <param name="selected">Whether the button is currently selected</param>
        /// <param name="setColourOnClick">Whether to set the colour of the button once it is clicked</param>
        public void Initialise(Action callback, string icon, bool selected = false, bool setColourOnClick = true)
        {
            _callback = callback;
            _setColour = setColourOnClick;

            // check that image was found
            if (string.IsNullOrEmpty(icon))
                imgIcon.Source = null;
            else
            {
                try
                {
                    // set image
                    imgIcon.Source = new BitmapImage(new Uri(
                        "pack://application:,,,/" + Assembly.GetExecutingAssembly().FullName + $";component/Images/LHS_Icons/{icon}.png", UriKind.RelativeOrAbsolute));
                }
                catch (Exception ex) { imgIcon.Source = null; }
            }

            // mark as selected if told to do so
            SelectedBackground.Visibility = selected ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Un-select the button
        /// </summary>
        internal void Deselect()
        {
            SelectedBackground.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Event handler for when the mouse enters the button
        /// </summary>
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            // highlight
            HoverPopup.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Event handler for when the mouse leaves the button
        /// </summary>
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            // remove the highlight
            HoverPopup.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Event handler for when the button is clicked
        /// </summary>
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _callback?.Invoke();

            // update appearance
            if (_setColour)
                SelectedBackground.Visibility = Visibility.Visible;
        }
    }
}