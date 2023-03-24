using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace mutiWindowSync.service
{
    public class SynHandleService
    {
        KeySyncer keySyncer;
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool ReleaseCapture();

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int X;
            public int Y;
        }

        private IntPtr _mainHandle;
        private List<IntPtr> _handles;

        public SynHandleService(IntPtr mainHandle, List<IntPtr> handles)
        {
            _mainHandle = mainHandle;
            _handles = handles;
        }

        public void SyncHandleAsync()
        {
            ThreadPool.QueueUserWorkItem(state => SyncHandle());
        }
        
        private void SyncHandle()
        {
            keySyncer = new KeySyncer(_mainHandle, _handles);
            GC.KeepAlive(keySyncer);
        }
    }
}