using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    [AddComponentMenu("XGame/GridView")]
    public class GridView : AbstractListView
    {
        public GridLayoutGroup GridLG;
        public int horizontalSpacing = 0;
        public int verticleSpacing = 0;

        public override void Awake()
        {
            base.Awake();
            GridLG.spacing = new Vector2(verticleSpacing, horizontalSpacing);
        }

        public override GameObject LayoutView()
        {
            return GridLG.gameObject;
        }
    }
}
