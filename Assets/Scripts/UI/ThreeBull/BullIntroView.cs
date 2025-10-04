using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BullIntroView : MonoBehaviour, IPointerClickHandler
{

    public Text SGTabTitle;
    public Text NNTabTitle;

    public Text SGTypeIntroTxt1;
    public Text SGTypeNameTxt1;
    public Text SGTypeIntroTxt2;
    public Text SGTypeNameTxt2;
    public Text SGTypeIntroTxt3;
    public Text SGTypeNameTxt3;
    public Text SGTypeIntroTxt4;
    public Text SGTypeNameTxt4;
    public Text SGTypeIntroTxt5;
    public Text SGTypeNameTxt5;
    public Text SGHintTxt;

    public Text NNTypeIntroTxt1;
    public Text NNTypeNameTxt1;
    public Text NNTypeIntroTxt2;
    public Text NNTypeNameTxt2;
    public Text NNTypeIntroTxt3;
    public Text NNTypeNameTxt3;
    public Text NNTypeIntroTxt4;
    public Text NNTypeNameTxt4;
    public Text NNTypeIntroTxt5;
    public Text NNTypeNameTxt5;
    public Text NNTypeIntroTxt6;
    public Text NNTypeNameTxt6;
    public Text NNTypeIntroTxt7;
    public Text NNTypeNameTxt7;
    public Text NNHintTxt;

    void Start () {
		
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

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
