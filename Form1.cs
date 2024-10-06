// THIS IS NEEDED BECAUSE WE NEED THE HOST APPLICATION TO WAIT A BIT WHIST THE EXTERNAL APPLICATION IS LAUNCHED (System.Threading.Thread.Sleep(100);)
using System.Threading;

// THIS IS NEEDED TO RUN A PROCESS .. e.g new Process();
using System.Diagnostics; 

// This IS NEEDED FOR ALL THE 'DllImport' BULLSHIT, which you'll see below.
using System.Runtime.InteropServices;



namespace cSharpAppWithinApp;



// YOU PROBABLY HAVE A BETTER WAY TO CREATE A FORM IN RIDER, BUT I'M DOING THIS FROM VSCODE.
public  class Form1 : Form
{
    
    // LOTS OF BULLSHIT HERE TO IMPORT FUNCTIONS FROM THE OLDER (CLASSIC) WIN32 API...>>>>>
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
    // << END OF THE WIN32 API BULLSHIT


    public void FormLayout()
    {

        // BUILD A BASIC FORM
        this.Name = "SiMainForm";
        this.Text = "Si Main Form";
        this.Size = new System.Drawing.Size(500, 500);
        this.StartPosition = FormStartPosition.CenterScreen;

        // HOOKUP AN EVENT HANDLER FOR WHEN STUFF IS RESIZED
        this.Resize += new EventHandler(Form_Resize);

        // CREATE A NEW PROCESS TO LAUNCH AN APPLICATION INSIDE THIS FORM (Change 'Notepad' to be the name of application you need to launch e.g: C://ProgramFiles/SomeOtherShite/Bullshit.exe')
        Process program_to_embed = new Process();
        ProcessStartInfo psi = new ProcessStartInfo("Notepad");
        psi.WindowStyle = ProcessWindowStyle.Maximized;
        psi.CreateNoWindow = true;
        program_to_embed.StartInfo = psi;

        // START THE PROCESS AND WAIT FOR IT TO LAUNCH
        program_to_embed.Start();
        while (string.IsNullOrEmpty(program_to_embed.MainWindowTitle))
        {
            System.Threading.Thread.Sleep(100);
            program_to_embed.Refresh();
        }


        // GRAB A 'Handle' TO THE NEW APPLICATION (YOU'LL NEED THIS LATER TO HANDLE RESIZE).
        contained_application_handle = program_to_embed.MainWindowHandle;

        // YOU MIGHT NEED TO DESTROY THIS 'old' HANDLE LATER - I HAVEN'T THOUGHT MUCH ABOUT IT
        IntPtr old = SetParent(contained_application_handle, this.Handle);

        // SETUP THE INNER APPLICATION SO THAT IT IS MAXIMIZED WITHIN YOUR FORM.
        SetWindowLong(contained_application_handle, GWL_STYLE,WS_VISIBLE + WS_MAXIMIZE);

        // MOVE IT AND RESIZE IT, MAKING SURE ITS A GOOD FIT WITHIN YOUR FORM
        MoveWindow(contained_application_handle, 0,0, this.ClientSize.Width, this.ClientSize.Height, true);

        // MAKE THE APPLICATION YOU'VE LAUNCHED ACTIVE FOR THE USER
        SetActiveWindow(contained_application_handle);
        SwitchToThisWindow(contained_application_handle, true);  
    }


    // HANDLE RESIZING THE FORM, SO THE INNER APPLICATION FITS NICELY
    public void Form_Resize(object? sender, EventArgs e) {
        MoveWindow(contained_application_handle, 0,0, this.ClientSize.Width, this.ClientSize.Height, true);
    }

    // PROBABLY MORE BULLSHIT TO DO - LIKE HANDLING ACTIVATING AND DEACTIVATING THE APP, MINIMIZE, MAXIMIZE ETC
}
