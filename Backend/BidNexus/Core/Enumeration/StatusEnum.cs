using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Enumeration
{
    public enum StatusEnum
    {
        Draft = 1,
        Authorized = 2,
        OpenForIntent = 3,
        IntentEvaluation = 4,
        Scheduled = 5,
        Open = 6,
        Completed = 7
    }
}
