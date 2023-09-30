using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    /// Логика взаимодействия для CancelingWindow.xaml
    /// </summary>
    public partial class CancelingWindow : Window
    {
        private CancellationTokenSource _cancellationToken1;
        private CancellationTokenSource _cancellationToken2;
        private CancellationTokenSource _cancellationToken3;
        private int activeTaskCount1 = 0;
        private int activeTaskCount3 = 0;
        private readonly Object countLocker1 = new();
        private readonly Object countLocker3 = new();
        public CancelingWindow()
        {
            InitializeComponent();
        }

        private void StopBtn1_Click(object sender, RoutedEventArgs e)
        {
            _cancellationToken1.Cancel();
        }


        private async void StartBtn1_Click(object sender, RoutedEventArgs e)
        {
            _cancellationToken1 = new CancellationTokenSource();
            runProgressBarCancelable(ProgressBar10, _cancellationToken1.Token);
            runProgressBarCancelable(ProgressBar11, _cancellationToken1.Token, 4);
            runProgressBarCancelable(ProgressBar12, _cancellationToken1.Token, 2);
        }


        // parallel
        private async void runProgressBar(ProgressBar progressBar, CancellationToken cancellationToken, int time = 3)
        {
            progressBar.Value = 0;
            lock (countLocker3) { activeTaskCount3++; }
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    progressBar.Value += 10;
                    await Task.Delay(1000 * time / 10);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                if (progressBar.Value != 100)
                {
                    progressBar.Foreground = Brushes.Red;
                    int a = Convert.ToInt32(progressBar.Value) / 10;
                    for (int i = a; i > 0; i--)
                    {
                        progressBar.Value -= 10;
                        await Task.Delay(1000 * time / 10);
                    }
                }
                return;
            }
            finally
            {
                progressBar.Foreground = Brushes.Green;
                bool isLast;
                lock (countLocker3)
                {
                    activeTaskCount3--;
                    isLast = activeTaskCount3 == 0;
                }
                if (isLast)
                {
                    MessageBox.Show("All tasks have finished.");
                }
            }
        }

        // sequential
        private async Task runProgressBarWaitable(ProgressBar progressBar, CancellationToken cancellationToken, int time = 3)
        {
            progressBar.Value = 0;
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    progressBar.Value += 10;
                    await Task.Delay(1000 * time / 10);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                if (progressBar.Value != 100)
                {
                    progressBar.Foreground = Brushes.Red;
                    int a = Convert.ToInt32(progressBar.Value) / 10;
                    for (int i = a; i > 0; i--)
                    {
                        progressBar.Value -= 10;
                        await Task.Delay(1000 * time / 10);
                    }
                }
                return;
            }
            finally
            {
                progressBar.Foreground = Brushes.Green;
            }
        }

        private async void runProgressBarCancelable(ProgressBar progressBar, CancellationToken cancellationToken, int time = 3)
        {
            progressBar.Value = 0;
            lock (countLocker1) { activeTaskCount1++; }
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    progressBar.Value += 10;
                    await Task.Delay(1000 * time / 10);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException e)
            {
                if(progressBar.Value != 100)
                {
                    progressBar.Foreground = Brushes.Red;
                    int a = Convert.ToInt32(progressBar.Value) / 10;
                    for (int i = a; i > 0; i--)
                    {
                        progressBar.Value -= 10;
                        await Task.Delay(1000 * time / 10);
                    }
                }
                return;
            }
            finally{
                progressBar.Foreground = Brushes.Green;
                bool isLast;
                lock (countLocker1)
                {
                    activeTaskCount1--;
                    isLast = activeTaskCount1 == 0;
                }
                if(isLast)
                {
                    MessageBox.Show("All tasks have finished.");
                }
            }
        }

        private async void StartBtn2_Click(object sender, RoutedEventArgs e)
        {
            _cancellationToken2 = new CancellationTokenSource();
            await Task.WhenAll(runProgressBarWaitable(ProgressBar20, _cancellationToken2.Token),
                runProgressBarWaitable(ProgressBar21, _cancellationToken2.Token, 4),
                runProgressBarWaitable(ProgressBar22, _cancellationToken2.Token, 2)
                );
            MessageBox.Show("All tasks have finished.");
        }

        private void StopBtn2_Click(object sender, RoutedEventArgs e)
        {
            _cancellationToken2.Cancel();   
        }

        private void StartBtn3_Click(object sender, RoutedEventArgs e)
        {
            _cancellationToken3 = new CancellationTokenSource();
            runProgressBar(ProgressBar30, _cancellationToken3.Token);
            runProgressBar(ProgressBar31, _cancellationToken3.Token, 4);
            runProgressBar(ProgressBar32, _cancellationToken3.Token, 2);
        }

        private void StopBtn3_Click(object sender, RoutedEventArgs e)
        {
            _cancellationToken3.Cancel();
        }

        // parallel
        // runProgressBar(ProgressBar10);
        // runProgressBar(ProgressBar11, 4);
        // runProgressBar(ProgressBar12, 2);

        // sequential

        // await runProgressBarWaitable(ProgressBar10);
        // await runProgressBarWaitable(ProgressBar11, 4);
        // await runProgressBarWaitable(ProgressBar12, 2);

        // sequential with void. Invoke needed for UI
        //await Task.Run(() =>
        // {
        // runProgressBar(ProgressBar10);
        // });
        // await Task.Run(() =>
        // {
        // runProgressBar(ProgressBar11, 4);
        // });
        // await Task.Run(() =>
        // {
        // runProgressBar(ProgressBar12, 2);
        //});
        //
    }
}
