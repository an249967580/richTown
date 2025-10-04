using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{
    public delegate void LevelChangeEvent(ItemLevelCardData data);
    /// <summary>
    /// 等级卡列表
    /// </summary>
    public class LevelCardsView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose;
        public Text tvLevel, tvDiamond;
        public Slider sLevel;
        public ListView lstView;

        private MdLevelCards _md;

        public LevelChangeEvent OnLevelChangeEvent;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            _md = new MdLevelCards();
        }

        private void Start()
        {
            _md.FindList((result)=>
            {
                if (result.IsOk)
                {
                    _md.DataItems = result.data;
                    initList();
                }
                else
                {
                    Game.Instance.ShowTips(result.errorMsg);
                }
            });

        }

        void initList()
        {
            for (int i = 0; i < _md.Count; i++)
            {
                ItemLevelCardView vi = lstView.Add(_md.DataItems[i]) as ItemLevelCardView;
                vi.OnItemClickEvent = onItemClickListener;
            }
        }

        public void InitView(long diamond, int leval)
        {
            _md.diamond = diamond;
            _md.level = leval;
            tvLevel.text = "Lv." + leval;
            tvDiamond.text = diamond.ToString();
            sLevel.value = leval;
        }

        void onItemClickListener(ItemView vi)
        {
            ItemLevelCardData data = vi.Data as ItemLevelCardData;
            if(!_md.IsDiamondEnough(data.diamond))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5409"));
            }
            else
            {
                LevelCardBuyView view = UIClubSpawn.Instance.CreateLevelCardBuyView();
                view.InitView(data);
                view.OnBuyLevelCardEvent = (item)=>
                {
                    _md.Buy(item.id, (result)=>
                    {
                        if(result.IsOk)
                        {
                            _md.MinusDiamond(item.diamond);
                            _md.level = item.level;
                            InitView(_md.diamond, item.level);
                            if(OnLevelChangeEvent != null)
                            {
                                OnLevelChangeEvent(item);
                            }
                            view.HideAndDestory();
                        }
                        else
                        {
                            Game.Instance.ShowTips(result.errorMsg);
                        }
                    });
                };
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
