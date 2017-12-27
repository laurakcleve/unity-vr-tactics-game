using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {

	public int startingX;
	public int startingZ;
	public int rotation;
	public int move;
	public GameObject currentTile;

	private List<GameObject> validMoves;
	private bool hasMoved = false;


    /* TAKE TURN
	-------------------------------------------------------- */
    public void TakeTurn() {
		hasMoved = false;
		DisplayValidMoves();
	}


	/* HIDE VALID MOVES
	-------------------------------------------------------- */
	public void HideValidMoves() {
		foreach (GameObject tile in validMoves) {
            tile.GetComponent<Renderer>().enabled = false;
        }
	}


	/* DISPLAY VALID MOVES
	-------------------------------------------------------- */
	private void DisplayValidMoves()
    {
        validMoves = new List<GameObject>();
        validMoves = GetValidMoves(currentTile);

        foreach (GameObject tile in validMoves) {
            tile.GetComponent<Renderer>().enabled = true;
        }
    }


	/* GET VALID MOVES
	-------------------------------------------------------- */
	private List<GameObject> GetValidMoves(GameObject startTile)
    {
        validMoves.Add(startTile);
        List<GameObject> connectedTiles = startTile.GetComponent<Tile>().connected;
        foreach (GameObject tile in connectedTiles) {
            Tile currentTileScript = currentTile.GetComponent<Tile>();
            Tile targetTileScript = tile.GetComponent<Tile>();
            if ((Mathf.Abs(targetTileScript.X - currentTileScript.X) + Mathf.Abs(targetTileScript.Z - currentTileScript.Z) <= move) && !validMoves.Contains(tile)) {
                GetValidMoves(tile);
            }
        }

        return validMoves;
    }


	/* MOVE TO TILE
	-------------------------------------------------------- */
	public void MoveToTile(GameObject tile) {
		if (!hasMoved) {
			transform.position = tile.transform.position;
			currentTile = tile;
			hasMoved = true;
			HideValidMoves();
		}
	}

}
