using Obi.Assets;

namespace Obi.Events
{
    // Generic handlers for Obi nodes
    public delegate void ObiNodeHandler(object sender, ObiNode node);
    public delegate void PhraseNodeHandler(object sender, PhraseNode node);
    public delegate void SectionNodeHandler(object sender, SectionNode node);
    
    // More specific handlers for events requiring more arguments
    public delegate void MergePhraseNodesHandler(object sender, PhraseNode n1, PhraseNode n2);
    public delegate void MergeSectionNodesHandler(object sender, SectionNode n1, SectionNode n2);  // coming soon!
    public delegate void PhraseNodeSetMediaHandler(object sender, PhraseNode node, Node.SetMediaEventArgs e);
    public delegate void PhraseNodeUpdateTimeHandler(object sender, PhraseNode node, double time);
    public delegate void SetPageNumberHandler(object sender, PhraseNode node, int number);
    public delegate void SplitPhraseNodeHandler(object sender, PhraseNode node, AudioMediaAsset asset);
}
