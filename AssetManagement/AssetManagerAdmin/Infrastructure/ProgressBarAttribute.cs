using System;

namespace AssetManagerAdmin.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ProgressBarAttribute : Attribute
    {
        public ProgressBarAttribute(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}
