using urakawa.core;

namespace Obi
{
    public class PhraseNode : ObiNode
    {
        public static readonly string Name = "phrase";

        internal PhraseNode(Project project, int id)
            : base(project, id)
        {
        }

        protected override string getLocalName()
        {
            return "phrase";
        }
    }
}
