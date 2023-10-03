using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Логика взаимодействия для DLLWindow.xaml
    /// </summary>
    public partial class DLLWindow : Window
    {
        private readonly SoundPlayer soundPlayer;
        [DllImport("user32.dll")]
        public static extern int MessageBoxA(IntPtr hWnd, String lpText, String lpCaption, uint uType);
        public DLLWindow()
        {
            soundPlayer = new SoundPlayer();
            InitializeComponent();
        }

        private void AlertButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxA(
                IntPtr.Zero, // null
                "Message", // неявний маршалінг - передача
                "Title", // рядків у некерований код
                0x40
            );
        }

        /*
         HANDLE CreateThread(
            [in, optional]  LPSECURITY_ATTRIBUTES   lpThreadAttributes,
            [in]            SIZE_T                  dwStackSize,
            [in]            LPTHREAD_START_ROUTINE  lpStartAddress,
            [in, optional]  __drv_aliasesMem LPVOID lpParameter,
            [in]            DWORD                   dwCreationFlags,
            [out, optional] LPDWORD                 lpThreadId
        );
         */
        public delegate void ThreadMethod();
        [DllImport("Kernel32.dll", EntryPoint = "CreateThread")]
        public static extern IntPtr NewThread(
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            ThreadMethod lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            IntPtr lpThreadId
        );
        // описуємо сам метод за сигнатурою делегата
        public void ErrorMessage()
        {
            MessageBoxA(
                IntPtr.Zero, // null
                "Error message", // неявний маршалінг - передача
                "Place of origin", // рядків у некерований код
                0x14
            );
            methodHandle.Free();
        }
        GCHandle methodHandle;
        private void ThreadButton_Click(object sender, RoutedEventArgs e)
        {
            var method = new ThreadMethod(ErrorMessage);
            methodHandle = GCHandle.Alloc(method);
            NewThread(IntPtr.Zero, 0, method, IntPtr.Zero, 0, IntPtr.Zero);
        }

        private async void SoundButton_Click(object sender, RoutedEventArgs e)
        {
            await soundPlayer.PlaySoundAsync(440, 300);
        }

        private async void SoundButton1_Click(object sender, RoutedEventArgs e)
        {
            await soundPlayer.PlaySoundAsync(540, 300);
        }

        private async void SoundButton2_Click(object sender, RoutedEventArgs e)
        {
            await soundPlayer.PlaySoundAsync(640, 300);
        }

        private async void SoundButton4_Click(object sender, RoutedEventArgs e)
        {
            await soundPlayer.PlaySoundAsync(740, 300);
        }
    }

    public class SoundPlayer
    {
        [DllImport("Kernel32.dll", EntryPoint = "Beep")]
        public static extern bool Sound(uint frequency, uint duration);

        public async Task PlaySoundAsync(uint frequency, uint duration)
        {
            await Task.Run(() => Sound(frequency, duration));
        }
    }
}
