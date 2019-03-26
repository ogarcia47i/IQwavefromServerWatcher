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
            while (true)
            {
                Console.WriteLine("TYPE SOMETHING");
                string line = Console.ReadLine();
                Console.WriteLine("TYPED: " + line);
            }
        }

        /// <summary>
        /// Watcher.
        /// </summary>
        static FileSystemWatcher _watcher;
        static string directory = @"\\cifs.casinas03.lvn.broadcom.net\cifs\wsd_iqdata\iqwavedata\Released";
        static string iqFolderStructureFilename = "ByFolderStructure.xml";
        /// <summary>
        /// Init.
        /// </summary>
        static void Init()
        {
            Console.WriteLine("INIT");
            Program._watcher = new FileSystemWatcher(directory);
            Program._watcher.Changed +=
                new FileSystemEventHandler(Program._watcher_Changed);
            Program._watcher.EnableRaisingEvents = true;
            Program._watcher.IncludeSubdirectories = true;
        }

        /// <summary>
        /// Handler.
        /// </summary>
        static void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("CHANGED, NAME: " + e.Name);
            Console.WriteLine("CHANGED, FULLPATH: " + e.FullPath);
            string remoteXmlStructureFile = Path.Combine(directory, iqFolderStructureFilename);

            var iqXml = new IqXmlToTreeView(remoteXmlStructureFile);

            iqXml.UpdateXml(directory);

            //CopyIqStructureFileToLocal();
   

            Console.WriteLine("Update ByFolderStructure.xml is complete");

            // Can change program state (set invalid state) in this method.
            // ... Better to use insensitive compares for file names.
        }
    }
}
