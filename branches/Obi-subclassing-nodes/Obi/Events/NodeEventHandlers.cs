using Obi.Assets;

namespace Obi.Events
{
    public delegate void MergePhraseNodesHandler(object sender, PhraseNode n1, PhraseNode n2);
    public delegate void MergeSectionNodesHandler(object sender, SectionNode n1, SectionNode n2);
    public delegate void PhraseNodeHandler(object sender, PhraseNode node);
    public delegate void PhraseNodeSetMediaHandler(object sender, PhraseNode node, Node.SetMediaEventArgs e);
    public delegate void PhraseNodeUpdateTimeHandler(object sender, PhraseNode node, double time);
    public delegate void SectionNodeHandler(object sender, SectionNode node);
    public delegate void SplitPhraseNodeHandler(object sender, PhraseNode node, AudioMediaAsset asset);
}
