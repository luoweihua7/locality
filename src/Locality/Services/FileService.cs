using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

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
                    if (fileHook.Path.EndsWith(fileName))
                    {
                        filePath = fileHook.Path;
                    }
                    break;
                case HookType.Folder:
                    filePath = FindFile(fileName, fileHook.Path);
                    break;
                default:
                    break;
            }

            return filePath;
        }

        public static string FindFile(string fileName, string folderName)
        {
            string filePath = null;

            string[] files = Directory.GetFiles(folderName);


            return filePath;
        }
    }
}
