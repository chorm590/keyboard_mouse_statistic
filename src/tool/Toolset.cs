using System;
using System.Collections.Generic;
using System.Text;

namespace KMS.src.tool
{
    internal static class Toolset
    {
        internal static string GetParentPath(string path)
        {
            if (path == null || path.Length == 0)
                return null;

            return path.Substring(0, path.LastIndexOf('/'));
        }
    }
}
