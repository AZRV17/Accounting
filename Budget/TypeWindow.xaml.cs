using System.Windows;

namespace Budget
{
    public partial class TypeWindow : Window
    {
        public TypeWindow()
        {
            InitializeComponent();
        }

        private void AddTypeButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelTypeButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;   
        }
    }
}