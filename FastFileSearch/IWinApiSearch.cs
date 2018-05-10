using System.Collections.Generic;

namespace FastFileSearch
{
    public interface IWinApiSearch
    {
        List<Win32FileInfo> RecursiveScan(string directory, string searchMask = @"\*");
    }
}
