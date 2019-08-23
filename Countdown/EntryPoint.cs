using System;
using System.Threading;

namespace Countdown
{
  public class EntryPoint
  {
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
