using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    class ReplaceEmptyNode : Command
    {
                private EmptyNode mENode;
        private PhraseNode mPNode;
        private SectionNode mParent;
        private int mIndex;

        public ReplaceEmptyNode(ProjectView.ProjectView view, EmptyNode ENode, PhraseNode PNode):base(view)   
        {
                        mENode = ENode;
            mParent = ENode.ParentAs<SectionNode>() ;
            mIndex = ENode.Index;
            mPNode = PNode;
        }

        public override void execute()
        {
            mPNode.NodeKind = mENode.NodeKind;
            mPNode.PageNumber = mENode.PageNumber;
                        mENode.detach();
                        mParent.insert( mPNode, mIndex);
                        
                    }

        public override void unExecute()
        {
            mENode.NodeKind = mPNode.NodeKind;
            mENode.PageNumber = mPNode.PageNumber;
                        mPNode.detach();
            mParent.insert(mENode, mIndex);
            base.unExecute();
                    }



    }
}
