using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace FIRPy.Notifications
{
    public class NotifFTP
    {
        private static string ftpServerIP = ConfigurationSettings.AppSettings["FtpServerIP"];
        private static string ftpUserName = ConfigurationSettings.AppSettings["FtpUserName"];
        private static string ftpPassword = ConfigurationSettings.AppSettings["FtpPassword"];
        
        public static void UploadFile(string filename, string addtionalFolder = "")
        {
            string tempLocation = string.Empty;
            FileInfo objFile = new FileInfo(filename);
            FtpWebRequest objFTPRequest;

            if (!string.IsNullOrEmpty(addtionalFolder))
            {
                tempLocation = ftpServerIP + addtionalFolder;
            }
            else
            {
                tempLocation = ftpServerIP;
            }

            // Create FtpWebRequest object 
            objFTPRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + tempLocation + "/" + objFile.Name));

            // Set Credintials
            objFTPRequest.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

            // By default KeepAlive is true, where the control connection is 
            // not closed after a command is executed.
            objFTPRequest.KeepAlive = false;

            // Set the data transfer type.
            objFTPRequest.UseBinary = true;

            // Set content length
            objFTPRequest.ContentLength = objFile.Length;

            // Set request method
            objFTPRequest.Method = WebRequestMethods.Ftp.UploadFile;

            // Set buffer size
            int intBufferLength = 16 * 1024;
            byte[] objBuffer = new byte[intBufferLength];

            // Opens a file to read
            FileStream objFileStream = objFile.OpenRead();

            try
            {
                // Get Stream of the file
                Stream objStream = objFTPRequest.GetRequestStream();

                int len = 0;

                while ((len = objFileStream.Read(objBuffer, 0, intBufferLength)) != 0)
                {
                    // Write file Content 
                    objStream.Write(objBuffer, 0, len);

                }

                objStream.Close();
                objFileStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void UploadFiles(string[] files, string addtionalFolder = "")
        {
            Parallel.ForEach(files, file =>
            {
                UploadFile(file, addtionalFolder);
            });
        }
    }
}
