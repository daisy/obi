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
        private readonly string m_PipelineCabFile;
        private readonly string m_ExtractDirName;

        private List<FileSystemInfo> m_PathsInfoToRemove;

        public FileOperations ()
            {
            InitializeComponent ();
            //m_DirectoryPath = System.AppDomain.CurrentDomain.BaseDirectory;
            m_DirectoryPath = Directory.GetParent( System.Reflection.Assembly.GetExecutingAssembly ().Location).FullName ;
            m_PipelineCabFile = "Pipeline-lite.cab";
            m_ExtractDirName = "ExtractedFiles\\";

            m_PathsInfoToRemove = new List<FileSystemInfo> ();
            m_PathsInfoToRemove.Add ( new FileInfo ( Path.Combine ( m_DirectoryPath, "CABARC.EXE" ) ) );
            m_PathsInfoToRemove.Add ( new FileInfo ( Path.Combine ( m_DirectoryPath, "CABINET.DLL" ) ) );
            System.Windows.Forms.MessageBox.Show ("Dir path" +  m_DirectoryPath );
            }
        

        public void ExtractFiles ()
            {

            if (File.Exists ( Path.Combine ( m_DirectoryPath, m_PipelineCabFile ) ))
                {
                DirectoryInfo extractDirInfo  = new DirectoryInfo ( Path.Combine ( m_DirectoryPath, m_ExtractDirName )) ;
                if (extractDirInfo.Exists) extractDirInfo.Delete ( true );
                extractDirInfo.Create ( );
                

                Process extractProcess = new Process ();
                extractProcess.StartInfo.WorkingDirectory = m_DirectoryPath;
                extractProcess.StartInfo.FileName = Path.Combine ( m_DirectoryPath, "CABARC.EXE" );
                extractProcess.StartInfo.Arguments = "-p x " + m_PipelineCabFile + " " + m_ExtractDirName;
                                                System.Windows.Forms.MessageBox.Show ( extractProcess.StartInfo.Arguments );
                extractProcess.Start ();
                extractProcess.WaitForExit ();

                // move extracted files and directories to desired location
                DirectoryInfo pipelineExtractDirInfo = new DirectoryInfo ( Path.Combine ( m_DirectoryPath, "ExtractedFiles\\Pipeline-lite" ) );
                //System.Windows.Forms.MessageBox.Show ( pipelineExtractDirInfo.FullName );d
                string newDirectory = Path.Combine ( m_DirectoryPath, "Pipeline-lite" );
                if (Directory.Exists ( newDirectory )) Directory.Delete ( newDirectory, true );
                pipelineExtractDirInfo.MoveTo ( newDirectory);

                // delete temprorary files and directories
                m_PathsInfoToRemove.Add ( extractDirInfo );
                m_PathsInfoToRemove.Add( new FileInfo ( Path.Combine (m_DirectoryPath, m_PipelineCabFile )) );
                DeleteTemporaryFiles ();
                System.Windows.Forms.MessageBox.Show ( "Done" );
                }
            else
                System.Windows.Forms.MessageBox.Show ( "File not found" );
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

        private void FileOperations_Committed ( object sender, InstallEventArgs e )
            {
            try
                {
                ExtractFiles ();
                                }
            catch (System.Exception ex)
                {
                System.Windows.Forms.MessageBox.Show ( ex.ToString () );
                }
            }


        }
    }