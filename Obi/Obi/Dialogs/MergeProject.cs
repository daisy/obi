using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Obi.Dialogs
{
    public partial class MergeProject : Form
    {
        private List<string> m_filePaths;
        public MergeProject()
        {
            InitializeComponent();
        }
        public MergeProject(string filepath)
        {
            InitializeComponent();
            m_filePaths = new List<string>();
            m_filePaths.Add(filepath);
            string[] arr = filepath.Split(Path.DirectorySeparatorChar);
            lstManualArrange.Items.Add(arr[arr.Length-2]+"\\"+arr[arr.Length-1] );    
        }
        public string[] FilesPaths
        {
            get
            {
                return m_filePaths.ToArray();

            }
        }
        private void m_btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog select_File = new OpenFileDialog();
            select_File.Filter = Localizer.Message("Obi_ProjectMergeFilter");
            if (select_File.ShowDialog() == DialogResult.OK)
            {
                mOKButton.Enabled = true;
                string fileName = select_File.FileName;
                string nameOfFile = System.IO.Path.GetFullPath(fileName);
                m_filePaths.Add(fileName);
                string[] arr = fileName.Split(Path.DirectorySeparatorChar);
                lstManualArrange.Items.Add((arr[arr.Length - 2] + "\\" + arr[arr.Length - 1]));

                lstManualArrange.SelectedIndex = -1;

            }
        }

        private void m_btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstManualArrange.Items.Count != 0)
                {
                    if (lstManualArrange.SelectedIndex >= 0)
                    {
                        object item = lstManualArrange.SelectedItem;
                        int tempIndex = lstManualArrange.SelectedIndex;
                        lstManualArrange.Items.Remove(item);
                         m_filePaths.RemoveAt(tempIndex);
                      
                        if (lstManualArrange.Items.Count != 0)
                        {
                            if (tempIndex > lstManualArrange.Items.Count - 1)
                            {
                                lstManualArrange.SelectedIndex = lstManualArrange.Items.Count - 1;
                            }
                            else
                            {
                                lstManualArrange.SelectedIndex = tempIndex;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (lstManualArrange.Items.Count < 1)
            {
                mOKButton.Enabled = false;
                m_btnRemove.Enabled = false;
                m_btnMoveUp.Enabled = false;
                m_btnMoveDown.Enabled = false;
            }
        }

        private void m_btnMoveUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstManualArrange.Items.Count != 0)
                {

                    if (lstManualArrange.SelectedIndex != 0 && lstManualArrange.SelectedIndex != -1)
                    {
                        int tempIndexStore = lstManualArrange.SelectedIndex;
                        object item = lstManualArrange.SelectedItem;

                        int index = lstManualArrange.SelectedIndex;
                       
                        object itemInList = m_filePaths[index];

                        lstManualArrange.Items.RemoveAt(index);
                        m_filePaths.RemoveAt(index);


                        lstManualArrange.Items.Insert(index - 1, item);
                        m_filePaths.Insert(index - 1, itemInList.ToString());
                        if ((tempIndexStore - 1) != -1)
                            lstManualArrange.SelectedIndex = tempIndexStore - 1;

                    }

                }

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void m_btnMoveDown_Click(object sender, EventArgs e)
        {
            try
            {
                int index = lstManualArrange.SelectedIndex;

                if (lstManualArrange.Items.Count != 0)
                {
                    if (lstManualArrange.SelectedIndex != lstManualArrange.Items.Count - 1 && lstManualArrange.SelectedIndex != -1)
                    {
                        int tempIndexStore = lstManualArrange.SelectedIndex;
                        object item = lstManualArrange.SelectedItem;

                        object itemInList = m_filePaths[index];
                        lstManualArrange.Items.RemoveAt(index);

                        m_filePaths.RemoveAt(index);

                        lstManualArrange.Items.Insert(index + 1, item);

                        m_filePaths.Insert(index + 1, itemInList.ToString());
                        //   if ((tempIndexStore+1) != lstManualArrange.Items.Count - 1)
                        lstManualArrange.SelectedIndex = tempIndexStore + 1;
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void mbtnAscendingOrder_Click(object sender, EventArgs e)
        {


            m_filePaths.Sort();
            lstManualArrange.Items.Clear();
            foreach (string str in m_filePaths)
            {
                if (str != null)
                {
                    string[] arr = str.Split(Path.DirectorySeparatorChar);
                    lstManualArrange.Items.Add(arr[arr.Length - 2] + "\\" + arr[arr.Length - 1]);                      
                }
            }
            m_btnMoveUp.Enabled = false;
            m_btnMoveDown.Enabled = false;
            m_btnRemove.Enabled = false;
        }

        private void mbtnDesendingOrder_Click(object sender, EventArgs e)
        {


            m_filePaths.Sort();
            int totLength = m_filePaths.Count;
            List<string> tempDescending = new List<string>();



            for (int i = totLength - 1; i >= 0; i--)
            {
                tempDescending.Add(m_filePaths[i]);
            }

            m_filePaths = tempDescending;

            lstManualArrange.Items.Clear();
            foreach (string str in m_filePaths)
            {
                if (str != null)
                {
                    string[] arr = str.Split(Path.DirectorySeparatorChar);
                    lstManualArrange.Items.Add(arr[arr.Length - 2] + "\\" + arr[arr.Length - 1]);   
                }
            }
            m_btnMoveUp.Enabled = false;
            m_btnMoveDown.Enabled = false;
            m_btnRemove.Enabled = false;
        }

        private void lstManualArrange_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstManualArrange.SelectedIndex == 0)
            {
                m_btnRemove.Enabled = true;
                if (lstManualArrange.Items.Count != 1)
                {
                    m_btnMoveUp.Enabled = false;
                    m_btnMoveDown.Enabled = true;

                }
                else
                {
                    m_btnMoveUp.Enabled = false;
                    m_btnMoveDown.Enabled = false;

                }
            }
            else if (lstManualArrange.SelectedIndex == lstManualArrange.Items.Count - 1)
            {
                m_btnRemove.Enabled = true;
                if (lstManualArrange.Items.Count != 1)
                {
                    m_btnMoveUp.Enabled = true;
                    m_btnMoveDown.Enabled = false;

                }
                else
                {
                    m_btnMoveUp.Enabled = false;
                    m_btnMoveDown.Enabled = false;

                }
            }
            else if (lstManualArrange.SelectedIndex > 0)
            {
                m_btnRemove.Enabled = true;
                if (lstManualArrange.Items.Count != 1)
                {
                    m_btnMoveUp.Enabled = true;
                    m_btnMoveDown.Enabled = true;

                }
                else
                {
                    m_btnMoveUp.Enabled = false;
                    m_btnMoveDown.Enabled = false;

                }
            }
            else if (lstManualArrange.SelectedIndex < 0)
            {
                m_btnMoveUp.Enabled = false;
                m_btnMoveDown.Enabled = false;
                m_btnRemove.Enabled = false;
            }
        }

    }
}