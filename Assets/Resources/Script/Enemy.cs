using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	// Fire時、自分のどの程度下にBeamを出現させるか
	private static readonly float BeamOffsetYRate = 2.5f;

	// デバッグ用
	public string Id { get; set; }

	// 生存しているか
	public bool Alive {
		get { return true; }
	}

	public float Height {
		get { return transform.localScale.y; }
	}

	private GameObject beamPrefab;

	void Awake () {
		beamPrefab = (GameObject)Resources.Load ("Prefab/Beam");
	}

	// Fire the beam
	public void Fire() {
		Debug.Log ("Fire! " + this.Id);

		var myPos = this.transform.position;
		var beamPos = myPos + (Vector3.down * Height * BeamOffsetYRate);
		var obj = ObjectPool.Instance.Get (beamPrefab, beamPos, Quaternion.identity);			
	}
}
