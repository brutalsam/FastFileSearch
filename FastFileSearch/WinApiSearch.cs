using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace FastFileSearch
{
    public class WinApiSearch : IWinApiSearch
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindFirstFileW(string lpFileName, out Win32FindDataw lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool FindNextFile(IntPtr hFindFile, out Win32FindDataw lpFindFileData);

        [DllImport("kernel32.dll")]
        public static extern bool FindClose(IntPtr hFindFile);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct Win32FindDataw
        {
            public FileAttributes dwFileAttributes;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public int dwReserved0;
            public int dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        public List<Win32FileInfo> RecursiveScan(string directory, string searchMask = @"\*")
        {
            var invalidHandleValue = new IntPtr(-1);
            var findHandle = invalidHandleValue;

            var info = new List<Win32FileInfo>();
            try
            {
                findHandle = FindFirstFileW(directory + searchMask, out var findData);
                if (findHandle != invalidHandleValue)
                {
                    do
                    {
                        if (findData.cFileName == "." || findData.cFileName == "..") continue;

                        var fullpath = directory + (directory.EndsWith("\\") ? "" : "\\") + findData.cFileName;

                        if ((findData.dwFileAttributes & FileAttributes.Directory) != 0)
                        {
                            info.AddRange(RecursiveScan(fullpath));
                        }
                        else
                        {
                            info.Add(new Win32FileInfo
                            {
                                Path = fullpath,
                                Size = CombineHighLowInts(findData.nFileSizeHigh, findData.nFileSizeLow)
                            });
                        }
                    }
                    while (FindNextFile(findHandle, out findData));

                }
            }
            finally
            {
                if (findHandle != invalidHandleValue) FindClose(findHandle);
            }
            return info;
        }


        private long CombineHighLowInts(uint high, uint low)
        {
            return ((long)high << 0x20) | low;
        }
    }
}
