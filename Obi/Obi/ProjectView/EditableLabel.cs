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
        private bool mAllowEmptyLabel;                // accept empty labels or not
        private bool mEditable;                       // change the label status

        public event EventHandler LabelEditedByUser;  // raised when the user has edited the label

        /// <summary>
        /// Create a new label. Don't forget to set its Label property.
        /// </summary>
        public EditableLabel()
        {
            InitializeComponent();
            mAllowEmptyLabel = false;
        }


        /// <summary>
        /// True if an empty label can be input.
        /// </summary>
        public bool AllowEmptyLabel
        {
            get { return mAllowEmptyLabel; }
            set
            {
                if (!value && mLabel.Text == "") throw new Exception("Cannot disallow empty labels when the label is empty.");
                mAllowEmptyLabel = value;
            }
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
                mEditable = value;
                mOKButton.Enabled = mEditable;
                mOKButton.Visible = mEditable;
                mCancelButton.Enabled = mEditable;
                mCancelButton.Visible = mEditable;
                mTextBox.Enabled = mEditable;
                mTextBox.Visible = mEditable;
                mLabel.Visible = !mEditable;
                if (mEditable)
                {
                    mTextBox.Text = "";
                    mTextBox.SelectedText = mLabel.Text;
                    mTextBox.SelectAll();
                    mTextBox.Focus();
                    Size = new Size(Width, mOKButton.Location.Y + mOKButton.Height + mOKButton.Margin.Bottom);
                }
                else
                {
                    Size = new Size(Width, mLabel.Location.Y + mLabel.Height + mLabel.Margin.Bottom);
                }
            }
        }

        /// <summary>
        /// The font size (in points of the label). 
        /// </summary>
        public float FontSize
        {
            get { return mLabel.Font.Size; }
            set
            {
                mLabel.Font = new Font(mLabel.Font.FontFamily, value);
                mTextBox.Font = new Font(mLabel.Font.FontFamily, value);
                int h = mTextBox.Location.Y + mTextBox.Height + mTextBox.Margin.Bottom;
                mOKButton.Location = new Point(mOKButton.Location.X, h);
                mCancelButton.Location = new Point(mCancelButton.Location.X, h);
                if (mEditable)
                {
                    Size = new Size(Width, h + mOKButton.Height + mOKButton.Margin.Bottom);
                }
                else
                {
                    Size = new Size(Width, mLabel.Location.Y + mLabel.Height + mLabel.Margin.Bottom);
                }
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
                if (!mAllowEmptyLabel && value == "") throw new Exception("Empty label is not allowed.");
                mLabel.Text = value;
                mTextBox.Text = value;
                int wb = mCancelButton.Location.X + mCancelButton.Width + mCancelButton.Margin.Right;
                int wl = mTextBox.Location.X + mTextBox.Width + mTextBox.Margin.Right;
                MinimumSize = new Size(wb > wl ? wb : wl, MinimumSize.Height);
            }
        }

        /// <summary>
        /// Make sure that the label is not editable when it is first loaded.
        /// </summary>
        private void EditableLabel_Load(object sender, EventArgs e)
        {
            Editable = false;
        }

        /// <summary>
        /// When the user clicks the OK button, the label is updated
        /// and the control returns back to non-editable.
        /// </summary>
        private void mOKButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(mEditable,
                "This button cannot be clicked when the label is not editable.");
            UpdateText();
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
        }

        /// <summary>
        /// Clicking the label makes it editable.
        /// </summary>
        private void EditableLabel_Click(object sender, EventArgs e)
        {
            if (!mEditable) Editable = true;
        }

        private void mLabel_Click(object sender, EventArgs e)
        {
            if (!mEditable) Editable = true;
        }

        /// <summary>
        /// Pressing enter is like pressing OK.
        /// Escape is like cancel.
        /// </summary>
        private void mTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                UpdateText();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Editable = false;
            }
        }

        private void UpdateText()
        {
            if (mAllowEmptyLabel || mTextBox.Text != "")
            {
                Label = mTextBox.Text;
                if (LabelEditedByUser != null) LabelEditedByUser(this, new EventArgs());
            }
            Editable = false;
        }

        /// <summary>
        /// If the label loses its focus then it becomes non-editable.
        /// </summary>
        private void EditableLabel_Leave(object sender, EventArgs e)
        {
            Editable = false;
        }
    }
}