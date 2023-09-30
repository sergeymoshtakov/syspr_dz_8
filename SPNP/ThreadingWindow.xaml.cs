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
using System.Security.Cryptography.Xml;

namespace SPNP
{
    /// <summary>
    /// Логика взаимодействия для ThreadingWindow.xaml
    /// </summary>
    public partial class ThreadingWindow : Window
    {
        private bool isStoped3;
        private bool isStoped4 { set; get; }
        private Thread? thread4;
        public ThreadingWindow()
        {
            CheckPreviousLunch();
            InitializeComponent();
        }

        private void StartButton1_Click(object sender, RoutedEventArgs e)
        {
            // zavysanna
            // protagom vypovnena podiy, vsi inshi stayut v chergu i ne vypovnayutsa
            for (int i = 0; i < 10; i++)
            {
                ProgressBar1.Value = i * 10;
                Thread.Sleep(300);
            }
            ProgressBar1.Value = 100;
            // onovlenna vikna - odna iz podiy, to bigunok odrazu zapovnenyy, a ne pokrokovo
        }

        private void StopButton1_Click(object sender, RoutedEventArgs e)
        {
            // cherez zavysana interfeysu knopka ne zamykayetsa iz za dii start
            //zhodni dii ne mozhut zupynyty ii roboty
        }

        private void StartButton2_Click(object sender, RoutedEventArgs e)
        {
            new Thread(IncrementProgress).Start();
        }

        private void StopButton2_Click(object sender, RoutedEventArgs e)
        {

        }

        void IncrementProgress()
        {
            // p данного потоку неможна зминюваті елементи що належать иншому потоку
            for (int i = 0; i < 10; i++)
            {
                ProgressBar2.Value = i * 10;
                Thread.Sleep(300);
            }
            ProgressBar2.Value = 100;
        }

        private void StartButton3_Click(object sender, RoutedEventArgs e)
        {
            new Thread(IncrementProgress3).Start();
            isStoped3 = false;
        }

        private void StopButton3_Click(object sender, RoutedEventArgs e)
        {
            isStoped3 = true;
        }

        void IncrementProgress3()
        {
            // p данного потоку неможна зминюваті елементи що належать иншому потоку
            for (int i = 0; i < 10 && !isStoped3; i++)
            {
                Dispatcher.Invoke(
                    () => ProgressBar3.Value = i * 10
                    );
                Thread.Sleep(300);
            }

        }
        #region 4
        private void StartButton4_Click(object sender, RoutedEventArgs e)
        {
            if (thread4 == null)
            {
                isStoped4 = false;
                thread4 = new Thread(IncrementProgress4);
                StartButton4.IsEnabled = false;
                StopButton4.IsEnabled = true;
                thread4.Start();
            }
        }

        private void StopButton4_Click(object sender, RoutedEventArgs e)
        {
            isStoped4 = true;
            thread4 = null;
            StartButton4.IsEnabled = true;
            StopButton4.IsEnabled = false;
        }

        void IncrementProgress4()
        {
            // p данного потоку неможна зминюваті елементи що належать иншому потоку
            for (int i = 0; i <= 10 && !isStoped4; i++)
            {
                this.Dispatcher.Invoke(
                    () => ProgressBar4.Value = i * 10
                    );
                Thread.Sleep(300);
            }
            thread4 = null;
            this.Dispatcher.Invoke(
                () =>
                {
                    StartButton4.IsEnabled = true;
                    StopButton4.IsEnabled = false;
                }
                );
        }
        #endregion
        #region 5
        private Thread? thread5;
        CancellationTokenSource? cts; // джерело токенiв
        private void StartButton5_Click(object sender, RoutedEventArgs e)
        {
            int workTime = Convert.ToInt32(WorktimeTextBox.Text);
            thread5 = new Thread(IncrementProgress5);
            cts = new();
            thread5.Start(new ThreadData5
            {
                Worktime = workTime,
                CancelToken = cts.Token
            });
            // thread5.Join();
        }

        private void StopButton5_Click(object sender, RoutedEventArgs e)
        {
            // скасування потоків здійснюється через джерело
            cts?.Cancel();
            // писля циєї комманди усі токени данного джерела переходять у скасований стан
            // але безпосередньо на потоки це не вплине, перевирка стану токенив має окремо здийснюватись
            // у кожному потоци: у тих мисцях 
        }
        void IncrementProgress5(Object? parameter)
        {
            if (parameter is ThreadData5 data)
            {
                for (int i = 0; i <= 10; i++)
                {
                    this.Dispatcher.Invoke(
                        () => ProgressBar5.Value = i * 10
                        );
                    Thread.Sleep(100 * data.Worktime);
                    // задача перевиркі токена на скасованисть 
                    // частини роботи потоку (скасування не впливає на потік, якщо
                    // ми це будемо ігнорувати)
                    if (data.CancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                    // or data.CancelToken.ThrowIfCancelationRequested()
                }

            }
            else
            {
                MessageBox.Show("Thread 5 started with wrong parameter");
            }
        }

        class ThreadData5
        {
            public int Worktime { get; set; }
            // token created by CTS delivers with data to thread
            public CancellationToken CancelToken { get; set; }
        }
        #endregion
        #region 6
        private Thread? thread6;
        private Thread? thread7;
        private Thread? thread8;
        CancellationTokenSource? cts6;
        private void StopButton6_Click(object sender, RoutedEventArgs e)
        {
            cts6?.Cancel();
        }

        private void StartButton6_Click(object sender, RoutedEventArgs e)
        {
            int workTime1 = Convert.ToInt32(WorktimeTextBox1.Text);
            int workTime2 = Convert.ToInt32(WorktimeTextBox2.Text);
            int workTime3 = Convert.ToInt32(WorktimeTextBox3.Text);
            thread6 = new Thread(IncrementProgress6);
            thread7 = new Thread(IncrementProgress7);
            thread8 = new Thread(IncrementProgress8);
            cts6 = new();
            thread6.Start(new ThreadData6
            {
                Worktime1 = workTime1,
                Worktime2 = workTime2,
                Worktime3 = workTime3,
                CancelToken = cts6.Token
            });
            thread7.Start(new ThreadData6
            {
                Worktime1 = workTime1,
                Worktime2 = workTime2,
                Worktime3 = workTime3,
                CancelToken = cts6.Token
            });
            thread8.Start(new ThreadData6
            {
                Worktime1 = workTime1,
                Worktime2 = workTime2,
                Worktime3 = workTime3,
                CancelToken = cts6.Token
            });
        }
        void IncrementProgress6(Object? parameter)
        {
            if (parameter is ThreadData6 data)
            {
                for (int i = 0; i <= 10; i++)
                {
                    this.Dispatcher.Invoke(
                        () => {
                            ProgressBar6.Value = i * 10;
                        }
                        );
                    Thread.Sleep(100 * data.Worktime1);
                    if (data.CancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                }

            }
            else
            {
                MessageBox.Show("Thread 6 started with wrong parameter");
            }
        }

        void IncrementProgress7(Object? parameter)
        {
            if (parameter is ThreadData6 data)
            {
                for (int i = 0; i <= 10; i++)
                {
                    this.Dispatcher.Invoke(
                        () => {
                            ProgressBar7.Value = i * 10;
                        }
                        );
                    Thread.Sleep(100 * data.Worktime2);
                    if (data.CancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                }

            }
            else
            {
                MessageBox.Show("Thread 6 started with wrong parameter");
            }
        }
        void IncrementProgress8(Object? parameter)
        {
            if (parameter is ThreadData6 data)
            {
                for (int i = 0; i <= 10; i++)
                {
                    this.Dispatcher.Invoke(
                        () => {
                            ProgressBar8.Value = i * 10;
                        }
                        );
                    Thread.Sleep(100 * data.Worktime3);
                    if (data.CancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                }

            }
            else
            {
                MessageBox.Show("Thread 6 started with wrong parameter");
            }
        }
        class ThreadData6
        {
            public int Worktime1 { get; set; }
            public int Worktime2 { get; set; }
            public int Worktime3 { get; set; }
            public CancellationToken CancelToken { get; set; }
        }
        #endregion

        private static Mutex mutex;
        private static String mutexName = "SPNP_TW_MUTEX";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mutex?.ReleaseMutex();
        }

        public void CheckPreviousLunch()
        {
            try
            {
                mutex = Mutex.OpenExisting(mutexName);
            }
            catch { }
            if (mutex != null)
            {
                if (!mutex.WaitOne(1))
                {
                    String message = "Enother exemplar started";
                    MessageBox.Show("Enother exemplar started");
                    throw new Exception(message);
                }

            }
            else
            {
                mutex = new Mutex(true, mutexName);
            }
        }
    }
}