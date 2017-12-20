using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	public int startingX;
	public int startingZ;
	public int rotation;
	// public GameManager gm;

	void Awake() {
		// gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		// Debug.Log("unit's gm instance: " + gm.GetInstanceID);
	}

	public void TakeTurn() {
		Debug.Log(gameObject.name + " taking turn");
		GameManager.instance.PassTurn();
	}

}
