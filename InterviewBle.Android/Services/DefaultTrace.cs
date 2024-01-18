using System;
using InterviewBle.Helpers;

namespace InterviewBle.Droid.Services
{
    static class DefaultTrace
    {
        static DefaultTrace()
        {
            Trace.TraceImplementation = Console.WriteLine;
        }
    }
}
