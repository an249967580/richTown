using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TexasBuyChipView : MonoBehaviour, IPointerClickHandler
{
    public Button BuyBtn;
    public Button CloseBtn;
    public Button AddBtn;
    public Button RedBtn;


    public Slider ChipSlider;
    public Text BuyChipTxt;
    public Text CurChipTxt;

    void Start ()
    {
        gameObject.SetActive(true);
        ChipSlider.onValueChanged.AddListener(delegate {
            RaiseSliderChanged();
        });
        CloseBtn.onClick.AddListener(delegate {
            Close();
        });
        AddBtn.onClick.AddListener(delegate {
            if (ChipSlider.value < ChipSlider.maxValue) {
                ChipSlider.value = ChipSlider.value + 1;
            }
        });
        RedBtn.onClick.AddListener(delegate {
            if (ChipSlider.value > 0)
            {
                ChipSlider.value = ChipSlider.value - 1;
            }
        });

    }

    public void RaiseSliderChanged()
    {
        BuyChipTxt.text = ChipSlider.value.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != gameObject)
        {
            return;
        }
        if (gameObject.activeSelf)
        {
            Close();
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
