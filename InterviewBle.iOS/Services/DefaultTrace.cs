using System;
using InterviewBle.Helpers;

namespace InterviewBle.iOS.Services
{
    static class DefaultTrace
    {
        static DefaultTrace()
        {
            Trace.TraceImplementation = Console.WriteLine;
        }
    }
}
