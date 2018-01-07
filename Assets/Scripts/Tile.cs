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

    private int gCost;
    private int hCost;
    private int fCost;
    private Tile parent;

    public int X { 
		get { return x; } set { x = value; } }

    public int Z { 
		get { return z; } set { z = value; } }

    public float Height { 
		get { return height; } set { height = value; } }

    public int GCost { 
        get { return gCost; } set { gCost = value; } }
    
    public int HCost { 
        get { return hCost; } set { hCost = value; } }
    
    public int FCost { 
        get { return GCost + HCost; } }

    public Tile Parent { 
        get { return parent; } set { parent = value; } }




    /* AWAKE
    -------------------------------------------------------- */
    void Awake() {
		GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += new InteractableObjectEventHandler(MoveUnitHere);

		gm = GameManager.instance;
	}


	/* MOVE UNIT HERE
	-------------------------------------------------------- */
	void MoveUnitHere(object sender, InteractableObjectEventArgs e) {
        Unit unitScript = gm.Units[gm.ActiveUnit].GetComponent<Unit>();
        List<Tile> path = GetComponent<Pathfinding>().FindPath(unitScript.CurrentTile.GetComponent<Tile>(), this);
		unitScript.MoveToTile(gameObject, path);
	}


    public void SetCosts(Tile start, Tile end) {
        GCost = Mathf.Abs(X - start.X) + Mathf.Abs(Z - start.Z);
        HCost = Mathf.Abs(X - end.X) + Mathf.Abs(Z - end.Z);
    }
}
