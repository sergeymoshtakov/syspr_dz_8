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
using System.Diagnostics;
using System.Threading;

namespace SPNP
{
    /// <summary>
    /// Логика взаимодействия для ProcessWindow.xaml
    /// </summary>
    public partial class ProcessWindow : Window
    {
        public ProcessWindow()
        {
            CheckPreviousLunch();
            InitializeComponent();
        }

        private void ShowProcesses_Click(object sender, RoutedEventArgs e)
        {
            Process[] processes = Process.GetProcesses();
            // ProcTextBlock.Text = "";
            // foreach (Process process in processes)
            // {
            // ProcTextBlock.Text += String.Format("{0} {1}\n", process.Id, process.ProcessName);
            // }
            String prevName = "";
            TreeViewItem? item = null;
            foreach (Process process in processes.OrderBy(p => p.ProcessName))
            {
                if(prevName != process.ProcessName)
                {
                    prevName = process.ProcessName;
                    item = new TreeViewItem() { Header = prevName };
                    ProcTreeBlock.Items.Add(item);
                }
                var subItem = new TreeViewItem()
                {
                    Header = String.Format("{0} {1}", process.Id, process.ProcessName),
                    Tag = process
                };
                subItem.MouseDoubleClick += TreeViewItem_MouseDoubleClick;
                item?.Items.Add(subItem);
            }
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(sender is TreeViewItem item)
            {
                String message = "";
                if(item.Tag is Process process)
                {
                    TimeSpan cpuTime = process.TotalProcessorTime;
                    double cpuTimeMilliseconds = cpuTime.TotalMilliseconds;
                    long memoryUsage = process.WorkingSet64;
                    double memoryUsageKB = memoryUsage / 1024.0;
                    double memoryUsageMB = memoryUsageKB / 1024.0;
                    int threadCount = process.Threads.Count;
                    message += "споживання процесорного часу: " + cpuTimeMilliseconds + " miliseconds\r\n";
                    message += "споживання оперативної пам'яті: " + memoryUsageMB.ToString("F2") + " MB\r\n";
                    message += "загальнa кількість потоків: " + threadCount + "\r\n";
                }
                else
                {
                    message = "No process in tag";
                }
                MessageBox.Show(message);
            }
        }
        private Process notePadProcess;
        private void StartNotepad_Click(object sender, RoutedEventArgs e)
        {
            notePadProcess ??= Process.Start("notepad.exe");
        }

        private void StopNotepad_Click(object sender, RoutedEventArgs e)
        {
            notePadProcess?.CloseMainWindow();
            notePadProcess?.Kill(true);
            notePadProcess?.WaitForExit();
            notePadProcess?.Dispose();
            notePadProcess = null;
        }

        private void StopEdit_Click(object sender, RoutedEventArgs e)
        {
            String dir = AppContext.BaseDirectory;
            int binPosition = dir.LastIndexOf("bin");
            String projectRoot = dir[..binPosition];
            notePadProcess ??= Process.Start("notepad.exe", 
                $"{projectRoot}ProcessWindow.xaml"
            );
        }
        Process? processBrowser;
        private void StartBrowser_Click(object sender, RoutedEventArgs e)
        {
            String fileName = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
            if (System.IO.File.Exists(fileName))
            {
                processBrowser ??= Process.Start(fileName, "-url itstep.org");
            }
            else
            {
                MessageBox.Show("File doesn't exist");
            }
        }

        private void StartChrome_Click(object sender, RoutedEventArgs e)
        {
            String fileName = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            if (System.IO.File.Exists(fileName))
            {
                processBrowser ??= Process.Start(fileName, "-url itstep.org");
            }
            else
            {
                MessageBox.Show("File doesn't exist");
            }
            
        }

        private void StartSafary_Click(object sender, RoutedEventArgs e)
        {
            String fileName = @"C:\Program Files (x86)\Safari\safari.exe";
            if (System.IO.File.Exists(fileName))
            {
                processBrowser ??= Process.Start(fileName, "-url itstep.org");
            }
            else
            {
                MessageBox.Show("File doesn't exist");
            }
        }

        private Process? processCalculator;
        private void StartCalculator_Click(object sender, RoutedEventArgs e)
        {
            string fileName = @"C:\Windows\System32\calc.exe";
            if (System.IO.File.Exists(fileName))
            {
                processCalculator ??= Process.Start(fileName);
            }
            else
            {
                MessageBox.Show("You don't have calculator");
            }
        }

        private void StopCalculator_Click(object sender, RoutedEventArgs e)
        {
            processCalculator?.CloseMainWindow();
            processCalculator?.Kill(true);
            processCalculator?.WaitForExit();
            processCalculator?.Dispose();
            var temp = Process.GetProcessesByName("CalculatorApp");
            foreach(var item in temp)
            {
                item.Kill();
            }
            processCalculator = null;
        }
        private Process? processDispatcher;
        private void StartDispatcher_Click(object sender, RoutedEventArgs e)
        {
            processDispatcher ??= Process.Start("taskmgr");
        }

        private void StopDispatcher_Click(object sender, RoutedEventArgs e)
        {
            processDispatcher?.CloseMainWindow();
            processDispatcher?.Kill(true);
            processDispatcher?.WaitForExit();
            processDispatcher?.Dispose();
            var temp = Process.GetProcessesByName("Taskmgr");
            foreach (var item in temp)
            {
                item.CloseMainWindow();
                item.Kill(true);
                item.WaitForExit(); 
                item.Dispose();
            }
            processDispatcher = null;
        }
        private Process? processParameters;
        private void StartParameters_Click(object sender, RoutedEventArgs e)
        {
            string settingsUri = "ms-settings:";

            processParameters ??= Process.Start(new ProcessStartInfo
            {
                FileName = "explorer",
                Arguments = settingsUri,
                UseShellExecute = true
            });
        }

        private void StopParameters_Click(object sender, RoutedEventArgs e)
        {
            processParameters?.CloseMainWindow();
            processParameters?.Kill(true);
            processParameters?.WaitForExit();
            processParameters?.Dispose();
            processParameters = null;
        }
        private static Mutex mutex;
        private static String mutexName = "spnpMutex";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mutex?.ReleaseMutex();
            // mutex?.Dispose();
            
        }

        public void CheckPreviousLunch()
        {
            // synchronisation between processes
            try
            {
                mutex = Mutex.OpenExisting(mutexName);
            }
            catch { }
            if (mutex != null) // mutex registerd in system - another process has registered it
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
