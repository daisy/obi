using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace Obi
    {
    /// <summary>
    /// The Obi work session. In the future, it may handle several presentations (i.e. several Obi projects.)
    /// </summary>
    public class Session
        {
        private DataModelFactory mDataModelFactory;  // the Obi data model factory (see below)
        private urakawa.Project mProject;            // the current project (as of now 1 presentation = 1 project)
        private string mPath;                        // path of the XUK file to save to
        private int mChangesCount;                   // changes since (or before) last save
        private bool mCanDeleteLock;                 // flag to delete the lock on closign (when obtained successfully)

        private string m_BackupProjectFilePath_temp;
        private string m_BackupDirPath;
        private int m_BackupFileCounter;
        private readonly string m_Backup_LastFilename = "Prev_Session_Last.obi";

        public event ProjectClosedEventHandler ProjectClosed;   // the project was closed
        public event EventHandler ProjectCreated;               // a new project was created
        public event EventHandler ProjectOpened;                // a project was opened
        public event EventHandler ProjectSaved;                 // a project was saved

        /// <summary>
        /// Create a new session for Obi.
        /// </summary>
        public Session ()
            {
            mDataModelFactory = new DataModelFactory ();
            mProject = null;
            mPath = null;
            mChangesCount = 0;
            mCanDeleteLock = false;
            }


        /// <summary>
        /// True if the project can be safely closed.
        /// </summary>
        public bool CanClose { get { return mChangesCount == 0; } }

        /// <summary>
        /// True if the redo stack is non-empty.
        /// </summary>
        public bool CanRedo { get { return mProject != null && Presentation.getUndoRedoManager ().canRedo (); } }

        /// <summary>
        /// True if the project has unsaved changes.
        /// </summary>
        public bool CanSave { get { return mChangesCount != 0; } }

        /// <summary>
        /// True if the redo stack is non-empty.
        /// </summary>
        public bool CanUndo { get { return mProject != null && Presentation.getUndoRedoManager ().canUndo (); } }

        /// <summary>
        /// True if there is a project currently open.
        /// </summary>
        public bool HasProject { get { return mProject != null; } }

        /// <summary>
        /// Get the path of the XUK file of the current presentation.
        /// </summary>
        public string Path { get { return mPath; } }

        /// <summary>
        /// Path of directory containing exported DAISY book in raw PCM format
        /// </summary>
        public string PrimaryExportPath
            {
            get { return Presentation.RootNode.PrimaryExportDirectory; }
            //set
                //{
                //Presentation.RootNode.PrimaryExportDirectory = value;
                //PresentationHasChanged ( 1 );
                //}
            }

        /// <summary>
        /// Get the current (Obi) presentation.
        /// </summary>
        public Presentation Presentation { get { return mProject == null ? null : (Presentation)mProject.getPresentation ( 0 ); } }

        /// <summary>
        /// Get the description of the top redo command.
        /// </summary>
        public string RedoLabel { get { return Presentation.getUndoRedoManager ().getRedoShortDescription (); } }

        /// <summary>
        /// Get the description of the top undo command.
        /// </summary>
        public string UndoLabel { get { return Presentation.getUndoRedoManager ().getUndoShortDescription (); } }


        /// <summary>
        /// Close the last project.
        /// Will close no matter what, so check with CanClose before doing anything.
        /// </summary>
        public void Close ()
            {
            if (mProject != null)
                {
                // save to backup
                if (Presentation != null)
                    {
                    //SaveLastBackupAndClearIncrementalFiles ();
                    SaveToBackup ();
                    }

                mProject.dataIsMissing -= new EventHandler<urakawa.events.media.data.DataIsMissingEventArgs> ( OnDataIsMissing );

                // if the project could not be opened, there is no presentation so this call may fail
                Presentation presentation = null;
                try { presentation = Presentation; }
                catch (Exception) { }
                mProject = null;
                RemoveLock_safe ( mPath );
                mPath = null;
                mChangesCount = 0;
                if (ProjectClosed != null) ProjectClosed ( this, new ProjectClosedEventArgs ( presentation ) );
                }
            }

        /// <summary>
        /// Remove the lock file for the project; fail silently.
        /// </summary>
        public void RemoveLock_safe ( string path )
            {
            if (mCanDeleteLock)
                {
                string path_lock = path + ".lock";
                try { System.IO.File.Delete ( path_lock ); }
                catch (Exception) { }
                mCanDeleteLock = false;
                }
            }
        /// <summary>
        /// Removes additional lock file without disturbing process for main lock file of project
        /// </summary>
        /// <param name="path"></param>
        public void RemoveLock_Additional_safe ( string path )
            {
            // This is temporary function, will be incoperated in main function. But it may be risky to do at this time.
            if (mCanDeleteLock)
                {
                string path_lock = path + ".lock";
                try { System.IO.File.Delete ( path_lock ); }
                catch (Exception) { }
                // set mDeleteLock flag to false only if lock for active session is removed
                if (System.IO.Path.GetFullPath ( mPath )
    == System.IO.Path.GetFullPath ( path ))
                    {
                    mCanDeleteLock = false;
                    }
                }
            }


        /// <summary>
        /// Notify the session that the presentation has changed.
        /// </summary>
        public void PresentationHasChanged ( int change ) { mChangesCount += change; }

        /// <summary>
        /// Create a new presentation in the session, with a path to save its XUK file.
        /// </summary>
        public void NewPresentation ( string path, string title, bool createTitleSection, string id, Settings settings )
            {
            mProject = new urakawa.Project ();
            mProject.setDataModelFactory ( mDataModelFactory );
            mProject.setPresentation ( mDataModelFactory.createPresentation (), 0 );
            mPath = path;
            GetLock ( mPath );
            mChangesCount = 0;
            Presentation.Initialize ( this, title, createTitleSection, id, settings );
            Presentation.setRootUri ( new Uri ( path ) );
            if (ProjectCreated != null) ProjectCreated ( this, null );

            SetupBackupFilesForNewSession ( path );

            ForceSave ();
            }

        /// <summary>
        /// Open a project from a XUK file.
        /// </summary>
        public void Open ( string path )
            {
            mProject = new urakawa.Project ();
            mProject.setDataModelFactory ( mDataModelFactory );
            mProject.dataIsMissing += new EventHandler<urakawa.events.media.data.DataIsMissingEventArgs> ( OnDataIsMissing );
            mProject.openXUK ( new Uri ( path ) );
            mPath = path;
            GetLock ( mPath );
            Presentation.Initialize ( this );
            // Hack to ignore the empty commands saved by the default undo/redo manager
            Presentation.getUndoRedoManager ().flushCommands ();

            SetupBackupFilesForNewSession ( path );

            if (ProjectOpened != null) ProjectOpened ( this, null );
            }

        void OnDataIsMissing ( object sender, urakawa.events.media.data.DataIsMissingEventArgs e )
            {
            MessageBox.Show ( Localizer.Message ( "OpenError_UseCleanUp" ) + "\n" + e.Exception.Message, Localizer.Message ( "open_project_error_caption" ),
                       MessageBoxButtons.OK, MessageBoxIcon.Warning );
            }

        // Get a lock file, and throw an exception if there is already one.
        private void GetLock ( string path )
            {
            mCanDeleteLock = false;
            string path_lock = path + ".lock";
            if (System.IO.File.Exists ( path_lock ))
                {
                throw new Exception ( string.Format ( Localizer.Message ( "project_locked" ), path_lock ) );
                }
            try
                {
                System.IO.File.Create ( path_lock ).Close ();
                mCanDeleteLock = true;
                }
            catch (Exception e)
                {
                throw new Exception ( string.Format ( Localizer.Message ( "project_lock_error" ), path_lock, e.Message ), e );
                }
            }

        /// <summary>
        /// Save the current presentation to XUK.
        /// </summary>
        public void Save ()
            {

            if (CanSave) ForceSave ();
            }

        /// <summary>
        /// Always save, regardless of change count (which gets reset.)
        /// </summary>
        public void ForceSave ()
            {
            Save ( mPath );
            mChangesCount = 0;
            if (ProjectSaved != null) ProjectSaved ( this, null );
            }

        /// <summary>
        /// Save the project under a given location (used by save for the regular location,
        /// or save as for a different location.)
        /// </summary>
        public void Save ( string path )
            {
            try
                {
                Uri prevRootUri = Presentation.getRootUri ();
                // Make sure that saving is finished before returning
                System.Threading.EventWaitHandle wh = new System.Threading.AutoResetEvent ( false );
                urakawa.xuk.SaveXukAction save = new urakawa.xuk.SaveXukAction ( mProject, new Uri ( path ) );
                save.finished += new EventHandler<urakawa.events.progress.FinishedEventArgs>
                    ( delegate ( object sender, urakawa.events.progress.FinishedEventArgs e ) { wh.Set (); } );
                save.execute ();
                wh.WaitOne ();
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( Localizer.Message ( "ErrorInSaving" ) + "\n\n" + ex.ToString (),
                        Localizer.Message ( "Caption_Error" ), MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
            }

        /// <summary>
        /// save project to backup file for recovery purpose
        /// </summary>
        public string SaveToBackup ()
            {
            if (m_BackupDirPath != null && Directory.Exists ( m_BackupDirPath ))
                {
                try
                    {
                    if (!File.Exists ( m_BackupProjectFilePath_temp ))
                        {
                        File.Create ( m_BackupProjectFilePath_temp ).Close ();
                        }

                    Uri prevUri = Presentation.getRootUri ();
                    Presentation.setRootUri ( new Uri ( m_BackupProjectFilePath_temp ) );
                    Save ( m_BackupProjectFilePath_temp );
                    Presentation.setRootUri ( prevUri );

                    string backupPath = GetNextBackupFilePath ();
                    // move backup file to backupfolder
                    File.Move ( m_BackupProjectFilePath_temp, backupPath );
                    return backupPath;
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( Localizer.Message ( "AutoSave_Error" ) + "\n\n" +
                        ex.ToString () );
                    }
                } // backup dir check ends

            return null;
            }

        /// <summary>
        /// Cleanup after a project failed to be created.
        /// </summary>
        public void CleanupAfterFailure ()
            {
            if (mPath != null) System.IO.File.Delete ( mPath );
            Close ();
            }

        /// <summary>
        /// Setup backup directory and temp backup file when a project is loaded
        /// </summary>
        /// <param name="path"></param>
        private void SetupBackupFilesForNewSession ( string path )
            {
            string projectDirPath = Directory.GetParent ( path ).FullName;
            m_BackupDirPath = System.IO.Path.Combine ( projectDirPath, "Backup" );
            try
                {
                m_BackupProjectFilePath_temp = System.IO.Path.Combine (
            System.IO.Path.GetDirectoryName ( path ),
            "backup_" + System.IO.Path.GetFileName ( path ) );

                if (!Directory.Exists ( m_BackupDirPath ))
                    {
                    Directory.CreateDirectory ( m_BackupDirPath );
                    }
                else
                    {
                    string[] filesArray = Directory.GetFiles ( m_BackupDirPath );

                    string previousSessionLastFilePath = System.IO.Path.Combine ( m_BackupDirPath, m_Backup_LastFilename );

                    int lastFileNo = 0;

                    for (int i = 0; i < filesArray.Length; i++)
                        {
                        int fileNumericName = 0;
                        //MessageBox.Show ( System.IO.Path.GetFileNameWithoutExtension ( filesArray[i] ) );
                        int.TryParse ( System.IO.Path.GetFileNameWithoutExtension ( filesArray[i] ), out fileNumericName );

                        if (fileNumericName > lastFileNo)
                            {
                            lastFileNo = fileNumericName;
                            }
                        }

                    string lastFileToRename = "";
                    for (int i = 0; i < filesArray.Length; i++)
                        {
                        if (System.IO.Path.GetFileNameWithoutExtension ( filesArray[i] ) != lastFileNo.ToString ())
                            {
                            File.Delete ( filesArray[i] );
                            //MessageBox.Show ( filesArray[i] );
                            }
                        else
                            {
                            lastFileToRename = filesArray[i];
                            }
                        }

                    // renaming last file to prev session name.
                    if (File.Exists ( lastFileToRename ))
                        {
                        if ( File.Exists ( previousSessionLastFilePath ) )
                            {
                            File.Delete ( previousSessionLastFilePath ) ;
                            }
                        File.Move ( lastFileToRename, previousSessionLastFilePath ) ;
                        }
                    }

                
                } // try ends
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }

            m_BackupFileCounter = 0;
            }

        private void SaveLastBackupAndClearIncrementalFiles ()
            {
            if (m_BackupDirPath != null && Directory.Exists ( m_BackupDirPath ))
                {
                string lastIncrmentalFile = SaveToBackup ();

                try
                    {
                    // rename the last backup file
                    string previousSessionFile = System.IO.Path.Combine ( m_BackupDirPath, m_Backup_LastFilename );

                    if (lastIncrmentalFile != null)
                        {
                        if (File.Exists ( previousSessionFile ))
                            {
                            File.Delete ( previousSessionFile );
                            }

                        File.Move ( lastIncrmentalFile, previousSessionFile );
                        }

                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( ex.ToString () );
                    }
                }
            }


        private string GetNextBackupFilePath ()
            {
            m_BackupFileCounter++;
            string nextBackupPath = System.IO.Path.Combine ( m_BackupDirPath,
                m_BackupFileCounter.ToString () + ".obi" );

            while (File.Exists ( nextBackupPath ) && m_BackupFileCounter < 2000)
                {
                m_BackupFileCounter++;
                nextBackupPath = System.IO.Path.Combine ( m_BackupDirPath,
                m_BackupFileCounter.ToString () + ".obi" );
                }
            return nextBackupPath;
            }

        }

    public class ProjectClosedEventArgs : EventArgs
        {
        private Presentation mClosedPresentation;
        public ProjectClosedEventArgs ( Presentation p ) : base () { mClosedPresentation = p; }
        public Presentation ClosedPresentation { get { return mClosedPresentation; } }
        }

    public delegate void ProjectClosedEventHandler ( object sender, ProjectClosedEventArgs e );
    }