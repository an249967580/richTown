using Assets.Scripts.TableView;
using UnityEngine.UI;

namespace RT
{
    public enum ApplyOp : uint
    {
        Agree,
        Reject
    }

    public delegate void OpClickEvent(ApplyOp op, ItemApplyData data);

    /// <summary>
    /// 申请列表项
    /// </summary>
    public class ItemApplyCell : TableViewCell
    {
        public CircleImage imgAvatar;
        public Button btnAgree, btnReject;
        public Text tvName, tvId;
        public OpClickEvent OnOpClickEvent;

        public ItemApplyData data;

        public override string ReuseIdentifier
        {
            get
            {
                return "ItemApplyCRI";
            }
        }

        protected override void Awake()
        {
            base.Awake();
            btnAgree.onClick.AddListener(()=>
            {
                if (OnOpClickEvent != null)
                {
                    OnOpClickEvent(ApplyOp.Agree, data);
                }
            });
            btnReject.onClick.AddListener(()=>
            {
                if (OnOpClickEvent != null)
                {
                    OnOpClickEvent(ApplyOp.Reject, data);
                }
            });
        }

        public override void SetHighlighted()
        {
            
        }

        public override void SetSelected()
        {
            
        }

        public override void Display()
        {
            if(data == null)
            {
                return;
            }
            // tvName.text = data.nickname;
            LimitText.LimitAndSet(data.nickname, tvName, 140);
            tvId.text = "ID：" + data.uid.ToString();
        }
        
    }
}
