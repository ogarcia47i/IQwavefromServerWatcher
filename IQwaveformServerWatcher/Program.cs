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
        //static FileSystemWatcher _watcher;
        static string directory = @"\\cifs.casinas03.lvn.broadcom.net\cifs\wsd_iqdata\iqwavedata\Released\WCDMA";
        //static string directory = @"C:\Avago.ATF.2.2.6\Data\TestPlans_SupportFiles\FlexTest";
        static string iqFolderStructureFilename = "ByFolderStructure.xml";
        /// <summary>
        /// Init.
        /// </summary>
        static void Init()
        {
            Console.WriteLine("INIT");
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
                //Program._watcher.Created += new FileSystemEventHandler(Program.fileSystemWatcher1_Created);
                //Program._watcher.Renamed += new RenamedEventHandler(Program.fileSystemWatcher1_Renamed);
                //Program._watcher.Deleted += new FileSystemEventHandler(Program.fileSystemWatcher1_Deleted);
                _watcher.EnableRaisingEvents = true;

                // Wait for the user to quit the program.
                Console.WriteLine("Press 'q' to quit the sample.");
                while (Console.Read() != 'q') ;
            }
        }

        /// <summary>
        /// Handler.
        /// </summary>
        static void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            //Console.WriteLine(string.Format("Changed: {0} {1}", e.FullPath, e.ChangeType));

            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                UpdateXml();
                Console.WriteLine(string.Format("Changed: {0} {1}", e.FullPath, e.ChangeType));
                Console.WriteLine("Update ByFolderStructure.xml is complete");

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

    }
}
