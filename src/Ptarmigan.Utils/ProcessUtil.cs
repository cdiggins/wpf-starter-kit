using System;
using System.Diagnostics;
using System.IO;

namespace Ptarmigan.Utils
{

    // https://stackoverflow.com/questions/5762526/how-can-i-make-something-that-catches-all-unhandled-exceptions-in-a-winforms-a

    public static class ProcessUtil
    {
        public static Process Current
            => Process.GetCurrentProcess();

        public static void SetExitCallback(Action<object, EventArgs> handler)
            => Current.SetExitCallback(handler);

        public static void SetExitCallback(this Process p, Action<object, EventArgs> handler)
        {
            p.EnableRaisingEvents = true;
            p.Exited += (sender, args) => handler(sender, args);
            //AppDomain.CurrentDomain.ProcessExit += (sender, args) => handler(sender, args);
        }

        public static ProcessData ToProcessData(this Process process)
            => new ProcessData(process);

        public static Process OpenFolderInExplorer(string folderPath)
            => Process.Start("explorer.exe", folderPath);

        public static Process SelectFileInExplorer(string filePath)
            => Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"/select,\"{filePath}\"",
                UseShellExecute = false
            });

        public static Process ShellExecute(string filePath)
            => Process.Start(new ProcessStartInfo { FileName = filePath, UseShellExecute = true });

        public static Process OpenFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("", filePath);

            // Expand the file name
            filePath = new FileInfo(filePath).FullName;

            // Open the file with the default file extension handler.
            try
            {
                return Process.Start(filePath);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }

            // If there is no default file extension handler, use shell execute
            try
            {
                return ShellExecute(filePath);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }

            // If that didn't work, show the file in explorer.
            return SelectFileInExplorer(filePath);
        }

        /// <summary>
        /// Closes a process if it isin't null and hasn't already exited.
        /// </summary>
        /// <param name="process"></param>
        public static void SafeClose(this Process process)
        {
            if (process != null && !process.HasExited)
                process.CloseMainWindow();
        }


        public static string ReadOneLine(this ProcessStartInfo psi)
        {
            psi.RedirectStandardOutput = true;
            using (var p = new Process { StartInfo = psi })
            {
                p.Start();
                return p.StandardOutput.ReadLine() ?? "";
            }
        }

    }
}
