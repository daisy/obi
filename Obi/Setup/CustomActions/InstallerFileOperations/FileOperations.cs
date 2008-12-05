using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Diagnostics;

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
                DirectoryInfo extractDirInfo = new DirectoryInfo ( Path.Combine ( m_DirectoryPath, m_ExtractDirName ) );
                if (extractDirInfo.Exists) extractDirInfo.Delete ( true );
                extractDirInfo.Create ();
                m_PathsInfoToRemove.Add ( extractDirInfo );
                
                Process extractProcess = new Process ();
                extractProcess.StartInfo.WorkingDirectory = m_DirectoryPath;
                extractProcess.StartInfo.FileName = Path.Combine ( m_DirectoryPath, "CABARC.EXE" );
                extractProcess.StartInfo.Arguments = "-p x " + m_PipelineCabFile + " " + m_ExtractDirName;
                extractProcess.StartInfo.UseShellExecute = false;
                extractProcess.StartInfo.CreateNoWindow = true;
                                extractProcess.Start ();
                extractProcess.WaitForExit ();

                // move extracted files and directories to desired location
                DirectoryInfo pipelineExtractDirInfo = new DirectoryInfo ( Path.Combine ( m_DirectoryPath, "ExtractedFiles\\Pipeline-lite" ) );
                
                //System.Windows.Forms.MessageBox.Show ( pipelineExtractDirInfo.FullName );d
                string newDirectory = Path.Combine ( m_DirectoryPath, "Pipeline-lite" );
                                if (Directory.Exists ( newDirectory )) Directory.Delete ( newDirectory, true );
                pipelineExtractDirInfo.MoveTo ( newDirectory );
                
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
                System.Windows.Forms.MessageBox.Show ( ex.ToString () );
                }
                       
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
                catch (System.Exception ex)
                    {
                    System.Windows.Forms.MessageBox.Show ( "Some files could not be removed. Please remove them manually", "Warning" );
                    }
                                                                        }


        private void FileOperations_Committed ( object sender, InstallEventArgs e )
            {
                        }

        private void FileOperations_AfterInstall ( object sender, InstallEventArgs e )
            {
            ExecuteExtraction ();
            }

        private void FileOperations_BeforeUninstall ( object sender, InstallEventArgs e )
            {
            RemoveCustomInstalledFiles ();
            }

        private void FileOperations_BeforeRollback ( object sender, InstallEventArgs e )
            {
            RemoveCustomInstalledFiles ();
            }



        }
    }