namespace cSharpAppWithinApp;


using System;
using System.Windows.Forms;

public class Program
{
    public static Form1 form = new Form1();
    [STAThread]
    static void Main(string[] args)
    {
        form.FormLayout();;
        Application.Run(form);
    }
}


/*
static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {


        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }    
}
*/