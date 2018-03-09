using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	// 移動速度
	private static readonly float Speed = 0.2F;
	// Fire時、自分のどの程度上にBeamを出現させるか
	private static readonly float BeamOffsetYRate = 0.1f;

	public float Height {
		get { return transform.localScale.y; }
	}

	private GameObject beamPrefab;

	void Awake()
	{
		beamPrefab = (GameObject)Resources.Load ("Prefab/PlayerBeam");
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

		if (Input.GetKeyDown(KeyCode.Space)) {
			this.Fire ();
		}
	}

	public void Fire() {
		// the beam instance can only exist one at the same time
		if (ObjectPool.Instance.CountActive(beamPrefab) != 0) {
			return;
		}

		var myPos = this.transform.position;
		var beamPos = myPos + (Vector3.up * Height * BeamOffsetYRate);
		var obj = ObjectPool.Instance.Get (beamPrefab, beamPos, Quaternion.identity);			

		// beam callback
		obj.GetComponent<Beam> ().OnCollided = (other) => {
			ObjectPool.Instance.Release (obj);

			// check other is Enemy or not
			var component = other.gameObject.GetComponent<Enemy>();
			if (component != null) {
				component.Dead();
			}
		};
	}

	public void Dead() {
		// TODO
	}

//	void OnTriggerEnter (Collider other)
//	{
//		Debug.Log ("Player OBJ collide other:: " + other.gameObject.name);
//	}
}
