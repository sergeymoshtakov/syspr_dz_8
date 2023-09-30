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
using System.Windows.Shapes;
using System.Threading;

namespace SPNP
{
    /// <summary>
    /// Логика взаимодействия для MultiTasking.xaml
    /// </summary>
    public partial class MultiTasking : Window
    {
        public MultiTasking()
        {
            InitializeComponent();
        }

        private void DemoButton1_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(demo1);
            task.Start();
            Task.Run(demo1);
        }

        private void demo1()
        {
            Dispatcher.Invoke(() =>
            {
                LogTextBox.Text += "demo1 start";
            });
            Thread.Sleep(3);
            Dispatcher.Invoke(() =>
            {
                LogTextBox.Text += "demo1 end";
            });
        }

        private async Task<String> demo2()
        {
            LogTextBox.Text += "demo1 start\n";
            await Task.Delay(1000); // alternative to Thread.Sleep()
            return "Done";
        }

        private async void DemoButton2_Click(object sender, RoutedEventArgs e)
        {
            Task<String> task1 = demo2();
            Task<String> task2 = demo2();
            // string str = await task;
            string res = $"Demo-2-1 result: {await task1}\n";
            LogTextBox.Text += res;
            res = $"Demo-2-2 result: {await task2}\n";
            LogTextBox.Text += res;
        }

        private async void SequentialButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar1.Value = 0;
            ProgressBar2.Value = 0;
            ProgressBar3.Value = 0;

            await UpdateProgressBarAsync(ProgressBar1, 500);
            await UpdateProgressBarAsync(ProgressBar2, 500);
            await UpdateProgressBarAsync(ProgressBar3, 500);
        }

        private async void ParallelButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar1.Value = 0;
            ProgressBar2.Value = 0;
            ProgressBar3.Value = 0;

            Task task1 = UpdateProgressBarAsync(ProgressBar1, 1500);
            Task task2 = UpdateProgressBarAsync(ProgressBar2, 500);
            Task task3 = UpdateProgressBarAsync(ProgressBar3, 1000);

            await Task.WhenAll(task1, task2, task3);
        }

        private async Task UpdateProgressBarAsync(ProgressBar progressBar, int delayMilliseconds)
        {
            for (int i = 0; i <= 10; i++)
            {
                progressBar.Value = i * 10;
                await Task.Delay(delayMilliseconds);
            }
        }
    }
}
