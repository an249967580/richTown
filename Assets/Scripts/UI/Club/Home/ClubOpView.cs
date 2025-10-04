using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 点击添加弹出窗，查找或创建俱乐部
    /// </summary>
    public class ClubOpView : HideMonoBehaviour
    {
        public Button btnClose, btnSearch, btnCreate;
        public InputField ipClubId;

        private void Awake()
        {
            if (btnClose)
            {
                btnClose.onClick.AddListener(HideAndDestory);
            }

            btnSearch.onClick.AddListener(searchClub);

            btnCreate.onClick.AddListener(onCreate);
        }

        // 搜索俱乐部
        void searchClub()
        {
            string clubId = ipClubId.text.Trim();
            if(string.IsNullOrEmpty(clubId))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5100"));
                return;
            }

            ClubApi.SearchClub(int.Parse(clubId), (result)=>
            {
                if (result.IsOk)
                {
                    if (result.data != null)
                    {
                        onApply(result.data);
                    }
                    else
                    {
                        Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5101"));
                    }
                }
                else
                {
                    Game.Instance.ShowTips(result.errorMsg);
                }
            });
            
        }

        void onApply(ClubSearch md)
        {
            ClubSearchView vi = ClubHomeView.Instance.CreateClubSearchView();
            vi.InitView(md);
            vi.OnApplyEvent = (clubId)=>
            {
                ClubApi.ApplyJoinClub(clubId, (result)=>
                {
                    if(result.IsOk)
                    {
                        ClubHomeView.Instance.CreateClubApplyTipView();
                    }
                    else
                    {
                        Game.Instance.ShowTips(result.errorMsg);
                    }
                });
            };
        }

        // 创建俱乐部
        void onCreate()
        {
            ClubCreateView vi = ClubHomeView.Instance.CreateClubCreateView();
            vi.OnClubCreateEvent = (name, intro, cid)=>
            {
                ClubApi.CreateClub(name, intro, cid, (result)=>
                {
                    if (result.IsOk)
                    {
                        ItemClubData data = new ItemClubData();
                        data.clubId = result.data;
                        data.name = name;
                        data.level = 0;
                        data.role = 2;
                        ClubHomeView.Instance.OnCreateClub(data);
                        vi.HideAndDestory();
                        if(btnClose)
                        {
                            HideAndDestory();
                        }
                        else
                        {
                            Hide();
                        }

                    }
                    else
                    {
                        Game.Instance.ShowTips(result.errorMsg);
                    }
                });
            };
        }
    }

}
