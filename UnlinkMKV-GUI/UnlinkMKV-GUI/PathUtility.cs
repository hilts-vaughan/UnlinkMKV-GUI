using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnlinkMKV_GUI
{
    public static class PathUtility
    {
        // Thanks!
        // http://csharptest.net/526/how-to-search-the-environments-path-for-an-exe-or-dll/

        /// <summary>
        /// Expands environment variables and, if unqualified, locates the exe in the working directory
        /// or the evironment's path.
        /// </summary>
        /// <param name="exe">The name of the executable file</param>
        /// <returns>The fully-qualified path to the file</returns>
        /// <exception cref="System.IO.FileNotFoundException">Raised when the exe was not found</exception>
        public static string FindExePath(string exe)
        {

            // If we're not on Windows, remove ".EXE"
            int p = (int)Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128))
            {
                // Running on Unix, strip EXE
                exe = exe.Replace(".exe", "");
            }
      

            exe = Environment.ExpandEnvironmentVariables(exe);
            if (!File.Exists(exe))
            {
                if (Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
                    {
                        string path = test.Trim();
                        path = Path.Combine(path, exe);
                        if (!String.IsNullOrEmpty(path) && File.Exists(path))
                            return Path.GetFullPath(path);
                    }
                }
                throw new FileNotFoundException(new FileNotFoundException().Message, exe);
            }
            return Path.GetFullPath(exe);
        }

    }
}
