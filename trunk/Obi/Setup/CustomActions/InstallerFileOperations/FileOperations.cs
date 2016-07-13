using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace InstallerFileOperations
    {
    [RunInstaller ( true )]
    public partial class FileOperations : Installer
        {
        private string m_DirectoryPath;
        private string m_PipelineCabFile;
        private  string m_ExtractDirName;

        private List<FileSystemInfo> m_PathsInfoToRemove;
        List<DirectoryInfo> m_UninstallDeleteDirList;

        public FileOperations ()
            {
            InitializeComponent ();

            m_UninstallDeleteDirList = new List<DirectoryInfo> ();
                                    }

        private void InitializeExtractionVariables ()
            {
            m_DirectoryPath = Directory.GetParent ( System.Reflection.Assembly.GetExecutingAssembly ().Location ).FullName;
            m_PipelineCabFile = "Pipeline-lite.cab";
            m_ExtractDirName = "ExtractedFiles\\";

            m_PathsInfoToRemove = new List<FileSystemInfo> ();
            m_PathsInfoToRemove.Add ( new FileInfo ( Path.Combine ( m_DirectoryPath, "CABARC.EXE" ) ) );
            m_PathsInfoToRemove.Add ( new FileInfo ( Path.Combine ( m_DirectoryPath, "CABINET.DLL" ) ) );
                        }

        private void ExtractFiles ()
            {
            InitializeExtractionVariables ();

            if (File.Exists ( Path.Combine ( m_DirectoryPath, m_PipelineCabFile ) )
                && File.Exists ( Path.Combine ( m_DirectoryPath, "CABARC.EXE" ) )
                && File.Exists ( Path.Combine ( m_DirectoryPath, "CABINET.DLL" ) ))
                {
                //DirectoryInfo extractDirInfo = new DirectoryInfo ( Path.Combine ( m_DirectoryPath, m_ExtractDirName ) );
                //if (extractDirInfo.Exists) extractDirInfo.Delete ( true );
                // added on 24 July 2009 for direct extract
                string pipelineLitePath = Path.Combine ( m_DirectoryPath, "Pipeline-lite") ;
                if ( Directory.Exists ( pipelineLitePath)) Directory.Delete ( pipelineLitePath, true ) ;
                
                //extractDirInfo.Create ();
                //m_PathsInfoToRemove.Add ( extractDirInfo );
                
                Process extractProcess = new Process ();
                extractProcess.StartInfo.WorkingDirectory = m_DirectoryPath;
                extractProcess.StartInfo.FileName = Path.Combine ( m_DirectoryPath, "CABARC.EXE" );
                // pipeline-lite should be extracted directly instead of using extract directory
                //extractProcess.StartInfo.Arguments = "-p x " + m_PipelineCabFile + " " + m_ExtractDirName;
                //extractProcess.StartInfo.Arguments = "-p x " + m_PipelineCabFile + " " ;
                extractProcess.StartInfo.Arguments = "-p x " + m_PipelineCabFile ;
                extractProcess.StartInfo.UseShellExecute = false;
                extractProcess.StartInfo.CreateNoWindow = true;
                                extractProcess.Start ();
                extractProcess.WaitForExit ();

                // move extracted files and directories to desired location
                //DirectoryInfo pipelineExtractDirInfo = new DirectoryInfo ( Path.Combine ( m_DirectoryPath, "ExtractedFiles\\Pipeline-lite" ) );
                
                //System.Windows.Forms.MessageBox.Show ( pipelineExtractDirInfo.FullName );d
                string newDirectory = Path.Combine ( m_DirectoryPath, "Pipeline-lite" );
                m_PathsInfoToRemove.Add ( new DirectoryInfo ( newDirectory ) ) ;
                                //if (Directory.Exists ( newDirectory )) Directory.Delete ( newDirectory, true );
                //pipelineExtractDirInfo.MoveTo ( newDirectory );
                
                // delete temprorary files and directories
                                m_PathsInfoToRemove.Add ( new FileInfo ( Path.Combine ( m_DirectoryPath, m_PipelineCabFile ) ) );
                
                //System.Windows.Forms.MessageBox.Show ( "Done" );
                }
            else
                System.Windows.Forms.MessageBox.Show ( "Pipeline-lite could not be installed. Please install by yourself after installation is complete." , "Warning");
            }

        public void DeleteTemporaryFiles ()
            {
            for (int i = 0; i < m_PathsInfoToRemove.Count; i++)
                {
                if (m_PathsInfoToRemove[i].Exists)
                    {
                    if (m_PathsInfoToRemove[i] is DirectoryInfo) 
                                                ((DirectoryInfo)m_PathsInfoToRemove[i]).Delete ( true );
                    else 
                        m_PathsInfoToRemove[i].Delete ();
                    }
                }
            }


        private void ExecuteExtraction ()
            {
            try
                {
                ExtractFiles ();
                //DeleteTemporaryFiles ();
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( "Error in installing Pipeline-lite. Please install it by yourself after installation of Obi is complete. Press OK to continue with installation." + "\n\n" +  ex.ToString () );
                }
                       
            }

        public void StartJREInstallOptionally ()
            {
            if ( !IsJREAlreadyInstalled () )
                {
            try
                {
                System.Media.SystemSounds.Exclamation.Play (); // audio clue to alert.
                if (MessageBox.Show ( "Obi will need Java runtime environment (JRE version 7 for 32 bit) installed on this computer  for some operations. If it is not already installed on this computer, press yes to install it from internet." + "\n" + "Please note that JRE installation will take some time so it may continue even after installation of Obi is finished", "JRE installation?", MessageBoxButtons.YesNo , MessageBoxIcon.Question) 
                    == DialogResult.Yes)
                    {
                    Process JREInstallationProcess = new Process ();
                    JREInstallationProcess.StartInfo.FileName = "http://java.com/en/download/windows_ie.jsp";
                    JREInstallationProcess.Start ();
                    }
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( "Error in downloading Java runtime environment. Please install it yourself after installation of Obi is complete" + "\n\n" + ex.ToString ());
                }

            }
            }

        private bool IsJREAlreadyInstalled ()
            {
            try
                {
                RegistryKey JREKey = Registry.CurrentUser.OpenSubKey ( "Software\\JavaSoft\\Java Runtime Environment" );

                if (JREKey == null
                    || (JREKey != null && JREKey.GetValueNames ().Length == 0))
                    {
                    JREKey = Registry.LocalMachine.OpenSubKey ( "Software\\JavaSoft\\Java Runtime Environment" );
                    }

                if (JREKey != null)
                    {//1
                    foreach (string subKeyString in JREKey.GetSubKeyNames ())
                        {//2
                        if (subKeyString.Length > 3)
                            {//3
                            string keyName = subKeyString.Substring ( 0, 3 );

                            double numericSubKey = Convert.ToDouble ( keyName );
                            if (numericSubKey >= 1.6)
                                {//4
                                return true;
                                }//-4
                            }//-3
                        }//-2
                    }//-1
                } // try ends
            catch ( System.Exception )
            { return false; }

            return false;
            }
        private void RemoveCustomInstalledFiles ()
            {
                try
                    {
                    m_DirectoryPath = Directory.GetParent ( System.Reflection.Assembly.GetExecutingAssembly ().Location ).FullName;
                    m_UninstallDeleteDirList.Add ( new DirectoryInfo ( Path.Combine ( m_DirectoryPath, "ExtractedFiles" ) ) );
                    m_UninstallDeleteDirList.Add ( new DirectoryInfo ( Path.Combine ( m_DirectoryPath, "Pipeline-lite" ) ) );

                    for (int i = 0; i < m_UninstallDeleteDirList.Count; i++)
                        {
                        
                        if (m_UninstallDeleteDirList[i].Exists) m_UninstallDeleteDirList[i].Delete ( true );
                        }
                    }
                 catch (System.Exception )
                    {
                    MessageBox.Show ( "Some files could not be removed. Please remove them manually", "Warning" );
                    }
                                                                        }


        private void FileOperations_Committed ( object sender, InstallEventArgs e )
            {
                        }

        private void FileOperations_AfterInstall ( object sender, InstallEventArgs e )
            {
            ExecuteExtraction ();
            StartJREInstallOptionally ();
            }

        private void FileOperations_BeforeUninstall ( object sender, InstallEventArgs e )
            {
            RemoveCustomInstalledFiles ();
            }

        private void FileOperations_BeforeRollback ( object sender, InstallEventArgs e )
            {
            RemoveCustomInstalledFiles ();
            }

        private void FileOperations_AfterUninstall ( object sender, InstallEventArgs e )
            {

            }



        }
    }