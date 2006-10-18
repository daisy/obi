namespace Obi.Events.Node.Section
{
    public delegate void CutSectionNodeHandler(object sender, EventArgs e);
    public delegate void DeletedSectionNodeHandler(object sender, EventArgs e);
    public delegate void RequestToAddChildSectionNodeHandler(object sender, EventArgs e);
    public delegate void RequestToAddSiblingSectionNodeHandler(object sender, EventArgs e);
    public delegate void RequestToCutSectionNodeHandler(object sender, EventArgs e);
    public delegate void RequestToDeleteSectionNodeHandler(object sender, EventArgs e);
    public delegate void UndidPasteSectionNodeHandler(object sender, EventArgs e);


    /// <summary>
    /// Base class for SectionNode event arguments.
    /// </summary>
    public class EventArgs: System.EventArgs
    {
        private object mOrigin;     // the origin of the event (initial requester)
        private SectionNode mNode;  // the node on which the operation is performed
        
        public object Origin
        {
            get { return mOrigin; }
        }

        public SectionNode Node
        {
            get { return mNode; }
        }

        public EventArgs(object origin, SectionNode node)
        {
            mOrigin = origin;
            mNode = node;
        }
    }
}