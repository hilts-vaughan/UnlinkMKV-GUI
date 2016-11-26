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
	        "/mnt/media/AnimeToBeProcessed/[Coalgirls]_Serial_Experiments_Lain_(1520x1080_Blu-Ray_FLAC)/[Coalgirls]_Serial_Experiments_Lain_01_(1520x1080_Blu-Ray_FLAC)_[573CDDD6].mkv";
	    var x = new MergeJob(new MergeOptions(), myDisk);

	    x.PerformMerge();

	    return;

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FormApplication());
	}
}
