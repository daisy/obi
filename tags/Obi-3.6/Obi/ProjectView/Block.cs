using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class Block : UserControl, ISelectableInContentViewWithColors, ISearchable
    {
        private int mBaseHeight;                                      // base height for scaling
        protected float mBaseFontSize;                                // base font size for scaling
        private bool mHighlighted;                                    // if true show as highlighted
        protected EmptyNode mNode;                                    // the corresponding node
        private ISelectableInContentViewWithColors mParentContainer;  // not necessarily a strip (in the future)


        // Used by the designer
        public Block() { InitializeComponent(); }

        /// <summary>
        /// Create a new empty block from an empty node.
        /// </summary>
        public Block(EmptyNode node, ISelectableInContentViewWithColors parent): this()
        {
            mNode = node;
            mParentContainer = parent;
            mHighlighted = false;
            this.Disposed += new EventHandler(Block_Disposed);
            node.ChangedRole += new EmptyNode.ChangedRoleEventHandler(Node_ChangedKind);
            node.ChangedPageNumber += new NodeEventHandler<EmptyNode>(Node_ChangedPageNumber);
            node.ChangedTODOStatus += new NodeEventHandler<EmptyNode>(Node_ChangedTODOStatus);
            ((ObiPresentation)node.Presentation).UsedStatusChanged += new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
            UpdateColors();
            UpdateLabel();
            mBaseHeight = Height;
            mBaseFontSize = mLabel.Font.SizeInPoints;
        }
        
        /// <summary>
        /// Get or set the color settings. Set from the strip/parent container, get from the waveform.
        /// </summary>
        public ColorSettings ColorSettings
        {
            get { return mParentContainer == null ? null : mParentContainer.ColorSettings; }
            set { UpdateColors(value); }
        }

        /// <summary>
        /// Get the current content view.
        /// </summary>
        public ContentView ContentView
        {
            get { return mParentContainer == null ? null : mParentContainer.ContentView; }
        }

        /// <summary>
        /// Get or set the highlighted flag for the block.
        /// </summary>
        public virtual bool Highlighted
        {
            get { return mHighlighted; }
            set
            {
                if (value != mHighlighted)
                {
                    mHighlighted = value;
                    
                    UpdateColors();
                }
            }
        }

        /// <summary>
        /// Get the tab index of the block.
        /// </summary>
        public int LastTabIndex { get { return TabIndex; } }

        /// <summary>
        /// Get the empty node for this block.
        /// </summary>
        public EmptyNode Node { get { return mNode; } }

        /// <summary>
        /// Get the Obi node for this block.
        /// </summary>
        public ObiNode ObiNode { get { return mNode; } }

        /// <summary>
        /// Set the selection from the parent view
        /// </summary>
        public virtual void SetSelectionFromContentView(NodeSelection selection) { Highlighted = selection != null; }

        /// <summary>
        /// Get the strip that contains this block.
        /// </summary>
        public Strip Strip
        {
            get { return mParentContainer is Strip ? (Strip)mParentContainer : ((Block)mParentContainer).Strip; }
        }

        private bool m_IsFineNavigationMode ;
        /// <summary>
        /// Used to update background color for fine navigation. It is switched false when highlight is set false.
        /// </summary>
        public bool IsFineNavigationMode
        {
            get
            {
                return m_IsFineNavigationMode;
            }
            set
            {
                m_IsFineNavigationMode = value;
            }
        }

        /// <summary>
        /// Set the zoom factor and the height.
        /// We cheat for the height so that it fits exactly in the parent container.
        /// </summary>
        public virtual void SetZoomFactorAndHeight(float zoom, int height)
        {
            if (zoom > 0.0f)
            {
                mLabel.Font = new Font(Font.FontFamily, zoom * mBaseFontSize);
                Size = new Size(LabelFullWidth, height - Margin.Vertical);
            }
        }

        /// <summary>
        /// Update the colors of the block when the state of its node has changed using the current color settings.
        /// </summary>
        public void UpdateColors() { UpdateColors(ColorSettings); }

        /// <summary>
        /// Update the colors with new color settings.
        /// </summary>
        public virtual void UpdateColors(ColorSettings settings)
        {
            if (mNode != null && settings != null)
            {
                BackColor =
                    m_IsFineNavigationMode? settings.FineNavigationColor:
                    mHighlighted ? settings.BlockBackColor_Selected :
                    Strip.RecordingNode != null && Strip.RecordingNode == mNode? settings.RecordingHighlightPhraseColor ://@ recording node color should be different
                    mNode.Role_ == EmptyNode.Role.Silence ? settings.BlockBackColor_Silence :
                    !mNode.Used ? settings.BlockBackColor_Unused :
                    mNode.TODO ? settings.BlockBackColor_TODO :
                    mNode.Role_ == EmptyNode.Role.Custom ? settings.BlockBackColor_Custom :
                    mNode.Role_ == EmptyNode.Role.Anchor? settings.BlockBackColor_Anchor:
                    mNode.Role_ == EmptyNode.Role.Heading ? settings.BlockBackColor_Heading :
                    mNode.Role_ == EmptyNode.Role.Page ? settings.BlockBackColor_Page :
                    !(mNode is PhraseNode) ? settings.BlockBackColor_Empty :
                        settings.BlockBackColor_Plain;
                ForeColor =
                    mHighlighted ? settings.BlockForeColor_Selected :
                    mNode.Role_ == EmptyNode.Role.Silence ? settings.BlockForeColor_Silence :
                    !mNode.Used ? settings.BlockForeColor_Unused :
                    mNode.TODO ? settings.BlockForeColor_TODO :
                    mNode.Role_ == EmptyNode.Role.Custom ? settings.BlockForeColor_Custom :
                    mNode.Role_ == EmptyNode.Role.Anchor? settings.BlockForeColor_Anchor:
                    mNode.Role_ == EmptyNode.Role.Heading ? settings.BlockForeColor_Heading :
                    mNode.Role_ == EmptyNode.Role.Page ? settings.BlockForeColor_Page :
                    !(mNode is PhraseNode) ? settings.BlockForeColor_Empty :
                        settings.BlockForeColor_Plain;
            }
        }

        private delegate void UpdateLabelsTextDelegate();

        /// <summary>
        /// Update label and tooltips.
        /// </summary>
        public virtual void UpdateLabelsText()
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateLabelsTextDelegate(UpdateLabelsText));
            }
            else
            {
                mLabel.Text = Node.BaseStringShort();
                //mLabel.AccessibleName = Node.BaseString(); // commented for removing update block label threads.
                mLabel.AccessibleName = GetAccessibleName;
                mToolTip.SetToolTip(this, Node.BaseStringShort());
                mToolTip.SetToolTip(mLabel, Node.BaseStringShort());
                AccessibleName = mLabel.AccessibleName;
            }
        }


        #region ISearchable Members

        /// <summary>
        /// Return the string to be matched, which is the one displayed by
        /// the label (including role and duration.)
        /// </summary>
        public string ToMatch() { return mLabel.Text.ToLowerInvariant(); }

        #endregion


        // Width of the label (including margins)
        protected int LabelFullWidth { get { return mLabel.Width + mLabel.Margin.Horizontal; } }

        protected int BorderHeight { get { return Bounds.Height - ClientSize.Height; } }

        /// <summary>
        /// Update the tab index of the block with the new value and return the next index.
        /// </summary>
        public int UpdateTabIndex(int index)
        {
            TabIndex = index;
            return index + 1;
        }


        // Select/deselect on click
        private void Block_Click(object sender, EventArgs e) { Strip.SetSelectedBlockFromBlock(this); }

        // Select on tabbing
        protected void Block_Enter(object sender, EventArgs e)
        {
            if (!Strip.ContentView.Focusing) { Strip.SetSelectedBlockFromBlock(this); }
        }

        // Update label when the page number changes
        private void Node_ChangedPageNumber(object sender, NodeEventArgs<EmptyNode> e) { UpdateLabel(); }

        // Update the label when the role of the node changes
        private void Node_ChangedKind(object sender, ChangedRoleEventArgs e) 
            {
            UpdateColors ();
            UpdateLabel();
            if (e.PreviousRole == EmptyNode.Role.Anchor && e.Node.Role_ != EmptyNode.Role.Anchor)
            {
                ObiPresentation pres = (ObiPresentation)mNode.Presentation;
                pres.ListOfAnchorNodes_Remove( e.Node ) ;
            }
            else if (e.Node.Role_ == EmptyNode.Role.Anchor && e.PreviousRole != EmptyNode.Role.Anchor)
            {
                ObiPresentation pres = (ObiPresentation)mNode.Presentation;
                pres.ListOfAnchorNodes_Add( e.Node ) ;
            }
            }

        // update label when to do status changes
        private void Node_ChangedTODOStatus(object sender, NodeEventArgs<EmptyNode> e)
        {
            UpdateColors();
            UpdateLabel();
        }

        private void Presentation_UsedStatusChanged ( object sender, NodeEventArgs<ObiNode> e )
            {
            if (e.Node == mNode)
                {
                UpdateColors ();
                UpdateLabel ();
                }
            }

        protected virtual void UpdateLabel()
        {
            UpdateLabelsText();
            Size = new Size(LabelFullWidth, Height);
        }

        private string GetAccessibleName
            {
            get
                {
                if (mNode == null) return "";
                double durationMS = mNode is PhraseNode ? mNode.Duration : 0 ;
                return String.Format ( Localizer.Message ( "Block_AccessibleLabel" ),
                mNode.TODO ? Localizer.Message ( "phrase_short_TODO" ) : "",
                mNode.Used ? "" : Localizer.Message ( "unused" ),
                                                durationMS == 0.0 ? Localizer.Message ( "empty" ) : Program.FormatDuration_Long ( durationMS),
                mNode.Role_ == EmptyNode.Role.Custom ? String.Format ( Localizer.Message ( "phrase_extra_custom" ), mNode.CustomRole) :
                mNode.Role_== EmptyNode.Role.Page ? String.Format ( Localizer.Message ( "phrase_extra_page" ), mNode.PageNumber!= null ? mNode.PageNumber.ToString () : "" ) :
                mNode.Role_ == EmptyNode.Role.Anchor && mNode.AssociatedNode == null ? Localizer.Message ( "phrase_extra_" + mNode.Role_.ToString ()  ) + "= ?":
                    Localizer.Message ( "phrase_extra_" + mNode.Role_.ToString () ) );
        

                }
            }

        public void DestroyBlockHandle () { base.DestroyHandle (); }

        protected virtual void Block_Disposed(object sender, EventArgs e)
        {
            if (mNode != null)
            {
                mNode.ChangedRole -= new EmptyNode.ChangedRoleEventHandler(Node_ChangedKind);
                mNode.ChangedPageNumber -= new NodeEventHandler<EmptyNode>(Node_ChangedPageNumber);
                mNode.ChangedTODOStatus -= new NodeEventHandler<EmptyNode>(Node_ChangedTODOStatus);
                ((ObiPresentation)mNode.Presentation).UsedStatusChanged -= new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
                
            }
        }

    }
}
