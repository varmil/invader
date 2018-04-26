using UnityEngine;

/// <summary>
/// GameObject 型の拡張メソッドを管理するクラス
/// </summary>
public static class GameObjectExtensions
{
    /// <summary>
    /// 指定されたコンポーネントがアタッチされているかどうかを返します
    /// </summary>
    public static bool HasComponent<T>(this GameObject self) where T : Component
    {
        return self.GetComponent<T>() != null;
    }

    /// <summary>
    /// 子供が持つ全てのMeshRendererのenableを切り替える
    /// </summary>
    public static void ToggleMeshRenderer(this GameObject self, bool enable)
    {
        var renderers = self.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = enable;
        }
    }

    /// <summary>
    /// 子供が持つ全てのBoxColliderのenableを切り替える
    /// </summary>
    public static void ToggleBoxCollider(this GameObject self, bool enable)
    {
        var colliders = self.GetComponentsInChildren<BoxCollider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = enable;
        }
    }
}