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

namespace SPNP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ThreadingButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            try
            {
                new ThreadingWindow().ShowDialog();
            }
            catch { }
            this.Show();
        }

        private void SynchronisationButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new SynchroWindow().ShowDialog();
            this.Show();
        }

        private void MultitaskingButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            try
            {
                new MultiTasking().ShowDialog();
            }
            catch { }
            this.Show();
        }

        private void CancelingButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new CancelingWindow().ShowDialog();
            this.Show();
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            try
            {
                new ProcessWindow().ShowDialog();
            }
            catch { }
            this.Show();
        }

        private void CHainingButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new ChainingWindow().ShowDialog();
            this.Show();
        }
    }
}