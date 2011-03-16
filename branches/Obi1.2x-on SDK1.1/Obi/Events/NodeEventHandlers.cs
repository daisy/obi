using urakawa.core;
using Obi.Events.Node;
using Obi.Events.Strip;

namespace Obi.Events
{
    // Applies to all kinds of Obi nodes
    public delegate void ObiNodeHandler(object sender, ObiNodeEventArgs e);

    //Sections
    //added, pasted, request anything on a section, decreased level, undid paste, shallow-deleted, cut/copied,
    //deleted
    public delegate void SectionNodeHandler(object sender, SectionNodeEventArgs e);
    public delegate void MovedSectionNodeHandler(object sender, MovedSectionNodeEventArgs e);
    public delegate void RenameSectionNodeHandler(object sender, RenameSectionNodeEventArgs e);
    public delegate void ShallowSwappedSectionNodesHandler(object sender, ShallowSwappedSectionNodesEventArgs e);
    public delegate void SectionNodeHeadingHandler(object sender, SectionNodeHeadingEventArgs e);

    //Phrases
    //added, request anything, 
    public delegate void PhraseNodeHandler(object sender, PhraseNodeEventArgs e);
    public delegate void SplitPhraseNodeHandler(object sender, SplitPhraseNodeEventArgs e);
  
    //Other
    public delegate void MergePhraseNodesHandler(object sender, MergeNodesEventArgs e);
    public delegate void RequestToApplyPhraseDetectionHandler(object sender, PhraseDetectionEventArgs e);
    public delegate void RequestToSetPageNumberHandler(object sender, SetPageEventArgs e);
    public delegate void SelectedHandler(object sender, Obi.Events.Node.SelectedEventArgs e);
    public delegate void SetMediaHandler(object sender, SetMediaEventArgs e);
    public delegate void RequestToImportAssetHandler(object sender, ImportAssetEventArgs e);
    public delegate void UpdateTimeHandler(object sender, UpdateTimeEventArgs e);

    public delegate void NodeEventHandler(object sender, NodeEventArgs e);

 }