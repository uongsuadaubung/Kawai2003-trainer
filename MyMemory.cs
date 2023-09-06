using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Kawai2003_trainer
{
    internal class MyMemory
    {

        private IntPtr _processHandle;
        private IntPtr _hWnd;
        private Process? _process;
        private const int ProcessAllAccess = 0x1F0FFF;

        private readonly string _processName;

        private int _baseAddress;
        //const int PROCESS_WM_READ = 0x0010;
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string? lpszWindow);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);


        private byte[] ReadBytes(IntPtr handle, int address, int size)
        {
            if (!IsOk())
            {
                return Array.Empty<byte>();
            }

            int bytesRead = 0;
            byte[] buffer = new byte[size];
            ReadProcessMemory(handle, address, buffer, size, ref bytesRead);
            return buffer;
        }
        /// <summary>
        /// Lấy Base Address của chương trình
        /// </summary>
        /// <returns>Base address</returns>
        public int GetBaseAddress()
        {
            return _baseAddress;
        }

        private void GetProcess()
        {
            Process[] processes = Process.GetProcessesByName(_processName);

            if (processes.Length > 0)
            {
                _process = Process.GetProcessesByName(_processName)[0];

                _processHandle = OpenProcess(ProcessAllAccess, false, _process.Id);
                if (_process.MainModule != null) _baseAddress = _process.MainModule.BaseAddress.ToInt32();
            }
            else
            {
                _process = null;
            }
        }

        private void GetWindow()
        {
            //Kawai2003 v1.00

            IntPtr parentHWnd = FindWindow("ThunderRT5Form", "Kawai2003 v1.00");
            IntPtr res = IntPtr.Zero;

            while (true)
            {
                res = FindWindowEx(parentHWnd, res, "ThunderRT5PictureBox", null);
                if (res == IntPtr.Zero) break;
                _hWnd = res;
            }


        }

        public MyMemory(string processName)
        {
            this._processName = processName; 

            GetProcess();
            GetWindow();
        }

        private int ReadInt(int address)
        {
            return BitConverter.ToInt32(ReadBytes(_processHandle, address, 4), 0);
        }
        public ushort ReadUShort(int address)
        {
            return BitConverter.ToUInt16(ReadBytes(_processHandle, address, 2), 0);
        }

        public int GetAddressFromPointer(int[] offsets)
        {

            int value = 0;

            for (int i = 0; i < offsets.Length - 1; i++)
            {
                value = ReadInt(offsets[i] + value);
            }
            int addr = value + offsets[^1];
            return addr;
        }

        public bool IsOk()
        {
            bool result = _process is { HasExited: false };
            if (!result)
            {
                GetProcess();
            }
            return result;
        }

        public void ClickToCell(int row, int column, int button)
        {
            int x = 120 + (80 * column);
            int y = 120 + (90 * row);

            IntPtr lpa = (IntPtr)((y << 16) | x);
            SendMessage(_hWnd, (uint)button, (IntPtr)1, lpa);
        }
    }
}
