using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{
    public delegate void ClubCreateEvent(string name, string intro, long cId);

    /// <summary>
    /// 创建俱乐部
    /// </summary>
    public class ClubCreateView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose, btnCreate, btnMore;
        public InputField ipName, ipIntro;
        public CountryView countryView;
        public Text tvCountry;
        List<ItemCountryData> countries;
        ItemCountryData _curCountry;

        public ClubCreateEvent OnClubCreateEvent;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);

            btnCreate.onClick.AddListener(createClub);

            initCountries();
        }

        void createClub()
        {
            string name = ipName.text.Trim();
            string intro = ipIntro.text.Trim();
            if(string.IsNullOrEmpty(name))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5109"));
                return;
            }

            if(string.IsNullOrEmpty(intro))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5110"));
                return;
            }

            if(_curCountry == null)
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5112"));
                return;
            }

            if(OnClubCreateEvent != null)
            {
                OnClubCreateEvent(name, intro, _curCountry.cid);
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

        void initCountries()
        {
            if (PlayerPrefs.HasKey("country"))
            {
                ItemCountryData data = JsonConvert.DeserializeObject<ItemCountryData>(PlayerPrefs.GetString("country"));
                if (data != null)
                {
                    _curCountry = data;
                    tvCountry.text = data.title;
                }
            }

            if (PlayerPrefs.HasKey("countries"))
            {
                countries = JsonConvert.DeserializeObject<List<ItemCountryData>>(PlayerPrefs.GetString("countries"));
                if (_curCountry == null)
                {
                    _curCountry = countries[0];
                    tvCountry.text = _curCountry.title;
                }
            }
            else
            {
                SystemApi.FindCountry((rsp) =>
                {
                    if (rsp.IsOk && Validate.IsNotEmpty(rsp.data))
                    {
                        // 保存文件
                        countries = rsp.data;
                        if (_curCountry == null)
                        {
                            _curCountry = countries[0];
                            tvCountry.text = _curCountry.title;
                        }
                        PlayerPrefs.SetString("countries", JsonConvert.SerializeObject(rsp.data));
                    }
                });
            }

            btnMore.onClick.AddListener(() =>
            {
                countryView.Show();
                countryView.InitView(countries);
            });
            countryView.OnCountryClickEvent = onCountryClickEvent;
        }

        void onCountryClickEvent(ItemCountryData data)
        {
            tvCountry.text = data.title;
            _curCountry = data;
            PlayerPrefs.SetString("country", JsonConvert.SerializeObject(data));
        }
    }
}
