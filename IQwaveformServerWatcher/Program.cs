using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Tap.Plugins.IQ;
using System.Xml.Linq;


namespace IQwaveformServerWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Init();
        }

        /// <summary>
        /// Watcher.
        /// </summary>
        static string directory = @"\\cifs.casinas03.lvn.broadcom.net\cifs\wsd_iqdata\iqwavedata\Released";
        //static string directory = @"C:\Avago.ATF.2.2.6\Data\TestPlans_SupportFiles\FlexTest";
        static string iqFolderStructureFilename = "ByFolderStructure.xml";
        /// <summary>
        /// Init.
        /// </summary>
        static void Init()
        {
            try
            {

            }
            catch (Exception e)
            {
            }
            using (FileSystemWatcher _watcher = new FileSystemWatcher())
            {
                _watcher.Path = directory;
                _watcher.IncludeSubdirectories = true;

                //                _watcher.Changed += new FileSystemEventHandler(Program._watcher_Changed);
                _watcher.NotifyFilter = NotifyFilters.LastAccess
                                    | NotifyFilters.LastWrite
                                    | NotifyFilters.FileName
                                    | NotifyFilters.DirectoryName;

                _watcher.Changed += new FileSystemEventHandler(Program._watcher_Changed);
                _watcher.Created += new FileSystemEventHandler(Program._watcher_Changed);
                _watcher.Deleted += new FileSystemEventHandler(Program._watcher_Changed);
                _watcher.Renamed += new RenamedEventHandler(Program._watcher_Renamed);
                _watcher.Filter = "*.*";
                _watcher.InternalBufferSize = 32768;
                _watcher.EnableRaisingEvents = true;

                // Wait for the user to quit the program.
               Console.WriteLine("Press 'q' to quit the sample.");
                while (Console.Read() != 'q') ;
            }
        }

        /// <summary>
        /// Handler.
        /// </summary>
        /// 
        private static string[] extensions = { ".txt", ".zip"};
        static void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var ext = (Path.GetExtension(e.FullPath) ?? string.Empty).ToLower();

            if (extensions.Any(ext.Equals))
            {
                // initialize the value of filename 
                string filename = null;

                // using the method 
                filename = Path.GetFileName(e.FullPath);
                if (filename == "comment_v2.txt" || ext == ".zip")
                {
                    UpdateXml();
                    OpenLog(e);
                }
            }


        }
        static void _watcher_Renamed(object sender, RenamedEventArgs e)
        {
            // FullPath is the new file name.
            UpdateXml();
            Console.WriteLine(string.Format("Renamed: {0} {1}", e.FullPath, e.ChangeType));
        }

        static void UpdateXml()
        {
            string remoteXmlStructureFile = Path.Combine(directory, iqFolderStructureFilename);
            var iqXml = new IqXmlToTreeView(remoteXmlStructureFile);
            iqXml.UpdateXml(directory);
        }

        static void OpenLog(FileSystemEventArgs e)
        {
            string path = @"c:\temp\IQwaveformLog.txt";

            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("IQ Waveform Folder Structer Log\n\n");
                }
            }
            // Create a file to write to.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(string.Format("Changed: {0} {1}", e.FullPath, e.ChangeType));
                sw.WriteLine("Date Modified: " + DateTime.Now);
                sw.WriteLine("Update ByFolderStructure.xml is complete");
                sw.WriteLine("-----------------------------------\n");

            }            
        }
    }
}
