using Obi.Assets;
using urakawa.core;

namespace Obi.Events
{
    //Sections
    public delegate void AddedSectionNodeHandler(object sender, AddedSectionNodeEventArgs e);
    public delegate void PastedSectionNodeHandler(object sender, AddedSectionNodeEventArgs e);
    public delegate void MovedSectionNodeHandler(object sender, MovedSectionNodeEventArgs e);
    public delegate void RequestToRenameSectionNodeHandler(object sender, RenameSectionNodeEventArgs e);
    public delegate void RenamedNodeHandler(object sender, RenameSectionNodeEventArgs e);
    //requests for adding and deleting section nodes
    public delegate void RequestToAddSiblingNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToAddChildSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToDeleteNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToShallowDeleteSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void ShallowSwappedSectionNodesHandler(object sender, ShallowSwappedSectionNodesEventArgs e);
    //requests for moving section nodes
    public delegate void RequestToIncreaseSectionNodeLevelHandler(object sender, NodeEventArgs e);
    public delegate void RequestToDecreaseSectionNodeLevelHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveSectionNodeUpHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveSectionNodeDownHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveSectionNodeUpLinearHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveSectionNodeDownLinearHandler(object sender, NodeEventArgs e);
    //section node clipboard requests
    public delegate void RequestToCutSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToCopySectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToPasteSectionNodeHandler(object sender, NodeEventArgs e);


    //Phrases
    public delegate void AddedPhraseNodeHandler(object sender, AddedPhraseNodeEventArgs e);
    public delegate void SplitPhraseNodeHandler(object sender, SplitPhraseNodeEventArgs e);

    // phrase node clipboard requests (JQ 20060816)
    public delegate void RequestToCutPhraseNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToCopyPhraseNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToPastePhraseNodeHandler(object sender, NodeEventArgs e);

    //Other
    public delegate void MergeNodesHandler(object sender, MergeNodesEventArgs e);
    public delegate void RequestToApplyPhraseDetectionHandler(object sender, PhraseDetectionEventArgs e);
    public delegate void RequestToSetPageNumberHandler(object sender, SetPageEventArgs e);
    public delegate void SelectedHandler(object sender, SelectedEventArgs e);
    public delegate void SetMediaHandler(object sender, SetMediaEventArgs e);
    public delegate void MediaSetHandler(object sender, SetMediaEventArgs e);
    public delegate void RequestToImportAssetHandler(object sender, ImportAssetEventArgs e);
    public delegate void UpdateTimeHandler(object sender, UpdateTimeEventArgs e);

 
    public delegate void RequestToDeleteBlockHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveBlockHandler(object sender, NodeEventArgs e);

    //section node handlers which communicate that requested actions have been done
    //most likely so that the views may update themselves accordingly
    public delegate void DecreasedSectionNodeLevelHandler(object sender, NodeEventArgs e);
    public delegate void DeletedNodeHandler(object sender, NodeEventArgs e);
    public delegate void ShallowDeletedSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void CutSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void CopiedSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void UndidPasteSectionNodeHandler(object sender, NodeEventArgs e);

    public delegate void ImportedAssetHandler(object sender, NodeEventArgs e);
    public delegate void DeletedBlockHandler(object sender, NodeEventArgs e);

    public delegate void TouchedNodeHandler(object sender, NodeEventArgs e);

    public delegate void RequestToRemovePageNumberHandler(object sender, NodeEventArgs e);
    public delegate void SetPageNumberHandler(object sender, NodeEventArgs e);
    public delegate void RemovedPageNumberHandler(object sender, NodeEventArgs e);
   
  
}