using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Countdown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string timeLeft;
        private DateTime targetDate;
        private Brush textColorBrush;
        private Color textColor;
        private Color shadowColor;
        private HwndSource _source;
        private const int HOTKEY_ID = 9000;
        private ControlWindow controlWindow;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            this.TargetDate = Properties.Settings.Default.TargetDate;
            this.TextColor = Properties.Settings.Default.TextColor;
            this.ShadowColor = Properties.Settings.Default.ShadowColor;

            PeriodicTask.Run(updateCountdown, TimeSpan.FromMilliseconds(1));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            var hwnd = helper.Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
            this._source = HwndSource.FromHwnd(hwnd);
            this._source.AddHook(hwndHook);
            registerHotKey();
        }

        protected override void OnClosed(EventArgs e)
        {
            this._source.RemoveHook(hwndHook);
            this._source = null;
            unregisterHotKey();
            base.OnClosed(e);
        }

        public DateTime TargetDate
        {
            get { return this.targetDate; }
            set
            {
                if (this.targetDate != value)
                {
                    this.targetDate = value;
                    Properties.Settings.Default.TargetDate = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("TargetDate");
                }
            }
        }

        public Brush TextColorBrush
        {
            get { return this.textColorBrush; }
            set
            {
                if (this.textColorBrush != value)
                {
                    this.textColorBrush = value;
                    RaisePropertyChangedEvent("TextColorBrush");
                }
            }
        }

        public Color TextColor
        {
            get { return this.textColor; }
            set
            {
                if (this.textColor != value)
                {
                    this.textColor = value;
                    this.TextColorBrush = new SolidColorBrush(value);
                    Properties.Settings.Default.TextColor = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("TextColor");
                }
            }
        }

        public Color ShadowColor
        {
            get { return this.shadowColor; }
            set
            {
                if (this.shadowColor != value)
                {
                    this.shadowColor = value;
                    Properties.Settings.Default.ShadowColor = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("ShadowColor");
                }
            }
        }

        public string TimeLeft
        {
            get { return this.timeLeft; }
            set
            {
                if (this.timeLeft != value)
                {
                    this.timeLeft = value;
                    RaisePropertyChangedEvent("TimeLeft");
                }
            }
        }

        private IntPtr hwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            onHotKeyPressed();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void registerHotKey()
        {
            var helper = new WindowInteropHelper(this);
            if (!WindowsServices.RegisterHotKey(helper.Handle, HOTKEY_ID, WindowsServices.MOD_CTRL | WindowsServices.MOD_WIN | WindowsServices.MOD_ALT, (uint)WindowsServices.Keys.J))
            {
                // handle error
            }
        }

        private void unregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            WindowsServices.UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        private void onHotKeyPressed()
        {
            if (this.controlWindow == null)
            {
                this.controlWindow = new ControlWindow(this);
            }
            
            this.controlWindow.Show();
            this.controlWindow.Activate();
        }

        private void updateCountdown()
        {
            TimeSpan difference = this.targetDate - DateTime.Now;
            string timeString = "";

            if (difference.Days > 0)
            {
                timeString += formatUnits(difference.Days, "day") + ", ";
            }
            if ((difference.Hours > 0) || !String.IsNullOrWhiteSpace(timeString))
            {
                timeString += formatUnits(difference.Hours, "hour") + ", ";
            }
            if ((difference.Minutes > 0) || !String.IsNullOrWhiteSpace(timeString))
            {
                timeString += formatUnits(difference.Minutes, "minute") + ", ";
            }
            if ((difference.Seconds > 0) || !String.IsNullOrWhiteSpace(timeString))
            {
                timeString += formatUnits(difference.Seconds, "second") + ", ";
            }
            if ((difference.Milliseconds > 0) || !String.IsNullOrWhiteSpace(timeString))
            {
                timeString += formatUnits(difference.Milliseconds, "millisecond");
            }

            this.TimeLeft = timeString;
        }

        private string formatUnits(int count, string units)
        {
            if (units == "millisecond")
            {
                return String.Format("{0:D3} {1}{2}", count, units, (count == -1) ? "" : "s");
            }

            return String.Format("{0} {1}{2}", count, units, (count == 1) ? "" : "s");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Left = (SystemParameters.PrimaryScreenWidth / 2) - (e.NewSize.Width / 2);
            this.Top = 0;
        }
    }
}
