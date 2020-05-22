using System;
using System.IO;
using System.Net;

namespace EHDMiner
{
    public class FileDownloadUtil
    {
        private string url; //文件下载网络地址
        private string path; //文件下载位置，如d:/download
        private string filename; //文件名，如test.jpg
        private string fileId; //文件ID，文件唯一标识，一般为UUID

        public delegate void ProgressChangedHandler(int progress, string fileId);
        public event ProgressChangedHandler ProgressChanged;
        protected virtual void OnProgressChanged(int progress, string fileId)
        {
            ProgressChanged?.Invoke(progress, fileId);
        }
        public delegate void DownloadFinishHandler(bool isSuccess, string downloadPath, string fileId, string msg = null);
        public event DownloadFinishHandler DownloadFinish;
        protected virtual void OnDownloadFinish(bool isSuccess, string downloadPath, string fileId, string msg = null)
        {
            DownloadFinish?.Invoke(isSuccess, downloadPath, fileId, msg);
        }

        //通过网络链接直接下载任意文件
        public FileDownloadUtil(string url, string path, string filename, string fileId)
        {
            this.url = url;
            this.path = path;
            this.filename = filename;
            this.fileId = fileId;
        }

        public void Download()
        {
            Download(url, path, filename, fileId);
        }
        private void Download(string url, string path, string filename, string fileId)
        {

            if (!Directory.Exists(path))   //判断文件夹是否存在
                Directory.CreateDirectory(path);
            path = path + "/" + filename;
            string tempFile = path + ".temp"; //临时文件
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);    //存在则删除
            }
            if (File.Exists(path))
            {
                File.Delete(path);    //存在则删除
            }
            FileStream fs = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream responseStream = null;
            try
            {
                //创建临时文件
                fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                request = WebRequest.Create(url) as HttpWebRequest;
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                responseStream = response.GetResponseStream();
                byte[] bArr = new byte[1024];
                long totalBytes = response.ContentLength; //通过响应头获取文件大小，前提是响应头有文件大小
                int size = responseStream.Read(bArr, 0, (int)bArr.Length); //读取响应流到bArr，读取大小
                float percent = 0;  //用来保存计算好的百分比
                long totalDownloadedByte = 0;  //总共下载字节数
                while (size > 0)  //while循环读取响应流
                {
                    fs.Write(bArr, 0, size); //写到临时文件
                    totalDownloadedByte += size;
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                    percent = (float)totalDownloadedByte / (float)totalBytes * 100;
                    OnProgressChanged((int)percent, fileId);  //下载进度回调
                }
                if (File.Exists(path))
                {
                    File.Delete(path);    //存在则删除
                }
                File.Move(tempFile, path); //重命名为正式文件
                OnDownloadFinish(true, path, fileId, null);  //下载完成，成功回调
            }
            catch (Exception ex)
            {
                OnDownloadFinish(false, null, fileId, ex.Message);  //下载完成，失败回调
            }
            finally
            {
                if (fs != null)
                    fs.Close();
                if (request != null)
                    request.Abort();
                if (response != null)
                    response.Close();
                if (responseStream != null)
                    responseStream.Close();
            }
        }
    }
}
