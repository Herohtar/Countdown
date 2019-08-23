using System;
using System.Threading;

namespace Countdown
{
  public class EntryPoint
  {
    [STAThread]
    public static void Main()
    {
      using (Mutex mutex = new Mutex(true, "8D062F74-D870-47C3-BB61-73C516E58919", out bool isFirstApp))
      {
        if (!isFirstApp)
        {
          return;
        }

        App app = new App();
        app.Run();
      }
    }
  }
}
