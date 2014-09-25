using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InvokerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Overlay : Window
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        Dictionary<string, Point[]> resolutions = new Dictionary<string, Point[]>();



        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }


        static Dictionary<string, string> ALL_SPELLS = new Dictionary<string, string>() {
    {"coldsnap","qqq"},
    { "ghostwalk","qqw"},
    { "emp","www"},
    { "sunstrike","eee"},
    { "tornado","qww"},
    { "icewall","qqe"},
    { "deafeningblast","qwe"},
    { "alacrity","wwe"},
    { "forgespirit","eeq"},
    { "chaosmeteor","eew"}
    };
        const int ICON_SIZE = 100;

      
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        static string[] fav_spells = {};

        static Dictionary<string, string> ActiveSpells;

        public Overlay()
        {


            fav_spells = Config.Get<string>("shownspells", "coldsnap ghostwalk emp tornado").Split(' ');
            ActiveSpells = ALL_SPELLS.OrderBy(x => x.Value).Where(x => fav_spells.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value); // oh god

            InitializeComponent();
         
            
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            dispatcherTimer.Start();
            Visibility = System.Windows.Visibility.Hidden;



        }

        private void Appear()
        {
            Visibility = System.Windows.Visibility.Visible;
            //   MouseOrigin = Utils.GetMousePosition();
 

        }
        private void Disappear()
        {
            Visibility = System.Windows.Visibility.Hidden;
        }
        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
        //   if (GetActiveWindowTitle() != "DOTA 2")
      //          return;
            Point d = Utils.GetMousePosition();
            d.Offset(-Utils.GetScreenCenter().X, -Utils.GetScreenCenter().Y);

            double angle = Math.Atan2(d.Y, d.X);
            if (angle < 0)
                angle += 2 * Math.PI;

            int dist = (int)Math.Sqrt(d.X * d.X + d.Y * d.Y);

            int min_mouse_dist = 50;
            bool far_enough = dist >= min_mouse_dist;

            int selected = (int)Math.Round((angle / (2.0 * Math.PI)) * ActiveSpells.Count);

            if (selected >= ActiveSpells.Count)
                selected -= ActiveSpells.Count;

            if (Win32.GetAsyncKeyState((int)MainWindow.hotkey) != 0)
            {
             
                if (Visibility == System.Windows.Visibility.Hidden)
                    Appear();

                if (Process.GetProcessesByName("Dota").Length >= 1)
                    SetForegroundWindow(Process.GetProcessesByName("Dota")[0].MainWindowHandle);
                if (canvas.Children.Count > 0)
                {
                    

                    for (int i = 0; i < ActiveSpells.Count; i += 1)
                    {
                        canvas.Children[2 * i + 1].Visibility = System.Windows.Visibility.Hidden;

                    }
                    if(far_enough)
                    canvas.Children[(selected * 2) + 1].Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                if (Visibility == System.Windows.Visibility.Visible)
                {
            
                    if (far_enough)
                       Invoke(ActiveSpells[ActiveSpells.Keys.ToArray()[selected]]);
                    Disappear();
                }
            }


        }


        void Invoke(string pattern)
        {

            pattern += "r";
            foreach (char c in pattern)
            {
                Win32.KeyPress((WindowsInput.Native.VirtualKeyCode)Win32.VkKeyScan(c));
                   
            }

        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            Utils.SetWindowExTransparent(hwnd);



        }

        private void Ellipse_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Top = Utils.GetScreenCenter().Y - Height / 2;
            Left = Utils.GetScreenCenter().X - Width / 2;


            int i = 0;
            foreach (var k in ActiveSpells)
            {

                Point center = new Point(Width / 2, Height / 2);
                float theta = (float)((float)i / ActiveSpells.Count * 2 * Math.PI);

                Image img = new Image();
                img.Effect = new DropShadowEffect();
                img.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Images/" + k.Key + ".png"));
                img.Width = ICON_SIZE;
                img.Height = ICON_SIZE;
                int r = 100;
                int x = (int)(Math.Cos(theta) * r + center.X - ICON_SIZE / 2);
                int y = (int)(Math.Sin(theta) * r + center.Y - ICON_SIZE / 2);
                Canvas.SetLeft(img, x);
                Canvas.SetTop(img, y);
                
                canvas.Children.Add(img);
                Rectangle select = new Rectangle();
                select.Width = ICON_SIZE;
                select.Height = ICON_SIZE;
                Canvas.SetLeft(select, x);
                Canvas.SetTop(select, y);
                select.Fill = Brushes.Transparent;
                select.Stroke = Brushes.LawnGreen;
                select.StrokeThickness = 4;
                canvas.Children.Add(select);
                i++;
            }



        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dispatcherTimer.Stop();
        }
    }




    public static class Utils
    {
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
        public static Point GetScreenCenter()
        {

            System.Drawing.Rectangle r = Screen.PrimaryScreen.Bounds;
            return new Point(r.Width/2,r.Height/2);

        }
    }
}
