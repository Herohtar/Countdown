using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Countdown
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        [STAThread]
        public static void Main()
        {
            using (Mutex mutex = new Mutex(false, "8D062F74-D870-47C3-BB61-73C516E58919"))
            {
                if (!mutex.WaitOne(0, false))
                {
                    return;
                }

                App app = new App();
                MainWindow window = new MainWindow();
                app.Run(window);
            }
        }
    }
}
