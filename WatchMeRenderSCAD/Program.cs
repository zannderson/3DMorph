using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatchMeRenderSCAD
{
    class Program
    {
        private static List<FileSystemEventArgs> _handleThese;
        static void Main(string[] args)
        {
            //_handleThese = new List<FileSystemEventArgs>();
            //FileSystemWatcher watcher = new FileSystemWatcher();
            //watcher.Path = "../../../Leonardo/bin/Debug";
            //watcher.NotifyFilter = NotifyFilters.LastWrite;
            //watcher.Filter = "*.scad";
            //watcher.Changed += Watcher_Changed;
            //watcher.EnableRaisingEvents = true;
            //watcher.
            

            while(true)
            {
                List<Process> psis = new List<Process>();
                List<string> moveThese = new List<string>();
                foreach(var scadFile in Directory.EnumerateFiles("../../../Leonardo/bin/Debug", "*.scad"))
                {
                    moveThese.Add(scadFile);
                    Console.Out.WriteLine(scadFile);
                    var psi = Process.Start("openscad", string.Format("-o {0}.png --camera=0,0,0,77,0,0,750 --imgsize=500,500 {0}", scadFile));
                    var psi1 = Process.Start("openscad", string.Format("-o {0}-1.png --camera=0,0,0,77,0,90,750 --imgsize=500,500 {0}", scadFile));
                    var psi2 = Process.Start("openscad", string.Format("-o {0}-2.png --camera=0,0,0,77,0,180,750 --imgsize=500,500 {0}", scadFile));
                    var psi3 = Process.Start("openscad", string.Format("-o {0}-3.png --camera=0,0,0,77,0,270,750 --imgsize=500,500 {0}", scadFile));
                    psis.Add(psi);
                    psis.Add(psi1);
                    psis.Add(psi2);
                    psis.Add(psi3);
                }
                foreach (var psi in psis)
                {
                    psi.WaitForExit();
                }
                foreach (string scadFile in moveThese)
                {
                    int bsIndex = scadFile.IndexOf('\\');
                    File.Move(scadFile, scadFile.Substring(0, bsIndex) + "/Rendered/" + scadFile.Substring(bsIndex, scadFile.Length - bsIndex));
                }

                Thread.Sleep(1000);
            }
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
        }
    }
}
