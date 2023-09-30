using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPNP
{
    /// <summary>
    /// Логика взаимодействия для ChainingWindow.xaml
    /// </summary>
    public partial class ChainingWindow : Window
    {
        private CancellationTokenSource _cancellationToken1;
        private CancellationTokenSource _cancellationToken2;
        private int activeTaskCount1 = 0;
        private readonly Object countLocker1 = new();
        public ChainingWindow()
        {
            InitializeComponent();
        }

        private void StartBtn1_Click(object sender, RoutedEventArgs e)
        {
            _cancellationToken1 = new CancellationTokenSource();
            StopBtn1.IsEnabled = true;
            showProgress(ProgressBar10, _cancellationToken1.Token).ContinueWith(task10 =>
            {
                showProgress(ProgressBar11, _cancellationToken1.Token).ContinueWith(task11 =>
                {
                    showProgress(ProgressBar12, _cancellationToken1.Token);
                });
            });
            showProgress(ProgressBar20, _cancellationToken1.Token).ContinueWith(task10 =>
            {
                showProgress(ProgressBar21, _cancellationToken1.Token).ContinueWith(task11 =>
                {
                    showProgress(ProgressBar22, _cancellationToken1.Token);
                });
            });
        }

        private void StopBtn1_Click(object sender, RoutedEventArgs e)
        {
            _cancellationToken1.Cancel();
        }

        private async Task showProgress(ProgressBar progressBar, CancellationToken cancellationToken)
        {
            int delay = 100;
            if (progressBar == ProgressBar10) { delay = 100; }
            if (progressBar == ProgressBar11) { delay = 200; }
            if (progressBar == ProgressBar11) { delay = 300; }
            if (progressBar == ProgressBar20) { delay = 300; }
            if (progressBar == ProgressBar21) { delay = 200; }
            if (progressBar == ProgressBar22) { delay = 100; }
            progressBar.Value = 0;
            progressBar.Foreground = Brushes.Green;

            lock (countLocker1) { activeTaskCount1++; }
            try
            {
                for (int i = 0; i <= 10; i++)
                {
                    await Task.Delay(delay);
                    Dispatcher.Invoke(() =>
                    {
                        progressBar.Value = i * 10;
                    });
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException e)
            {
                if (progressBar.Value != 100)
                {
                    progressBar.Foreground = Brushes.Red;
                    int a = Convert.ToInt32(progressBar.Value) / 10;
                    for (int i = a; i > 0; i--)
                    {
                        
                        await Task.Delay(delay);
                        Dispatcher.Invoke(() =>
                        {
                            progressBar.Value -= 10;
                        });
                    }
                }
                return;
            }
            finally
            {
                bool isLast;
                lock (countLocker1)
                {
                    activeTaskCount1--;
                    isLast = activeTaskCount1 == 0;
                }
                if (isLast)
                {
                    MessageBox.Show("All tasks have finished.");
                }
            }
        }

        private async void StartBtn2_Click(object sender, RoutedEventArgs e)
        {
            _cancellationToken2 = new CancellationTokenSource();
            StopBtn2.IsEnabled = true;
            var task10 = showProgress(ProgressBar10, _cancellationToken2.Token);
            var task20 = showProgress(ProgressBar20, _cancellationToken2.Token);
            await task10; var task11 = showProgress(ProgressBar11, _cancellationToken2.Token);
            await task20; var task21 = showProgress(ProgressBar21, _cancellationToken2.Token);
            await task11; var task12 = showProgress(ProgressBar12, _cancellationToken2.Token);
            await task21; var task22 = showProgress(ProgressBar22, _cancellationToken2.Token);
        }

        private void StopBtn2_Click(object sender, RoutedEventArgs e)
        {
            _cancellationToken2.Cancel();
        }

        private void StartBtn3_Click(object sender, RoutedEventArgs e)
        {
            String str = "";
            AddHello(str).ContinueWith(task =>
            {
                String res = task.Result;
                Dispatcher.Invoke(() => LogTextBlock.Text = res);
                return AddWorld(res);
            })
            .Unwrap()
            .ContinueWith(task2 =>
            {
                String res = task2.Result;
                Dispatcher.Invoke(() => LogTextBlock.Text = res);
                return AddExclamation(res);
            })
            .Unwrap()
            .ContinueWith(task3 =>
            {
                String res = task3.Result;
                Dispatcher.Invoke(() => LogTextBlock.Text = res);
            });
        }

        async Task<String> AddHello(String text)
        {
            await Task.Delay(1000);
            return text + "Hello ";
        }
        async Task<String> AddWorld(String text)
        {
            await Task.Delay(1000);
            return text + "World";
        }
        async Task<String> AddExclamation(String text)
        {
            await Task.Delay(1000);
            return text + "!!!";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StopBtn1.IsEnabled = false;
            StopBtn2.IsEnabled = false;
        }
    }
}
