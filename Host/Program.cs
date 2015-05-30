using System;
using System.IO;
using System.Net;

namespace Host
{
    class Program
    {
        static string hostFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc\\hosts");
        const String defaultURL = "http://pastebin.com/raw.php?i=5H377wRa";
        private static void Main(string[] args)
        {
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("Host file patcher - NoFap");
            Console.WriteLine("Please execute this program as administrator");
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("THIS SOFTWARE IS BEING PROVIDED \"AS IS\", WITHOUT ANY EXPRESS OR IMPLIED");
            Console.WriteLine("WARRANTY.  IN PARTICULAR, THE AUTHOR DOES NOT MAKES ANY");
            Console.WriteLine("REPRESENTATION OR WARRANTY OF ANY KIND CONCERNING THE MERCHANTABILITY");
            Console.WriteLine("OF THIS SOFTWARE OR ITS FITNESS FOR ANY PARTICULAR PURPOSE.");
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("Please leave the default URL (leave field empty & press enter) if you don't know what you're doing.");
            Console.WriteLine("The content of the URL must be RAW :");
            Console.WriteLine("Only text containing lines like this : {IP} {domain name}");
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("Your host file will be backed up before doing any changes.");
            Console.WriteLine("-------------------------------------------------------------------------------");

            Console.WriteLine("Type URL of the file [" + defaultURL + "]");
            String url = Console.ReadLine();
            if(url.Trim() == "")
                url = defaultURL;

            StreamReader sr;
            try
            {
                //Get the stream of the target
                sr = HTTPGet(url);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to fetch your URL : " + ex.Message);
                Console.ReadKey();
                return;
            }

            try
            {
                //Generate a name for the backup
                string backupName = GetBackupName();

                Console.WriteLine("A backup file will be created at : " + backupName);

                Console.WriteLine("Creating backup file...");
                BackupHost(backupName);
                Console.WriteLine("Backup created successfully !");

                Console.WriteLine("Writing in host file...");
                int nb = WriteToHostFile(sr);
                Console.WriteLine("Wrote " + nb + " lines to the host file.");

                Console.WriteLine("You're good to go ! Good luck !");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Please run this program as administrator: " + ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Unhandled error : " + ex.Message);
            }
            Console.ReadKey();
            return;
        }

        private static int WriteToHostFile(StreamReader sr)
        {
            int i = 0;
            using (StreamWriter w = File.AppendText(hostFile))
            {
                w.WriteLine("#GENERATED SECTION");
                while (!sr.EndOfStream)
                {
                    w.WriteLine(sr.ReadLine().Trim());
                    i++;
                }
                w.WriteLine("#END GENERATED SECTION");
            }
            return i;
        }

        private static void BackupHost(string backupName)
        {
            File.Copy(hostFile, backupName);
        }

        private static string GetBackupName()
        {
            string backupFile = "";
            //Limit to 5 backup file / day
            for (int i = 1; i < 5; i++)
            {
                backupFile = hostFile + DateTime.Now.ToString("yy-MM-dd") + "." + i + ".bak";
                if (!File.Exists(backupFile))
                    return backupFile;
            }
            throw new Exception("Too many backup files are already created. Please remove them before executing this.");
        }

        private static StreamReader HTTPGet(string URL)
        {
            WebRequest wreq = WebRequest.Create(URL);
            WebResponse wres = wreq.GetResponse();
            StreamReader objReader = new StreamReader(wres.GetResponseStream());
            return objReader;
        }
    }
}
