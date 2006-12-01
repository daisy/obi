using Obi.Assets;
using urakawa.core;
using Obi.Events.Node;
using Obi.Events.Strip;

namespace Obi.Events
{
    //Sections
    //added, pasted, request anything on a section, decreased level, undid paste, shallow-deleted, cut/copied,
    //deleted
    public delegate void SectionNodeHandler(object sender, SectionNodeEventArgs e);
    
    public delegate void MovedSectionNodeHandler(object sender, MovedSectionNodeEventArgs e);
    public delegate void RenameSectionNodeHandler(object sender, RenameSectionNodeEventArgs e);
    public delegate void ShallowSwappedSectionNodesHandler(object sender, ShallowSwappedSectionNodesEventArgs e);

    //Phrases
    //added, request anything
    public delegate void PhraseNodeHandler(object sender, PhraseNodeEventArgs e);
    public delegate void SplitPhraseNodeHandler(object sender, SplitPhraseNodeEventArgs e);
    public delegate void PastePhraseNodeHandler(object sender, NodeEventArgs e);

    //Other
    public delegate void MergeNodesHandler(object sender, MergeNodesEventArgs e);
    public delegate void RequestToApplyPhraseDetectionHandler(object sender, PhraseDetectionEventArgs e);
    public delegate void RequestToSetPageNumberHandler(object sender, SetPageEventArgs e);
    public delegate void SelectedHandler(object sender, Obi.Events.Node.SelectedEventArgs e);
    public delegate void SetMediaHandler(object sender, SetMediaEventArgs e);
    public delegate void MediaSetHandler(object sender, SetMediaEventArgs e);
    public delegate void RequestToImportAssetHandler(object sender, ImportAssetEventArgs e);
    public delegate void UpdateTimeHandler(object sender, UpdateTimeEventArgs e);

 
    public delegate void RequestToDeleteBlockHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveBlockHandler(object sender, NodeEventArgs e);

  
    public delegate void ImportedAssetHandler(object sender, NodeEventArgs e);
    public delegate void DeletedBlockHandler(object sender, NodeEventArgs e);

    public delegate void TouchedNodeHandler(object sender, NodeEventArgs e);

    public delegate void RequestToRemovePageNumberHandler(object sender, NodeEventArgs e);
    public delegate void SetPageNumberHandler(object sender, NodeEventArgs e);
    public delegate void RemovedPageNumberHandler(object sender, NodeEventArgs e);
   
  
}