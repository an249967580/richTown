using UnityEngine.UI;
namespace RT
{
    public class ItemDropView : ItemView
    {
        public Text tvTitle;

        public override void RegisterEvent()
        {
            GetComponent<Button>().onClick.AddListener(()=>
            {
                if(OnItemClickEvent != null)
                {
                    OnItemClickEvent(this);
                }
            });
        }

        public override void Render()
        {
            ItemDropData data = Data as ItemDropData;
            tvTitle.text = data.title;
        }
    }


}
