using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    [AddComponentMenu("XGame/HListView")]
    public class HListView : AbstractListView
    {
        public HorizontalLayoutGroup HorizontalLG;
        public int spacing = 5;

        public override void Awake()
        {
            base.Awake();
            HorizontalLG.spacing = spacing;
        }

        public override GameObject LayoutView()
        {
            return HorizontalLG.gameObject;
        }
    }
}
