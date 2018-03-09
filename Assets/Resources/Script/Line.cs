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

	public List<Enemy> Enemies {
		get { return enemies; }
	}
	private List<Enemy> enemies = new List<Enemy>(11);

	private GameObject basicEnemyPrefab;

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

	public IEnumerable<Enemy> CreateEnemies() {
		for (int i = 0; i < EnemyAmountPerLine; i++) {
			var position = new Vector3((float)(i * EnemyXSpace - OffsetX), 0f, 0f);
			var obj = (GameObject)Instantiate (basicEnemyPrefab, position, Quaternion.identity);			
			obj.transform.SetParent(this.transform, false);

			var enemy = obj.GetComponent<Enemy> ();
			enemy.Id = this.name + "-enemy::" + i;
			enemies.Add (enemy);
		}
		return enemies;
	}
}
