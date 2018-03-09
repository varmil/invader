using UnityEngine;
using System.Linq;

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
}