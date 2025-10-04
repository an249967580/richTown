using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    [AddComponentMenu("XGame/ListView")]
    public class ListView : AbstractListView
    {

        public VerticalLayoutGroup VerticleLG;
        public int spacing = 5;

        public override void Awake()
        {
            base.Awake();
            VerticleLG.spacing = spacing;
        }

        public override GameObject LayoutView()
        {
            return VerticleLG.gameObject;
        }
    }
}
