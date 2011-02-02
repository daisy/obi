using urakawa.core;

namespace Obi.Events.Node
{
    public class SetPageEventArgs : PhraseNodeEventArgs
    {
        private int mPageNumber;  // new page number

        public int PageNumber
        {
            get { return mPageNumber; }
        }

        public SetPageEventArgs(object origin, PhraseNode node, int pageNumber)
            : base(origin, node)
        {
            mPageNumber = pageNumber;
        }
    }
}
