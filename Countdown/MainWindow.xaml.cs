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
        private DateTime target = new DateTime(2017, 4, 8, 16, 32, 0);
        private HwndSource _source;
        private const int HOTKEY_ID = 9000;
        private ControlWindow controlWindow;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

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
                this.controlWindow = new ControlWindow();
            }
            
            this.controlWindow.Show();
            this.controlWindow.Activate();
        }

        private void updateCountdown()
        {
            TimeSpan difference = target - DateTime.Now;
            this.TimeLeft = String.Format("{0} days, {1:D2} hours, {2:D2} minutes, {3:D2} seconds, {4:D3} milliseconds", difference.Days, difference.Hours, difference.Minutes, difference.Seconds, difference.Milliseconds);
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
