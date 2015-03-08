using Microsoft.Build.Framework;
using System;

namespace Lithogen.TaskShim
{
    static class MessageImportanceValidator
    {
        public static MessageImportance Validate(string importance)
        {
            if (String.IsNullOrWhiteSpace(importance) || importance.Equals("High", StringComparison.InvariantCultureIgnoreCase))
            {
                return Microsoft.Build.Framework.MessageImportance.High;
            }
            else if (importance.Equals("Normal", StringComparison.InvariantCultureIgnoreCase))
            {
                return Microsoft.Build.Framework.MessageImportance.Normal;
            }
            else if (importance.Equals("Low", StringComparison.InvariantCultureIgnoreCase))
            {
                return Microsoft.Build.Framework.MessageImportance.Low;
            }
            else
            {
                string msg = "Invalid MessageImportance of '" + importance + "'. Valid values are High, Normal and Low. High is the default.";
                throw new ArgumentOutOfRangeException(msg);
            }
        }
    }
}
