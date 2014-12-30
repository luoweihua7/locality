using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Locality
{
    public class FileService
    {
        public static string GetPath(string fileName, FileHook fileHook)
        {
            string filePath = null;

            switch (fileHook.Type)
            {
                case HookType.File:

                    break;
                case HookType.Folder:

                    break;
                default:
                    break;
            }



            return filePath;
        }
    }
}
