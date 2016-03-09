using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using urakawa.command;

namespace Obi.ProjectView
    {
    public partial class MetadataView : UserControl, IControlWithSelection
        {
        private ProjectView mView;             // parent project view
        private MetadataSelection mSelection;  // current selection
        private float mBaseFontSize;           // base font size
        private bool m_IsImportingMetadata; // flag to indicate if importing of metadata is going on
        Dictionary<string, string> mMetadataTooltipDictionary = new Dictionary<string,string>();

        public MetadataView ()
            {
            InitializeComponent ();
            mView = null;
            mSelection = null;
            mBaseFontSize = Font.SizeInPoints;
            m_IsImportingMetadata = false;
            initializeMetadataDictionary();
            m_BtnContextMenu.Location = new Point((mUpdateButton.Right + 10), mUpdateButton.Location.Y);
            }


        /// <summary>
        /// A new entry of the given kind can be added if this is not readonly, and if it is repeatable
        /// or there is not yet any occurrence of it.
        /// </summary>
        public bool CanAdd ( MetadataEntryDescription d )
            {
            return
                d == null ||
                (!d.ReadOnly && (d.Repeatable || mView.Presentation.GetMetadata( d.Name ).Count == 0));
            }

        /// <summary>
        /// A particular entry can be removed if it is not readonly and not the only occurrence in case of a required entry. 
        /// </summary>
        public bool CanRemove ( MetadataEntryDescription d )
            {
            // do not allow delete metadata node if focus is in text boxes
            if (mContentTextbox.ContainsFocus || mNameTextbox.ContainsFocus) return false;

            return
                d == null ||
                (!d.ReadOnly && (d.Occurrence != MetadataOccurrence.Required || mView.Presentation.GetMetadata( d.Name ).Count > 1));
            }

        /// <summary>
        /// A metadata entry can be removed if it is selected.
        /// </summary>
        public bool CanRemoveMetadata
            {
            get
                {
                return mSelection != null && CanRemove ( mSelection.Item.Description ) &&
                    mView.CanDeleteMetadataEntry ( mSelection.Item.Entry );
                }
            }

        /// <summary>
        /// The project view is showing a new presentation;
        /// reset the list of metadata panels.
        /// </summary>
        public void NewPresentation ()
            {
            if (mView.Presentation != null)
                {
                ImportMetadata ();
                mView.Presentation.MetadataEntryAdded += new MetadataEventHandler ( Presentation_MetadataEntryAdded );
                mView.Presentation.MetadataEntryDeleted += new MetadataEventHandler ( Presentation_MetadataEntryDeleted );
                mView.Presentation.MetadataEntryContentChanged += new MetadataEventHandler ( Presentation_MetadataEntryContentChanged );
                mView.Presentation.MetadataEntryNameChanged += new MetadataEventHandler ( Presentation_MetadataEntryNameChanged );
                }
            }
        private void initializeMetadataDictionary()
        {
            mMetadataTooltipDictionary.Add(Metadata.DC_CONTRIBUTOR, "METADATA_DC_CONTRIBUTOR_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DC_COVERAGE, "METADATA_DC_COVERAGE_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DC_CREATOR, "METADATA_DC_CREATOR_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DC_DATE, "METADATA_DC_DATE_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DC_DESCRIPTION, "METADATA_DC_DESCRIPTION_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DC_IDENTIFIER, "METADATA_DC_IDENTIFIER_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DC_LANGUAGE, "METADATA_DC_LANGUAGE_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DC_PUBLISHER, "METADATA_DC_PUBLISHER_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DC_RELATION, "METADATA_DC_RELATION_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DC_RIGHTS, "METADATA_DC_RIGHTS_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DC_SOURCE, "METADATA_DC_SOURCE_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DC_SUBJECT, "METADATA_DC_SUBJECT_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DC_TITLE, "METADATA_DC_TITLE_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DC_TYPE, "METADATA_DC_TYPE_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DTB_NARRATOR, "METADATA_DTB_NARRATOR_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DTB_PRODUCED_DATE, "METADATA_DTB_PRODUCED_DATE_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DTB_PRODUCER, "METADATA_DTB_PRODUCER_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DTB_REVISION, "METADATA_DTB_REVISION_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DTB_REVISION_DATE, "METADATA_DTB_REVISION_DATE_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DTB_REVISION_DESCRIPTION, "METADATA_DTB_REVISION_DESC_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DTB_SOURCE_DATE, "METADATA_DTB_SOURCE_DATE_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DTB_SOURCE_EDITION, "METADATA_DTB_SOURCE_EDITION_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DTB_SOURCE_PUBLISHER, "METADATA_DTB_SOURCE_PUBLISHER_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DTB_SOURCE_RIGHTS, "METADATA_DTB_SOURCE_RIGHTS_HELP");
            mMetadataTooltipDictionary.Add(Metadata.DTB_SOURCE_TITLE, "METADATA_DTB_SOURCE_TITLE_HELP");
            mMetadataTooltipDictionary.Add(Metadata.GENERATOR, "METADATA_GENERATOR_HELP");
            mMetadataTooltipDictionary.Add(Metadata.OBI_XUK_VERSION, "METADATA_OBI_XUK_VERSION_HELP");
            mMetadataTooltipDictionary.Add(Localizer.Message("metadata_custom"), "METADATA_CUSTOM_HELP");
            mMetadataTooltipDictionary.Add(Metadata.OBI_DAISY2ExportPath, "METADATA_DAISY202_EXPORT_PATH_HELP");
            mMetadataTooltipDictionary.Add(Metadata.OBI_DAISY3ExportPath, "METADATA_DAISY3_EXPORT_PATH_HELP");
            mMetadataTooltipDictionary.Add(Metadata.OBI_EPUB3ExportPath, "METADATA_EPUB3_EXPORT_PATH_HELP");
        }

        public float ZoomFactor
            {
            set
                {
                float size = mBaseFontSize * value;
                int labelOffset = mNameTextbox.Location.Y - mNameLabel.Location.Y;
                mMetadataListView.Font = new Font ( mMetadataListView.Font.FontFamily, size );
                mNameLabel.Font = new Font ( mNameLabel.Font.FontFamily, size );
                mNameTextbox.Font = new Font ( mNameTextbox.Font.FontFamily, size );
                mContentLabel.Font = new Font ( mContentLabel.Font.FontFamily, size );
                mContentTextbox.Font = new Font ( mContentTextbox.Font.FontFamily, size );
                mUpdateButton.Font = new Font ( mUpdateButton.Font.FontFamily, size );
                int labelEdge = mNameLabel.Width > mContentLabel.Width ? mNameLabel.Location.X + mNameLabel.Width + mNameLabel.Margin.Right :
                    mContentLabel.Location.X + mContentLabel.Width + mContentLabel.Margin.Right;
                mContentTextbox.Location = new Point ( labelEdge + mContentTextbox.Margin.Left, mUpdateButton.Location.Y - mUpdateButton.Margin.Top - mContentTextbox.Margin.Bottom - mContentTextbox.Height );
                
                m_BtnContextMenu.Location = new Point((mUpdateButton.Right + 10), mUpdateButton.Location.Y);

                mContentTextbox.Width = Width - mContentTextbox.Location.X - mContentTextbox.Margin.Right;
                mContentLabel.Location = new Point ( labelEdge - mContentLabel.Margin.Right - mContentLabel.Width,
                    mContentTextbox.Location.Y + labelOffset );
                mNameTextbox.Location = new Point ( labelEdge + mNameTextbox.Margin.Left,
                    mContentTextbox.Location.Y - mContentTextbox.Margin.Top - mNameTextbox.Margin.Bottom - mNameTextbox.Height );
                mNameTextbox.Width = mContentTextbox.Width;
                mNameLabel.Location = new Point ( labelEdge - mNameLabel.Margin.Right - mNameLabel.Width,
                    mNameTextbox.Location.Y + labelOffset );
                mMetadataListView.Height = mNameTextbox.Location.Y - mNameTextbox.Margin.Top - mMetadataListView.Margin.Vertical;
                }
            }

        private delegate void UpdateMetadataInvokation ( MetadataEventArgs e );

        // A new metadata entry was added.
        private void Presentation_MetadataEntryAdded ( object sender, MetadataEventArgs e )
            {
            UpdateMetaDataListForItemAdd ( e );
            // Let's not try to be clever now.
            //ImportMetadata();
            //ClearTextBoxesFromCommandEvents ();
            }

        private void UpdateMetaDataListForItemAdd ( MetadataEventArgs e )
            {
            if (InvokeRequired)
                {
                Invoke ( new UpdateMetadataInvokation ( UpdateMetaDataListForContentChange ), e );
                }
            else
                {
                mMetadataListView.ItemChecked -= new System.Windows.Forms.ItemCheckedEventHandler ( mMetadataListView_ItemChecked );
                this.mMetadataListView.SelectedIndexChanged -= new System.EventHandler ( this.mMetadataListView_SelectedIndexChanged );
                m_IsImportingMetadata = true;

                string[] nameContent = new string[2];
                List<urakawa.metadata.Metadata> items = mView.Presentation.Metadatas.ContentsAs_ListCopy;
                items.Sort ( delegate ( urakawa.metadata.Metadata a, urakawa.metadata.Metadata b )
                {
                    int names = a.NameContentAttribute.Name.CompareTo(b.NameContentAttribute.Name);
                    return names == 0 && a.NameContentAttribute.Value != null && b.NameContentAttribute.Value != null
                        ? a.NameContentAttribute.Value.CompareTo(b.NameContentAttribute.Value)
                        : names;
                } );
                string itemToRemove = "";
                List<string> ExistingItemsList = new List<string> ();
                int i = 0;

                if (mMetadataListView.Items.Count == 0)
                    return;

                for (i = 0; i < items.Count; i++)
                    {
                    string metaDataListString = mMetadataListView.Items[i].SubItems[0].Text;
                    string itemsListString = items[i].NameContentAttribute.Name;

                    if (string.CompareOrdinal ( metaDataListString, itemsListString ) != 0)
                        {
                        itemToRemove = itemsListString;
                        urakawa.metadata.Metadata m = items[i];
                        nameContent[0] = m.NameContentAttribute.Name;
                        nameContent[1] = m.NameContentAttribute.Value;
                        ListViewItem item = new ListViewItem ( nameContent );
                        mMetadataListView.Items.Insert ( i, item );
                        item.Checked = true;
                        item.Tag = m;
                        MetadataEntryDescription metDes = MetadataEntryDescription.GetDAISYEntry(m.NameContentAttribute.Name);
                        if (metDes != null)
                        {
                            if (metDes.ReadOnly)
                                item.BackColor = SystemColors.InactiveCaption;
                        }
                        }

                    ExistingItemsList.Add ( itemsListString );
                    
                    }

                // remove only if metadata is not custom
                if (itemToRemove != Localizer.Message ( "metadata_custom" ))
                    {
                    // now search items below checked items and remove the item which we have just added above.
                    for (i = i; i < mMetadataListView.Items.Count; i++)
                        {
                        if (string.CompareOrdinal ( itemToRemove, mMetadataListView.Items[i].SubItems[0].Text ) == 0)
                            {
                            mMetadataListView.Items.RemoveAt ( i );
                            break;
                            }
                        }
                    } // check for custom ends

                m_IsImportingMetadata = false;
                this.mMetadataListView.SelectedIndexChanged += new System.EventHandler ( this.mMetadataListView_SelectedIndexChanged );
                mMetadataListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler ( mMetadataListView_ItemChecked );

                }

            }


        // An entry was deleted.
        void Presentation_MetadataEntryDeleted ( object sender, MetadataEventArgs e )
            {
            UpdateMetaDataListForItemDelete ( e );
            // Let's not try to be clever now.
            //ImportMetadata ();
            ClearTextBoxesFromCommandEvents ();
            mMetadataListView.Focus ();
            }

        private void UpdateMetaDataListForItemDelete ( MetadataEventArgs e )
            {
            if (InvokeRequired)
                {
                Invoke ( new UpdateMetadataInvokation ( UpdateMetaDataListForContentChange ), e );
                }
            else
                {
                if (e.Entry != null)
                    {
                    mMetadataListView.ItemChecked -= new System.Windows.Forms.ItemCheckedEventHandler ( mMetadataListView_ItemChecked );
                    this.mMetadataListView.SelectedIndexChanged -= new System.EventHandler ( this.mMetadataListView_SelectedIndexChanged );
                    m_IsImportingMetadata = true;

                    string[] nameContent = new string[2];
                    List<urakawa.metadata.Metadata> items = mView.Presentation.Metadatas.ContentsAs_ListCopy;
                    items.Sort ( delegate ( urakawa.metadata.Metadata a, urakawa.metadata.Metadata b )
                    {
                        int names = a.NameContentAttribute.Name.CompareTo(b.NameContentAttribute.Name);
                        return names == 0 ? a.NameContentAttribute.Value.CompareTo(b.NameContentAttribute.Value) : names;
                    } );
                    //string itemToRemove = "";
                    List<string> ExistingItemsList = new List<string> ();
                    int i = 0;

                    if (mMetadataListView.Items.Count == 0)
                        return;

                    for (i = 0; i < mMetadataListView.Items.Count; i++)
                        {
                        if (mMetadataListView.Items[i].Tag != null
                            && e.Entry.NameContentAttribute.Name == ((urakawa.metadata.Metadata)mMetadataListView.Items[i].Tag).NameContentAttribute.Name)
                            {
                            mMetadataListView.Items.RemoveAt ( i );
                            }

                        }

                    // if delete entry was not custom entry, then add it blank entry to list below.
                    List<string> addables = mView.AddableMetadataNames;
                    addables.Sort ();

                    if (addables.Contains(e.Entry.NameContentAttribute.Name))
                        {
                        bool inserted = false;
                        for (i = 0; i < mMetadataListView.Items.Count; i++)
                            {
                            if (mMetadataListView.Items[i].Tag == null
                                && string.CompareOrdinal(e.Entry.NameContentAttribute.Name, mMetadataListView.Items[i].SubItems[0].Text) < 0)
                                {
                                    nameContent[0] = e.Entry.NameContentAttribute.Name;
                                nameContent[1] = "";
                                ListViewItem item = new ListViewItem ( nameContent );
                                mMetadataListView.Items.Insert ( i, item );
                                inserted = true;

                                break;
                                }
                            }
                        if (!inserted)
                            {
                                nameContent[0] = e.Entry.NameContentAttribute.Name;
                            nameContent[1] = "";
                            ListViewItem item = new ListViewItem ( nameContent );
                            mMetadataListView.Items.Insert ( mMetadataListView.Items.Count - 1, item );

                            }
                        }


                    m_IsImportingMetadata = false;
                    this.mMetadataListView.SelectedIndexChanged += new System.EventHandler ( this.mMetadataListView_SelectedIndexChanged );
                    mMetadataListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler ( mMetadataListView_ItemChecked );
                    }
                }

            }


        // Metadata content has changed.
        void Presentation_MetadataEntryContentChanged ( object sender, MetadataEventArgs e )
            {

            UpdateMetaDataListForContentChange ( e );
            // Let's not try to be clever now.
            //ImportMetadata();
            ClearTextBoxesFromCommandEvents ();
            }

        private void UpdateMetaDataListForContentChange ( MetadataEventArgs e )
            {
            if (InvokeRequired)
                {
                Invoke ( new UpdateMetadataInvokation ( UpdateMetaDataListForContentChange ), e );
                }
            else
                {
                if (e.Entry != null)
                    {
                        string entryName = e.Entry.NameContentAttribute.Name;
                    for (int i = 0; i < mMetadataListView.Items.Count; i++)
                        {
                        if (mMetadataListView.Items[i].Tag != null)
                            {
                            urakawa.metadata.Metadata metadataObj = (urakawa.metadata.Metadata)mMetadataListView.Items[i].Tag;
                            if (metadataObj.NameContentAttribute.Name == entryName)
                                {
                                    mMetadataListView.Items[i].SubItems[1].Text = metadataObj.NameContentAttribute.Value;

                                // check if custom entry at last of metadataListView is selected.
                                // move selection to one item before so as to prevent user from adding more custom entries by pressing enter multiple times there.
                                if (mMetadataListView.SelectedIndices.Count > 0 && mMetadataListView.SelectedIndices[0] == mMetadataListView.Items.Count - 1
                                    && mMetadataListView.Items.Count > 1)
                                    {
                                    mMetadataListView.Items[mMetadataListView.Items.Count - 2].Selected = true;
                                    }

                                break;
                                }

                            }
                        }

                    }

                }
            }


        // Metadata name has changed.
        void Presentation_MetadataEntryNameChanged ( object sender, MetadataEventArgs e )
            {
            // Let's not try to be clever now.
            ImportMetadata ();
            }

        /// <summary>
        /// The parent project view. Should be set ASAP, and only once.
        /// </summary>
        public ProjectView ProjectView
            {
            set
                {
                if (mView != null) throw new Exception ( "Cannot set the project view again!" );
                mView = value;
                }
            }

        /// <summary>
        /// Get or set the current selection in the view.
        /// </summary>
        public NodeSelection Selection
            {
            get { return mSelection; }
            set
                {
                if (mSelection != null) ClearSelection ();
                mSelection = value as MetadataSelection;
                if (mSelection != null) SetSelection ();
                }
            }


        // Clear the selection
        private void ClearSelection ()
            {
            if (mSelection != null) mSelection.Item.Item.Selected = false;
            mSelection = null;
            mNameTextbox.Text = "";
            mContentTextbox.Text = "";
            }

        private delegate void ImportMetadataInvokation ();

        // Import metadata entries from the presentation
        private void ImportMetadata ()
            {
                if (InvokeRequired)
                {
                    Invoke(new ImportMetadataInvokation(ImportMetadata));
                }
                else
                {
                    mMetadataListView.ItemChecked -= new System.Windows.Forms.ItemCheckedEventHandler(mMetadataListView_ItemChecked);
                    m_IsImportingMetadata = true;
                    mMetadataListView.Items.Clear();
                    string[] nameContent = new string[2];
                    List<urakawa.metadata.Metadata> items = mView.Presentation.Metadatas.ContentsAs_ListCopy;
                    items.Sort(delegate(urakawa.metadata.Metadata a, urakawa.metadata.Metadata b)
                    {
                        int names = a.NameContentAttribute.Name.CompareTo(b.NameContentAttribute.Name);
                        if (a.NameContentAttribute.Value != null && b.NameContentAttribute.Value != null)
                        {   
                            return names == 0 ? a.NameContentAttribute.Value.CompareTo(b.NameContentAttribute.Value) : names;
                        }
                        else
                        {
                            if (a.NameContentAttribute.Value == null) a.NameContentAttribute.Value =Localizer.Message ("Metadata_Empty");
                            if (b.NameContentAttribute.Value == null) b.NameContentAttribute.Value = Localizer.Message("Metadata_Empty");
                            return names ;
                        }

                    });

                    // a list of metadata names which already exists
                    // purpose is to compare addable items and prevent their addition to list if it is already there
                    List<string> ExistingItemsList = new List<string>();

                    foreach (urakawa.metadata.Metadata m in items)
                    {
                        nameContent[0] = m.NameContentAttribute.Name;
                        nameContent[1] = m.NameContentAttribute.Value;
                        ListViewItem item = new ListViewItem(nameContent);
                        mMetadataListView.Items.Add(item);
                        item.Checked = true;
                        item.Tag = m;                     
                                            
                        ExistingItemsList.Add(nameContent[0]);
                        MetadataEntryDescription metDes = MetadataEntryDescription.GetDAISYEntry(m.NameContentAttribute.Name);
                        if (metDes != null)
                        {
                            if (metDes.ReadOnly)
                                item.BackColor = SystemColors.InactiveCaption;
                        }
                    }
                    List<string> addables = mView.AddableMetadataNames;
                    addables.Sort();
                    addables.Add(Localizer.Message("metadata_custom"));
                    foreach (string name in addables)
                    {
                        if (!ExistingItemsList.Contains(name))
                        {
                            nameContent[0] = name;
                            nameContent[1] = "";
                            ListViewItem item = new ListViewItem(nameContent);
                            mMetadataListView.Items.Add(item);
                            item.Checked = false;
                            item.Tag = null;
                            MetadataEntryDescription metDes = MetadataEntryDescription.GetDAISYEntry(name);
                            if (metDes != null)
                            {
                                if (metDes.ReadOnly)
                                    item.BackColor = SystemColors.InactiveCaption;
                            }                           

                        }
                    }
                    m_IsImportingMetadata = false;
                    mMetadataListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(mMetadataListView_ItemChecked);
                }
            }

        private void ClearTextBoxesFromCommandEvents ()
            {
            if (InvokeRequired)
                {
                Invoke ( new ImportMetadataInvokation ( ImportMetadata ) );
                }
            else
                {
                if (mMetadataListView.SelectedIndices.Count <= 0)
                    {
                    mContentTextbox.Text = "";
                    mContentTextbox.AccessibleName = mContentLabel.Text.Replace ( "&", "" );
                    mNameTextbox.Text = "";
                    }
                }
            }

        // Commit a change
        private void mCommitButton_Click ( object sender, EventArgs e ) { CommitValues (); }

        private void CommitValues ()
            {
                if (mSelection != null && !String.IsNullOrEmpty(mNameTextbox.Text))
                {
                if (GetTextFromContentTextbox () != "")
                    {
                    string contentBoxText = GetTextFromContentTextbox ();
                    int itemIndex = 0;
                    if (mMetadataListView.SelectedItems.Count > 0) itemIndex = mMetadataListView.SelectedItems[0].Index;
                    urakawa.metadata.Metadata entry = mSelection.Item.Entry;
                    if (entry == null && mNameTextbox.Text != "")
                        {
                        if (mMetadataListView.Items.Count > 0 && mMetadataListView.Items[itemIndex].Tag == null
                            && CanAdd ( MetadataEntryDescription.GetDAISYEntry ( mNameTextbox.Text ) ))
                            {
                            entry = mView.AddMetadataEntry ( mNameTextbox.Text );
                            }
                        else
                            {
                            return;
                            }
                        }
                    if (mSelection == null) return;
                    CompositeCommand command =
                        mView.Presentation.CreateCompositeCommand ( Localizer.Message ( "modify_metadata_entry" ) );
                    if (entry.NameContentAttribute.Name != mNameTextbox.Text)
                        {
                        if (CanModify ( mSelection.Item.Description, mNameTextbox.Text ))
                            {
                            command.ChildCommands.Insert( command.ChildCommands.Count, new Commands.Metadata.ModifyName ( mView, entry, mNameTextbox.Text ) );
                            }
                        else
                            {
                            MessageBox.Show ( String.Format ( Localizer.Message ( "metadata_name_error_text" ), mNameTextbox.Text ),
                                Localizer.Message ( "metadata_name_error_caption" ),
                                MessageBoxButtons.OK, MessageBoxIcon.Error );
                            mNameTextbox.Text = entry.NameContentAttribute.Name;
                            }
                        }
                    if (entry.NameContentAttribute.Value != contentBoxText)
                        {
                            command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Metadata.ModifyContent(mView, entry, contentBoxText));
                        }
                    if (command.ChildCommands.Count > 0) mView.Presentation.UndoRedoManager.Execute ( command );
                    }
                else
                    {
                    if (!IsDateMetadataSelected || string.IsNullOrEmpty ( mContentTextbox.Text )) MessageBox.Show ( Localizer.Message ( "MetadataView_CannotCommitEmptyContent" ), Localizer.Message ( "Caption_Error" ) );
                    mContentTextbox.Focus();
                    if (mSelection != null && mSelection.Item.Entry == null
                        && mMetadataListView.SelectedItems.Count > 0 && mMetadataListView.SelectedItems[0] != null && mMetadataListView.SelectedItems[0].Checked)
                        mMetadataListView.SelectedItems[0].Checked = false;
                    } // contentbox check ends

                }
            }


        private string GetTextFromContentTextbox ()
            {
            if (mSelection != null && mMetadataListView.SelectedItems.Count > 0)
                {
                string entryName = mMetadataListView.SelectedItems[0].Text;
                if (IsDateMetadataSelected)
                    {
                    // first check if this is year only format: this format is not accepted by default date stuff of c#
                    string strYearVal = mContentTextbox.Text;
                    int yearOnlyDate = 0;
                    int.TryParse ( strYearVal, out yearOnlyDate );
                    if (yearOnlyDate > 0 && strYearVal.Length == 4)
                        {
                        return strYearVal;
                        }
                    try
                        {
                        DateTime dateEntry = DateTime.Parse ( mContentTextbox.Text );
                        string strDate = dateEntry.Year.ToString () + "-" + AdjustSingleDigitDate ( dateEntry.Month.ToString () ) + "-" + AdjustSingleDigitDate ( dateEntry.Day.ToString () );
                        return strDate;
                        }
                    catch (System.FormatException ex)
                        {
                        mView.WriteToLogFile(ex.ToString());
                        MessageBox.Show ( Localizer.Message ( "Metadata_InvalidDateFormat" ) );
                        return "";
                        }
                    catch (System.Exception ex)
                        {
                        mView.WriteToLogFile(ex.ToString());
                        MessageBox.Show ( Localizer.Message("MetadataView_InvalidMetadata") + "\n\n" + ex.ToString () );  //@Messagecorrected
                        return "";
                        }
                    }
                else
                    {
                    return mContentTextbox.Text;
                    }
                }
            return "";
            }

        private string AdjustSingleDigitDate ( string dateFragment )
            {
            if (dateFragment.Length == 1)
                return string.Concat ( "0", dateFragment );
            else
                return dateFragment;
            }

        private bool IsDateMetadataSelected
            {
            get
                {
                if (mSelection != null && mMetadataListView.SelectedItems.Count > 0)
                    {
                    string entryName = mMetadataListView.SelectedItems[0].Text;
                    return entryName != null &&
    (entryName == "dtb:producedDate" || entryName == "dtb:revisionDate" || entryName == "dc:Date" || entryName == "dtb:sourceDate");
                    }
                return false;
                }
            }


        // Check an item to 
        private void mMetadataListView_ItemChecked ( object sender, ItemCheckedEventArgs e )
            {
            if (e.Item.Checked)
                {
                //if (e.Item.Tag == null) mView.AddMetadataEntry(e.Item.Text); // Avn: checking disabled.
                }
            else
                {
                if (mNameTextbox.Enabled) mNameTextbox.Focus ();
                else if (mContentTextbox.Enabled) mContentTextbox.Focus ();
                }
            }

        // Verify that the name of an entry can be modified to a new name.
        private bool CanModify ( MetadataEntryDescription description, string newName )
            {
            MetadataEntryDescription newDescription =
                MetadataEntryDescription.GetDAISYEntries ().ContainsKey ( newName ) ?
                MetadataEntryDescription.GetDAISYEntries ()[newName] : null;
            return CanRemove ( description ) &&
                (newDescription == null || CanAdd ( newDescription ));
            }

        private void mMetadataListView_SelectedIndexChanged ( object sender, EventArgs e )
            {
            if (mMetadataListView.SelectedItems.Count == 0)
                {
                mView.Selection = null;
                }
            else
                {
                ListViewItem item = mMetadataListView.SelectedItems[0];
                if ( item != null)  mView.Selection = new MetadataSelection ( (ObiNode)mView.Presentation.RootNode, this,
                    new MetadataItemSelection ( item, MetadataEntryDescription.GetDAISYEntry ( item.Text ) ) );
                }
            }

        private void SetSelection ()
            {
            if (mSelection != null)
                {
                    mMetadataListView.SelectedIndexChanged -= new EventHandler(mMetadataListView_SelectedIndexChanged);
                mSelection.Item.Item.Selected = true;
                mMetadataListView.SelectedIndexChanged += new EventHandler(mMetadataListView_SelectedIndexChanged);
                mNameTextbox.Text = mSelection.Item.Item.Text;
                mNameTextbox.AccessibleName = mNameLabel.Text.Replace ( "&", "" );
                //bool editableName = (mSelection.Item.Item.Checked || CanRemoveMetadata) && mSelection.Item.Item.Text == Localizer.Message("metadata_custom") ;
                bool editableName = mSelection.Item.Item.Text == Localizer.Message ( "metadata_custom" );

                mNameTextbox.TabStop = editableName;
                mNameTextbox.Enabled = editableName;
                mContentTextbox.Text = mSelection.Item.Item.SubItems[1].Text;
                bool editableContent = mSelection.Item.Description == null || !mSelection.Item.Description.ReadOnly;
                mContentTextbox.TabStop = editableContent;
                mContentTextbox.Enabled = editableContent;
                if (editableName)
                    mContentTextbox.AccessibleName = mContentLabel.Text.Replace ( "&", "" );
                else
                    mContentTextbox.AccessibleName = mNameTextbox.Text;
                }
            }

        // Handle the tab key
        protected override bool ProcessDialogKey ( Keys key )
            {
            if (key == Keys.Tab && ActiveControl != null)
                {
                Control c = ActiveControl;
                SelectNextControl ( c, true, true, true, true );
                if (ActiveControl != null && c.TabIndex > ActiveControl.TabIndex) System.Media.SystemSounds.Beep.Play ();
                return true;
                }
            else if (key == (Keys)(Keys.Shift | Keys.Tab) && ActiveControl != null)
                {
                Control c = ActiveControl;
                SelectNextControl ( c, false, true, true, true );
                if (ActiveControl != null && c.TabIndex < ActiveControl.TabIndex) System.Media.SystemSounds.Beep.Play ();
                return true;
                }
            else
                {
                return base.ProcessDialogKey ( key );
                }
            }

        private void MetadataView_VisibleChanged ( object sender, EventArgs e ) { if (Visible && mView!=null &&  mView.MetadataViewVisible) Focus (); }

        private void mContentTextbox_KeyDown ( object sender, KeyEventArgs e )
            {
            if (e.KeyData == Keys.Enter) CommitValues ();
            }

        private void mMetadataListView_ItemCheck ( object sender, ItemCheckEventArgs e )
            {
            if (mSelection == null && mMetadataListView.SelectedIndices.Count > 0)
                e.NewValue = e.CurrentValue;
            else if (!m_IsImportingMetadata)
                {
                if (e.CurrentValue == CheckState.Checked && !CanRemoveMetadata
                    && mMetadataListView.Items[e.Index].Tag != null)
                    e.NewValue = CheckState.Checked;
                else if (mMetadataListView.Items[e.Index].Tag == null)
                    e.NewValue = CheckState.Unchecked;
                }
            }

        private void mContentTextbox_Leave ( object sender, EventArgs e )
            {
            if (mContentTextbox.Text == "")
                {
                //MessageBox.Show ( Localizer.Message ( "MetadataView_CannotCommitEmptyContent" ), Localizer.Message ( "Caption_Error" ) );
                if (mSelection != null && mSelection.Item.Entry == null
                    && mMetadataListView.SelectedItems.Count > 0 && mMetadataListView.SelectedItems[0] != null && mMetadataListView.SelectedItems[0].Checked)
                    mMetadataListView.SelectedItems[0].Checked = false;
                }
            }

        private void mNameTextbox_Leave ( object sender, EventArgs e )
            {
            if (mNameTextbox.Text == "")
                {
                //MessageBox.Show ( Localizer.Message ( "MetadataView_CannotCommitEmptyContent" ), Localizer.Message ( "Caption_Error" ) );
                }
            }

        private void mMetadataListView_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
             if ( mMetadataTooltipDictionary.ContainsKey (e.Item.Text))    e.Item.ToolTipText = Localizer.Message(mMetadataTooltipDictionary[e.Item.Text]);            
        }

        private void m_BtnContextMenu_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            mMetadataContextMenuStrip.Show(ptLowerLeft);        
        }

        private void SetDefaultMetadataStripMenuItem_Click(object sender, EventArgs e)
        {
            mView.LoadDefaultMetadata(false);
        }

        private void SetDefaultMetadataOverwriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mView.LoadDefaultMetadata(true);
        }

        private void SaveAsDefaultMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mView.SaveDefaultMetadatas();
        }
        public void SetFont() //@fontconfig
        {
            this.Font = new Font(mView.ObiForm.Settings.ObiFont, this.Font.Size, FontStyle.Regular);
            mMetadataListView.Font = mNameLabel.Font = mNameTextbox.Font = mContentLabel.Font = mContentTextbox.Font = mUpdateButton.Font = new Font(mView.ObiForm.Settings.ObiFont, this.Font.Size, FontStyle.Regular);
            mMetadataContextMenuStrip.Font = new Font(mView.ObiForm.Settings.ObiFont, mMetadataContextMenuStrip.Font.Size, FontStyle.Regular);
        }
        }

    public class MetadataItemSelection
        {
        private ListViewItem mItem;                     // item in the list view
        private MetadataEntryDescription mDescription;  // and corresponding description (may be null for free metadata)

        public MetadataItemSelection ( ListViewItem item, MetadataEntryDescription description )
            {
            mItem = item;
            mDescription = description;
            }

        public urakawa.metadata.Metadata Entry { get { return (urakawa.metadata.Metadata)mItem.Tag; } }
        public MetadataEntryDescription Description { get { return mDescription; } }
        public ListViewItem Item { get { return mItem; } }
        }
    }
