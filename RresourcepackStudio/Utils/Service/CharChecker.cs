using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ResourcepackStudio.Utils.UI
{
    public static class CharChecker
    {
        public static bool Check(string? str)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
                return false;

            foreach (var c in str)
            {
                if (Path.GetInvalidFileNameChars().Contains(c))
                    return false;
            }


            return true;
        }
    }
}
