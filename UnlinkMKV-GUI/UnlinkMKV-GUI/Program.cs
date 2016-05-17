using System;
using System.Windows.Forms;
using UnlinkMKV_GUI;

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
