using System.Windows;
using System.Threading;
using System;
using System.Reflection;
using System.IO;
using System.Security.Principal;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ZingitWizard
{
    public partial class ZingitWelcomeWindow : Window
    {
        [DllImport("user32")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwnd2, int x, int y, int cx, int cy, int flags); public ZingitWelcomeWindow()
        {
            InitializeComponent();

            if (IsRunAsAdmin() == false)
            {
                //MessageBox.Show("Please run this application in Administrator mode.");
                //Close();

                //var processInfo = new Process( //new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                //// The following properties run the new process as administrator
                //processInfo.UseShellExecute = false;
                //processInfo.Verb = "runas";

                Process proc = new Process();
                proc.EnableRaisingEvents = true;
                proc.StartInfo.UseShellExecute = true;
                //proc.StartInfo.RedirectStandardOutput = StandardOutput;
                proc.StartInfo.FileName = Assembly.GetExecutingAssembly().CodeBase;
                //proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                proc.StartInfo.Verb = "runas";

                try
                {
                    //proc.StartInfo.Arguments = Arguments;
                    proc.Start();
                    //Added code
                    Thread.Sleep(500);
                    SetWindowPos(proc.MainWindowHandle, new IntPtr(-1), 0, 0, 0, 0, 0x1 | 0x2);
                    //SetForegroundWindow(proc.MainWindowHandle);
                    //........................................
                    //if (StandardOutput == true)
                    //{
                    //    string output = proc.StandardOutput.ReadToEnd();
                    //    DumpOutput(WorkingDirectory + "\\" + OutputFileName, output);
                    //}
                    //proc.WaitForExit();
                    //proc.Close();
                    // Start the new process
                    //try
                    //{
                    //    Process.Start(processInfo);
                    //    SetForegroundWindow(processInfo.ma)
                }
                catch (Exception)
                {
                    // The user did not allow the application to run as administrator
                    MessageBox.Show("Sorry, this application must be run as Administrator.");
                }

                // Shut down the current process
                Application.Current.Shutdown();
            }

            ResolveAssembly();

            Loaded += MainWindowLoaded;
        }

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindowLoaded;
            ZingitWizardDialog dlg = new ZingitWizardDialog();

            dlg.Dispatcher.BeginInvoke((SendOrPostCallback)delegate
            {
                Thread.Sleep(3000);
                Close();
                dlg.ShowDialog();
            }, new object[] { null });
        }

        private Assembly ResolveAssembly()
        {
            Assembly schedulerDll = null;
            string dllPath = string.Empty;
            try
            {
                dllPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + "Microsoft.Win32.TaskScheduler.dll";
                schedulerDll = Assembly.LoadFile(dllPath);
            }
            catch(FileNotFoundException /*fnfEx*/)
            {
                MessageBox.Show("Dll file " + dllPath + " not found.\nCopy this Dll in executable directory and run again.");
                Close();
            }

            return schedulerDll;
        }

        private bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}