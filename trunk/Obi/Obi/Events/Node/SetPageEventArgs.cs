using urakawa.core;

namespace Obi.Events.Node
{
    class SetPageEventArgs : NodeEventArgs
    {
        private int mPageNumber;  // new page number

        public int PageNumber
        {
            get { return mPageNumber; }
        }

        public SetPageEventArgs(object origin, CoreNode node, int pageNumber)
            : base(origin, node)
        {
            mPageNumber = pageNumber;
        }
    }
}
