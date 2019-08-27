using System;
using System.Threading;

namespace Countdown
{
    public class EntryPoint
    {
        [STAThread]
        public static void Main()
        {
            using (var mutex = new Mutex(true, "8D062F74-D870-47C3-BB61-73C516E58919", out bool isFirstApp))
            {
                if (!isFirstApp)
                {
                    return;
                }

                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}
