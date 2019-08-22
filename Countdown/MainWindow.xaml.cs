using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Countdown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataObject dataObject;
        private HwndSource _source;
        private const int HOTKEY_ID = 9000;

        private ControlWindow controlWindow;

        public MainWindow()
        {
            InitializeComponent();
            this.dataObject = new DataObject();
            this.DataContext = this.dataObject;

            this.dataObject.WhenPropertyChanged.Where(x => string.Equals(x, "SelectedMonitor")).Subscribe(x => this.updateWindowPosition(this.Width));

            Observable.Interval(TimeSpan.FromMilliseconds(1)).Subscribe(x => this.dataObject.UpdateCountdown());
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
                this.controlWindow = new ControlWindow(this.dataObject);
            }
            
            this.controlWindow.Show();
            this.controlWindow.Activate();
        }

        private void updateWindowPosition(double width)
        {
            Monitor m = Monitor.AllMonitors.ElementAt(this.dataObject.SelectedMonitor);
            this.Top = m.Bounds.Top;

            var move = new DoubleAnimation(this.Left, m.Bounds.Left + ((m.Bounds.Width / 2) - (width / 2)), TimeSpan.FromMilliseconds(250));
            move.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };
            
            Storyboard.SetTarget(move, this);
            Storyboard.SetTargetProperty(move, new PropertyPath(Window.LeftProperty));

            Storyboard sb = new Storyboard();
            sb.Children.Add(move);
            sb.Begin();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            updateWindowPosition(e.NewSize.Width);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
