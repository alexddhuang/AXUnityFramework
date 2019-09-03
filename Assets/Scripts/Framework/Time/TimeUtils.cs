using System;

namespace AXUnityFramework {

namespace Time {

public class TimeUtils 
{
    public static Int32 UnixTime()
    {
        return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }
}

} // End of namespace AXUnityFramework

} // End of namespace Time