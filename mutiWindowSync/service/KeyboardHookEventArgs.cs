using System;

namespace mutiWindowSync.service
{
    public class KeyboardHookEventArgs : EventArgs
    {
        public int Message { get; }
        public IntPtr WParam { get; }
        public IntPtr LParam { get; }

        public KeyboardHookEventArgs(int message, IntPtr wParam, IntPtr lParam)
        {
            Message = message;
            WParam = wParam;
            LParam = lParam;
        }
    }
}