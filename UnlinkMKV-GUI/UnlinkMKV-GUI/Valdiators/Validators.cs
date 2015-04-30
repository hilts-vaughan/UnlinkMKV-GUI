using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnlinkMKV_GUI.Valdiators
{
    public class MkvToolNixValidatorTask : IValidatorTask
    {
        public string GetStatusText()
        {
            return "Detecting MkvToolNix in path and FFMPEG...";
        }

        public bool IsRequirementMet()
        {

            // Check for the path's to the tools we required
            try
            {

                PathUtility.FindExePath("mkvextract.exe");
                PathUtility.FindExePath("mkvinfo.exe");
                PathUtility.FindExePath("mkvmerge.exe");
            }
            catch (FileNotFoundException exception)
            {
                return false;
            }

            return true;
        }

        public bool AttemptFixRequirement()
        {
            int p = (int)Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128))
            {
                return false;
            }


            // Attempt to find them in the path and adjust the path silently, so there's no errors    
        

            // It looks like it was installed on C:
            foreach (var drive in Directory.GetLogicalDrives())
            {
                var hasNixInstalled = string.Format(@"{0}Program Files (x86)\MKVToolNix\", drive);

                if (Directory.Exists(hasNixInstalled) && File.Exists(Path.Combine(hasNixInstalled, "mkvinfo.exe")))
                {
                    var modPath = Environment.GetEnvironmentVariable("PATH") + ";" + hasNixInstalled;

                    // OK, looks like it was installed in the default directory
                    Environment.SetEnvironmentVariable("PATH",
                        modPath);

                    PathUtility.ExceptionalPath = modPath;

                    return true;
                }


                var hasNixInstalled32 = string.Format(@"{0}Program Files\MKVToolNix\", drive);


                if (Directory.Exists(hasNixInstalled32) && File.Exists(Path.Combine(hasNixInstalled32, "mkvinfo.exe")))
                {
                    var modPath = Environment.GetEnvironmentVariable("PATH") + ";" + hasNixInstalled32;
                    // OK, looks like it was installed in the default directory
                    Environment.SetEnvironmentVariable("PATH",
                        modPath);

                    PathUtility.ExceptionalPath = modPath;

                    return true;
                }
            }

            MessageBox.Show(
                "MKVToolNix and/or FFMPEG are missing from your system path. Please refer to the manual provided to install them for your platform.");
            return false;
        }
    }

    public class IsAdministratorValidatorTask : IValidatorTask
    {
        public string GetStatusText()
        {
            return "Checking for administrator rights...";
        }

        public bool IsRequirementMet()
        {

            // If we're not on Windows, remove ".EXE"
            int p = (int)Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128))
            {
                return true;
            }


            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
              .IsInRole(WindowsBuiltInRole.Administrator);
        }

        public bool AttemptFixRequirement()
        {
            MessageBox.Show("This tool requires administrator rights. Please rerun as an administrator.");
            return false;
        }
    }

    public class PerlExistsValidatorTask : IValidatorTask
    {
        public string GetStatusText()
        {
            return "Checking for Strawberry Perl...";
        }

        public bool IsRequirementMet()
        {
            // Check for the path's to the tools we required
            try
            {


                PathUtility.FindExePath("perl.exe");
            }
            catch (FileNotFoundException exception)
            {
                return false;
            }

            return true;
        }

        public bool AttemptFixRequirement()
        {
            MessageBox.Show(
                "Strawberry Perl was not found. Please include a copy inside your working directory with installed modules. You should have got a copy with this release.");
            return false;
        }
    }


}
