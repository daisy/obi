using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace UrakawaPrototype
{
    public partial class StructureView : UserControl
    {

        private TreeNode mClipboard;
        private int mFontSizeIterations;
        private Font mOriginalFont;

        public StructureView()
        {
            InitializeComponent();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editLabel();
        }

        private void asChildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addChild();
        }
        
        private void indentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            indent();
        }

        private void outdentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outdent();
        }

        private void renameLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rename();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            delete();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            //this function needs to be here (i think.??)
            //although it does nothing.
        }

        private void asSiblingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.addSibling();

        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            moveDown();
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            moveUp();
        }
        private void renameLabelToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            rename();
        }

        private void load(object sender, EventArgs e)
        {
            this.mOriginalFont = (Font)this.treeView1.Font.Clone();
            this.mFontSizeIterations = 0;
            this.treeView1.ExpandAll();
        }

        private void addNewNode(TreeNode parent)
        {
            TreeNode newnode = new TreeNode("new node");
            if (parent != null)
            {
                parent.Nodes.Add(newnode);
                newnode.EnsureVisible();
                newnode.BeginEdit(); 
            }
            else
            {
                this.treeView1.Nodes.Add(newnode);
                newnode.EnsureVisible();
                newnode.BeginEdit();
            }

            treeView1.SelectedNode = newnode;
        }

        public void delete()
        {
            TreeNode sel = this.treeView1.SelectedNode;

            if (sel != null)
            {
                this.treeView1.Nodes.Remove(sel);
            }
        }

        public void addChild()
        {
            TreeNode sel = this.treeView1.SelectedNode;
            addNewNode(sel);
        }

        public void addSibling()
        {
            if (this.treeView1.SelectedNode == null)
            {
                addNewNode(null);
            }

            else
            {
                if (this.treeView1.SelectedNode.Parent == null)
                {
                    addNewNode(null);
                }
                else
                {
                    addNewNode(this.treeView1.SelectedNode.Parent);
                }
            }

        }

        public void editLabel()
        {
            LabelEditor dlg = new LabelEditor();
            TreeNode sel = this.treeView1.SelectedNode;
            if (sel != null)
            {
                dlg.setText(sel.Text);

                dlg.ShowDialog();

                sel.Text = dlg.getText();

                if (dlg.didImageChange() == true)
                {
                    Image newImage = dlg.getNewImage();
                    this.imageList1.Images.Add(newImage);
                    sel.ImageIndex = this.imageList1.Images.Count - 1;
                    sel.SelectedImageIndex = this.imageList1.Images.Count - 1;
                }
            }
        }

        public void indent()
        {
            //if possible, make this a child of its previous sibling
            TreeNode sel = this.treeView1.SelectedNode;

            TreeNode sib = sel.PrevNode;

            if (sib != null)
            {
                TreeNode clone = (TreeNode)sel.Clone();

                sib.Nodes.Add(clone);

                this.treeView1.Nodes.Remove(sel);
            }

        }

        public void outdent()
        {
            //if possible, make this child node a sibling of its parent
            TreeNode sel = this.treeView1.SelectedNode;

            TreeNode new_parent = sel.Parent.Parent;

            if (new_parent != null)
            {
                TreeNode clone = (TreeNode)sel.Clone();

                new_parent.Nodes.Add(clone);

                this.treeView1.Nodes.Remove(sel);
            }
            else
            {
                //must be at the top of the tree levels
                TreeNode clone = (TreeNode)sel.Clone();

                treeView1.Nodes.Add(clone);

                this.treeView1.Nodes.Remove(sel);
            }
        }

        public void moveUp()
        {
            
        }

        public void moveDown()
        {
        }

        public void next()
        {
            TreeNode sel = this.treeView1.SelectedNode;

            if (sel != null)
            {
                TreeNode next = this.treeView1.SelectedNode.NextVisibleNode;

                this.treeView1.SelectedNode = next;
            }
            else
            {
                if (this.treeView1.Nodes.Count > 0)
                {
                    this.treeView1.SelectedNode = this.treeView1.Nodes[0];
                }
            }
        }

        public void previous()
        {
            TreeNode sel = this.treeView1.SelectedNode;

            if (sel != null)
            {
                TreeNode prev = this.treeView1.SelectedNode.PrevVisibleNode;

                this.treeView1.SelectedNode = prev;
            }
            else
            {
                if (this.treeView1.Nodes.Count > 0)
                {
                    this.treeView1.SelectedNode = this.treeView1.Nodes[0];
                }
            }
        }

        public void cut()
        {
            TreeNode sel = this.treeView1.SelectedNode;

            if (sel != null)
            {
                mClipboard = (TreeNode)sel.Clone();
                this.treeView1.Nodes.Remove(sel);
            }
        }

        public void copy()
        {
            TreeNode sel = this.treeView1.SelectedNode;

            if (sel != null)
            {
                mClipboard = (TreeNode)sel.Clone();
            }
        }

        public void paste()
        {
            TreeNode sel = this.treeView1.SelectedNode;

            if (sel != null)
            {
                if (mClipboard != null)
                    sel.Nodes.Add(mClipboard);
            }
            else
            {
                if (mClipboard != null)
                    this.treeView1.Nodes.Add(mClipboard);
            }
        }


        public void selectAll()
        {
          
        }

        public void rename()
        {
            TreeNode sel = this.treeView1.SelectedNode;
            
            if (sel != null)
                sel.BeginEdit();
        }

        public void rename(string newname)
        {
            TreeNode sel = this.treeView1.SelectedNode;

            if (sel != null)
                sel.Text = newname;
        }

        public void resizeFont()
        {
            if (this.mFontSizeIterations > 3)
            {
                this.treeView1.Font = mOriginalFont;
                this.mFontSizeIterations = 0;
            }
            else
            {
                Font newfont = new Font(
                    this.treeView1.Font.FontFamily,
                    this.treeView1.Font.Size + 3,
                    this.treeView1.Font.Style);

                this.treeView1.Font = newfont;

                this.mFontSizeIterations++;
            }
        }
    }
}
