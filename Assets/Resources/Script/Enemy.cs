using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	// Fire時、自分のどの程度下にBeamを出現させるか
	private static readonly float BeamOffsetYRate = 2.5f;

	// デバッグ用
	public string Id { get; set; }

	// 生存しているか
	public bool Alive {
		get;
		private set;
	}

	public float Height {
		get { return transform.localScale.y; }
	}

	private GameObject beamPrefab;

	void Awake () {
		beamPrefab = (GameObject)Resources.Load ("Prefab/EnemyBeam");
		this.Alive = true;
	}

	public void Fire() {
		var myPos = this.transform.position;
		var beamPos = myPos + (Vector3.down * Height * BeamOffsetYRate);
		var obj = ObjectPool.Instance.Get (beamPrefab, beamPos, Quaternion.identity);			

		// beam callback
		obj.GetComponent<Beam> ().OnCollided = (other) => {
			ObjectPool.Instance.Release (obj);
		};
	}

	public void Dead() {
		this.Alive = false;
		this.gameObject.SetActive (false);
	}

//	void OnTriggerEnter (Collider other)
//	{
//		Debug.Log ("Enemy OBJ collide other:: " + other.gameObject.name);
//	}
}
