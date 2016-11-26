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
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FormApplication());
	}
}
