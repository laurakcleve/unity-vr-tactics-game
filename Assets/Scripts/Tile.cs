using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Tile : MonoBehaviour {

	private int x;
	private int z;
	private float height;
	private GameManager gm;
    public List<GameObject> connected;

    public int X { 
		get { return x; } set { x = value; } }

    public int Z { 
		get { return z; } set { z = value; } }

    public float Height { 
		get { return height; } set { height = value; } }

    // public List<GameObject> Connected { 
	// 	get { return connected; } set { connected = value; } }


    void Awake() {
		GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += new InteractableObjectEventHandler(MoveUnitHere);

		gm = GameManager.instance;
	}


	void MoveUnitHere(object sender, InteractableObjectEventArgs e) {
		gm.Units[gm.ActiveUnit].GetComponent<Unit>().MoveToTile(gameObject);
	}
}
