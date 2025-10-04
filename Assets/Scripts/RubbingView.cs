using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{
    public delegate void RubbingEndEvent(RubbingView vi);

    public class RubbingView : HideMonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Image imgCard, imgBack;
        public RubbingEndEvent OnRubbingEndEvent;
        public Image imgLeft, imgRight, imgUp, imgDown;

        private Vector2 fingerBeginPos;      // 开始位置
        private Vector2 fingerCurrentPos;    // 当前位置
        private Vector2 fingerLastPos;       // 上一次的位置
        private Vector2 fingerSegment;
        private float fingerActionSensitivity;
        private bool isTouch;
        private RectTransform _rt;
        private float minX, minY;        // x、y轴能够移动的最大范围
        private float deltaX, deltaY;    // x、y轴累计移动的范围

        private const int state_none = 0;
        private const int state_move = 1;
        private const int state_add = 2;
        private int state = state_none;

        private bool left, right, up, down; // 移动方向

        void Start()
        {
            fingerBeginPos = Vector2.zero;
            fingerCurrentPos = Vector2.zero;
            fingerSegment = Vector2.zero;
            _rt = imgBack.gameObject.GetComponent<RectTransform>();
            minX = _rt.sizeDelta.x * 3f / 5;
            minY = _rt.sizeDelta.y * 3.5f / 5;
            Debug.Log(minX + ", " + minY);
            fingerActionSensitivity = _rt.sizeDelta.x * 0.02f;
        }

        public void SetCard(int card)
        {
            Sprite sprite = Resources.Load<Sprite>("Textures/Poker/Bull/" + card);
            if(sprite)
            {
                imgCard.sprite = sprite;
            }
        }

        public override void Show()
        {
            base.Show();
            imgBack.gameObject.SetActive(true);
            isTouch = false;
            state = state_none;
            fingerBeginPos = Vector2.zero;
            fingerCurrentPos = Vector2.zero;
            fingerLastPos = Vector2.zero;
            imgLeft.gameObject.SetActive(false);
            imgRight.gameObject.SetActive(false);
            imgUp.gameObject.SetActive(false);
            imgDown.gameObject.SetActive(false);
            left = false;
            right = false;
            up = false;
            down = false;
            deltaX = 0;
            deltaY = 0;
            if (_rt != null)
            {
                _rt.anchoredPosition3D = Vector3.zero;
            }
        }

        public override void Hide()
        {
            base.Hide();
            imgBack.gameObject.SetActive(true);
            isTouch = false;
            state = state_none;
            fingerBeginPos = Vector2.zero;
            fingerCurrentPos = Vector2.zero;
            fingerLastPos = Vector2.zero;
            imgLeft.gameObject.SetActive(false);
            imgRight.gameObject.SetActive(false);
            imgUp.gameObject.SetActive(false);
            imgDown.gameObject.SetActive(false);
            left = false;
            right = false;
            up = false;
            down = false;
            deltaX = 0;
            deltaY = 0;
            if (_rt != null)
            {
                _rt.anchoredPosition3D = Vector3.zero;
            }
        }

        public void Reback()
        {
            deltaX = 0;
            deltaY = 0;
            if (_rt != null)
            {
                _rt.anchoredPosition3D = Vector3.zero;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isTouch)
            {
                //var rectPos = new Vector2(_rt.position.x, _rt.position.y);
                //var rect = new Rect(rectPos - _rt.sizeDelta / 2, _rt.sizeDelta);
                //if (rect.Contains(eventData.position))
                //{
                isTouch = true;
                fingerBeginPos = eventData.pressPosition;
                //}
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isTouch) return;
            if(state == state_none)
            {
                fingerCurrentPos = eventData.position;
                fingerSegment = fingerCurrentPos - fingerBeginPos;
                #region 判断方向
                //这边计算你需要拖动的范围才算拖动了
                if (fingerSegment.SqrMagnitude() > Mathf.Pow(fingerActionSensitivity, 2))
                {
                    // move();
                    if(Math.Abs(fingerSegment.x) > Math.Abs(fingerSegment.y))
                    {
                        if(fingerSegment.x > 0)
                        {
                            left = true;
                            right = false;
                            up = false;
                            down = false;
                        }
                        else
                        {
                            left = false;
                            right = true;
                            up = false;
                            down = false;
                        }
                    }
                    else
                    {
                        if (fingerSegment.y > 0)
                        {
                            left = false;
                            right = false;
                            up = true;
                            down = false;
                        }
                        else
                        {
                            left = false;
                            right = false;
                            up = false;
                            down = true;
                        }
                    }
                    state = state_move;
                }
                #endregion
            }
            else
            {
                state = state_add;
                fingerLastPos = fingerCurrentPos;
                fingerCurrentPos = eventData.position;
                fingerSegment = fingerCurrentPos - fingerLastPos;
                move(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isTouch = false;
            state = state_none;
            fingerBeginPos = Vector2.zero;
            fingerCurrentPos = Vector2.zero;
            fingerLastPos = Vector2.zero;
            imgLeft.gameObject.SetActive(false);
            imgRight.gameObject.SetActive(false);
            imgUp.gameObject.SetActive(false);
            imgDown.gameObject.SetActive(false);
            left = false;
            right = false;
            up = false;
            down = false;
            _rt.anchoredPosition3D = Vector3.zero;
        }

        void move(PointerEventData eventData)
        {
            if(Math.Abs(fingerSegment.x) > 20)
            {
                fingerSegment.x /= 2;
            }
            if(Math.Abs(fingerSegment.y) > 20)
            {
                fingerSegment.y /= 2;
            }

            if (left || right)
            {
                var x = fingerSegment.x * 0.5f;
                deltaX += x;
                _rt.anchoredPosition3D += new Vector3(x, 0, 0);
            }
            if (up || down)
            {
                var y = fingerSegment.y * 0.7f;
                deltaY += y;
                _rt.anchoredPosition3D += new Vector3(0, y, 0);
            }

            if (left)
            {
                imgRight.gameObject.SetActive(true);
            }
            else
            {
                imgRight.gameObject.SetActive(false);
            }

            if (right)
            {
                imgLeft.gameObject.SetActive(true);
            }
            else
            {
                imgLeft.gameObject.SetActive(false);
            }

            if (up)
            {
                imgUp.gameObject.SetActive(true);
            }
            else
            {
                imgUp.gameObject.SetActive(false);
            }

            if (down)
            {
                imgDown.gameObject.SetActive(true);
            }
            else
            {
                imgDown.gameObject.SetActive(false);
            }


            if ((deltaX >= minX || deltaX <= -minX) || (deltaY >= minY || deltaY <= -minY))
            {
                if (OnRubbingEndEvent != null)
                {
                    imgBack.gameObject.SetActive(false);
                    OnEndDrag(eventData);
                    OnRubbingEndEvent(this);
                    //Hide();
                }
            }
        }  


    }

}