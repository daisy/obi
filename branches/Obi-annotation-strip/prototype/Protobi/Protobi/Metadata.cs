using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    public class Metadata
    {
        private MetadataModel mModel;
        private MetadataDialog mUserControl;

        public MetadataModel MetadataModel { get { return mModel; } }

        public Metadata()
        {
            mModel = new MetadataModel();
            mUserControl = new MetadataDialog();
        }

        public void Edit()
        {
            mUserControl.TitleBox.Text = mModel.DcTitle.Content;
            mUserControl.PublisherBox.Text = mModel.DcPublisher.Content;
            mUserControl.DateBox.Text = mModel.DcDate.Date;
            mUserControl.DateEventBox.Text = mModel.DcDate.Event;
            mUserControl.IdBox.Text = mModel.DcIdentifier.Content;
            mUserControl.IdSchemeBox.Text = mModel.DcIdentifier.AttrValue;
            mUserControl.LanguageBox.SelectedItem = mModel.DcLanguage.CultureInfo;
            mUserControl.ShowDialog();
            if (mUserControl.DialogResult == DialogResult.OK)
            {
                if (mUserControl.DidTitleChange)
                    mModel.DcTitle.Content = mUserControl.TitleBox.Text;
                if (mUserControl.DidPublisherChange)
                    mModel.DcPublisher.Content = mUserControl.PublisherBox.Text;
                mModel.DcDate.Date = mUserControl.DateBox.Text;
                mModel.DcDate.Event = mUserControl.DateEventBox.Text;
                if (mUserControl.DidIdChange)
                    mModel.DcIdentifier.Content = mUserControl.IdBox.Text;
                mModel.DcIdentifier.AttrValue = mUserControl.IdSchemeBox.Text;
                mModel.DcLanguage.CultureInfo = (CultureInfo)mUserControl.LanguageBox.SelectedItem;
            }
        }
    }

    public class MetadataModel
    {
        private MetadataItem mDcTitle;
        private MetadataItem mDcPublisher;
        private MetadataDate mDcDate;
        private MetadataOptAttr mDcIdentifier;
        private MetadataLanguage mDcLanguage;

        private FixedMetadataItem mDcFormat;

        public MetadataItem DcTitle { get { return mDcTitle; } }
        public MetadataItem DcPublisher { get { return mDcPublisher; } }
        public MetadataDate DcDate { get { return mDcDate; } }
        public MetadataOptAttr DcIdentifier { get { return mDcIdentifier; } }
        public FixedMetadataItem DcFormat { get { return mDcFormat; } }
        public MetadataLanguage DcLanguage { get { return mDcLanguage; } }

        public bool AllSet { get { return DcTitle.IsSet && DcPublisher.IsSet && DcIdentifier.IsSet; } }

        public MetadataModel()
        {
            mDcTitle = new MetadataItem("dc:Title", true);
            mDcPublisher = new MetadataItem("dc:Publisher", true);
            mDcDate = new MetadataDate();
            mDcIdentifier = new MetadataOptAttr("dc:identifier", true, "scheme");
            mDcLanguage = new MetadataLanguage();
            mDcFormat = new FixedMetadataItem("dc:Format", "ANSI/NISO Z39.86-2005");
        }
    }

    public class FixedMetadataItem
    {
        protected string mName;
        protected string mContent;

        public string Name { get { return mName; } }
        public string Content { get { return mContent; } }

        public FixedMetadataItem(string name, string content)
        {
            mName = name;
            mContent = content;
        }
    }

    public class MetadataItem: FixedMetadataItem
    {
        private bool mRequired;
        private bool mSet;

        public new string Content
        {
            // get { return mContent == null && mRequired ? Localizer.GetString("required") : mContent; }
            get { return mContent; }
            set
            {
                mContent = value;
                mSet = true;
            }
        }

        public bool IsSet { get { return mSet; } }

        public MetadataItem(string name, bool required)
            : base(name, null)
        {
            mRequired = required;
            mSet = false;
        }
    }

    public class MetadataOptAttr : MetadataItem
    {
        private string mAttrName;
        private string mAttrValue;

        public string AttrName { get { return mAttrName; } }
        public string AttrValue
        {
            get { return mAttrValue; }
            set { mAttrValue = value; }
        }

        public MetadataOptAttr(string name, bool required, string optname)
            : base(name, required)
        {
            mAttrName = optname;
            mAttrValue = null;
        }
    }

    public class MetadataDate
    {
        private DateTime mDate;
        private string mEvent;

        public string Name { get { return "dc:date"; } }

        public string Date
        {
            get { return mDate.ToString("yyyy-MM-dd"); }
            set
            {
                DateTime d = new DateTime();
                if (DateTime.TryParse(value, out d)) mDate = d;
            }
        }

        public string Event
        {
            get { return mEvent; }
            set { mEvent = value; }
        }

        public MetadataDate()
        {
            mDate = DateTime.Today;
            mEvent = null;
        }
    }

    public class MetadataLanguage
    {
        private CultureInfo mCultureInfo;
        
        public string Name { get { return "dc:Language"; } }

        public CultureInfo CultureInfo
        {
            get { return mCultureInfo;  }
            set { mCultureInfo = value; }
        }

        public MetadataLanguage()
        {
            mCultureInfo = CultureInfo.CurrentCulture;
        }
    }
}
