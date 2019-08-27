using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace Countdown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataObject dataObject;
        private HwndSource source;
        private const int HOTKEY_ID = 9000;

        private ControlWindow controlWindow;

        public MainWindow()
        {
            InitializeComponent();
            dataObject = new DataObject();
            DataContext = dataObject;

            dataObject.WhenPropertyChanged.Where(x => string.Equals(x, "SelectedMonitor")).Subscribe(x => updateWindowPosition(Width));

            Observable.Interval(TimeSpan.FromMilliseconds(1)).Subscribe(x => dataObject.UpdateCountdown());
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            var hwnd = helper.Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
            source = HwndSource.FromHwnd(hwnd);
            source.AddHook(hwndHook);
            registerHotKey();
        }

        protected override void OnClosed(EventArgs e)
        {
            source.RemoveHook(hwndHook);
            source = null;
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
            if (controlWindow == null)
            {
                controlWindow = new ControlWindow(dataObject);
            }
            
            controlWindow.Show();
            controlWindow.Activate();
        }

        private void updateWindowPosition(double width)
        {
            var m = Monitor.AllMonitors.ElementAt(dataObject.SelectedMonitor);
            Top = m.Bounds.Top;

            var move = new DoubleAnimation(Left, m.Bounds.Left + ((m.Bounds.Width / 2) - (width / 2)), TimeSpan.FromMilliseconds(250))
            {
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(move, this);
            Storyboard.SetTargetProperty(move, new PropertyPath(LeftProperty));

            Storyboard sb = new Storyboard();
            sb.Children.Add(move);
            sb.Begin();
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            updateWindowPosition(e.NewSize.Width);
        }

        private void settingsItem_Click(object sender, RoutedEventArgs e)
        {
            onHotKeyPressed();
        }

        private void exitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
