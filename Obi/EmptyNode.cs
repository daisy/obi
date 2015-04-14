using System;
using System.Collections.Generic ;

using urakawa.command;
using urakawa.core;
using urakawa.property.xml;


namespace Obi
{
    /// <summary>
    /// The empty node class is the base class for content nodes.
    /// It doesn't have any actual content yet and will either become a PhraseNode or a ContainerNode.
    /// It can have a role and be marked as TODO (attributes of the node.)
    /// </summary>
    [XukNameUglyPrettyAttribute("empty", "empty")]
    public class EmptyNode: ObiNode
    {
        //public new static string XukString = "empty";
        //public override string GetTypeNameFormatted()
        //{
        //    return XukString;
        //}

        public override string XmlPropertyString { get { return "p";} } 
        private Role mRole;              // this node's kind
        private string mCustomRole;      // custom role name
        private PageNumber mPageNumber;  // page number
        private bool mTODO;              // marked as TODO

        private static readonly string XUK_ATTR_NAME_ROLE = "kind";           // name of the role attribute
        private static readonly string XUK_ATTR_NAME_CUSTOM = "custom";       // name of the custom attribute
        private static readonly string XUK_ATTR_NAME_PAGE = "page";           // name of the page attribute
        private static readonly string XUK_ATTR_NAME_PAGE_KIND = "pageKind";  // name of the pageKind attribute
        private static readonly string XUK_ATTR_NAME_PAGE_TEXT = "pageText";  // name of the pageText attribute
        private static readonly string XUK_ATTR_NAME_TODO = "TODO";           // name of the TODO attribute
        public static readonly string Footnote = "footnote";
        public static readonly string Sidebar = "sidebar";
        public static readonly string ProducerNote = "prodnote";
        public static readonly string Note = "note";
        public static readonly string EndNote = "endnote";
        public static readonly string Annotation = "annotation";
        private EmptyNode m_AssociatedNode = null;                            //@AssociateNode  
        private static readonly string XUK_ATTR_NAME_AssociateNode = "ANode"; //attribute for associate node location
        private string m_AssociatedNodeLocation;                              //@AssociateNode
        

        /// <summary>
        /// Different roles for content nodes.
        /// Plain is the default.
        /// Page is a page node with a page number.
        /// Heading is the audio heading for a section.
        /// Silence is a silence node for phrase detection.
        /// Custom is a node with a custom class (e.g. sidebar, etc.)
        /// </summary>
        public enum Role { Plain, Page, Heading, Silence, Anchor, Custom };   //@AssociateNode
         
        public static readonly LocalizedRole LOCALIZED_PLAIN = new LocalizedRole(Role.Plain);
        public static readonly LocalizedRole LOCALIZED_PAGE = new LocalizedRole(Role.Page);
        public static readonly LocalizedRole LOCALIZED_HEADING = new LocalizedRole(Role.Heading);
        public static readonly LocalizedRole LOCALIZED_SILENCE = new LocalizedRole(Role.Silence);
        public static readonly LocalizedRole LOCALIZED_CUSTOM = new LocalizedRole(Role.Custom);
        public static readonly LocalizedRole LOCALIZED_ANCHOR = new LocalizedRole(Role.Anchor);  //@AssociateNode
        
        public override string ToString() { return BaseString(); }

        public virtual string BaseString(double durationMs)
        {
            return String.Format(Localizer.Message("phrase_to_string"),
                TODO ? Localizer.Message ("phrase_short_TODO") : "",
                Used ? "" : Localizer.Message("unused"),
                IsRooted ? Index + 1 : 0,
                IsRooted ? ParentAs<ObiNode>().PhraseChildCount : 0,
                durationMs == 0.0 ? Localizer.Message("empty") : Program.FormatDuration_Long(durationMs),
                mRole == Role.Custom ? String.Format(Localizer.Message("phrase_extra_custom"), mCustomRole) :
                mRole == Role.Page ? String.Format(Localizer.Message("phrase_extra_page"), mPageNumber != null ? mPageNumber.ToString() : "" ) :
                mRole == Role.Anchor && this.AssociatedNode == null? Localizer.Message("phrase_extra_" + mRole.ToString()) + "= ?":
                    Localizer.Message("phrase_extra_" + mRole.ToString()));
        }

        public virtual string BaseString() { return BaseString(0.0); }

        public virtual string BaseStringShort(double durationMs)
        {
            return String.Format(Localizer.Message("phrase_short_to_string"),
                TODO ? Localizer.Message("phrase_short_TODO") : "",
                mRole == Role.Custom ? String.Format(Localizer.Message("phrase_short_custom"), mCustomRole) :
                mRole == Role.Page ? String.Format(Localizer.Message("phrase_short_page"), mPageNumber != null ? mPageNumber.ToString() : "") :
                mRole == Role.Anchor && this.AssociatedNode == null ? Localizer.Message("phrase_short_" + mRole.ToString()) + "= ?" :
                    Localizer.Message("phrase_short_" + mRole.ToString()),
                durationMs == 0.0 ? Localizer.Message("empty") : Program.FormatDuration_Smart(durationMs));
        }

        public virtual string BaseStringShort() { return BaseStringShort(0.0); }

        public override double Duration { get { return 0.0; } }

        private static List<string> m_SkippableNamesList;
        public static List<string> SkippableNamesList
        {
            get
            {
                if (m_SkippableNamesList == null)
                {
                    m_SkippableNamesList = new List<string>();
                    m_SkippableNamesList.Add(EmptyNode.Annotation);
                    m_SkippableNamesList.Add(EmptyNode.EndNote);
                    m_SkippableNamesList.Add(EmptyNode.Footnote);
                    m_SkippableNamesList.Add(EmptyNode.Note);
                    m_SkippableNamesList.Add(EmptyNode.ProducerNote);
                    m_SkippableNamesList.Add(EmptyNode.Sidebar);
                }
                return m_SkippableNamesList;
            }
        }

private static Dictionary <string,string> m_SkippableLocalizedNameMap = null ;
        /// <summary>
        /// <maps the localized names of skippable elements to actual DAISY elements name
        /// </summary>
        public static Dictionary<string, string> SkippableLocalizedNameMap
        {
            get
            {
                if (m_SkippableLocalizedNameMap == null
                    || !m_SkippableLocalizedNameMap.ContainsValue(Localizer.Message(Footnote)))
                {   
                    m_SkippableLocalizedNameMap = new Dictionary<string, string>();
                    foreach (string s in SkippableNamesList)
                    {
                        m_SkippableLocalizedNameMap.Add(Localizer.Message(s), s);
                    }
                }
                return m_SkippableLocalizedNameMap;
            }
        }

        /// <summary>
        /// This event is sent when we change the kind or custom class of a node.
        /// </summary>
        public event ChangedRoleEventHandler ChangedRole;
        public delegate void ChangedRoleEventHandler(object sender, ChangedRoleEventArgs e);

        /// <summary>
        /// This event is sent when the page number changes on a node (for a node which previously had a page number.)
        /// </summary>
        public event NodeEventHandler<EmptyNode> ChangedPageNumber;

        /// <summary>
        /// This event is sent when the TODO status of the node is toggled.
        /// </summary>
        public event NodeEventHandler<EmptyNode> ChangedTODOStatus;


        /// <summary>
        /// Create a new empty node of a given kind in a presentation.
        /// </summary>
        public EmptyNode(Role role, string customRole)
        {
            mRole = role;
            mCustomRole = customRole;
            mPageNumber = null;
            m_AssociatedNodeLocation = null;   //@AssociateNode
        }

        /// <summary>
        /// Create a plain empty node in a presentation.
        /// </summary>
        public EmptyNode(): this(Role.Plain, null) {}

        /// <summary>
        /// Create an empty node of a pre-defined kind a presentation.
        /// </summary>
        public EmptyNode(Role role) : this(role, null) { }

        /// <summary>
        /// Create an empty node with a custom class in a presentation.
        /// </summary>
        public EmptyNode(string customRole) : this(Role.Custom, customRole) { }

        
        /// <summary>
        /// Copy all attributes of node in parameter to another.
        /// </summary>
        public void CopyAttributes(EmptyNode node)
        {
            mRole = node.mRole;
            mCustomRole = node.mCustomRole;
            mPageNumber = node.mPageNumber != null ? node.mPageNumber.Clone() : null;
            mTODO = node.TODO;
        }

        /// <summary>
        /// Copy the node.
        /// </summary>
        protected override TreeNode CopyProtected(bool deep, bool inclProperties)
        {
            EmptyNode copy = (EmptyNode)base.CopyProtected(deep, inclProperties);
            copy.CopyAttributes(this);
            return copy;
        }

        /// <summary>
        /// Get or set the custom role name, if the role is "Custom." (When setting, set to custom as well.)
        /// </summary>
        public string CustomRole
        {
            get { return mCustomRole; }
            set { SetRole(Role.Custom, value); }
        }

        public EmptyNode AssociatedNode   //@AssociateNode
        {
            get 
            {
                if (m_AssociatedNode == null && !string.IsNullOrEmpty(m_AssociatedNodeLocation))
                {
                    string [] locationArray = m_AssociatedNodeLocation.Split('_') ;
                    if (locationArray.Length > 0)
                    {//1
                        TreeNode iterationNode = this.Root;
                        for (int i = locationArray.Length - 1; i >= 0; i--)
                        {//2
                            int childIndex = -1;
                            int.TryParse(locationArray[i], out childIndex);

                            iterationNode =(childIndex >= 0 &&  childIndex< iterationNode.Children.Count)?  iterationNode.Children.Get(childIndex): null;
                            if (iterationNode == null) break;
                        }//-2
                        m_AssociatedNode =iterationNode != null? (EmptyNode)iterationNode: null;
                    }//-1
                    m_AssociatedNodeLocation = null;
                }
                if (Role_ != Role.Anchor) m_AssociatedNode = null;
                return m_AssociatedNode != null &&  m_AssociatedNode.IsRooted? m_AssociatedNode:null; 
            }
            set 
            { if(this.Role_ == Role.Anchor)
                m_AssociatedNode =  value;
                ChangedRoleEventArgs args = new ChangedRoleEventArgs(this, mRole, mCustomRole);
                if (ChangedRole != null) ChangedRole(this, args);
            }
        }

        // to do: call this function in getter of AssociatedNode property
        public void AssignAssociatedNodeByParsingLocationString(int firstDecemdentOffset)
        {
            if (m_AssociatedNode == null && !string.IsNullOrEmpty(m_AssociatedNodeLocation))
            {
                string[] locationArray = m_AssociatedNodeLocation.Split('_');
                if (locationArray.Length > 0)
                {//1
                    TreeNode iterationNode = this.Root;
                    for (int i = locationArray.Length - 1; i >= 0; i--)
                    {//2
                        int childIndex = -1;
                        int.TryParse(locationArray[i], out childIndex);
                        if (childIndex > -1 && i == locationArray.Length - 1) childIndex = childIndex + firstDecemdentOffset;

                        iterationNode = (childIndex >= 0 && childIndex < iterationNode.Children.Count) ? iterationNode.Children.Get(childIndex) : null;
                        if (iterationNode == null) break;
                    }//-2
                    m_AssociatedNode = iterationNode != null ? (EmptyNode)iterationNode : null;
                }//-1
            }
        }

        //public void UpdateAssociatedNodeString(string locationString)
        //{
            //m_AssociatedNodeLocation = locationString;
        //}


        /// <summary>
        /// Get or set the TODO status of the phrase.
        /// </summary>
        public bool TODO
        {
            get { return mTODO; }
            set { mTODO = value;  }
        }

        /// <summary>
        /// Has no audio so is never a used phrase.
        /// </summary>
        public override PhraseNode FirstUsedPhrase { get { return null; } }

        /// <summary>
        /// Get or set the role. For custom, use CustomRole directly.
        /// </summary>
        public Role Role_
        {
            get { return mRole; }
            set { SetRole(value, null); }
        }

        /// <summary>
        /// Get or set the page number for this node.
        /// Will discard previous role if it was not already a page node.
        /// </summary>
        public PageNumber PageNumber
        {
            get { return mPageNumber; }
            set
            {
                mPageNumber = value;
                if (mRole == Role.Page)            
                {
                    if (ChangedPageNumber != null) ChangedPageNumber(this, new NodeEventArgs<EmptyNode>(this));
                }
                else
                {
                    Role_ = Role.Page;
                }
            }
        }

        /// <summary>
        /// Renumber this node and all following pages of the same kind starting from this number.
        /// </summary>
        public override CompositeCommand RenumberCommand(ProjectView.ProjectView view, PageNumber from)
        {
            if (mRole == Role.Page && mPageNumber.Kind == from.Kind)
            {
                CompositeCommand k = base.RenumberCommand(view, from.NextPageNumber());
                if (k == null)
                {
                    k = Presentation.CommandFactory.CreateCompositeCommand();
                    k.ShortDescription = string.Format(Localizer.Message("renumber_pages"),
                        Localizer.Message(string.Format("{0}_pages", from.Kind.ToString())));
                }
                k.ChildCommands.Insert(k.ChildCommands.Count, new Commands.Node.SetPageNumber(view, this, from));
                return k;
            }
            else
            {
                return base.RenumberCommand(view, from);
            }
        }

        /// <summary>
        /// Set the kind or custom class of the node.
        /// </summary>
        public void SetRole(Role role, string customRole)
        {
            if (mRole != role || mCustomRole != customRole)
            {
                ChangedRoleEventArgs args = new ChangedRoleEventArgs(this, mRole, mCustomRole);
                if (mRole == EmptyNode.Role.Heading && this.IsRooted) AncestorAs<SectionNode> ().UnsetHeading ( this as PhraseNode ); //@singleSection: allow clearing of heading role without a parent
                if (mRole == Role.Anchor && mRole != Role.Anchor) m_AssociatedNode = null;
                mRole = role;
                mCustomRole = customRole;
                if (role != Role.Page) mPageNumber = null;
                if (role != Role.Custom) mCustomRole = null;
                if (role == Role.Heading && !AncestorAs<SectionNode>().DidSetHeading(this))
                {
                    mRole = args.PreviousRole;
                    mCustomRole = args.PreviousCustomRole;
                    return;
                }
                if (ChangedRole != null) ChangedRole(this, args);
            }
        }

        /// <summary>
        /// Set the TODO status of this node and send an event on change.
        /// </summary>
        /// <param name="todo"></param>
        public void SetTODO(bool todo)
        {
            if (todo != TODO)
            {
                TODO = todo;
                if (ChangedTODOStatus != null) ChangedTODOStatus(this, new NodeEventArgs<EmptyNode>(this));
            }
        }

        protected override TreeNode ExportProtected(urakawa.Presentation destPres)
        {
            UpdateAssociatedNodeLocationString();
            EmptyNode exportedNode = (EmptyNode) base.ExportProtected(destPres);
            exportedNode.CopyAttributes(this);
            exportedNode.m_AssociatedNodeLocation = this.m_AssociatedNodeLocation;
            //exportedNode.AssociatedNode = this.AssociatedNode;
            return exportedNode;
        }

        public override void Insert(ObiNode node, int index) { throw new Exception("Empty nodes have no children."); }
        public override SectionNode SectionChild(int index) { throw new Exception("Empty nodes have no children."); }
        public override int SectionChildCount { get { return 0; } }
        public override EmptyNode PhraseChild(int index) { throw new Exception("Emtpy nodes have no children."); }
        public override int PhraseChildCount { get { return 0; } }
        public override EmptyNode LastUsedPhrase { get { throw new Exception("Empty nodes have no children."); } }


        protected override void XukInNodeProperties ()
        {
            base.XukInNodeProperties();
            if (Role_ != Role.Plain) return;
            try
            {
                
                XmlProperty xmlProp = this.GetProperty<XmlProperty>();
                if (xmlProp != null)
                {
                    
                    urakawa.property.xml.XmlAttribute attrRole = xmlProp.GetAttribute(XUK_ATTR_NAME_ROLE);

                    if (attrRole != null)
                    {
                        string role = attrRole.Value;
                        if (role != null) mRole = role == Role.Custom.ToString() ? Role.Custom :
                                                  role == Role.Heading.ToString() ? Role.Heading :
                                                  role == Role.Page.ToString() ? Role.Page :
                                                  role == Role.Silence.ToString() ? Role.Silence : Role.Plain;
                        if (role != null && role != mRole.ToString()) throw new Exception("Unknown kind: " + role);
                        if (mRole == Role.Custom) mCustomRole = xmlProp.GetAttribute(XUK_ATTR_NAME_CUSTOM).Value;
                        //System.Windows.Forms.MessageBox.Show(mRole.ToString());
                        if (mRole == Role.Heading)
                        {
                            if (!AncestorAs<SectionNode>().DidSetHeading(this)) mRole = Role.Plain;
                        }
                        else if (mRole == Role.Page)
                        {
                            string pageKind = xmlProp.GetAttribute(XUK_ATTR_NAME_PAGE_KIND).Value;
                            if (pageKind != null)
                            {
                                string page = xmlProp.GetAttribute(XUK_ATTR_NAME_PAGE).Value;
                                int number = SafeParsePageNumber(page);
                                if (pageKind == "Front")
                                {
                                    if (number == 0) throw new Exception(string.Format("Invalid front page number \"{0}\".", page));
                                    mPageNumber = new PageNumber(number, PageKind.Front);
                                }
                                else if (pageKind == "Normal")
                                {
                                    if (number == 0) throw new Exception(string.Format("Invalid page number \"{0}\".", page));
                                    mPageNumber = new PageNumber(number);
                                }
                                else if (pageKind == "Special")
                                {
                                    if (page == null || page == "") throw new Exception("Invalid empty special page number.");
                                    mPageNumber = new PageNumber(page);
                                }
                                else
                                {
                                    throw new Exception(string.Format("Invalid page kind \"{0}\".", pageKind));
                                }
                            }
                        }
                        if (mRole != Role.Custom && mCustomRole != null)
                        {
                            throw new Exception("Extraneous `custom' attribute.");
                        }
                        else if (mRole == Role.Custom && mCustomRole == null)
                        {
                            throw new Exception("Missing `custom' attribute.");
                        }
                        // add it to the presentation
                        ((ObiPresentation)Presentation).AddCustomClass(mCustomRole, this);

                        string todo = xmlProp.GetAttribute(XUK_ATTR_NAME_TODO).Value;
                        if (todo != null) mTODO = todo == "True";
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this.ToString() + " " + ex.ToString());
            }
        }

        protected override void XukInAttributes(System.Xml.XmlReader source)
        {
            if (source.AttributeCount > 1 ||  ( source.AttributeCount == 1 && source.GetAttribute("xmlns") == null))
            {   
                string role = source.GetAttribute(XUK_ATTR_NAME_ROLE);
                if (role != null) mRole = role == Role.Custom.ToString() ? Role.Custom :
                                          role == Role.Heading.ToString() ? Role.Heading :
                                          role == Role.Page.ToString() ? Role.Page :
                                          role == Role.Silence.ToString() ? Role.Silence :
                        role == Role.Anchor.ToString() ? Role.Anchor : Role.Plain;  //@AssociateNode
                if (role != null && role != mRole.ToString()) throw new Exception("Unknown kind: " + role);
                mCustomRole = source.GetAttribute(XUK_ATTR_NAME_CUSTOM);
                if (mRole == Role.Heading)
                {
                    if (!AncestorAs<SectionNode>().DidSetHeading(this)) mRole = Role.Plain;
                }
                else if (mRole == Role.Page)
                {
                    string pageKind = source.GetAttribute(XUK_ATTR_NAME_PAGE_KIND);
                    if (pageKind != null)
                    {
                        string page = source.GetAttribute(XUK_ATTR_NAME_PAGE);
                        int number = SafeParsePageNumber(page);
                        if (pageKind == "Front")
                        {
                            if (number == 0) throw new Exception(string.Format("Invalid front page number \"{0}\".", page));
                            mPageNumber = new PageNumber(number, PageKind.Front);
                        }
                        else if (pageKind == "Normal")
                        {
                            if (number == 0) throw new Exception(string.Format("Invalid page number \"{0}\".", page));
                            mPageNumber = new PageNumber(number);
                        }
                        else if (pageKind == "Special")
                        {
                            if (page == null || page == "") throw new Exception("Invalid empty special page number.");
                            mPageNumber = new PageNumber(page);
                        }
                        else
                        {
                            throw new Exception(string.Format("Invalid page kind \"{0}\".", pageKind));
                        }
                    }
                }
                if (mRole != Role.Custom && mCustomRole != null)
                {
                    throw new Exception("Extraneous `custom' attribute.");
                }
                else if (mRole == Role.Custom && mCustomRole == null)
                {
                    throw new Exception("Missing `custom' attribute.");
                }
                // add it to the presentation
                ((ObiPresentation)Presentation).AddCustomClass(mCustomRole, this);

                string todo = source.GetAttribute(XUK_ATTR_NAME_TODO);
                if (todo != null) mTODO = todo == "True";
                m_AssociatedNodeLocation = source.GetAttribute(XUK_ATTR_NAME_AssociateNode);  //@AssociateNode

            }
            if (Role_ == Role.Anchor) ((ObiPresentation)Presentation).ListOfAnchorNodes_Add(this);
            base.XukInAttributes(source);
        }

        
        protected override void UpdateXmlProperties()
        {
            XmlProperty xmlProp = ObiNodeGetOrCreateXmlProperty();
            UpdateAttributesInXmlProperty(xmlProp, XUK_ATTR_NAME_ROLE, this.Role_.ToString());

            if (this.Role_ == Role.Custom)
            {
                UpdateAttributesInXmlProperty(xmlProp, XUK_ATTR_NAME_CUSTOM, mCustomRole);
            }

            if (this.Role_ == Role.Page)
            {
                UpdateAttributesInXmlProperty(xmlProp, XUK_ATTR_NAME_PAGE, mPageNumber.ArabicNumberOrLabel);
                UpdateAttributesInXmlProperty(xmlProp, XUK_ATTR_NAME_PAGE_KIND, mPageNumber.Kind.ToString());
                UpdateAttributesInXmlProperty(xmlProp, XUK_ATTR_NAME_PAGE_TEXT, mPageNumber.Unquoted);
            }

            UpdateAttributesInXmlProperty(xmlProp, XUK_ATTR_NAME_TODO, TODO.ToString());
            base.UpdateXmlProperties();
        }

        
        protected override void XukOutAttributes(System.Xml.XmlWriter wr, Uri baseUri)
        {
            //Presentation.UseXukFormat = true;
            if (!ObiPresentation.UseXukFormat)
            {
            
                if (mRole != Role.Plain) wr.WriteAttributeString(XUK_ATTR_NAME_ROLE, mRole.ToString());
                if (mRole == Role.Custom) wr.WriteAttributeString(XUK_ATTR_NAME_CUSTOM, mCustomRole);
                if (mRole == Role.Page)
                {
                    wr.WriteAttributeString(XUK_ATTR_NAME_PAGE, mPageNumber.ArabicNumberOrLabel);
                    wr.WriteAttributeString(XUK_ATTR_NAME_PAGE_KIND, mPageNumber.Kind.ToString());
                    wr.WriteAttributeString(XUK_ATTR_NAME_PAGE_TEXT, mPageNumber.Unquoted);
                }
                if (mTODO) wr.WriteAttributeString(XUK_ATTR_NAME_TODO, "True");
                if (AssociatedNode != null && AssociatedNode.IsRooted)       //@AssociateNode
                {
                    UpdateAssociatedNodeLocationString();
                    
                    wr.WriteAttributeString(XUK_ATTR_NAME_AssociateNode, m_AssociatedNodeLocation);
                }
            }
            base.XukOutAttributes(wr, baseUri);
        }

        private void UpdateAssociatedNodeLocationString()
        {
            if (AssociatedNode != null && AssociatedNode.IsRooted)       //@AssociateNode
            {
                ObiNode iterationNode = AssociatedNode;
                m_AssociatedNodeLocation = "";
                while (iterationNode != this.Root)
                {
                    if (AssociatedNode != iterationNode) m_AssociatedNodeLocation += "_";
                    m_AssociatedNodeLocation += iterationNode.Parent.Children.IndexOf(iterationNode).ToString();
                    iterationNode = iterationNode.ParentAs<ObiNode>();
                }

            }
        }


        /// <summary>
        /// Attempt to parse a page number from a string; return 0 on negative/non-number values.
        /// </summary>
        public static int SafeParsePageNumber(string str)
        {
            int page;
            return Int32.TryParse(str, out page) ? page : 0;
        }

        public static object LocalizedRoleFor(Role role)
        {
            return role == Role.Custom ? LOCALIZED_CUSTOM :
                role == Role.Heading ? LOCALIZED_HEADING :
                role == Role.Page ? LOCALIZED_PAGE :
                role == Role.Anchor ? LOCALIZED_ANCHOR :    //@AssociateNode
                role == Role.Plain ? LOCALIZED_PLAIN : LOCALIZED_SILENCE;
        }
    }

    /// <summary>
    /// Informs that a node's kind has changed and pass along its old kind.
    /// </summary>
    public class ChangedRoleEventArgs : NodeEventArgs<EmptyNode>
    {
        private EmptyNode.Role mRole;  // previous role
        private string mCustomRole;    // previous custom role name

        public ChangedRoleEventArgs(EmptyNode node, EmptyNode.Role role, string customRole): base(node)
        {
            mRole = role;
            mCustomRole = customRole;
        }

        /// <summary>
        /// Get the revious role.
        /// </summary>
        public EmptyNode.Role PreviousRole { get { return mRole; } }

        /// <summary>
        /// Get the previous custom role name.
        /// </summary>
        public string PreviousCustomRole { get { return mCustomRole; } }
    }

    /// <summary>
    /// Wrapper around the role enumeration for localization
    /// </summary>
    public class LocalizedRole
    {
        private EmptyNode.Role mRole;

        public LocalizedRole(EmptyNode.Role role) { mRole = role; }
        public EmptyNode.Role Role { get { return mRole; } }
        public override string ToString() { return Localizer.Message("role_" + mRole.ToString()); }

        

    }
}