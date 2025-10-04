using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace RT
{
    public class ItemClubView : ItemView
    {
        public Text tvName;

        public override void RegisterEvent()
        {
            base.RegisterEvent();
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
            ItemClubData data = Data as ItemClubData;
            tvName.text = data.name;
        }
    }
}
