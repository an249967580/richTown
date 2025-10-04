using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    [SerializeField]
    public class Banker
    {
        public string nickname;
        public long uid;
        public bool isBanker;
    }

    public class BankerChoiceView : MonoBehaviour
    {

        private List<Banker> bankers;
        private List<ItemBankChoice> itemViews;

        public HorizontalLayoutGroup hGroup;
        public ItemBankChoice itemPrefab;

        public float speed;
        public float time;

        private int i;
        private float _tick;
        private bool selected;


        void Start()
        {
            List<Banker> test = new List<Banker>();
            for (int i=0;i<5;i++)
            {
                Banker b = new Banker();
                b.nickname = "n_" + i;
                b.uid = i;
                if( i == 3)
                {
                    b.isBanker = true;
                }
                test.Add(b);
            }
            InitView(test);
        }

        public void InitView(List<Banker> bankers)
        {
            this.bankers = bankers;
            itemViews = new List<ItemBankChoice>();
            initList();
        }

        private void initList()
        {
            if(Validate.IsNotEmpty(bankers))
            {
                int padding =  (750 - (bankers.Count * 90 - 10)) / 2;
                hGroup.padding = new RectOffset(padding, padding, 0, 0);

                foreach(Banker b in bankers)
                {
                    var bankerView = Instantiate(itemPrefab);
                    if(bankerView)
                    {
                        bankerView.gameObject.transform.SetParent(hGroup.gameObject.transform);
                        bankerView.InitView(b);
                        itemViews.Add(bankerView);
                    }
                }
                InvokeRepeating("choice", 0.5f, speed);
            }
        }

        void choice()
        {
            _tick += speed;

            if (_tick >= time)
            {
                selected = true;
            }
            i++;
            int cur = i % itemViews.Count;
            for (int j = 0; j < itemViews.Count; j++)
            {
                if (cur == j)
                {
                    itemViews[j].SetSelected(true);
                }
                else
                {
                    itemViews[j].SetSelected(false);
                }
            }
            if (selected)
            {
                if (bankers[cur].isBanker)
                {
                    CancelInvoke("choice");
                }
            }
        }
     
    }
}
