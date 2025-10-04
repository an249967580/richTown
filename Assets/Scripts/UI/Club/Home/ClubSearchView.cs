using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{
    public delegate void ApplyEvent(int clubId);

    /// <summary>
    /// 俱乐部信息(查找弹出)
    /// </summary>
    public class ClubSearchView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose, btnApply;
        public Text tvName, tvId, tvCreater, tvMember, tvIntro;
        public Slider sLevel;
        public Image imgAvatar;

        private ClubSearch _md;

        public ApplyEvent OnApplyEvent;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            btnApply.onClick.AddListener(apply);
            imgAvatar.gameObject.SetActive(false);
        }

        public void InitView(ClubSearch md)
        {
            _md = md;
            tvName.text = _md.name;
            tvId.text = "ID：" + _md.clubId;
            tvCreater.text = _md.creator;
            tvMember.text = string.Format("{0}/{1}", _md.memberCount, _md.memberLimit);
            tvIntro.text = _md.intro;
            sLevel.value = _md.level;
            if(Validate.IsNotEmpty(md.avatar))
            {
                
                StartCoroutine(LoadImageUtil.LoadImage(md.avatar, (sprite) =>
                {
                    imgAvatar.gameObject.SetActive(true);
                    imgAvatar.sprite = sprite;
                  
                }));
            }
            else
            {
                imgAvatar.gameObject.SetActive(false);
            }
        }

        void apply()
        {
            if(OnApplyEvent != null)
            {
                OnApplyEvent(_md.clubId);
            }
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != gameObject)
            {
                return;
            }
            if (gameObject.activeSelf)
            {
                HideAndDestory();
            }
        }
    }
}
