using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class EditableLabel : UserControl
    {
        private float mButtonsBaseFontSize;           // base font size for the buttons
        private bool mEditable;                       // change the label status
        private float mLabelBaseFontSize;             // base font size for the label

        public event EventHandler EditableChanged;    // editable status changed
        public event EventHandler LabelEditedByUser;  // raised when the user has edited the label
        public event EventHandler AddComment;
        public event EventHandler CloseComment;

        public bool m_EditPhraseComment = false;
        
        private EmptyNode m_Node;
        private string m_CommentText;

        /// <summary>
        /// Create a new label. Don't forget to set its Label property.
        /// </summary>
        public EditableLabel()
        {
            InitializeComponent();
            mButtonsBaseFontSize = mOKButton.Font.SizeInPoints;
            mLabelBaseFontSize = mLabel.Font.SizeInPoints;
        }
        public EditableLabel(EmptyNode node) // @Comment-todo
        {
            m_Node = node;
            InitializeComponent();
            mTextBox.MaxLength = 200;
            mTextBox.Text = m_Node.CommentText;
            m_EditPhraseComment = true;
        }

       




        /// <summary>
        /// Make the label editable or not.
        /// When editable, controls are shown;
        /// otherwise they are hidden.
        /// </summary>
        public bool Editable
        {
            get { return mEditable; }
            set
            {
                if (mEditable != value)
                {
                    mEditable = value;
                    mOKButton.Enabled = mOKButton.Visible = mCancelButton.Enabled = mCancelButton.Visible =
                    mTextBox.Enabled = mTextBox.Visible =
                        mEditable;
                    mLabel.Visible = !mEditable;
                    if (mEditable)
                    {
                        if (!m_EditPhraseComment) // @Comment-todo
                        {
                            mTextBox.Text = mLabel.Text;
                        }
                        mTextBox.SelectAll();
                        mTextBox.Focus();
                    }
                    UpdateSize();
                    if (!m_EditPhraseComment && EditableChanged != null) EditableChanged(this, new EventArgs());
                }
            }
        }

        public string CommentText // @Comment-todo
        {
            get
            {
                return m_CommentText;
            }
        }

        /// <summary>
        /// The (uneditable) label that is displayed.
        /// When the label is changed, make sure that it shows,
        /// thus the control becomes uneditable.
        /// </summary>
        public string Label
        {
            get { return mLabel.Text; }
            set
            {
                if (value != null && value != "")
                {
                    mLabel.Text = value;
                    mTextBox.Text = value;
                    int wb = mCancelButton.Location.X + mCancelButton.Width + mCancelButton.Margin.Right;
                    int wl = mLabel.Location.X + mLabel.Width + mLabel.Margin.Right;
                    MinimumSize = new Size(wb > wl ? wb : wl, MinimumSize.Height);
                }
            }
        }

        /// <summary>
        /// Update the colors of the label when a change in color settings is made.
        /// </summary>
        /// <param name="settings"></param>
        public void UpdateColors(ColorSettings settings)
        {
            // the rest of the colors are handled by the strip
            mTextBox.BackColor = settings.EditableLabelTextBackColor;
        }

        /// <summary>
        /// Change the size of the control for the given zoom factor.
        /// </summary>
        public float ZoomFactor
        {
            set
            {
                if (value > 0.0f)
                {
                    mLabel.Font =
                    mTextBox.Font =
                         new Font(mLabel.Font.FontFamily, value * mLabelBaseFontSize);
                    mOKButton.Font =
                    mCancelButton.Font =
                        new Font(mOKButton.Font.FontFamily, value * mButtonsBaseFontSize);
                    UpdateSize();
                }
            }
        }


        /// <summary>
        /// When the user clicks the cancel button, the controls becomes
        /// non-editable again but the label is not updated.
        /// </summary>
        private void mCancelButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(mEditable,
                "This button cannot be clicked when the label is not editable.");
            Editable = false;
            if (m_EditPhraseComment) // @Comment-todo
            {
                if (CloseComment != null) CloseComment(this, new EventArgs());
            }
        }

        /// <summary>
        /// When the user clicks the OK button, the label is updated
        /// and the control returns back to non-editable.
        /// </summary>
        private void mOKButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(mEditable,
                "This button cannot be clicked when the label is not editable.");
            if (m_EditPhraseComment && m_Node != null) // @Comment-todo
            {
                //m_Node.CommentText = mTextBox.Text;
                m_CommentText = mTextBox.Text;
            }

            UpdateText();
        }

        /// <summary>
        /// Pressing enter is like pressing OK.
        /// Escape is like cancel.
        /// </summary>
        private void mTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (m_EditPhraseComment && m_Node != null) // @Comment-todo
                {
                    //m_Node.CommentText = mTextBox.Text;
                    m_CommentText = mTextBox.Text;
                }
                UpdateText();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Editable = false;
                if (m_EditPhraseComment) // @Comment-todo
                {
                    if (CloseComment != null) CloseComment(this, new EventArgs());
                }
            }
        }

        // Update the size of the label when its zoom factor changes, it becomes editable, or the text changes.
        private void UpdateSize()
        {
            int h = mTextBox.Location.Y + mTextBox.Height + mTextBox.Margin.Bottom;
            mOKButton.Location = new Point(mOKButton.Location.X, h);
            mCancelButton.Location =
                new Point(mOKButton.Location.X + mOKButton.Width + mOKButton.Margin.Right + mCancelButton.Margin.Left, h);
            int wlabel = mLabel.Location.X + mLabel.Width + mLabel.Margin.Right;
            if (m_EditPhraseComment) // @Comment-todo
            {
                wlabel = wlabel * 4;
                mLabel.Text = string.Empty;
            }
            int wbuttons = mCancelButton.Location.X + mCancelButton.Width + mCancelButton.Margin.Right;
            Size = new Size(wlabel > wbuttons ? wlabel : wbuttons,
                mEditable ? mOKButton.Location.Y + mOKButton.Height + mOKButton.Margin.Bottom :
                    mLabel.Location.Y + mLabel.Height + mLabel.Margin.Bottom);

        }

        // Update the text in the label/textbox
        private void UpdateText()
        {
            if (!m_EditPhraseComment) // @Comment-todo
            {
                if (mTextBox.Text != "")
                {
                    Label = mTextBox.Text;
                    if (LabelEditedByUser != null) LabelEditedByUser(this, new EventArgs());
                }
            }
            else
            {
                if (m_Node != null && m_CommentText != null)
                {
                    if (AddComment != null) AddComment(this, new EventArgs());
                }
            }
            Editable = false;
            m_EditPhraseComment = false;
        }

        // If the label loses its focus then it becomes non-editable.
        private void EditableLabel_Leave(object sender, EventArgs e)
        {
            if (Editable) Editable = false;
        }

        private void mLabel_Enter(object sender, EventArgs e) { OnEnter(e); }
        private void mLabel_Click(object sender, EventArgs e) { OnClick(e); }

        public void SetFont(Settings Settings) //@fontconfig
        {
            mCancelButton.Font = new Font(Settings.ObiFont, mCancelButton.Font.Size, FontStyle.Regular);
            mOKButton.Font = new Font(Settings.ObiFont, mOKButton.Font.Size, FontStyle.Regular);
            mTextBox.Font = new Font(Settings.ObiFont, mTextBox.Font.Size, FontStyle.Regular);

            mLabel.Font = new Font(Settings.ObiFont, this.mLabel.Font.Size, FontStyle.Regular);
            if (this.Width < this.mLabel.Width)
            {
                this.Width = this.mLabel.Width;
            }
        }
    }
}