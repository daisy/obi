namespace Obi.Events.Node
{
    public class EventArgs<NodeType>: System.EventArgs
    {
        NodeType mNode;

        public NodeType Node
        {
            get { return mNode; }
        }

        public EventArgs(NodeType node)
        {
            mNode = node;
        }
    }
}