using System;


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
    public class EmptyNode: ObiNode
    {
        private Role mRole;              // this node's kind
        private string mCustomRole;      // custom role name
        private PageNumber mPageNumber;  // page number
        private bool mTODO;              // marked as TODO

        public static readonly string XUK_ELEMENT_NAME = "empty";             // name of the element in the XUK file
        private static readonly string XUK_ATTR_NAME_ROLE = "kind";           // name of the role attribute
        private static readonly string XUK_ATTR_NAME_CUSTOM = "custom";       // name of the custom attribute
        private static readonly string XUK_ATTR_NAME_PAGE = "page";           // name of the page attribute
        private static readonly string XUK_ATTR_NAME_PAGE_KIND = "pageKind";  // name of the pageKind attribute
        private static readonly string XUK_ATTR_NAME_PAGE_TEXT = "pageText";  // name of the pageText attribute
        private static readonly string XUK_ATTR_NAME_TODO = "TODO";           // name of the TODO attribute

        /// <summary>
        /// Different roles for content nodes.
        /// Plain is the default.
        /// Page is a page node with a page number.
        /// Heading is the audio heading for a section.
        /// Silence is a silence node for phrase detection.
        /// Custom is a node with a custom class (e.g. sidebar, etc.)
        /// </summary>
        public enum Role { Plain, Page, Heading, Silence, Custom };

        public static readonly LocalizedRole LOCALIZED_PLAIN = new LocalizedRole(Role.Plain);
        public static readonly LocalizedRole LOCALIZED_PAGE = new LocalizedRole(Role.Page);
        public static readonly LocalizedRole LOCALIZED_HEADING = new LocalizedRole(Role.Heading);
        public static readonly LocalizedRole LOCALIZED_SILENCE = new LocalizedRole(Role.Silence);
        public static readonly LocalizedRole LOCALIZED_CUSTOM = new LocalizedRole(Role.Custom);
        
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
                    Localizer.Message("phrase_extra_" + mRole.ToString()));
        }

        public virtual string BaseString() { return BaseString(0.0); }

        public virtual string BaseStringShort(double durationMs)
        {
            return String.Format(Localizer.Message("phrase_short_to_string"),
                TODO ? Localizer.Message("phrase_short_TODO") : "",
                mRole == Role.Custom ? String.Format(Localizer.Message("phrase_short_custom"), mCustomRole) :
                mRole == Role.Page ? String.Format(Localizer.Message("phrase_short_page"), mPageNumber != null ? mPageNumber.ToString() : "") :
                    Localizer.Message("phrase_short_" + mRole.ToString()),
                durationMs == 0.0 ? Localizer.Message("empty") : Program.FormatDuration_Smart(durationMs));
        }

        public virtual string BaseStringShort() { return BaseStringShort(0.0); }

        public override double Duration { get { return 0.0; } }

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
        public EmptyNode(Presentation presentation, Role role, string customRole): base(presentation)
        {
            mRole = role;
            mCustomRole = customRole;
            mPageNumber = null;
        }

        /// <summary>
        /// Create a plain empty node in a presentation.
        /// </summary>
        public EmptyNode(Presentation presentation): this(presentation, Role.Plain, null) {}

        /// <summary>
        /// Create an empty node of a pre-defined kind a presentation.
        /// </summary>
        public EmptyNode(Presentation presentation, Role role): this(presentation, role, null) {}

        /// <summary>
        /// Create an empty node with a custom class in a presentation.
        /// </summary>
        public EmptyNode(Presentation presentation, string customRole): this(presentation, Role.Custom, customRole) {}


        /// <summary>
        /// Copy all attributes of this node to another.
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
        protected override TreeNode copyProtected(bool deep, bool inclProperties)
        {
            EmptyNode copy = (EmptyNode)base.copyProtected(deep, inclProperties);
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
                    k = getPresentation().getCommandFactory().createCompositeCommand();
                    k.setShortDescription(string.Format(Localizer.Message("renumber_pages"),
                        Localizer.Message(string.Format("{0}_pages", from.Kind.ToString()))));
                }
                k.append(new Commands.Node.SetPageNumber(view, this, from));
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

        public override void Insert(ObiNode node, int index) { throw new Exception("Empty nodes have no children."); }
        public override SectionNode SectionChild(int index) { throw new Exception("Empty nodes have no children."); }
        public override int SectionChildCount { get { return 0; } }
        public override EmptyNode PhraseChild(int index) { throw new Exception("Emtpy nodes have no children."); }
        public override int PhraseChildCount { get { return 0; } }
        public override EmptyNode LastUsedPhrase { get { throw new Exception("Empty nodes have no children."); } }



        public override string getXukLocalName() 
        { 
            return XUK_ELEMENT_NAME; 
        }

        
        protected override void XukInNodeProperties ()
        {
            base.XukInNodeProperties();
            if (Role_ != Role.Plain) return;
            try
            {
                
                XmlProperty xmlProp = this.getProperty<XmlProperty>();
                if (xmlProp != null)
                {
                    
                    urakawa.property.xml.XmlAttribute attrRole = xmlProp.getAttribute(XUK_ATTR_NAME_ROLE, xmlProp.getNamespaceUri());

                    if (attrRole != null)
                    {
                        string role = attrRole.getValue();
                        if (role != null) mRole = role == Role.Custom.ToString() ? Role.Custom :
                                                  role == Role.Heading.ToString() ? Role.Heading :
                                                  role == Role.Page.ToString() ? Role.Page :
                                                  role == Role.Silence.ToString() ? Role.Silence : Role.Plain;
                        if (role != null && role != mRole.ToString()) throw new Exception("Unknown kind: " + role);
                        if (mRole == Role.Custom) mCustomRole = xmlProp.getAttribute(XUK_ATTR_NAME_CUSTOM, xmlProp.getNamespaceUri()).getValue();
                        //System.Windows.Forms.MessageBox.Show(mRole.ToString());
                        if (mRole == Role.Heading)
                        {
                            if (!AncestorAs<SectionNode>().DidSetHeading(this)) mRole = Role.Plain;
                        }
                        else if (mRole == Role.Page)
                        {
                            string pageKind = xmlProp.getAttribute(XUK_ATTR_NAME_PAGE_KIND, xmlProp.getNamespaceUri()).getValue();
                            if (pageKind != null)
                            {
                                string page = xmlProp.getAttribute(XUK_ATTR_NAME_PAGE, xmlProp.getNamespaceUri()).getValue();
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
                        Presentation.AddCustomClass(mCustomRole, this);

                        string todo = xmlProp.getAttribute(XUK_ATTR_NAME_TODO, xmlProp.getNamespaceUri()).getValue();
                        if (todo != null) mTODO = todo == "True";
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this.ToString() + " " + ex.ToString());
            }
        }

        protected override void xukInAttributes(System.Xml.XmlReader source)
        {   
            //Presentation.UseXukFormat = false;
            if (!Presentation.UseXukFormat)
            {
                string role = source.GetAttribute(XUK_ATTR_NAME_ROLE);
                if (role != null) mRole = role == Role.Custom.ToString() ? Role.Custom :
                                          role == Role.Heading.ToString() ? Role.Heading :
                                          role == Role.Page.ToString() ? Role.Page :
                                          role == Role.Silence.ToString() ? Role.Silence : Role.Plain;
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
                Presentation.AddCustomClass(mCustomRole, this);

                string todo = source.GetAttribute(XUK_ATTR_NAME_TODO);
                if (todo != null) mTODO = todo == "True";
            }
            base.xukInAttributes(source);
        }

        
        protected override void UpdateXmlProperties()
        {
            XmlProperty xmlProp = GetOrCreateXmlProperty();
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

        
        protected override void xukOutAttributes(System.Xml.XmlWriter wr, Uri baseUri)
        {
            //Presentation.UseXukFormat = true;
            if (!Presentation.UseXukFormat)
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
            }
            base.xukOutAttributes(wr, baseUri);
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
                role == Role.Plain ? LOCALIZED_PLAIN : LOCALIZED_SILENCE;
        }
    }

    /// <summary>
    /// Informs that a node's kind has changed and pass along its old kind.
    /// </summary>
    class ChangedRoleEventArgs : NodeEventArgs<EmptyNode>
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