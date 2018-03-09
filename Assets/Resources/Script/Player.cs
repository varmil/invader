using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	private static readonly float Speed = 0.3F;

//	private CharacterController controller;

	void Awake()
	{
		// コンポーネントの取得
//		controller = GetComponent<CharacterController>();
	}

	void Update () {
		if (Input.GetKey(KeyCode.A)) {
			if (transform.position.x > Constants.Stage.LeftEnd) {
				transform.position += Vector3.left * Speed;
			}
		}

		if (Input.GetKey(KeyCode.D)) {
			if (transform.position.x < Constants.Stage.RightEnd) {
				transform.position += Vector3.right * Speed;
			}
		}
	}
}
