using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicDevel
{
    /// <summary>
    /// Interaction logic for MyWpfUserControl.xaml
    /// </summary>
    public partial class MyWpfUserControl : UserControl
    {
        public MyWpfUserControl()
        {
            InitializeComponent();
        }

        private void WpfButton_Click(object sender, RoutedEventArgs e)
        {
            // Raise a custom event so WinForms can listen
            ButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        // Define a public event that WinForms can subscribe to
        public event EventHandler ButtonClicked;

    }
}
