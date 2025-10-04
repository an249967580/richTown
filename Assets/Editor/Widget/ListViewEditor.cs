using UnityEditor;
using UnityEngine;

public class ListViewEditor : Editor {

    [MenuItem("GameObject/UI/ListView")]
    static void CreateBloodSlider()
    {
        Canvas canvas = FindObjectOfType<Canvas>(); ;

        if (canvas == null)
        {
            EditorUtility.DisplayDialog("提示", "场景中不存在至少一个画布！", "确定");
            return;
        }

        GameObject bloodSlider = Instantiate(Resources.Load("Widget/ListView") as GameObject);
        bloodSlider.name = "ListView";
        bloodSlider.transform.SetParent(canvas.gameObject.transform);
        bloodSlider.transform.localPosition = Vector3.zero;
        bloodSlider.transform.localScale = Vector3.one;
    }
}
