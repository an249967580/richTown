using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 升级卡列表项
    /// </summary>
    public class ItemLevelCardView : ItemView
    {
        public Text tvTitle, tvExpir, tvMgrNum, tvMemberNum, tvDiamond;
        public Image imgAvatar;

        public override void RegisterEvent()
        {
            GetComponent<Button>().onClick.AddListener(()=>
            {
                if (OnItemClickEvent != null)
                {
                    OnItemClickEvent(this);
                }
            });
        }

        public override void Render()
        {
            ItemLevelCardData data = Data as ItemLevelCardData;
            tvTitle.text = data.title;
            tvExpir.text = string.Format( LocalizationManager.Instance.GetText("6101"), data.expirDays);
            tvMemberNum.text = string.Format(LocalizationManager.Instance.GetText("5501"), data.memberNum);
            tvMgrNum.text = string.Format(LocalizationManager.Instance.GetText("5601"), data.managerNum);
            tvDiamond.text = data.diamond.ToString();

            Sprite sprite = Resources.Load<Sprite>("Textures/Club/Cards/star" + data.level);
            if(sprite)
            {
                imgAvatar.sprite = sprite;
            }
        }
    }
}
