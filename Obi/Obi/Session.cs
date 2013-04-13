using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using urakawa;
using urakawa.media.data.audio;
using urakawa.xuk;

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

        public event ProjectClosedEventHandler ProjectClosed;   // the project was closed
        public event EventHandler ProjectCreated;               // a new project was created
        public event EventHandler ProjectOpened;                // a project was opened
        public event EventHandler ProjectSaved;                 // a project was saved
        List<string> m_ListOfErrorMessages = new List<string>();

        /// <summary>
        /// Create a new session for Obi.
        /// </summary>
        public Session()
        {
            mDataModelFactory = new DataModelFactory();
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
        public bool CanRedo { get { return mProject != null && Presentation.UndoRedoManager.CanRedo; } }

        /// <summary>
        /// True if the project has unsaved changes.
        /// </summary>
        public bool CanSave { get { return mChangesCount != 0; } }

        /// <summary>
        /// True if the redo stack is non-empty.
        /// </summary>
        public bool CanUndo { get { return mProject != null && Presentation.UndoRedoManager.CanUndo; } }

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
            get { return ((ObiRootNode)Presentation.RootNode).PrimaryExportDirectory; }
            //set
            //{
            //Presentation.RootNode.PrimaryExportDirectory = value;
            //PresentationHasChanged ( 1 );
            //}
        }

        /// <summary>
        /// Get the current (Obi) presentation.
        /// </summary>
        public ObiPresentation Presentation
        {
            get
            {
                return mProject == null ? null :
                    mProject.Presentations.Count == 0 ? null :
                    (ObiPresentation)mProject.Presentations.Get(0);
            }
        }

        /// <summary>
        /// Get the description of the top redo command.
        /// </summary>
        public string RedoLabel { get { return Presentation.UndoRedoManager.RedoShortDescription; } }

        /// <summary>
        /// Get the description of the top undo command.
        /// </summary>
        public string UndoLabel { get { return Presentation.UndoRedoManager.UndoShortDescription; } }

        public string BackUpPath
        {
            get
            {
                return System.IO.Path.Combine(m_BackupDirPath,
System.IO.Path.GetFileName(m_BackupProjectFilePath_temp));
            }
        }

        /// <summary>
        /// Close the last project.
        /// Will close no matter what, so check with CanClose before doing anything.
        /// </summary>
        public void Close()
        {
            if (mProject != null)
            {
                // save to backup
                if (Presentation != null && CanSave)
                {

                    SaveToBackup();
                }

                mProject.dataIsMissing -= new EventHandler<urakawa.events.media.data.DataIsMissingEventArgs>(OnDataIsMissing);

                // if the project could not be opened, there is no presentation so this call may fail
                Presentation presentation = null;
                try { presentation = Presentation; }
                catch (Exception) { }
                mProject = null;
                RemoveLock_safe(mPath);
                mPath = null;
                mChangesCount = 0;
                if (ProjectClosed != null) ProjectClosed(this, new ProjectClosedEventArgs(presentation));
            }
        }

        /// <summary>
        /// Remove the lock file for the project; fail silently.
        /// </summary>
        public void RemoveLock_safe(string path)
        {
            if (mCanDeleteLock)
            {
                string path_lock = path + ".lock";
                try { System.IO.File.Delete(path_lock); }
                catch (Exception) { }
                mCanDeleteLock = false;
            }
        }
        /// <summary>
        /// Removes additional lock file without disturbing process for main lock file of project
        /// </summary>
        /// <param name="path"></param>
        public void RemoveLock_Additional_safe(string path)
        {
            // This is temporary function, will be incoperated in main function. But it may be risky to do at this time.
            if (mCanDeleteLock)
            {
                string path_lock = path + ".lock";
                try { System.IO.File.Delete(path_lock); }
                catch (Exception) { }
                // set mDeleteLock flag to false only if lock for active session is removed
                if (System.IO.Path.GetFullPath(mPath)
    == System.IO.Path.GetFullPath(path))
                {
                    mCanDeleteLock = false;
                }
            }
        }


        /// <summary>
        /// Notify the session that the presentation has changed.
        /// </summary>
        public void PresentationHasChanged(int change) { mChangesCount += change; }

        /// <summary>
        /// Create a new presentation in the session, with a path to save its XUK file.
        /// </summary>
        public void NewPresentation(string path, string title, bool createTitleSection, string id, Settings settings, int audioChannels, int audioSampleRate)
        {
            CreateNewPresentationInBackend(path, title, createTitleSection, id, settings, false, audioChannels,audioSampleRate);
            if (ProjectCreated != null) ProjectCreated(this, null);
        }

        public void NotifyProjectCreated() { if (ProjectCreated != null) ProjectCreated(this, null); }


        internal void CreateNewPresentationInBackend(string path, string title, bool createTitleSection, string id, Settings settings, bool isStubProjectForImport, int audioChannels, int audioSampleRate)
        {
            mProject = new Project();
#if false //(DEBUG)
            mProject.PrettyFormat = true;
#else
            mProject.PrettyFormat = false;
#endif
            string parentDirectory = System.IO.Path.GetDirectoryName(path);
            Uri obiProjectDirectory = new Uri(parentDirectory);

            //Presentation presentation = mProject.AddNewPresentation(obiProjectDirectory, System.IO.Path.GetFileName(path));
            //ObiPresentation newPres = mProject.PresentationFactory.Create(mProject, obiProjectDirectory, System.IO.Path.GetFileName(path));

            ObiPresentation newPres = mProject.PresentationFactory.Create<ObiPresentation>();
            newPres.Project = mProject;
            newPres.RootUri = obiProjectDirectory;

            //TODO: it would be good for Obi to separate Data folder based on project file name,
            //TODO: otherwise collision of Data folder may happen if several project files are in same directory.
            //newPres.DataProviderManager.SetDataFileDirectoryWithPrefix(System.IO.Path.GetFileName(path));

#if DEBUG
            newPres.WarmUpAllFactories();
#endif


            mProject.Presentations.Insert(mProject.Presentations.Count, newPres);


            PCMFormatInfo pcmFormat = new PCMFormatInfo((ushort)audioChannels , (uint)audioSampleRate, (ushort)settings.AudioBitDepth);
            newPres.MediaDataManager.DefaultPCMFormat = pcmFormat;
            newPres.MediaDataManager.EnforceSinglePCMFormat = true;

            newPres.ChannelsManager.GetOrCreateTextChannel();
            //m_textChannel = presentation.ChannelFactory.CreateTextChannel();
            //m_textChannel.Name = "The Text Channel";

            newPres.ChannelsManager.GetOrCreateAudioChannel();
            //m_audioChannel = presentation.ChannelFactory.CreateAudioChannel();
            //m_audioChannel.Name = "The Audio Channel";

            ObiRootNode rootNode = newPres.TreeNodeFactory.Create<ObiRootNode>();
            newPres.RootNode = rootNode;

            //sdk2
            //mProject.setDataModelFactory ( mDataModelFactory );
            //mProject.setPresentation ( mDataModelFactory.createPresentation (), 0 );

            mPath = path;
            GetLock(mPath);
            mChangesCount = 0;
            newPres.Initialize(this, title, createTitleSection, id, settings, isStubProjectForImport);

            //sdk2
            //Presentation.setRootUri ( new Uri ( path ) );

            //sdk2
            // create data directory if it is not created
            //string dataDirectory = ((urakawa.media.data.FileDataProviderManager)Presentation.getDataProviderManager ()).getDataFileDirectoryFullPath ();
            //if ( !Directory.Exists (dataDirectory ) )
            //    {
            //    Directory.CreateDirectory ( dataDirectory );
            //    }

            //if (ProjectCreated != null) ProjectCreated ( this, null );

            SetupBackupFilesForNewSession(path);
            ShouldDisableDiskSpaceCheck();
            Save(mPath);
            //ForceSave ();
        }

        private bool m_ErrorsInOpeningProject;
        public bool ErrorsInOpeningProject { get { return m_ErrorsInOpeningProject; } }

        /// <summary>
        /// Open a project from a XUK file.
        /// </summary>
        public void Open(string path)
        {
            m_ErrorsInOpeningProject = false;
            m_ListOfErrorMessages.Clear();
            mProject = new urakawa.Project();
            //sdk2
            //mProject.setDataModelFactory ( mDataModelFactory );
            mProject.dataIsMissing += new EventHandler<urakawa.events.media.data.DataIsMissingEventArgs>(OnDataIsMissing);

            //long memoryBefore = System.GC.GetTotalMemory(true);
            //sdk2
            //mProject.openXUK ( new Uri ( path ) );
            //System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            //stopWatch.Start();
            OpenXukAction action = new OpenXukAction(mProject, new Uri(path));
            action.ShortDescription = "DUMMY";
            action.LongDescription = "DUMMY";
            action.DoWork();
            //stopWatch.Stop();
            //Console.WriteLine("Time taken for xuk-in in milliseconds " + stopWatch.ElapsedMilliseconds);
            //Presentation = mProject.Presentations.Get(0);
            //long memoryAfter =  System.GC.GetTotalMemory(true);
            //long memoryDiff = memoryBefore - memoryAfter;
            //Console.WriteLine("opening project memory differenc is " + (memoryDiff / 1024));
            mPath = path;
            GetLock(mPath);

            Presentation.Initialize(this);


            // Hack to ignore the empty commands saved by the default undo/redo manager
            Presentation.UndoRedoManager.FlushCommands();
            ((ObiRootNode)mProject.Presentations.Get(0).RootNode).LocateBookMarkAndAssociatedNode();
            SetupBackupFilesForNewSession(path);

            if (ProjectOpened != null) ProjectOpened(this, null);
            if (m_ListOfErrorMessages.Count > 0)
            {
                m_ErrorsInOpeningProject = true;
                Dialogs.ReportDialog reportDialog = new Obi.Dialogs.ReportDialog(Localizer.Message("Warning"), Localizer.Message("Error_Message"), m_ListOfErrorMessages);
                reportDialog.ShowDialog();
            }
            ShouldDisableDiskSpaceCheck();
        }

        void OnDataIsMissing(object sender, urakawa.events.media.data.DataIsMissingEventArgs e)
        {
            m_ListOfErrorMessages.Add(e.Exception.Message);
            //Console.WriteLine(e.Exception.Message);
            //MessageBox.Show(Localizer.Message("OpenError_UseCleanUp") + "\n" + e.Exception.Message, Localizer.Message("open_project_error_caption"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // Get a lock file, and throw an exception if there is already one.
        private void GetLock(string path)
        {
            mCanDeleteLock = false;
            string path_lock = path + ".lock";
            if (System.IO.File.Exists(path_lock))
            {
                throw new Exception(string.Format(Localizer.Message("project_locked"), path_lock));
            }
            try
            {
                System.IO.File.Create(path_lock).Close();
                mCanDeleteLock = true;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(Localizer.Message("project_lock_error"), path_lock, e.Message), e);
            }
        }

        /// <summary>
        /// Save the current presentation to XUK.
        /// </summary>
        public void Save()
        {

            if (CanSave) ForceSave();
        }

        /// <summary>
        /// Always save, regardless of change count (which gets reset.)
        /// </summary>
        public void ForceSave()
        {
            if (CheckDiskSpace() <= 10)
            {
                DialogResult result = MessageBox.Show(string.Format(Localizer.Message("LimitedDiskSpaceWarning"), 10), Localizer.Message("Memory_Warning"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    return;
                }
            }
            if (Save(mPath))
            {
                m_ErrorsInOpeningProject = false;
                mChangesCount = 0;
                if (ProjectSaved != null) ProjectSaved(this, null);
            }
        }

        /// <summary>
        /// Save the project under a given location (used by save for the regular location,
        /// or save as for a different location.)
        /// </summary>
        public bool Save(string path)
        {
            string precautionBackupFilePath = null;
            bool isError = false;
            try
            {
                //Uri prevRootUri = Presentation.getRootUri ();
                precautionBackupFilePath = CreatePrecautionBackupBeforeSave(path);
                // Make sure that saving is finished before returning
                System.Threading.EventWaitHandle wh = new System.Threading.AutoResetEvent(false);
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                urakawa.xuk.SaveXukAction save = new urakawa.xuk.SaveXukAction(mProject, mProject, new Uri(path), true);
                save.Finished += new EventHandler<urakawa.events.progress.FinishedEventArgs>
    (delegate(object sender, urakawa.events.progress.FinishedEventArgs e) { wh.Set(); });
                save.DoWork();
                wh.WaitOne();
                stopWatch.Stop();
                Console.WriteLine("Time consumed in saving (in milliseconds) " + stopWatch.ElapsedMilliseconds);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(Localizer.Message("ErrorInSaving") + "\n\n" + ex.ToString(),
                        Localizer.Message("Caption_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                isError = true;
            }

            if (isError)
            {
                // restore the original file in case of error.
                try
                {
                    if (precautionBackupFilePath != null && File.Exists(precautionBackupFilePath))
                    {
                        string originalPath = Presentation.RootUri.LocalPath;
                        if (File.Exists(originalPath)) File.Delete(originalPath);
                        File.Move(precautionBackupFilePath, originalPath);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                precautionBackupFilePath = null;
                return false;
            }//error flag check

            // delete the precaution file if there was no error
            if (precautionBackupFilePath != null && File.Exists(precautionBackupFilePath)) File.Delete(precautionBackupFilePath);
            return true;
        }

        private string CreatePrecautionBackupBeforeSave(string path)
        {
            string precautionFilePath = Presentation.RootUri.LocalPath;
            if (precautionFilePath == null || !File.Exists(precautionFilePath) || System.IO.Path.GetFullPath(path) != precautionFilePath) return null;

            for (int i = 0; File.Exists(precautionFilePath += i.ToString()); i++)
            { }
            File.Copy(Presentation.RootUri.LocalPath, precautionFilePath);
            Console.WriteLine("Precaution file is created at " + precautionFilePath);

            return precautionFilePath;
        }

        /// <summary>
        /// save project to backup file for recovery purpose
        /// </summary>
        public string SaveToBackup()
        {
            if (m_BackupDirPath != null && Directory.Exists(m_BackupDirPath))
            {
                try
                {
                    if (!File.Exists(m_BackupProjectFilePath_temp))
                    {
                        File.Create(m_BackupProjectFilePath_temp).Close();
                    }

                    //sdk2
                    //Uri prevUri = Presentation.getRootUri ();
                    //Presentation.setRootUri ( new Uri ( m_BackupProjectFilePath_temp ) );


                    Uri oldUri = Presentation.RootUri;
                    string oldDataDir = Presentation.DataProviderManager.DataFileDirectory;

                    string dirPath = System.IO.Path.GetDirectoryName(m_BackupProjectFilePath_temp);
                    string prefix = System.IO.Path.GetFileName(m_BackupProjectFilePath_temp);


                    //TODO: it would be good for Obi to separate Data folder based on project file name,
                    //TODO: otherwise collision of Data folder may happen if several project files are in same directory.
                    //Presentation.DataProviderManager.SetDataFileDirectoryWithPrefix(prefix);
                    Presentation.RootUri = new Uri(dirPath + System.IO.Path.DirectorySeparatorChar, UriKind.Absolute);


                    Save(m_BackupProjectFilePath_temp);

                    //sdk2
                    //Presentation.setRootUri ( prevUri );

                    Presentation.RootUri = oldUri;
                    Presentation.DataProviderManager.DataFileDirectory = oldDataDir;

                    if (!Directory.Exists(m_BackupDirPath))
                    {
                        Directory.CreateDirectory(m_BackupDirPath);
                    }

                    string backupPath = System.IO.Path.Combine(m_BackupDirPath,
                        System.IO.Path.GetFileName(m_BackupProjectFilePath_temp));
                    // move backup file to backupfolder
                    if (File.Exists(backupPath))
                    {
                        File.Delete(backupPath);
                    }
                    File.Move(m_BackupProjectFilePath_temp, backupPath);

                    return backupPath;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(Localizer.Message("AutoSave_Error") + "\n\n" +
                        ex.ToString());
                }
            } // backup dir check ends

            return null;
        }

        /// <summary>
        /// Cleanup after a project failed to be created.
        /// </summary>
        public void CleanupAfterFailure()
        {
            if (mPath != null) System.IO.File.Delete(mPath);
            Close();
        }

        /// <summary>
        /// Setup backup directory and temp backup file when a project is loaded
        /// </summary>
        /// <param name="path"></param>
        private void SetupBackupFilesForNewSession(string path)
        {
            string projectDirPath = Directory.GetParent(path).FullName;
            m_BackupDirPath = System.IO.Path.Combine(projectDirPath, "Backup");
            try
            {
                m_BackupProjectFilePath_temp = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(path),
            "Backup_" + System.IO.Path.GetFileName(path));

                if (!Directory.Exists(m_BackupDirPath))
                {
                    Directory.CreateDirectory(m_BackupDirPath);
                }

            } // try ends
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        /// <summary>
        /// Imports a DAISY 3 book in Obi
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="title"></param>
        /// <param name="createTitleSection"></param>
        /// <param name="id"></param>
        /// <param name="settings"></param>
        /// <param name="importDTBPath"></param>
        public void ImportProjectFromDTB(string outputPath, string title, bool createTitleSection, string id, Settings settings, string importDTBPath, ref ImportExport.DAISY3_ObiImport import, int audioChannels, int audioSampleRate)
        {
            importDTBPath = System.IO.Path.GetFullPath(importDTBPath);
            CreateNewPresentationInBackend(outputPath, title, createTitleSection, id, settings, true, audioChannels,audioSampleRate);
            import = new Obi.ImportExport.DAISY3_ObiImport(this, settings, importDTBPath, System.IO.Path.GetDirectoryName(outputPath), false, 
                audioSampleRate ==44100? AudioLib.SampleRate.Hz44100: audioSampleRate == 22050?  AudioLib.SampleRate.Hz22050: AudioLib.SampleRate.Hz11025, 
                audioChannels  == 2);
            import.DoWork();
            if (import.RequestCancellation)
            {
                mProject = null;
                return;
            }
            Presentation.CheckAndCreateDefaultMetadataItems(settings.UserProfile);
            import.CorrectExternalAudioMedia();
            Save(Path);
            if (ProjectCreated != null) ProjectCreated(this, null);
        }

        private bool m_EnableDiskSpaceCheck = true;
        public bool EnableFreeDiskSpaceCheck
        {
            get { return m_EnableDiskSpaceCheck; }
            set
            {
                m_EnableDiskSpaceCheck = value;
                ShouldDisableDiskSpaceCheck();
            }
        }

        private void ShouldDisableDiskSpaceCheck()
        {
            if ( !m_EnableDiskSpaceCheck) return ;
            try
            {
                CheckDiskSpace();
            }
            catch ( System.Exception)
            {
                m_EnableDiskSpaceCheck = false ;
                
            }
        }

        public long CheckDiskSpace()
        {
            if (!m_EnableDiskSpaceCheck ||  string.IsNullOrEmpty (mPath) || !System.IO.Path.IsPathRooted(mPath)) return long.MaxValue ;
            string rootDir = System.IO.Path.GetPathRoot(mPath);
            
            long freeSpace = 0;
            if ( !string.IsNullOrEmpty(rootDir ))
            {//1
            DriveInfo driveSpace = new DriveInfo(rootDir);
            if (driveSpace.IsReady)
            {//2
                const int num = 1048576;// 1024*1024
                freeSpace = driveSpace.AvailableFreeSpace / num;
            }//-2
            
            return freeSpace;
        }//-1
            else
    {
        return long.MaxValue ;
    }
        }
    }

    public class ProjectClosedEventArgs : EventArgs
    {
        private Presentation mClosedPresentation;
        public ProjectClosedEventArgs(Presentation p) : base() { mClosedPresentation = p; }
        public Presentation ClosedPresentation { get { return mClosedPresentation; } }
    }

    public delegate void ProjectClosedEventHandler(object sender, ProjectClosedEventArgs e);
}