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
    /// Логика взаимодействия для SynchroWindow.xaml
    /// </summary>
    public partial class SynchroWindow : Window
    {
        private double sum;
        private int threadCount; // kilkist aktyvnuch potokiv
        private CancellationTokenSource cts;
        private object sumLocker = new(); //synchronisation object
        private object countLocker = new();
        public SynchroWindow()
        {
            WaitOtherInstance();
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            sum = 100;
            threadCount = 12;
            cts = new CancellationTokenSource();
            for (int i = 0; i < threadCount; i++)
            {
                double percentDifference = i * 10;
                new Thread(AddPercent).Start(new MonthData { Month = i + 1, 
                    PercentDifference = percentDifference,
                    CancelToken = cts.Token
                });
            }
        }
        private void AddPercent(object? data)
        {
            var monthData = data as MonthData;
            Thread.Sleep(200);
            double localSum;
            lock (sumLocker)
            {
                localSum =
                    sum = sum * (1 + monthData.PercentDifference / 100);
            }

            Dispatcher.Invoke(() =>
            {
                LogTextBlock.Text += $"{monthData?.Month} - {monthData?.PercentDifference}% :{localSum}\n";
            });
            Interlocked.Decrement(ref threadCount);
            int myNumber; // nomer zavershenya
            lock (countLocker)
            {
                myNumber = --threadCount;
            }
            Thread.Sleep(1);
            if (threadCount == 0)
            {
                Dispatcher.Invoke(() => {
                    if (!monthData.CancelToken.IsCancellationRequested)
                    {
                        cts.Cancel();
                        LogTextBlock.Text += $"----\nresult = {sum}";
                    }
                });
                
            }
        }
        private void AddPercent4()
        {
            Thread.Sleep(200);

            lock (sumLocker)
            {
                sum = sum * 1.1;
            }

            Dispatcher.Invoke(() => {
                LogTextBlock.Text += $"{sum}\n";
            });
        }

        private void AddPercent2()
        {
            Thread.Sleep(200);
            double localSum = sum;
            localSum *= 1.1;
            sum = localSum;
            Dispatcher.Invoke(() => {
                LogTextBlock.Text += $"{sum}\n";
            });
        }

        private void AddPercent1()
        {
            double localSum = sum;
            Thread.Sleep(200);
            localSum *= 1.1;
            sum = localSum;
            Dispatcher.Invoke(() => {
                LogTextBlock.Text += $"{sum}\n";
            });
        }
        private void AddPercent3()
        {
            lock (sumLocker)
            {
                double localSum = sum;
                Thread.Sleep(200);
                localSum *= 1.1;
                sum = localSum;
                Dispatcher.Invoke(() =>
                {
                    LogTextBlock.Text += $"{sum}\n";
                });
            }
        }
        private Semaphore semaphore = new Semaphore(2, 2);
        private void AddPercentS(object? data)
        {
            var monthData = data as MonthData;
            semaphore.WaitOne();
            Thread.Sleep(1000);
            double localSum;
            localSum = sum *= 1.1;
            semaphore.Release();
            Dispatcher.Invoke(() =>
            {
                LogTextBlock.Text += $"{monthData?.Month} - {localSum}\n";
            });
        }

        class MonthData
        {
            public int Month { set; get; }
            public double PercentDifference { set; get; }
            public CancellationToken CancelToken { get; set; }
        }
        private static Mutex? mutex;
        private static String mutexName = "SPNP_SW_MUTTEX";
        private void WaitOtherInstance()
        {
            try
            {
                mutex = Mutex.OpenExisting(mutexName);
            }
            catch { }
            if (mutex == null)
            {
                mutex = new Mutex(true, mutexName);
            }
            else
            {
                if (!mutex.WaitOne(1))
                {
                    if(new CountDownWindow(mutex).ShowDialog() == false)
                    {
                        throw new ApplicationException();
                    }
                    mutex.WaitOne();
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mutex?.ReleaseMutex();
        }
    }
}