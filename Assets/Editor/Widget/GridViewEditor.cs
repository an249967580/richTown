using UnityEditor;
using UnityEngine;

public class GridViewEditor : Editor
{

    [MenuItem("GameObject/UI/GridView")]
    static void CreateBloodSlider()
    {
        Canvas canvas = FindObjectOfType<Canvas>(); ;

        if (canvas == null)
        {
            EditorUtility.DisplayDialog("提示", "场景中不存在至少一个画布！", "确定");
            return;
        }

        GameObject bloodSlider = Instantiate(Resources.Load("Widget/GridView") as GameObject);
        bloodSlider.name = "GridView";
        bloodSlider.transform.SetParent(canvas.gameObject.transform);
        bloodSlider.transform.localPosition = Vector3.zero;
        bloodSlider.transform.localScale = Vector3.one;
    }
}
