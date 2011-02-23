using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;

using urakawa;
using urakawa.core;
using urakawa.daisy;
using urakawa.daisy.import;
using urakawa.media.data.audio;
using AudioLib ;


namespace Obi.Export
{
    public class DAISY3_ObiImport : urakawa.daisy.import.Daisy3_Import
    {
        ObiPresentation m_Presentation;
        Session m_Session;
        

        public DAISY3_ObiImport(Session session, string bookfile, string outDir, bool skipACM, SampleRate audioProjectSampleRate)
            : base(bookfile, outDir, skipACM, audioProjectSampleRate)
        {
            m_Session = session;
            XukPath = Path.Combine(m_outDirectory, "project.obi");
            
        }
        //protected override void CreateProjectFileAndDirectory()
        //{
            //if (!Directory.Exists(m_outDirectory))
            //{
                //Directory.CreateDirectory(m_outDirectory);
            //}
            //m_Xuk_FilePath = m_Session.Path;
        //}

        protected override void initializeProject()
        {
            
            //m_Project = new Project();
            Project = m_Session.Presentation.Project;
#if (DEBUG)
            Project.SetPrettyFormat(true);
#else
            Project.SetPrettyFormat(false);
#endif

            //Presentation presentation = m_Project.AddNewPresentation(new Uri(m_outDirectory), Path.GetFileName(m_Book_FilePath));
            m_Presentation = m_Session.Presentation;

            PCMFormatInfo pcmFormat = m_Presentation.MediaDataManager.DefaultPCMFormat.Copy();
            pcmFormat.Data.SampleRate = (ushort)m_audioProjectSampleRate;
            m_Presentation.MediaDataManager.DefaultPCMFormat = pcmFormat;

            m_Presentation.MediaDataManager.EnforceSinglePCMFormat = true;

            //m_textChannel = m_Presentation.ChannelFactory.CreateTextChannel();
            //m_textChannel.Name = "The Text Channel";
            m_textChannel = m_Presentation.ChannelsManager.GetOrCreateTextChannel();

            //m_audioChannel = m_Presentation.ChannelFactory.CreateAudioChannel();
            //m_audioChannel.Name = "The Audio Channel";
            m_audioChannel = m_Presentation.ChannelsManager.GetOrCreateAudioChannel();

            //m_ImageChannel = m_Presentation.ChannelFactory.CreateImageChannel();
            //m_ImageChannel.Name = "The Image Channel";

            /*string dataPath = presentation.DataProviderManager.DataFileDirectoryFullPath;
           if (Directory.Exists(dataPath))
           {
               Directory.Delete(dataPath, true);
           }*/
        }

        protected override TreeNode CreateTreeNodeForNavPoint(TreeNode parentNode, XmlNode navPoint)
        {
            SectionNode treeNode = m_Presentation.CreateSectionNode () ;

                //= parentNode.Presentation.TreeNodeFactory.Create();
            //parentNode.AppendChild(treeNode);
            if (parentNode is ObiRootNode ) 
            {
                ((ObiNode)parentNode).AppendChild (treeNode) ;
            }
            else 
            {
                ((SectionNode)parentNode).AppendChild (treeNode ) ;
            }
            //XmlProperty xmlProp = parentNode.Presentation.PropertyFactory.CreateXmlProperty();
            //treeNode.AddProperty(xmlProp);
            XmlNode textNode = XmlDocumentHelper.GetFirstChildElementWithName(navPoint, true, "text", navPoint.NamespaceURI);
            //xmlProp.LocalName = "level";//+":" + textNode.InnerText;
            // create urakawa tree node

            //TextMedia textMedia = parentNode.Presentation.MediaFactory.CreateTextMedia();
            //textMedia.Text = textNode.InnerText;
            treeNode.Label = textNode.InnerText; ;
            //ChannelsProperty cProp = parentNode.Presentation.PropertyFactory.CreateChannelsProperty();
            //cProp.SetMedia(m_textChannel, textMedia);

            //TreeNode txtWrapperNode = parentNode.Presentation.TreeNodeFactory.Create();
            //txtWrapperNode.AddProperty(cProp);
            //treeNode.AppendChild(txtWrapperNode);

            //XmlProperty TextNodeXmlProp = parentNode.Presentation.PropertyFactory.CreateXmlProperty();
            //txtWrapperNode.AddProperty(TextNodeXmlProp);
            //TextNodeXmlProp.LocalName = "hd";

            return treeNode;
        }

        protected override TreeNode CreateTreeNodeForAudioNode(TreeNode navPointTreeNode, bool isHeadingNode)
        {
            PhraseNode audioWrapperNode = m_Presentation.CreatePhraseNode ();
            ((SectionNode)navPointTreeNode).AppendChild(audioWrapperNode);
            return audioWrapperNode;
            /*
            if (isHeadingNode)
            {
                foreach (TreeNode txtNode in navPointTreeNode.Children.ContentsAs_YieldEnumerable)
                {
                    if (txtNode.GetTextMedia() != null)
                    {
                        audioWrapperNode = txtNode;
                        break;
                    }
                }
            }
            else
            {
                if (navPointTreeNode == null) return null;
                audioWrapperNode = navPointTreeNode.Presentation.TreeNodeFactory.Create();

                navPointTreeNode.AppendChild(audioWrapperNode);
            }
            //XmlProperty xmlProp = navPointTreeNode.Presentation.PropertyFactory.CreateXmlProperty();
            //audioWrapperNode.AddProperty(xmlProp);
            //xmlProp.LocalName = "phrase"; // +":" + navPointTreeNode.GetTextFlattened(false);
            return audioWrapperNode;
             */ 
        }

        protected override void AddPagePropertiesToAudioNode(TreeNode audioWrapperNode, XmlNode pageTargetNode)
        {
            string strKind = pageTargetNode.Attributes.GetNamedItem("type").Value;
            PageKind kind = strKind == "front" ? PageKind.Front :
                strKind == "normal" ? PageKind.Normal :
                PageKind.Special;

            string pageNumberString = XmlDocumentHelper.GetFirstChildElementWithName(pageTargetNode, true, "text", pageTargetNode.NamespaceURI).InnerText;
            PageNumber number = null;
            if (kind == PageKind.Special && pageNumberString != null && pageNumberString != "")
            {
                number = new PageNumber(pageNumberString);
            }
            else if (kind == PageKind.Front || kind == PageKind.Normal)
            {
                int pageNumber = EmptyNode.SafeParsePageNumber(pageNumberString);
                if (pageNumber > 0) number = new PageNumber(pageNumber, kind);
            }
            if (number != null)
            {
                EmptyNode node = (EmptyNode)audioWrapperNode ;
                node.PageNumber = number;
                
            }

            /*
            TextMedia textMedia = audioWrapperNode.Presentation.MediaFactory.CreateTextMedia();
            textMedia.Text = XmlDocumentHelper.GetFirstChildElementWithName(pageTargetNode, true, "text", pageTargetNode.NamespaceURI).InnerText;
            ChannelsProperty cProp = audioWrapperNode.Presentation.PropertyFactory.CreateChannelsProperty();
            cProp.SetMedia(m_textChannel, textMedia);
            audioWrapperNode.AddProperty(cProp);
            System.Xml.XmlAttributeCollection pageAttributes = pageTargetNode.Attributes;
            if (pageAttributes != null)
            {
                XmlProperty xmlProp = audioWrapperNode.GetXmlProperty();
                foreach (System.Xml.XmlAttribute attr in pageAttributes)
                {
                    xmlProp.SetAttribute(attr.Name, attr.NamespaceURI, attr.Value);
                }
            }
*/
        }


    }
}
