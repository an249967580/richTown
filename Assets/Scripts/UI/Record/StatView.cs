using UnityEngine;
using UnityEngine.UI;
namespace RT
{
    public class StatView : MonoBehaviour {

        public Text tvBoard, tvPoolRate, tvWinRate, tvHandNum;
        public Slider sRate;
        public Button btnData;

        MdCareer _md;
        
        public string game
        {
            get;
            set;
        }

        private void Awake()
        {
            btnData.onClick.AddListener(() =>
            {
                Transfer.Instance[TransferKey.Game] = game;
                MainView.Instance.CreateCareerDataView();
    
            });
            _md = new MdCareer();
        }

        private void Start()
        {
            _md.GetCareer(game, (rsp) =>
            {
                if(rsp.IsOk)
                {
                    if(rsp.data != null)
                    {
                        _md.career = rsp.data;
                        initView();
                    }
                }
                else
                {
                    Game.Instance.ShowTips(rsp.errorMsg);
                }
            });
        }

        void initView()
        {
            tvBoard.text = string.Format(LocalizationManager.Instance.GetText("3001"), _md.career.roomTotal);
            tvHandNum.text = _md.career.handTotal.ToString();
            tvWinRate.text = (_md.career.winRate * 100) + "%";
            if (GameType.IsBull(game))
            {
                sRate.value = _md.career.winRate * 100;
            }
            else
            {
                sRate.value = _md.career.poolRate * 100;
                tvPoolRate.text = (_md.career.poolRate * 100) + "%";
            }
        }
    }
}
