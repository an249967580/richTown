using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace RT
{
    public class PokerUtilCard {
        public int c;
        public int n;
    }

    public class PokerUtilCardType
    {
        public string type;
        public int typePoint;
        public long point;
        public int[] cards;
    }

    enum Type_COW {
        FIVE_CARD = 17,
        COW_NDG_CARD = 16,
        COW_BABY_TOW_CARD = 15,
        COW_BABY_CARD = 14,
        COWCOW_CARD = 13,
        COW_CARD = 12,
        SINGLE_CARD = 11,
    }
    public class BullSuitTypeUtil
    {
        public Dictionary<int, PokerUtilCard> pokers = new Dictionary<int, PokerUtilCard>();

        long maxPoint = 0;
        List<int[]> allCardType;
        int[] maxPointCards;
        PokerUtilCardType cardRs;

        private static BullSuitTypeUtil instance;

        public static BullSuitTypeUtil Instance
        {
            get
            {
                if (instance == null) {
                    instance = new BullSuitTypeUtil();
                }
                return instance;
            }
        }

        public int[] GetBestSuit(int[] arr) {

            TextAsset ta = Resources.Load<TextAsset>("Json/poker");
            if (ta)
            {
                pokers = JsonConvert.DeserializeObject<Dictionary<int, PokerUtilCard>>(ta.text);
            }
            allCardType = new List<int[]>();
            maxPoint = 0;
            maxPointCards = null;

            List<PokerUtilCard> cards = getCardObjArr(arr);
            if (isJQK(cards)) {
                return arr;
            }

            allCardType.Add((int [])arr.Clone());
            allRange(arr, 0);
            
            return maxPointCards;
        }

        public PokerUtilCardType GetBestSuitType(int[] arr)
        {
            TextAsset ta = Resources.Load<TextAsset>("Json/poker");
            if (ta)
            {
                pokers = JsonConvert.DeserializeObject<Dictionary<int, PokerUtilCard>>(ta.text);
            }
            allCardType = new List<int[]>();
            maxPoint = 0;
            maxPointCards = null;

            List<PokerUtilCard> cards = getCardObjArr(arr);
            if (isJQK(cards))
            {
                return cardRs;
            }

            allCardType.Add((int[])arr.Clone());
            allRange(arr, 0);
            return cardRs;
        }

        List<PokerUtilCard> getCardObjArr(int[] arr) {
            List<PokerUtilCard> list = new List<PokerUtilCard>();
            if (arr != null) {
                foreach (int i in arr) {
                    list.Add(pokers[i]);
                }
            }
            return list;
        }

        PokerUtilCardType getCardType_COW(int[] arr) {
            List<PokerUtilCard> cards = getCardObjArr(arr);
            //五公
            if (isJQK(cards)) {
                long tmpNum = (int)Type_COW.FIVE_CARD * 1000000;
                PokerUtilCardType pct = new PokerUtilCardType();
                pct.type = Type_COW.FIVE_CARD.ToString();
                pct.typePoint = 0;
                pct.point = tmpNum;
                pct.cards = arr;
                return pct;
            }

            List<PokerUtilCard> tmp1 = cards.GetRange(0,3);
            int hasCowPoint = cowCount(tmp1);

            //没牛
            if (hasCowPoint > 0)
            {
                long tmpNum = (int)Type_COW.SINGLE_CARD * 1000000;
                PokerUtilCardType pct = new PokerUtilCardType();
                pct.type = Type_COW.SINGLE_CARD.ToString();
                pct.typePoint = 0;
                pct.point = tmpNum;
                pct.cards = arr;
                return pct;
            }
            //牛宝宝
            if (cards[3].n == cards[4].n)
            {
                if (cards[3].n == 1)
                {
                    //牛双A
                    long tmpNum = (int)Type_COW.COW_BABY_TOW_CARD * 1000000;
                    tmpNum += cards[3].n * 100;//对子需要比大小
                    PokerUtilCardType pct = new PokerUtilCardType();
                    pct.type = Type_COW.COW_BABY_TOW_CARD.ToString();
                    pct.typePoint = 0;
                    pct.point = tmpNum;
                    pct.cards = arr;
                    return pct;
                }
                else
                {
                    //牛宝宝
                    long tmpNum = (int)Type_COW.COW_BABY_CARD * 1000000;
                    tmpNum += cards[3].n * 100;//对子需要比大小
                    PokerUtilCardType pct = new PokerUtilCardType();
                    pct.type = Type_COW.COW_BABY_CARD.ToString();
                    pct.typePoint = 0;
                    pct.point = tmpNum;
                    pct.cards = arr;
                    return pct;
                }
            }

            //牛冬菇
            if ((cards[3].c == 2 && cards[3].n == 1)|| cards[4].c == 2 && cards[4].n == 1)
            {
                if ((cards[3].n == 11 || cards[3].n == 12 || cards[3].n == 13) ||(cards[4].n == 11 || cards[4].n == 12 || cards[4].n == 13))
                {
                    long tmpNum = (int)Type_COW.COW_NDG_CARD * 1000000;
                    PokerUtilCardType pct = new PokerUtilCardType();
                    pct.type = Type_COW.COW_NDG_CARD.ToString();
                    pct.typePoint = 0;
                    pct.point = tmpNum;
                    pct.cards = arr;
                    return pct;
                }
            }

            //牛牛
            List<PokerUtilCard> tmp2 = cards.GetRange(3, 2);

            int cowPoint = cowCount(tmp2);
            if (cowPoint == 0)
            {
                long tmpNum = (int)Type_COW.COWCOW_CARD * 1000000;
                PokerUtilCardType pct = new PokerUtilCardType();
                pct.type = Type_COW.COWCOW_CARD.ToString();
                pct.typePoint = 0;
                pct.point = tmpNum;
                pct.cards = arr;
                return pct;
            }
            else
            {
                long tmpNum = (int)Type_COW.COW_CARD * 1000000;
                tmpNum += cowPoint * 100;//牛点数需要比大小
                PokerUtilCardType pct = new PokerUtilCardType();
                pct.type = Type_COW.COW_CARD.ToString();
                pct.typePoint = cowPoint;
                pct.point = tmpNum;
                pct.cards = arr;
                return pct;
            }
            
        }

        void swap(int[] arr,int a,int b) {
            int c = arr[b];
            arr[b] = arr[a];
            arr[a] = c;
            int[] arrClone = (int[])arr.Clone();
            allCardType.Add(arrClone);
            var rs = getCardType_COW(arr);
            if (maxPoint < rs.point) {
                maxPoint = rs.point;
                maxPointCards = arrClone;
                cardRs = rs;
            }
        }

        void allRange(int[] arr, int startI) {

            if (startI == arr.Length)
            {
                return;
            }

            if (startI == 0)
            {
                this.allRange(arr, startI + 1);
            }

            for (var i = startI + 1; i < 5; i++)
            {
                int[] arr2 = (int[])arr.Clone();
                this.swap(arr2, startI, i);

                this.allRange((int[])arr2.Clone(), startI + 1);
            }

        }

        int cowCount1(List<PokerUtilCard> cards)
        {
            int n = 0;
            Queue<int> tmp36 = new Queue<int>();
            int maxN = 0;

            for (int i = 0; i < cards.Count; i++)
            {
                PokerUtilCard p = cards[i];
                if (p.n > 9)
                {
                    continue;
                }
                n += p.n;
                if (p.n == 3 || p.n == 6)
                {
                    tmp36.Enqueue(i);
                }
            }

            n = n % 10;

            if (n == 0)
            {
                return 0;
            }

            maxN = n;
            while (tmp36.Count > 0)
            {
                int ci = tmp36.Dequeue();
                n = 0;
                for (int j = 0; j < cards.Count; j++)
                {
                    if (cards[j].n > 9)
                    {
                        continue;
                    }
                    if (ci == j)
                    {
                        if (cards[ci].n == 3)
                        {
                            n += 6;
                        }
                        else
                        {
                            n += 3;
                        }
                        continue;
                    }
                    n += cards[j].n;
                }

                n = n % 10;
                if (n == 0)
                {
                    return 0;
                }
                if (n > maxN)
                {
                    maxN = n;
                }
            }

            return maxN;
        }


        List<PokerUtilCard> copyCardDeeply(List<PokerUtilCard> cards)
        {
            List<PokerUtilCard> list = new List<PokerUtilCard>();
            if (cards != null) {
                foreach (PokerUtilCard c in cards) {
                    PokerUtilCard nc = new PokerUtilCard();
                    nc.n = c.n;
                    nc.c = c.c;
                    list.Add(nc);
                }
            }
            return list;
        }

        int cowCount(List<PokerUtilCard> cards)
        {
            bool rs = true;
            int n = 0;
            List<int> tmp36 = new List<int>();
            int maxN = 0;

            for (int i = 0; i < cards.Count; i++)
            {
                PokerUtilCard p = cards[i];
                if (p.n > 9)
                {
                    continue;
                }
                n += p.n;
                if (p.n == 3 || p.n == 6)
                {
                    tmp36.Add(i);
                }
            }

            n = n % 10;

            if (n == 0)
            {
                return 0;
            }

            maxN = n;

            if (tmp36.Count > 0) {
                Turn36 co1 = turn36(cards, tmp36[0],maxN);
                if (co1 == null) {
                    return 0;
                }
                maxN = co1.maxN;

                if (tmp36.Count > 1) {
                    Turn36 co2 = turn36(cards, tmp36[1], maxN);

                    if (co2 == null) {
                        return 0;
                    }
                    maxN = co2.maxN;

                    if (tmp36.Count > 2)
                    {
                        Turn36 co3 = turn36(cards, tmp36[2], maxN);
                        if (co3 == null)
                        {
                            return 0;
                        }
                        maxN = co3.maxN;

                        Turn36 co4 = turn36(co2.cards, tmp36[2], maxN);
                        if (co4 == null)
                        {
                            return 0;
                        }
                        maxN = co4.maxN;
                    }
                
                    co2 = turn36(co1.cards, tmp36[1], maxN);
                    if (co2 == null)
                    {
                        return 0;
                    }
                    maxN = co2.maxN;

                    if (tmp36.Count > 2)
                    {
                        Turn36 co3 = turn36(co1.cards, tmp36[2], maxN);
                        if (co3 == null)
                        {
                            return 0;
                        }
                        maxN = co3.maxN;

                        Turn36 co4 = turn36(co2.cards, tmp36[2], maxN);
                        if (co4 == null)
                        {
                            return 0;
                        }
                        maxN = co4.maxN;
                    }
                    }
            }
            return maxN;
        }

        class Turn36 {
            public List<PokerUtilCard> cards;
            public int maxN;
        }

        Turn36 turn36(List<PokerUtilCard> cards, int ci ,int maxN)
        {
            Turn36 result = new Turn36();
            List<PokerUtilCard> cards_2 = copyCardDeeply(cards);
            int n = 0;
            for (int j = 0; j < cards_2.Count; j++) {
                if (cards_2[j].n > 9)
                {
                    continue;
                }
                if (ci == j)
                {//3飞6 6飞3

                    if (cards_2[ci].n == 3)
                    {
                        cards_2[ci].n = 6;

                    }
                    else
                    {
                        cards_2[ci].n = 3;
                    }
                }
                n += cards_2[j].n;
            }

            n = n % 10;
            if (n == 0)
            {
                return null;
            }
            if (n > maxN)
            {
                maxN = n;
            }
            result.cards = cards_2;
            result.maxN = maxN;

            return result;
        }

        bool isJQK(List<PokerUtilCard> cards) {
            bool rs = true;
            int n = 0 ;
            if (cards != null)
            {
                foreach (PokerUtilCard p in cards)
                {
                    n = p.n;
                    if (n == 11 || n == 12 || n == 13)
                    {
                        continue;
                    }
                    else
                    {
                        rs = false;
                        break;
                    }
                }
            }
            else {
                rs = false;
            }
            return rs;
        }
    }
}
