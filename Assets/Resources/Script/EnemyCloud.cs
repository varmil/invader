using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/**
 * EnemyZone内に存在する敵の塊
 */
public class EnemyCloud : MonoBehaviour {
	// 列ごとの移動秒数差
	private static readonly float MovingIntervalPerLine = 0.15f;
	// １回の移動でどの程度X軸方向に動くか
	private static readonly float MovingAmountX = 0.25f;
	// 最も高い位置の初期Y座標
	private static readonly float FirstLineYPos = 15f;

	public List<Line> Lines {
		get { return lines; }
	}

	public float RightEnd {
		get {
			Debug.Log ("Cloud RightEnd::" + Mathf.Max (Lines.Select ((e) => e.RightEnd).ToArray()).ToString());
			return Mathf.Max (Lines.Select ((e) => e.RightEnd).ToArray());
		}
	}

	public float LeftEnd {
		get { 
			Debug.Log ("Cloud LeftEnd::" + Mathf.Min (Lines.Select ((e) => e.LeftEnd).ToArray()).ToString());
			return Mathf.Min (Lines.Select ((e) => e.LeftEnd).ToArray());
		}
	}

	private List<Line> lines = new List<Line>(5);

	void Awake () {
		this.lines.Add(transform.Find ("1st").GetComponent<Line>());
		this.lines.Add(transform.Find ("2nd").GetComponent<Line>());
		this.lines.Add(transform.Find ("3rd").GetComponent<Line>());
		this.lines.Add(transform.Find ("4th").GetComponent<Line>());
		this.lines.Add(transform.Find ("5th").GetComponent<Line>());

		SetLinesInitialPosition();

		// create enemies
		Enumerable
			.Range (0, this.Lines.Count)
			.Select ((i) => this.lines[i].CreateEnemies())
			.ToArray ();
	}
		
	public IEnumerator MoveRight() {
		for (int i = this.Lines.Count - 1; i >= 0; i--) {
			this.lines[i].transform.position += new Vector3(MovingAmountX, 0);
			yield return new WaitForSeconds (MovingIntervalPerLine);
		}
	}

	public IEnumerator MoveLeft() {
		for (int i = this.Lines.Count - 1; i >= 0; i--) {
			this.lines[i].transform.position += new Vector3(-MovingAmountX, 0f);
			yield return new WaitForSeconds (MovingIntervalPerLine);
		}
	}

	public IEnumerator MoveDown() {
		for (int i = this.Lines.Count - 1; i >= 0; i--) {
			this.lines[i].transform.position += new Vector3(0f, -1f);
			yield return new WaitForSeconds (MovingIntervalPerLine);
		}
	}

	private void SetLinesInitialPosition() {
		for (int i = 0; i < this.lines.Count; i++) {
			this.lines[i].transform.position = new Vector3 (0f, FirstLineYPos - (i * 1.5f), 0f);
		}
	}
}
