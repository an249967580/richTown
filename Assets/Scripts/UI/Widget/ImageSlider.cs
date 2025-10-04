using UnityEngine;
using UnityEngine.UI;

public class ImageSlider : RawImage {

    private Slider _slider;

    protected override void Awake()
    {
        base.Awake();
        _slider = transform.parent.parent.GetComponent<Slider>();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        if(_slider)
        {
            float value = _slider.value;
            uvRect = new Rect(0, 0, value, 1);
        }

    }
}
