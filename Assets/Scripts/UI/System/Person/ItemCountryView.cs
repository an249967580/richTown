using UnityEngine.UI;

namespace RT
{
    public class ItemCountryView : ItemView
    {
        public Text tvName;

        public override void RegisterEvent()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                if(OnItemClickEvent != null)
                {
                    OnItemClickEvent(this);
                }
            });
        }

        public override void Render()
        {
            ItemCountryData data = Data as ItemCountryData;
            tvName.text = data.title;
        }
    }
}
