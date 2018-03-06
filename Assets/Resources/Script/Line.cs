using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/**
 * EnemyCloud内に存在する敵の列（1列）
 */
public class Line : MonoBehaviour {
	// １列の敵の数
	private static readonly int EnemyAmountPerLine = 11;
	// 左右に隣り合う敵との距離
	private static readonly float EnemyXSpace = 1.6f;
	// 敵の初期位置をセンタリングするためのオフセット
	private static readonly float OffsetX = EnemyXSpace * (EnemyAmountPerLine - 1) / 2;

	private GameObject basicEnemyPrefab;
	private List<GameObject> enemies = new List<GameObject>(11);

	void Awake () {
		basicEnemyPrefab = (GameObject)Resources.Load ("Prefab/Enemy");
	}

	public float RightEnd {
		get { 
			var myPos = this.transform.position.x;
			var offset = Mathf.Max (enemies.Select ((e) => e.transform.position.x).ToArray());
			return myPos + offset; 
		}
	}

	public float LeftEnd {
		get { 
			var myPos = this.transform.position.x;
			var offset = Mathf.Min (enemies.Select ((e) => e.transform.position.x).ToArray());
			return myPos + offset; 
		}
	}

	public IEnumerable<GameObject> CreateEnemies() {
		for (int i = 0; i < EnemyAmountPerLine; i++) {
			var position = new Vector3((float)(i * EnemyXSpace - OffsetX), 0f, 0f);
			var obj = (GameObject)Instantiate (basicEnemyPrefab, position, Quaternion.identity);			
			obj.transform.SetParent(this.transform, false);
			enemies.Add (obj);
		}
		return enemies;
	}
}
