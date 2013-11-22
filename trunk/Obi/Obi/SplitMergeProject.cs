using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using urakawa;
using urakawa.core;
using urakawa.progress;
using urakawa.xuk;
using urakawa.property.xml;
using AudioLib;

namespace Obi
{
    class SplitMergeProject : DualCancellableProgressReporter
    {
        private readonly Session m_session;
        private string m_SourceProjectPath;
        private readonly string m_docPath;
        private List<SectionNode> m_SectionsToMerge = new List<SectionNode>();

        public SplitMergeProject(Session session, string sourceProjectPath)
        {
            m_session = session;
            m_SourceProjectPath = sourceProjectPath;
        }

        public List<SectionNode> SectionsToMerge { get { return m_SectionsToMerge; } }

        public override void DoWork()
        {
            MergeProject();
        }

        public void MergeProject()
        {
            m_SectionsToMerge.Clear();
            try
                            {//1
                                Uri uri = new Uri(m_SourceProjectPath, UriKind.Absolute);
                                bool pretty = m_session.Presentation.Project.PrettyFormat;
                                Project subproject = new Project();
                                subproject.PrettyFormat = pretty;
                                OpenXukAction action = new OpenXukAction(subproject, uri);
                                action.ShortDescription = "...";
                                action.LongDescription = "...";
                                action.Execute();

                                Presentation subpresentation = subproject.Presentations.Get(0);
                // compare audio formats
                                if (m_session.Presentation.MediaDataManager.DefaultPCMFormat.Data.BitDepth != subpresentation.MediaDataManager.DefaultPCMFormat.Data.BitDepth
                                    || m_session.Presentation.MediaDataManager.DefaultPCMFormat.Data.BlockAlign != subpresentation.MediaDataManager.DefaultPCMFormat.Data.BlockAlign
                                    || m_session.Presentation.MediaDataManager.DefaultPCMFormat.Data.SampleRate != subpresentation.MediaDataManager.DefaultPCMFormat.Data.SampleRate)
                                {
                                    throw new System.Exception("Audio format of project does not match");
                                }
                                TreeNode subroot = subpresentation.RootNode;
                int sectionCount = ((ObiRootNode)subroot).SectionChildCount  ;
                                if (sectionCount == 0) return ;
                int progressValue = 0 ;
                
                                reportProgress (progressValue, "") ;
                SectionNode section =((ObiRootNode) subroot).SectionChild(0);
                                while (section != null)
                                {//2
                                    if (RequestCancellation) return;
                                    TreeNode importedLevel = section.Export(m_session.Presentation);

                                    ObiRootNode parent = (ObiRootNode)m_session.Presentation.RootNode;
                                        //parent.AppendChild((SectionNode) importedLevel);
                                    m_SectionsToMerge.Add((SectionNode) importedLevel);
                                    progressValue += 90/sectionCount ;
                                    reportProgress(progressValue, 
                                        string.Format( Localizer.Message("MergeProject_SectionProgress"),section.Label));
                                        //System.Windows.Forms.MessageBox.Show(section.GetText().ToString());
section = section.NextSibling;
                                }//-2
                            }//-1
                            //catch (Exception ex)
                            //{//1
                                //messageBoxAlert("PROBLEM:\n " + xukPath, null);
                                //m_session.messageBoxText("MERGE PROBLEM", xukPath, ex.Message);

                                //throw ex;
                                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                            //}//-1
                        
                
                finally
                {
                    
                }
                reportProgress(100, "");
        }

        /*
        public SplitMergeProject (Session session, int total, string docPath, bool pretty, List<string> cleanupFolders, string splitDirectory, string fileNameWithoutExtension, string extension)
            {
                m_session = session;
                m_total = total;
                m_docPath = docPath;
                m_pretty = pretty;
                m_cleanupFolders = cleanupFolders;
                m_splitDirectory = splitDirectory;
                m_fileNameWithoutExtension = fileNameWithoutExtension;
                m_extension = extension;
            }

        
        public void Split()
            {
                for (int i = 0; i < m_total; i++)
                {
                    int j = i + 1;

                    //reportProgress(100 * j / m_total, j + " / " + m_total);

                    //Thread.Sleep(1000);

                    //if (RequestCancellation)
                    //{
                        //return;
                    //}

                    //// Open original (untouched)
                    //------
                    //Session newSession = new Session();
                    //newSession.RemoveLock_safe(m_docPath);
                    //System.Windows.Forms.MessageBox.Show("lock removed");
                    //newSession.OpenProjectInBackend(m_docPath);
                    //System.Windows.Forms.MessageBox.Show("openned in backend");
                    //---
                    Uri uri = new Uri(m_docPath, UriKind.Absolute);
                    Project project = new Project();
                    project.PrettyFormat = m_pretty;
                    OpenXukAction action = new OpenXukAction(project, uri);
                    action.ShortDescription = "...";
                    action.LongDescription = "...";
                    action.Execute();

                    //if (project.Presentations.Count <= 0)
                    //{
//#if DEBUG
                        //Debugger.Break();
//#endif //DEBUG
                        //RequestCancellation = true;
                        //return;
                    //}
                    //---
                    Presentation presentation = project.Presentations.Get(0);
                    TreeNode root = presentation.RootNode;
                    //root.GetOrCreateXmlProperty().SetAttribute("splitMerge", "", i.ToString());
                    
                    System.Windows.Forms.MessageBox.Show(i.ToString());
                    int counter = -1;

                    TreeNode level = root.GetFirstDescendantWithText();// ("section");
                    System.Windows.Forms.MessageBox.Show(level.GetTextFlattened());
                    //SectionNode level = presentation.FirstSection;
                    while (level != null)
                    {
                        counter++;

                        if (counter == i)
                        {
                            //XmlProperty xmlProp = level.GetOrCreateXmlProperty();
                            //xmlProp.SetAttribute("splitMergeId", "", i.ToString());

                            TreeNode parent = level.Parent;
                            TreeNode toKeep = level;
                            while (parent != null)
                            {
                                foreach (TreeNode child in parent.Children.ContentsAs_ListCopy)
                                {
                                    if (child != toKeep)
                                    {
                                        System.Windows.Forms.MessageBox.Show("removing: " + child.GetText());
                                        parent.RemoveChild(child);
                                    }
                                }

                                toKeep = parent;
                                parent = parent.Parent;
                            }

                            break;
                        }

                        level = level.GetNextSiblingWithText();// ("section");
                        //level = level.NextSibling;
                    }


                    string subDirectory = Path.Combine(m_splitDirectory, m_fileNameWithoutExtension + "_" + i);
                    string destinationFilePath = Path.Combine(subDirectory, i + m_extension);
                    System.Windows.Forms.MessageBox.Show(subDirectory);
                    if (Directory.Exists(subDirectory)) Directory.Delete(subDirectory, true);
                    Directory.CreateDirectory(subDirectory);

                    bool saved = false;
                    try
                    {
                        urakawa.xuk.SaveXukAction save = new urakawa.xuk.SaveXukAction(project, project, new Uri(destinationFilePath), true);
                        save.DoWork();

                        //newSession.Save(destinationFilePath);
                        //saved = m_session.saveAsCommand(destinationFilePath, true);
                        //SaveAsCommand.Execute();
                    }
                    catch (System.Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        //m_session.DocumentFilePath = null;
                        //m_session.DocumentProject = null;
                    }

                    if (!saved)
                    {
#if DEBUG
                        Debugger.Break();
#endif //DEBUG

                        //RequestCancellation = true;
                        //return;
                    }

                    //m_session.DocumentFilePath = destinationFilePath;
                    //m_session.DocumentProject = project;

                    //Uri oldUri = DocumentProject.Presentations.Get(0).RootUri;
                    //string oldDataDir = DocumentProject.Presentations.Get(0).DataProviderManager.DataFileDirectory;
                    // commented the lower part, not relevant 
                    //string dirPath_ = Path.GetDirectoryName(destinationFilePath);
                    //string prefix_ = Path.GetFileNameWithoutExtension(destinationFilePath);

                    //m_session.DocumentProject.Presentations.Get(0).DataProviderManager.SetCustomDataFileDirectory(prefix_);
                    //m_session.DocumentProject.Presentations.Get(0).RootUri = new Uri(dirPath_ + Path.DirectorySeparatorChar, UriKind.Absolute);

                    try
                    {
                        //string deletedDataFolderPath_ = m_session.DataCleanup(false);

                        ///if (!string.IsNullOrEmpty(deletedDataFolderPath_) && Directory.Exists(deletedDataFolderPath_))
                        //{
                            //m_cleanupFolders.Add(deletedDataFolderPath_);
                            
                        //}

                        //saved = m_session.save(true);
                    }
                    finally
                    {
                        //m_session.DocumentFilePath = null;
                        //m_session.DocumentProject = null;
                    }

                    //if (!saved)
                    //{
//#if DEBUG
                        //Debugger.Break();
//#endif //DEBUG
                        //RequestCancellation = true;
                        //return;
                    //}
                }
            }
*/        

    }

}
