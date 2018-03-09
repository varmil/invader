using UnityEngine;
using System.Collections;

/**
 * Enemyが射出するビーム
 */
public class Beam : MonoBehaviour {
	// BeamObjectを開放するY座標。
	private static readonly float ReleaseYPos = -20f;
	private static readonly float Speed = 0.2f;

	void Update () {
		transform.position += Vector3.down * Speed;

		if (transform.position.y < ReleaseYPos) {
			ObjectPool.Instance.Release (gameObject);
		}
	}

	// 弾が何らかのトリガーに当たった時に呼び出される
	void OnTriggerEnter (Collider other)
	{
		Debug.Log ("Beam collide other:: " + other.gameObject.name);

		// 弾の削除。実際には非アクティブにする
		ObjectPool.Instance.Release (gameObject);
	}
}
