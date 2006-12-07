using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    /// <summary>
    /// User profile dialog. The user can change her profile with this dialog.
    /// The name cannot be removed (if erased, reverts to the previous value.)
    /// </summary>
    public partial class UserProfile : Form
    {
        private Obi.UserProfile mProfile;  // the profile to edit

        /// <summary>
        /// Create a new user profile dialog for editing a given user profile.
        /// </summary>
        /// <param name="profile">The user profile to edit.</param>
        public UserProfile(Obi.UserProfile profile)
        {
            InitializeComponent();
            mProfile = profile;
            mNameBox.Text = mProfile.Name;
            mOrganizationBox.Text = mProfile.Organization;
            foreach (CultureInfo c in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                mCultureBox.Items.Add(c);
            }
            mCultureBox.SelectedItem = mProfile.Culture;
        }

        /// <summary>
        /// Make the changes when the user has clicked OK.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (mNameBox.Text != "")
            {
                mProfile.Name = mNameBox.Text;
            }
            mProfile.Organization = mOrganizationBox.Text == "" ? null : mOrganizationBox.Text;
            mProfile.Culture = (CultureInfo)mCultureBox.SelectedItem;
        }
    }
}