using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WindowsInput.Native;
namespace InvokerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool catchHotkey = false;
        static Overlay overlay;

        public static VirtualKeyCode hotkey;
        public MainWindow()
        {
            InitializeComponent();
            hotkey = (VirtualKeyCode)Config.Get<Int64>("hotkey", 18);
            UpdateButtonText();
            updateHotkeyButton();
        }
        void UpdateButtonText()
        {
            if (IsWindowOpen<Overlay>())
            {
                button.Content = "Stop";
            }
            else
                button.Content = "Start";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsWindowOpen<Overlay>())
            {
                overlay.Close();
            }
            else
            {
                overlay = new Overlay();
                overlay.Show();
            }
            UpdateButtonText();
        }
        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
               ? Application.Current.Windows.OfType<T>().Any()
               : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }
        void updateHotkeyButton()
        {
            if (catchHotkey)
                hotkeybutton.Content = "Press a key";
            else
            {
                hotkeybutton.Content = "Key: " + hotkey.ToString();
            }
        }
        private void hotkey_Click(object sender, RoutedEventArgs e)
        {
            catchHotkey = true;
            updateHotkeyButton();

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (catchHotkey)
            {
                if (e.Key != Key.Escape)
                {
                    hotkey = (VirtualKeyCode)KeyInterop.VirtualKeyFromKey(e.Key);
                    if (hotkey == 0)
                        hotkey = (VirtualKeyCode)18; //fix for alt
                   Config.Set("hotkey",(int)hotkey);
                 
                }
                catchHotkey = false;
                updateHotkeyButton();
                e.Handled = true;
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
          
            Process.Start(new ProcessStartInfo("https://github.com/efskap/dota2-poppedcollar"));
            e.Handled = true;
        }

    
    }
}
