#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AddCollidersMenu
{
    [MenuItem("Tools/Map/Thêm MeshCollider vào tất cả vật thể đang chọn")]
    private static void AddMeshCollidersToSelected()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("Thông báo", "Vui lòng chọn GameObject (ví dụ: '1' hoặc '2') trong Hierarchy trước!", "OK");
            return;
        }

        int count = 0;
        foreach (GameObject root in selectedObjects)
        {
            MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>(true);
            foreach (MeshFilter mf in meshFilters)
            {
                if (mf.GetComponent<Collider>() == null)
                {
                    Undo.AddComponent<MeshCollider>(mf.gameObject);
                    count++;
                }
            }
        }

        EditorUtility.DisplayDialog("Hoàn tất", $"Đã thêm thành công MeshCollider vào {count} mesh!", "OK");
    }
}
#endif
