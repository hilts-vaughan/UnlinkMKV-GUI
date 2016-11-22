using System;
using System.IO;
using System.Windows.Forms;
using UnlinkMKV_GUI;
using UnlinkMKV_GUI.merge;

public class Program
{
    [STAThread]
	public static void Main(String[] args)
	{
	    // TODO: Instead of running a WinForms application here... we can choose to just run some CLI against a hardcoded
	    // path if we must do so

	    var myDisk =
	        "/mnt/media/Downloads/complete/[Coalgirls]_Nisemonogatari_(1920x1080_Blu-ray_FLAC)/[Coalgirls]_Nisemonogatari_02_(1920x1080_Blu-ray_FLAC)_[F338F8D9].mkv";
	    var x = new MergeJob(new MergeOptions(), myDisk);

	    x.PerformMerge();

	    return;

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FormApplication());
	}
}
