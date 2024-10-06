// THIS IS NEEDED BECAUSE WE NEED THE HOST APPLICATION TO WAIT A BIT WHIST THE EXTERNAL APPLICATION IS LAUNCHED (System.Threading.Thread.Sleep(100);)
using System.Threading;

// THIS IS NEEDED TO RUN A PROCESS .. e.g new Process();
using System.Diagnostics; 

//using System.Runtime.InteropServices;



namespace cSharpAppWithinApp;


public  class Form1 : Form
{
    
    [DllImport("user32.dll")]
    static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

    [DllImport("user32.dll", SetLastError = true)]
    static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

    [DllImport("user32.dll")]
    static extern IntPtr SetActiveWindow(IntPtr hWnd);

    private const int GWL_STYLE = -16;

    private const int WS_VISIBLE = 0x10000000;
    private const int WS_MAXIMIZE = 0x01000000;


    private IntPtr contained_application_handle;


    public void FormLayout()
    {

        this.Name = "SiMainForm";
        this.Text = "Si Main Form";
        this.Size = new System.Drawing.Size(500, 500);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Resize += new EventHandler(Form_Resize);

        Process program_to_embed = new Process();
        ProcessStartInfo psi = new ProcessStartInfo("Notepad");
        psi.WindowStyle = ProcessWindowStyle.Maximized;
        psi.CreateNoWindow = true;
        program_to_embed.StartInfo = psi;

        program_to_embed.Start();

        // There are better ways to wait, more 
        while (string.IsNullOrEmpty(program_to_embed.MainWindowTitle))
        {
            System.Threading.Thread.Sleep(100);
            program_to_embed.Refresh();
        }



        contained_application_handle = program_to_embed.MainWindowHandle;
        IntPtr old = SetParent(contained_application_handle, this.Handle);

        SetWindowLong(contained_application_handle, GWL_STYLE,WS_VISIBLE + WS_MAXIMIZE);
        MoveWindow(contained_application_handle, 0,0, this.ClientSize.Width, this.ClientSize.Height, true);

        SetActiveWindow(contained_application_handle);
        SwitchToThisWindow(contained_application_handle, true);  
    }

    public void Form_Resize(object? sender, EventArgs e) {
        MoveWindow(contained_application_handle, 0,0, this.ClientSize.Width, this.ClientSize.Height, true);
    }
}
