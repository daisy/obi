using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using urakawa.daisy;

namespace Obi
{
    public class ProjectUpgrader
    {
        private string m_Source;
        private string m_Destination;
        private bool m_RequestCancellation = false;
        private Dictionary<string, string> m_OldToNewNodeElementNameMap;
        private Dictionary<string, string> m_OldToNewNodeAttributeNameMap;
        public event System.ComponentModel.ProgressChangedEventHandler ProgressChanged;

        public ProjectUpgrader(string source, string dest)
        {
            m_Source = source;
            m_Destination = Path.Combine( Path.GetDirectoryName (m_Source), "Obi2_"+ Path.GetFileName(m_Source)) ;
            PopulateNamesMapDictionary();
            m_RequestCancellation = false;
        }

        public bool RequestCancellation
        {
            get { return m_RequestCancellation; }
            set { m_RequestCancellation = value; }
        }

        private void PopulateNamesMapDictionary ()
        {
            m_OldToNewNodeElementNameMap = new Dictionary<string, string>();
            m_OldToNewNodeElementNameMap.Add("mProperties", "Properties");
            m_OldToNewNodeElementNameMap.Add("mChannelMappings", "ChannelMappings");
            m_OldToNewNodeElementNameMap.Add("mChannelMapping", "ChannelMapping");
            m_OldToNewNodeElementNameMap.Add("audioMediaDataUid", "MediaDataUid");
            m_OldToNewNodeElementNameMap.Add("mChildren", "Children");
            m_OldToNewNodeElementNameMap.Add("mText", "Text");
            //m_OldToNewNodeNameMap.Add("channel", "Channel");

            m_OldToNewNodeAttributeNameMap = new Dictionary<string, string>();
            m_OldToNewNodeAttributeNameMap.Add("audioMediaDataUid", "MediaDataUid");
            m_OldToNewNodeAttributeNameMap.Add("clipBegin", "ClipBegin");
            m_OldToNewNodeAttributeNameMap.Add("clipEnd", "ClipEnd");
            m_OldToNewNodeAttributeNameMap.Add("src", "Src");
        }

        private string GetNewElementName ( string oldName)
        {
            if ( m_OldToNewNodeElementNameMap == null || !m_OldToNewNodeElementNameMap.ContainsKey(oldName)) 
            {
                return oldName;
            }
            else
            {
            return m_OldToNewNodeElementNameMap[oldName] ;
            }
        }

        private string GetNewAttributeName(string oldName)
        {
            if (m_OldToNewNodeAttributeNameMap== null || !m_OldToNewNodeAttributeNameMap.ContainsKey(oldName))
            {
                return oldName;
            }
            else
            {
                return m_OldToNewNodeAttributeNameMap[oldName];
            }
        }

        public static bool IsObi1XProject(string projectPath)
        {
            XmlTextReader source = new XmlTextReader(projectPath);
            source.XmlResolver = null;
            source.WhitespaceHandling = WhitespaceHandling.Significant;
            String identificationString = null;
            try
            {
                //identificationString = source.ReadToFollowing("Xuk")? source.ReadAttributeValue (): "";
                if (source.ReadToFollowing("Xuk"))
                {

                    identificationString = source.GetAttribute(0);
                }
            }

            finally
            {
                source.Close();
            }
            if (identificationString == "http://www.daisy.org/urakawa/xuk/1.0 xuk.xsd")
            {
                return true;
            }
            return false;
            //"<Xuk xsi:noNamespaceSchemaLocation="http://www.daisy.org/urakawa/xuk/1.0 xuk.xsd" "
        }

        public void UpgradeProject()
        {
            string referencePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "EmptyProject.obi");
            XmlDocument sourceXmlDoc = new XmlDocument();
            sourceXmlDoc.Load(m_Source);

            XmlDocument destXmlDoc = new XmlDocument();
            destXmlDoc.Load(referencePath);

            int progressPercentage = 1;
            if (ProgressChanged != null) ProgressChanged(this, new System.ComponentModel.ProgressChangedEventArgs(progressPercentage, ""));
            if (RequestCancellation)
            {
                sourceXmlDoc = null;
                destXmlDoc = null;
                return;
            }

//start with adding data providers
            XmlNode oldDataProviders = sourceXmlDoc.GetElementsByTagName("mDataProviders")[0];
            XmlNode newDataProviders = destXmlDoc.GetElementsByTagName("DataProviders")[0];
            foreach (XmlNode dataProviderItem in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(oldDataProviders, true, "mDataProviderItem", oldDataProviders.NamespaceURI, false))
            {
                XmlNode newFileDataProvider = destXmlDoc.CreateElement("FileDataProvider", newDataProviders.NamespaceURI);
                newDataProviders.AppendChild(newFileDataProvider);
                XmlDocumentHelper.CreateAppendXmlAttribute(destXmlDoc, newFileDataProvider, "Uid", dataProviderItem.Attributes.GetNamedItem("uid").Value);
                XmlDocumentHelper.CreateAppendXmlAttribute(destXmlDoc, newFileDataProvider, "MimeType", "audio/x-wav");
                XmlDocumentHelper.CreateAppendXmlAttribute(destXmlDoc, newFileDataProvider, "DataFileRelativePath", dataProviderItem.FirstChild.Attributes.GetNamedItem("dataFileRelativePath").Value);
            }
            progressPercentage = 10;
            if (ProgressChanged != null) ProgressChanged(this, new System.ComponentModel.ProgressChangedEventArgs(progressPercentage, ""));

            if (RequestCancellation)
            {
                sourceXmlDoc = null;
                destXmlDoc = null;
                return;
            }

            // update audio format 
            XmlNode oldMediaDataManager = sourceXmlDoc.GetElementsByTagName("mMediaDataManager")[0];
            XmlNode oldPCMInfo = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(oldMediaDataManager, true, "PCMFormatInfo", oldMediaDataManager.NamespaceURI);

            XmlNode newMediaDataManager = destXmlDoc.GetElementsByTagName("MediaDataManager")[0];
            XmlNode newPCMInfo = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(newMediaDataManager, true, "PCMFormatInfo", newMediaDataManager.NamespaceURI);
            newPCMInfo.Attributes.GetNamedItem("NumberOfChannels").Value = oldPCMInfo.Attributes.GetNamedItem("numberOfChannels").Value;
            newPCMInfo.Attributes.GetNamedItem("SampleRate").Value = oldPCMInfo.Attributes.GetNamedItem("sampleRate").Value;
            
            // add AudioMediaDatas Next
            XmlNode oldAudioMediaDatas = sourceXmlDoc.GetElementsByTagName("mMediaData")[0];
            XmlNode newAudioMediaDatas = destXmlDoc.GetElementsByTagName("MediaDatas")[0];

            float audioMediaLoopPercentage = 0 ;

            foreach (XmlNode oldMediaDataItem in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(oldAudioMediaDatas, true, "mMediaDataItem", oldAudioMediaDatas.NamespaceURI, false))
            {
                audioMediaLoopPercentage += 0.05f;
                if (audioMediaLoopPercentage >= progressPercentage + 1 && progressPercentage <= 50)
                {
                    progressPercentage = (int)audioMediaLoopPercentage;
                    if (ProgressChanged != null) ProgressChanged(this, new System.ComponentModel.ProgressChangedEventArgs(progressPercentage, ""));
                }
                XmlNode newAudioMediaItem = destXmlDoc.CreateElement("WavAudioMediaData", newAudioMediaDatas.NamespaceURI);
                newAudioMediaDatas.AppendChild(newAudioMediaItem);
                XmlDocumentHelper.CreateAppendXmlAttribute(destXmlDoc, newAudioMediaItem, "Uid", oldMediaDataItem.Attributes.GetNamedItem("uid").Value);
                //XmlDocumentHelper.CreateAppendXmlAttribute(destXmlDoc, newItem, "MimeType", "audio/x-wav");
                //XmlDocumentHelper.CreateAppendXmlAttribute(destXmlDoc, newItem, "DataFileRelativePath", item.Attributes.GetNamedItem("dataFileRelativePath").Value);

                XmlNode wavClips = destXmlDoc.CreateElement("WavClips", newAudioMediaItem.NamespaceURI);
                newAudioMediaItem.AppendChild(wavClips);

                foreach (XmlNode wavItem in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(oldMediaDataItem, true, "WavClip", oldAudioMediaDatas.NamespaceURI, false))
                {
                    if (RequestCancellation)
                    {
                        sourceXmlDoc = null;
                        destXmlDoc = null;
                        return;
                    }
                    XmlNode newWavClip = destXmlDoc.CreateElement("WavClip", newAudioMediaItem.NamespaceURI);
                    wavClips.AppendChild(newWavClip);
                    // add attributes of wavClip
                    foreach (XmlAttribute attr in wavItem.Attributes)
                    {
                        string attrName = attr.Name;
                        string firstString = attrName.Substring (0,1) ;
                        attrName = firstString.ToUpper() + attrName.Substring(1, attrName.Length - 1);
                        attrName.ToUpperInvariant();
                        Console.WriteLine("name " + attr.Value);
                        XmlDocumentHelper.CreateAppendXmlAttribute(destXmlDoc, newWavClip, attrName, attr.Value);
                    }
                }

                
            }
            progressPercentage =55 ;
            if (ProgressChanged != null) ProgressChanged(this, new System.ComponentModel.ProgressChangedEventArgs(progressPercentage, ""));
            if (RequestCancellation)
            {
                sourceXmlDoc = null;
                destXmlDoc = null;
                return;
            }

            //change the root uri to null
            XmlNode newPresentations = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(destXmlDoc.DocumentElement, true, "Presentations", null);
            XmlNode newObiPresentation = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(destXmlDoc.DocumentElement, true, "ObiPresentation", null);
            newObiPresentation.Attributes.GetNamedItem("RootUri").Value = "" ;

            UpdateRegisteredTypes(destXmlDoc);
            progressPercentage = 60 ;
            if (ProgressChanged != null) ProgressChanged(this, new System.ComponentModel.ProgressChangedEventArgs(progressPercentage, ""));
            if (RequestCancellation)
            {
                sourceXmlDoc = null;
                destXmlDoc = null;
                return;
            }
            ImportMetadatas(sourceXmlDoc, destXmlDoc);

            progressPercentage = 70 ;
            if (ProgressChanged != null) ProgressChanged(this, new System.ComponentModel.ProgressChangedEventArgs(progressPercentage, ""));
            if (RequestCancellation)
            {
                sourceXmlDoc = null;
                destXmlDoc = null;
                return;
            }
            //import the section and phrase tree from the rootnode
            XmlNode oldRootNode  = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(sourceXmlDoc.DocumentElement, true, "mRootNode", null);
            XmlNode oldRootChildrenContainer = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(oldRootNode, true, "mChildren", oldRootNode.NamespaceURI);

            XmlNode newRootNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(destXmlDoc.DocumentElement, true, "RootNode", null);
            XmlNode newRootChildrenContainer = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(newRootNode, true, "Children", newRootNode.NamespaceURI);
            XmlNode namespaceNode= XmlDocumentHelper.GetFirstChildElementOrSelfWithName(newRootNode, true, "root", null);

            if (RequestCancellation)
            {
                sourceXmlDoc = null;
                destXmlDoc = null;
                return;
            }

            ParseAndCreateObiTree(oldRootChildrenContainer, newRootChildrenContainer, namespaceNode, newRootNode);

            if (RequestCancellation)
            {
                sourceXmlDoc = null;
                destXmlDoc = null;
                return;
            }
            progressPercentage = 90 ;
            if (ProgressChanged != null) ProgressChanged(this, new System.ComponentModel.ProgressChangedEventArgs(progressPercentage, ""));
WriteXmlDocumentToFile (destXmlDoc, m_Destination ) ;
sourceXmlDoc = null;
destXmlDoc = null;
RenameProjectFilesAfterOperation();

            progressPercentage = 100 ;
            if (ProgressChanged != null) ProgressChanged(this, new System.ComponentModel.ProgressChangedEventArgs(progressPercentage, ""));
        }

        private void ImportMetadatas(XmlDocument sourceXmlDoc, XmlDocument destXmlDoc)
        {
            XmlNode OldMetadataParent = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(sourceXmlDoc.DocumentElement, true, "mMetadata", null);
            XmlNode newMetadatas = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(destXmlDoc.DocumentElement, true, "Metadatas", null);

            //foreach (XmlNode childToRemove in newMetadatas.ChildNodes) newMetadatas.RemoveChild(childToRemove);
            Dictionary<string, XmlNode> existingMetadataAttributeNode = new Dictionary<string, XmlNode>();
            foreach (XmlNode newMetadataAttributeItem in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(newMetadatas, true, "MetadataAttribute", null, false)) 
                existingMetadataAttributeNode.Add(newMetadataAttributeItem.Attributes.GetNamedItem("Name").Value, newMetadataAttributeItem);

            foreach (XmlNode metadataItem in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(OldMetadataParent, true, "Metadata", OldMetadataParent.NamespaceURI, false))
            {
                if (existingMetadataAttributeNode.ContainsKey(metadataItem.Attributes.GetNamedItem("name").Value))
                {
                    existingMetadataAttributeNode[metadataItem.Attributes.GetNamedItem("name").Value].Attributes.GetNamedItem("Value").Value = metadataItem.Attributes.GetNamedItem("content").Value;
                    //System.Windows.Forms.MessageBox.Show(metadataItem.Attributes.GetNamedItem("name").Value);
                }
                else
                {
                    // create new metadata node in dest document
                    XmlNode newMetadata = destXmlDoc.CreateElement("Metadata", newMetadatas.NamespaceURI);
                    newMetadatas.AppendChild(newMetadata);
                    XmlNode newMetadataAttribute = destXmlDoc.CreateElement("MetadataAttribute", newMetadata.NamespaceURI);
                    newMetadata.AppendChild(newMetadataAttribute);

                    XmlDocumentHelper.CreateAppendXmlAttribute(destXmlDoc, newMetadataAttribute, "Name", metadataItem.Attributes.GetNamedItem("name").Value);
                    XmlDocumentHelper.CreateAppendXmlAttribute(destXmlDoc, newMetadataAttribute, "Value", metadataItem.Attributes.GetNamedItem("content") != null?metadataItem.Attributes.GetNamedItem("content").Value: "NA" );
                }
            }

        }

        private void UpdateRegisteredTypes(XmlDocument destXmlDoc)
        {
            XmlNode treeNodeFactory = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(destXmlDoc.DocumentElement, true, "TreeNodeFactory", null);
            XmlNode refRootNode = null;
            foreach (XmlNode typeNode in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(treeNodeFactory, true,"Type",null,false ) )
            {
                if (typeNode.Attributes.GetNamedItem("XukLocalName").Value == "root")
                {
                    refRootNode = typeNode;
                    break;
                }
            }

            //create section type
            XmlNode sectionNode = refRootNode.CloneNode(true);
            sectionNode.Attributes.GetNamedItem("XukLocalName").Value = "section";
            sectionNode.Attributes.GetNamedItem("FullName").Value = "Obi.SectionNode";
            refRootNode.ParentNode.AppendChild(sectionNode);

            //create empty type
            XmlNode emptyNode = refRootNode.CloneNode(true);
            emptyNode.Attributes.GetNamedItem("XukLocalName").Value = "empty";
            emptyNode.Attributes.GetNamedItem("FullName").Value = "Obi.EmptyNode";
            refRootNode.ParentNode.AppendChild(emptyNode);

            //Create type for phrase
            XmlNode phraseNode = refRootNode.CloneNode(true);
            phraseNode.Attributes.GetNamedItem("XukLocalName").Value = "phrase";
            
            XmlDocumentHelper.CreateAppendXmlAttribute(destXmlDoc, phraseNode, "BaseXukLocalName", "empty");
            XmlDocumentHelper.CreateAppendXmlAttribute(destXmlDoc, phraseNode, "BaseXukNamespaceUri", "http://www.daisy.org/urakawa/obi");
            phraseNode.Attributes.GetNamedItem("FullName").Value = "Obi.PhraseNode";
            refRootNode.ParentNode.AppendChild(phraseNode);

        }

        private void ParseAndCreateObiTree(XmlNode oldNode, XmlNode newNode, XmlNode nameSpaceNode, XmlNode urakawaNodeNamespace)
        {
            if (oldNode.ChildNodes.Count > 0)
            {
                foreach (XmlNode oldChild in oldNode.ChildNodes)
                {//1
                    if (RequestCancellation) return;
                    if (oldChild.Name== "mChannelMapping"
                        && oldChild.Attributes.GetNamedItem("channel") != null
                        && oldChild.Attributes.GetNamedItem("channel").Value == "CHID0002")
                    {
                        continue;
                    }
                    if (oldChild is XmlElement )
                    {//2
                        XmlNode newChild = null;
                        
                            newChild = newNode.OwnerDocument.CreateElement(GetNewElementName(oldChild.Name), 
                                oldChild.LocalName=="section" || oldChild.LocalName== "phrase" || oldChild.LocalName== "empty"?  nameSpaceNode.NamespaceURI: urakawaNodeNamespace.NamespaceURI);
                            newNode.AppendChild(newChild);
                        

                        foreach (XmlAttribute attr in oldChild.Attributes)
                        {//3
                            //if (attr.Name == "xmlns" && attr.Value == "http://www.daisy.org/urakawa/xuk/1.0") continue;
                            if (attr.Name == "xmlns" ) continue;
                            //{//4
                            if (oldChild.LocalName == "mChannelMapping" && attr.Name == "channel")
                            {//5
                                XmlDocumentHelper.CreateAppendXmlAttribute(newNode.OwnerDocument, newChild, "Channel", attr.Value == "CHID0000" ? "CH00001" : "CH00000");
                            }//-5
                            else
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(newNode.OwnerDocument, newChild, GetNewAttributeName(attr.Name), attr.Value);
                            }
                            //}//-4
                        }//-3
                        
                        ParseAndCreateObiTree(oldChild, newChild, nameSpaceNode, urakawaNodeNamespace);
                    }//-2
                    else if (oldChild is XmlText)
                    {//2
                        XmlText newChild = newNode.OwnerDocument.CreateTextNode(oldChild.InnerText);
                        newNode.AppendChild(newChild);
                    }//-2
                }//-1
            }
            else
            {
                return;
            }

        }

        private void RenameProjectFilesAfterOperation()
        {
            string oldProjectPath = Path.Combine(Path.GetDirectoryName(m_Source), "Old_" + Path.GetFileName(m_Source));
            File.Move(m_Source, oldProjectPath);
            // rename new file to project file name
            File.Move(m_Destination, m_Source);
        }

        public static void WriteXmlDocumentToFile(XmlDocument xmlDoc, string path)
        {
            XmlTextWriter writer = null;
            try
            {
                writer = new XmlTextWriter(path, null);
                writer.Formatting = Formatting.Indented;
                xmlDoc.Save(writer);
            }
            finally
            {
                writer.Close();
                writer = null;
            }
        }

    }
}
