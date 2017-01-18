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
        private int minimumLevel;
        private Units minimumUnits;
        private string completionText;
        private HwndSource _source;
        private const int HOTKEY_ID = 9000;
        private ControlWindow controlWindow;

        public enum Units
        {
            Milliseconds,
            Seconds,
            Minutes,
            Hours,
            Days
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            this.TargetDate = Properties.Settings.Default.TargetDate;
            this.TextColor = Properties.Settings.Default.TextColor;
            this.ShadowColor = Properties.Settings.Default.ShadowColor;
            this.MinimumLevel = Properties.Settings.Default.MinimumLevel;
            this.CompletionText = Properties.Settings.Default.CompletionText;

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

        public string CompletionText
        {
            get { return this.completionText; }
            set
            {
                if (this.completionText != value)
                {
                    this.completionText = value;
                    Properties.Settings.Default.CompletionText = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("CompletionText");
                }
            }
        }

        public int MinimumLevel
        {
            get { return this.minimumLevel; }
            set
            {
                if (this.minimumLevel != value)
                {
                    this.minimumLevel = value;
                    this.MinimumUnits = (Units)value;
                    Properties.Settings.Default.MinimumLevel = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChangedEvent("MinimumLevel");
                }
            }
        }

        public Units MinimumUnits
        {
            get { return this.minimumUnits; }
            set
            {
                if (this.minimumUnits != value)
                {
                    this.minimumUnits = value;
                    RaisePropertyChangedEvent("MinimumUnits");
                }
            }
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
            List<string> timeStrings = new List<string>();

            if (difference.Days > 0)
            {
                timeStrings.Add(formatUnits(difference.Days, "day"));
            }
            if (((this.MinimumUnits <= Units.Hours) || (timeStrings.Count == 0)) && ((difference.Hours > 0) || (timeStrings.Count > 0)))
            {
                timeStrings.Add(formatUnits(difference.Hours, "hour"));
            }
            if (((this.MinimumUnits <= Units.Minutes) || (timeStrings.Count == 0)) && ((difference.Minutes > 0) || (timeStrings.Count > 0)))
            {
                timeStrings.Add(formatUnits(difference.Minutes, "minute"));
            }
            if (((this.MinimumUnits <= Units.Seconds) || (timeStrings.Count == 0)) && ((difference.Seconds > 0) || (timeStrings.Count > 0)))
            {
                timeStrings.Add(formatUnits(difference.Seconds, "second"));
            }
            if (((this.MinimumUnits <= Units.Milliseconds) || (timeStrings.Count == 0)) && ((difference.Milliseconds > 0) || (timeStrings.Count > 0)))
            {
                timeStrings.Add(formatUnits(difference.Milliseconds, "millisecond"));
            }

            this.TimeLeft = (timeStrings.Count > 0) ? String.Join(", ", timeStrings) : this.CompletionText;
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
