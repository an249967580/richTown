using System;
using System.Collections.Generic;

namespace RT
{
    public class MdMail : MdList<ItemEmailData>
    {
        public void FindList(Action<HttpResult<List<ItemEmailData>>> action, bool showMask)
        {
            EmailApi.FindEmails(lastId, pageSize, action, showMask);
        }

        public void GetDatail(long id, Action<HttpResult<EmailDetail>> action)
        {
            EmailApi.GetDetail(id, action);
        }

        public void Receive(long id, Action<HttpResult<int>> action)
        {
            EmailApi.ReceiveDiamond(id, action);
        }

        long lastId
        {
            get
            {
                if(IsEmpty)
                {
                    return 0;
                }
                return DataItems[Count - 1].id;
            }
        }

        public void Read(long id)
        {
            for(int i=0;i<Count;i++)
            {
                if(this[i].id == id)
                {
                    this[i].readFlag = 1;
                    break;
                }
            }
        }

        public void Receive(long id)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].id == id)
                {
                    this[i].rmbFlag = 1;
                    break;
                }
            }
        }
    }
}
