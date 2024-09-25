using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

class Program
{
    // Import necessary functions from user32.dll to simulate mouse click
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

    // Mouse event constants
    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;

    // Interval in milliseconds
    private static int clickInterval = 10; // Default to 10 ms (1 second)

    // Define the timer
    private static System.Threading.Timer timer;

    // Variable to track if the timer should be running
    private static bool isRunning = true;

    static void Main(string[] args)
    {
        // Check if an interval argument was provided
        if (args.Length > 0)
        {
            if (int.TryParse(args[0], out int interval))
            {
                clickInterval = interval;
            }
        }

        Console.WriteLine($"Mouse clicker started. Clicking every {clickInterval} milliseconds.");
        Console.WriteLine("Press the space bar to stop.");

        // Create a timer that calls the ClickMouse method at the specified interval
        timer = new System.Threading.Timer(ClickMouse, null, 0, clickInterval);

        // Start a thread to listen for key presses
        Thread keyListenerThread = new Thread(ListenForKeyPress);
        keyListenerThread.Start();

        // Keep the application running
        Application.Run();
    }

    private static void ClickMouse(object state)
    {
        if (!isRunning) return;

        // Get current mouse position
        var x = (uint)Cursor.Position.X;
        var y = (uint)Cursor.Position.Y;

        // Simulate left mouse button down and up
        mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);

        Console.WriteLine($"Mouse clicked at position ({x}, {y})");
    }

    private static void ListenForKeyPress()
    {
        while (isRunning)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true).Key;
                if (key == ConsoleKey.Spacebar)
                {
                    isRunning = false;
                    timer.Dispose();
                    Console.WriteLine("Mouse clicking stopped.");
                    Application.Exit();
                }
            }
            Thread.Sleep(100); // Small delay to reduce CPU usage
        }
    }
}
