using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

internal class Program
{
    private static string m_DirectoryPath;
    private static string m_PipelineCabFile;
    private static string m_ExtractDirName;

    private static List<FileSystemInfo> m_PathsInfoToRemove;
    static List<DirectoryInfo> m_UninstallDeleteDirList;
    private static int Main(string[] args)
    {



        string logPath = @"C:\Temp\custom_action_log.txt";
        m_UninstallDeleteDirList = new List<DirectoryInfo>();

        Directory.CreateDirectory(Path.GetDirectoryName(logPath));
        try
        {
            File.AppendAllText(logPath, $"[{DateTime.Now}] Args: {string.Join(", ", args)}\n");

            if (args.Length == 0)
            {
                File.AppendAllText(logPath, "No arguments provided.\n");
                return 1;


                //Console.WriteLine("Usage: MyCustomActionExe install| revoke| uninstall");
                //return 1;
            }

            switch (args[0].ToLower())
            {
                case "install":
                    RunInstall(logPath);
                    break;
                case "rollback":
                    RollbackInstall(logPath);
                    break;
                case "uninstall":
                    RunUninstall(logPath);
                    break;
                default:
                    File.AppendAllText(logPath, "Unknown command.\n");
                    return 1;


            }
            File.AppendAllText(logPath, "Completed successfully.\n");
            return 0;






            //    string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "custom_action_log.txt");

            //try
            //{
            //    File.AppendAllText(logPath, $"[{DateTime.Now}] Args: {string.Join(", ", args)}\n");
            //    Console.WriteLine("This is just for testing");
            //    Console.ReadLine(); 
            //    if (args.Length == 0)
            //    {
            //        File.AppendAllText(logPath, "No arguments provided.\n");
            //        return 1;
            //    }

            //    switch (args[0].ToLower())
            //    {
            //        case "install":
            //            File.AppendAllText(logPath, "Running install logic...\n");
            //            break;
            //        case "uninstall":
            //            File.AppendAllText(logPath, "Running uninstall logic...\n");
            //            break;
            //        case "rollback":
            //            File.AppendAllText(logPath, "Running rollback logic...\n");
            //            break;
            //        default:
            //            File.AppendAllText(logPath, "Unknown command.\n");
            //            return 1;
            //    }

            //    File.AppendAllText(logPath, "Completed successfully.\n");
            //    return 0;
            //}
            //catch (Exception ex)
            //{
            //    File.AppendAllText(logPath, $"Exception: {ex}\n");
            //    return 1;
            //}

        }
        catch (Exception ex)
        {
            // Log the error to a file or event log
            File.AppendAllText(logPath, $"Exception: {ex}\n");
            return 1; // failure


        }
    }



    private static void InitializeExtractionVariables()
    {
        m_DirectoryPath = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;
        m_PipelineCabFile = "Pipeline-lite.cab";
        m_ExtractDirName = "ExtractedFiles\\";

        m_PathsInfoToRemove = new List<FileSystemInfo>();
        m_PathsInfoToRemove.Add(new FileInfo(Path.Combine(m_DirectoryPath, "CABARC.EXE")));
        m_PathsInfoToRemove.Add(new FileInfo(Path.Combine(m_DirectoryPath, "CABINET.DLL")));
    }
    private static void ExtractFiles()
    {
        InitializeExtractionVariables();

        if (File.Exists(Path.Combine(m_DirectoryPath, m_PipelineCabFile))
            && File.Exists(Path.Combine(m_DirectoryPath, "CABARC.EXE"))
            && File.Exists(Path.Combine(m_DirectoryPath, "CABINET.DLL")))
        {
            //DirectoryInfo extractDirInfo = new DirectoryInfo ( Path.Combine ( m_DirectoryPath, m_ExtractDirName ) );
            //if (extractDirInfo.Exists) extractDirInfo.Delete ( true );
            // added on 24 July 2009 for direct extract
            string pipelineLitePath = Path.Combine(m_DirectoryPath, "Pipeline-lite");
            if (Directory.Exists(pipelineLitePath)) Directory.Delete(pipelineLitePath, true);

            //extractDirInfo.Create ();
            //m_PathsInfoToRemove.Add ( extractDirInfo );

            Process extractProcess = new Process();
            extractProcess.StartInfo.WorkingDirectory = m_DirectoryPath;
            extractProcess.StartInfo.FileName = Path.Combine(m_DirectoryPath, "CABARC.EXE");
            // pipeline-lite should be extracted directly instead of using extract directory
            //extractProcess.StartInfo.Arguments = "-p x " + m_PipelineCabFile + " " + m_ExtractDirName;
            //extractProcess.StartInfo.Arguments = "-p x " + m_PipelineCabFile + " " ;
            extractProcess.StartInfo.Arguments = "-p x " + m_PipelineCabFile;
            extractProcess.StartInfo.UseShellExecute = false;
            extractProcess.StartInfo.CreateNoWindow = true;
            extractProcess.Start();
            extractProcess.WaitForExit();

            // move extracted files and directories to desired location
            //DirectoryInfo pipelineExtractDirInfo = new DirectoryInfo ( Path.Combine ( m_DirectoryPath, "ExtractedFiles\\Pipeline-lite" ) );

            //System.Windows.Forms.MessageBox.Show ( pipelineExtractDirInfo.FullName );d
            string newDirectory = Path.Combine(m_DirectoryPath, "Pipeline-lite");
            m_PathsInfoToRemove.Add(new DirectoryInfo(newDirectory));
            //if (Directory.Exists ( newDirectory )) Directory.Delete ( newDirectory, true );
            //pipelineExtractDirInfo.MoveTo ( newDirectory );

            // delete temprorary files and directories
            m_PathsInfoToRemove.Add(new FileInfo(Path.Combine(m_DirectoryPath, m_PipelineCabFile)));

            //System.Windows.Forms.MessageBox.Show ( "Done" );
        }
        else

            //Console.WriteLine("Pipeline-lite could not be installed. Please install by yourself after installation is complete.");
            System.Windows.Forms.MessageBox.Show("Pipeline-lite could not be installed. Please install by yourself after installation is complete.", "Warning");
    }

    private static void ExecuteExtraction()
    {
        try
        {
            ExtractFiles();
            //DeleteTemporaryFiles ();
        }
        catch (System.Exception ex)
        {
            //Console.WriteLine("Error in installing Pipeline-lite. Please install it by yourself after installation of Obi is complete. Press OK to continue with installation.{0}", ex.ToString());
            MessageBox.Show("Error in installing Pipeline-lite. Please install it by yourself after installation of Obi is complete. Press OK to continue with installation." + "\n\n" + ex.ToString());
        }
    }


    public static void StartJREInstallOptionally(string logPath)
    {
        File.AppendAllText(logPath, "Check for Java Runtime. \n");
        if (!IsJREAlreadyInstalled())
        {
            File.AppendAllText(logPath, "Java Runtime is now installed. \n");
            try
            {
                File.AppendAllText(logPath, "Java Runtime try block. \n");
                System.Media.SystemSounds.Exclamation.Play(); // audio clue to alert.
                if (MessageBox.Show("Obi will need Java runtime environment (JRE version 7 for 32 bit) installed on this computer  for some operations. If it is not already installed on this computer, press yes to install it from Obi Downlaod and Installtion page." + "\n" + "Please note that JRE installation will take some time so it may continue even after installation of Obi is finished", "JRE installation?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    //Process JREInstallationProcess = new Process();
                    //JREInstallationProcess.StartInfo.FileName = "http://java.com/en/download/windows_ie.jsp";

                    //JREInstallationProcess.Start();
                    //JREInstallationProcess.StartInfo.FileName = "https://daisy.org/activities/software/obi/download-and-installation/";

                    //Process.Start(new ProcessStartInfo("cmd", $"/c start {JREInstallationProcess.StartInfo.FileName}") { CreateNoWindow = true });

                    string JREDownloadLink = "https://daisy.org/activities/software/obi/download-and-installation";

                    File.AppendAllText(logPath, "Java Runtime can be downloaded from Obi website which should be opened now. \n");

                    //Process.Start(new ProcessStartInfo("cmd", $"/c start {JREDownloadLink}"));

                    Process.Start("explorer.exe", JREDownloadLink);
                    //Process.Start(new ProcessStartInfo("cmd", $"/c start {JREDownloadLink}"));
                }
            }
            catch (System.Exception ex)
            {
                File.AppendAllText(logPath, "Error in downloading Java runtime environment. Please install it yourself after installation of Obi \n");
                //Console.WriteLine("Error in downloading Java runtime environment. Please install it yourself after installation of Obi is complete {0}", ex.ToString());
                MessageBox.Show("Error in downloading Java runtime environment. Please install it yourself after installation of Obi is complete" + "\n\n" + ex.ToString());
            }

        }
    }


    private static bool IsJREAlreadyInstalled()
    {
        try
        {
            RegistryKey JREKey = Registry.CurrentUser.OpenSubKey("Software\\JavaSoft\\Java Runtime Environment");

            if (JREKey == null
                || (JREKey != null && JREKey.GetValueNames().Length == 0))
            {
                JREKey = Registry.LocalMachine.OpenSubKey("Software\\JavaSoft\\Java Runtime Environment");
            }

            if (JREKey != null)
            {//1
                foreach (string subKeyString in JREKey.GetSubKeyNames())
                {//2
                    if (subKeyString.Length > 3)
                    {//3
                        string keyName = subKeyString.Substring(0, 3);

                        double numericSubKey = Convert.ToDouble(keyName);
                        if (numericSubKey >= 1.6)
                        {//4
                            return true;
                        }//-4
                    }//-3
                }//-2
            }//-1
        } // try ends
        catch (System.Exception)
        { return false; }

        return false;
    }


    private static void RemoveCustomInstalledFiles()
    {
        try
        {
            Console.WriteLine("Pipeline lite will be removed shortly");
            m_DirectoryPath = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;
            m_UninstallDeleteDirList.Add(new DirectoryInfo(Path.Combine(m_DirectoryPath, "ExtractedFiles")));
            m_UninstallDeleteDirList.Add(new DirectoryInfo(Path.Combine(m_DirectoryPath, "Pipeline-lite")));

            for (int i = 0; i < m_UninstallDeleteDirList.Count; i++)
            {

                if (m_UninstallDeleteDirList[i].Exists) m_UninstallDeleteDirList[i].Delete(true);
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("Some files could not be removed. Please remove them manually {0}", ex);
            // MessageBox.Show("Some files could not be removed. Please remove them manually", "Warning");
        }
    }




      



    static void RunInstall(string logPath)
    {
        // Paste your install logic here
        //  Console.WriteLine("Running install logic...");
        File.AppendAllText(logPath, "Running install logic...\n");

        ExecuteExtraction();
        StartJREInstallOptionally(logPath);



    }

    static void RunUninstall(string logPath)
    {
        // Paste your uninstall logic here
        // Console.WriteLine("Running uninstall logic...");

        File.AppendAllText(logPath, "Running uninstall logic...\n");

        RemoveCustomInstalledFiles();


    }

    static void RollbackInstall(string logPath)
    {
        // Paste your uninstall logic here
        // Console.WriteLine("Roll Back logic...");

        File.AppendAllText(logPath, "Running rollback logic...\n");

        RemoveCustomInstalledFiles();

    }



}