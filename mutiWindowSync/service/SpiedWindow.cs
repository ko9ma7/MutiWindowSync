using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace mutiWindowSync.service
{
    public class SpiedWindow : EventArgs
    {
        #region Under the Hood

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);
        
        // 获取子窗口句柄
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(Rectangle r)
                : this(r.Left, r.Top, r.Right, r.Bottom)
            {
            }

            public int X
            {
                get { return Left; }
                set
                {
                    Right -= (Left - value);
                    Left = value;
                }
            }

            public int Y
            {
                get { return Top; }
                set
                {
                    Bottom -= (Top - value);
                    Top = value;
                }
            }

            public int Height
            {
                get { return Bottom - Top; }
                set { Bottom = value + Top; }
            }

            public int Width
            {
                get { return Right - Left; }
                set { Right = value + Left; }
            }

            public Point Location
            {
                get { return new Point(Left, Top); }
                set
                {
                    X = value.X;
                    Y = value.Y;
                }
            }

            public Size Size
            {
                get { return new Size(Width, Height); }
                set
                {
                    Width = value.Width;
                    Height = value.Height;
                }
            }

            public static implicit operator Rectangle(RECT r)
            {
                return new Rectangle(r.Left, r.Top, r.Width, r.Height);
            }

            public static implicit operator RECT(Rectangle r)
            {
                return new RECT(r);
            }

            public static bool operator ==(RECT r1, RECT r2)
            {
                return r1.Equals(r2);
            }

            public static bool operator !=(RECT r1, RECT r2)
            {
                return !r1.Equals(r2);
            }

            public bool Equals(RECT r)
            {
                return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            }

            public override bool Equals(object obj)
            {
                if (obj is RECT)
                    return Equals((RECT)obj);
                if (obj is Rectangle)
                    return Equals(new RECT((Rectangle)obj));
                return false;
            }

            public override int GetHashCode()
            {
                return ((Rectangle)this).GetHashCode();
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top,
                                     Right, Bottom);
            }
        }

        #endregion

        public SpiedWindow(IntPtr handle)
        {
            Handle = handle;
            RECT rect;
            GetWindowRect(Handle, out rect);
            Area = rect;

            SetCaption();
        }

        public IntPtr Handle { get; private set; }
        public Rectangle Area { get; private set; }
        public string Caption { get; private set; }

        private void SetCaption()
        {
            int len = GetWindowTextLength(Handle);
            var str = new StringBuilder(len + 1);
            GetWindowText(Handle, str, len + 1);
            Caption = str.ToString();
        }

        public SpiedWindow GetParentWindow(IntPtr Hwnd)
        {
            return new SpiedWindow(GetParent(Hwnd));
        }

        public void SeParentwindow(IntPtr parentHandle)
        {
            SetParent(Handle, parentHandle);
        }

        public IEnumerable<SpiedWindow> GetChildren()
        {
            var children = new List<SpiedWindow>();
            EnumChildWindows(Handle, (hWnd, lp) =>
            {
                children.Add(new SpiedWindow(hWnd));
                return true;
            }, IntPtr.Zero);
            return children;
        }

        public override string ToString()
        {
            return Caption;
        }
    }
    
    public class SpyAgent
    {
        private readonly Timer _timer;
        private SpyAgentLocator _locator;

        public SpyAgent()
        {
            _timer = new Timer { Interval = 200, Enabled = false };
            _timer.Tick += OnTimerTicked;
        }

        public event EventHandler<SpiedWindow> SpiedWindowSelected;

        private void OnTimerTicked(object sender, EventArgs e)
        {
            ShowLocator();
        }

        private void ShowLocator()
        {
            SpiedWindow window = GetHoveredWindow();

            if (window.Handle == IntPtr.Zero)
            {
                _locator.Hide();
                return;
            }

            _locator.Location = window.Area.Location;
            _locator.Size = window.Area.Size;
            _locator.TopLevel = true;
            _locator.TopMost = true;
            _locator.Show();
        }

        public void BeginSpying()
        {
            if (_locator != null)
            {
                _locator.Close();
                _locator.Dispose();
            }

            _locator = new SpyAgentLocator();
            _locator.KeyDown += OnLocatorSelected;

            MakePassThrough(_locator.Handle);

            _timer.Enabled = true;
        }

        private void OnLocatorSelected(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.S)
            {
                e.Handled = true;
                if (SpiedWindowSelected == null) return;
                SpiedWindowSelected(this, GetHoveredWindow());
            }
        }

        public void EndSpying()
        {
            _timer.Enabled = false;

            _locator.Close();
            _locator.Dispose();
            _locator = null;
        }

        private class SpyAgentLocator : Form
        {
            public SpyAgentLocator()
            {
                FormBorderStyle = FormBorderStyle.None;
                BackColor = Color.OrangeRed;
                Opacity = 0.25;
                TopLevel = true;
                TopMost = true;
            }
        }

        #region Under the Hood

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x20;

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT point);

        [DllImport("user32.dll")]
        private static extern IntPtr ChildWindowFromPoint(IntPtr hWndParent, POINT point);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        public static SpiedWindow GetHoveredWindow()
        {
            IntPtr handle = WindowFromPoint(Cursor.Position);
            return new SpiedWindow(handle);
        }

        private static void MakePassThrough(IntPtr handle)
        {
            int exstyle = GetWindowLong(handle, GWL_EXSTYLE);
            exstyle |= WS_EX_TRANSPARENT;
            SetWindowLong(handle, GWL_EXSTYLE, exstyle);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public readonly int X;
            public readonly int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public POINT(Point pt)
                : this(pt.X, pt.Y)
            {
            }

            public static implicit operator Point(POINT p)
            {
                return new Point(p.X, p.Y);
            }

            public static implicit operator POINT(Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        #endregion
    }
}