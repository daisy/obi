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


        /// <summary>
        /// Create a new label. Don't forget to set its Label property.
        /// </summary>
        public EditableLabel()
        {
            InitializeComponent();
            Editable = false;
            mButtonsBaseFontSize = mOKButton.Font.SizeInPoints;
            mLabelBaseFontSize = mLabel.Font.SizeInPoints;
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
                if (EditableChanged != null) EditableChanged(this, new EventArgs());
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
                if (value == "") throw new Exception("Empty label is not allowed.");
                mLabel.Text = value;
                mTextBox.Text = value;
                int wb = mCancelButton.Location.X + mCancelButton.Width + mCancelButton.Margin.Right;
                int wl = mLabel.Location.X + mLabel.Width + mLabel.Margin.Right;
                MinimumSize = new Size(wb > wl ? wb : wl, MinimumSize.Height);
            }
        }

        public void UpdateColors(ColorSettings settings)
        {
            mTextBox.BackColor = settings.EditableLabelTextBackColor;
        }

        /// <summary>
        /// Change the size of the control for the given zoom factor.
        /// </summary>
        public double ZoomFactor
        {
            set
            {
                if (value > 0.0)
                {
                    mLabel.Font =
                    mTextBox.Font =
                         new Font(mLabel.Font.FontFamily, (float)value * mLabelBaseFontSize);
                    mOKButton.Font =
                    mCancelButton.Font =
                        new Font(mOKButton.Font.FontFamily, (float)value * mButtonsBaseFontSize);
                    int h = mTextBox.Location.Y + mTextBox.Height + mTextBox.Margin.Bottom;
                    mOKButton.Location = new Point(mOKButton.Location.X, h);
                    mCancelButton.Location =
                        new Point(mOKButton.Location.X + mOKButton.Width + mOKButton.Margin.Right + mCancelButton.Margin.Left, h);
                    int wlabel = mLabel.Location.X + mLabel.Width + mLabel.Margin.Right;
                    int wbuttons = mCancelButton.Location.X + mCancelButton.Width + mCancelButton.Margin.Right;
                    Size = new Size(wlabel > wbuttons ? wlabel : wbuttons,
                        mEditable ? h + mOKButton.Height + mOKButton.Margin.Bottom :
                            mLabel.Location.Y + mLabel.Height + mLabel.Margin.Bottom);
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

        // Update the text in the label/textbox
        private void UpdateText()
        {
            if (mTextBox.Text != "")
            {
                Label = mTextBox.Text;
                if (LabelEditedByUser != null) LabelEditedByUser(this, new EventArgs());
            }
            Editable = false;
        }

        // If the label loses its focus then it becomes non-editable.
        private void EditableLabel_Leave(object sender, EventArgs e)
        {
            if (Editable) Editable = false;
        }
    }
}