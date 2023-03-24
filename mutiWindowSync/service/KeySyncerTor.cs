using System;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace mutiWindowSync.service
{
    public class KeySyncerTor : IDisposable
    {
        private IntPtr _mainHandle;
        private List<IntPtr> _handles;

        private KeyboardHook _keyboardHook;

        public KeySyncerTor(IntPtr mainHandle, List<IntPtr> handles)
        {
            _mainHandle = mainHandle;
            _handles = handles;

            _keyboardHook = new KeyboardHook();
            _keyboardHook.KeyboardEvent += OnKeyboardEvent;
        }

        private void OnKeyboardEvent(object sender, KeyboardHookEventArgs e)
        {
            foreach (IntPtr handle in _handles)
            {
                SendMessage(handle, e.Message, e.WParam, e.LParam);
            }
        }

        public void Dispose()
        {
            _keyboardHook.Dispose();
        }

        #region Native Methods

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        #endregion
    }
}