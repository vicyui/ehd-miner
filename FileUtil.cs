using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace EHDMiner
{
    internal class FileUtil
    {
        /// <summary>
        /// 从资源文件中抽取资源文件
        /// </summary>
        /// <param name="resFileName">资源文件名称（资源文件名称必须包含目录，目录间用“.”隔开,最外层是项目默认命名空间）</param>
        /// <param name="outputFile">输出文件</param>
        public static void ExtractResFile(string resFileName, string outputFile)
        {
            BufferedStream inStream = null;
            FileStream outStream = null;
            try
            {
                Assembly asm = Assembly.GetExecutingAssembly(); //读取嵌入式资源
                inStream = new BufferedStream(asm.GetManifestResourceStream(resFileName));
                outStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);

                byte[] buffer = new byte[1024];
                int length;

                while ((length = inStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outStream.Write(buffer, 0, length);
                }
                outStream.Flush();
            }
            finally
            {
                if (outStream != null)
                {
                    outStream.Close();
                }
                if (inStream != null)
                {
                    inStream.Close();
                }
            }
        }

        public static void CopyOldFilesToNewPath(string sourcePath, string savePath)
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            //拷贝labs文件夹到savePath下
            try
            {
                string[] labDirs = Directory.GetDirectories(sourcePath);//目录
                string[] labFiles = Directory.GetFiles(sourcePath);//文件
                if (labFiles.Length > 0)
                {
                    for (int i = 0; i < labFiles.Length; i++)
                    {
                        if (Path.GetExtension(labFiles[i]) != ".orig")//排除.orig文件
                        {
                            File.Copy(sourcePath + "\\" + Path.GetFileName(labFiles[i]), savePath + "\\" + Path.GetFileName(labFiles[i]), true);
                        }
                    }
                }
                if (labDirs.Length > 0)
                {
                    for (int j = 0; j < labDirs.Length; j++)
                    {
                        Directory.GetDirectories(sourcePath + "\\" + Path.GetFileName(labDirs[j]));
                        //递归调用
                        CopyOldFilesToNewPath(sourcePath + "\\" + Path.GetFileName(labDirs[j]), savePath + "\\" + Path.GetFileName(labDirs[j]));
                    }
                }
            }
            catch (Exception) { }
        }

        public static long PlotdataDictoryLength(string path)
        {
            long filesLength = 0L;
            FileInfo[] files = new DirectoryInfo(path).GetFiles();
            foreach (FileInfo fi in files)
            {
                if(fi.Extension == ".orig" || fi.Extension == ".dest")
                {
                    continue;
                }
                filesLength += fi.Length;
            }
            return filesLength;
        }

        public static Dictionary<string, string> GetDeviceInfo()
        {
            Dictionary<string,string> deviceInfo = new Dictionary< string,string> ();
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT  *  From  Win32_LogicalDisk ");
            ManagementObjectCollection queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                switch (int.Parse(mo["DriveType"].ToString()))
                {
                    case (int)DriveType.Fixed:   //本地磁盘     
                        {
                            decimal maxSize = Convert.ToDecimal(mo["Size"].ToString()) / 1024 / 1024 / 1024;
                            decimal FreeSize = Convert.ToDecimal(mo["FreeSpace"].ToString()) / 1024 / 1024 / 1024;
                            deviceInfo.Add(mo["DeviceID"].ToString(), Convert.ToInt32(FreeSize).ToString() + "/" + Convert.ToInt32(maxSize).ToString() + " GB");
                            break;
                        }
                    default:   //defalut   to   folder     
                        {
                            break;
                        }
                }
            }
            return deviceInfo;
        }

        public static long GetHardDiskSpace(string str_HardDiskName)
        {
            long totalSize = 0;
            str_HardDiskName += "\\";
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.Name == str_HardDiskName)
                {
                    totalSize = drive.TotalFreeSpace / (1024 * 1024 * 1024);
                }
            }
            return totalSize;
        }

        public static long[] GetHardDiskSpace()
        {
            long totalSize = 0;
            long plotSize = 0;
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    totalSize += drive.TotalSize;
                }
                if(Directory.Exists(drive.Name + "\\plotdata"))
                {
                    plotSize += PlotdataDictoryLength(drive.Name + "\\plotdata");
                }
            }
            return new long[] { totalSize, plotSize };
        }
    }
}
