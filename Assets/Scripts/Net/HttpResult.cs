namespace RT
{
    public class HttpResult<T>
    {
        private int _code;

        public int code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
                if(!IsOk)
                {
                    errorMsg = LocalizationManager.Instance.GetText(_code.ToString());
                    if(Validate.IsEmpty(errorMsg))
                    {
                        errorMsg = LocalizationManager.Instance.GetText("1012");
                    }
                }
            }
        }

        public bool IsOk
        {
            get
            {
                return code == 200;
            }
        }
        public T data;
        public string errorMsg;
    }
}
