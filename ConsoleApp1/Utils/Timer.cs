namespace ConsoleApp1.Utils;

public static class Timer
{
    public static bool ReadingRunning;

    public static int Delay(bool delayHour = false, bool delayMinutes = false, bool delaySeconds = false)
    {
        var rnd = new Random(DateTime.Now.Millisecond);
        var ticks = rnd.Next(60000, 90000);

        if (delaySeconds)
        {
            ticks = rnd.Next(10000, 60000);
        }

        if (delayHour)
        {
            ticks = 1000 * 60 * 60 * rnd.Next(3, 5) + rnd.Next(30000, 2500000);
        }

        if (delayMinutes)
        {
            ticks = rnd.Next(600000, 900000); // from:10 minutes; until:15 minutes
        }

        return 7000 + ticks;
    }
}