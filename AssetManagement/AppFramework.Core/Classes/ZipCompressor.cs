using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace AppFramework.Core.Classes
{
    public static class ZipCompressor
    {
        public static void Compress(List<string> filepaths, string targetZipFilePath, bool deleteSource)
        {
            if (string.IsNullOrWhiteSpace(targetZipFilePath))
                throw new ArgumentException("targetZipFilePath");

            using (var fs = File.Create(targetZipFilePath))
            using (var zipOut = new ZipOutputStream(fs))
            {
                foreach (string fName in filepaths)
                {
                    FileInfo fi = new FileInfo(fName);
                    ZipEntry entry = new ZipEntry(fi.Name);
                    using (var sReader = File.OpenRead(fName))
                    {
                        byte[] buff = new byte[Convert.ToInt32(sReader.Length)];
                        sReader.Read(buff, 0, (int)sReader.Length);
                        entry.DateTime = fi.LastWriteTime;
                        entry.Size = sReader.Length;
                        sReader.Close();
                        zipOut.PutNextEntry(entry);
                        zipOut.Write(buff, 0, buff.Length);
                    }
                }

                zipOut.Finish();

                if (deleteSource)
                {
                    foreach (string fName in filepaths)
                    {
                        File.Delete(fName);
                    }
                }
            }
        }
    }
}

