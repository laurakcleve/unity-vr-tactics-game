using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class Unit : MonoBehaviour {

	public int startingX;
	public int startingZ;
	public int rotation;
	public int move;
    public float moveTime = .7f;

    private GameObject currentTile;
	private List<GameObject> validMoves;
	private bool hasMoved = false;

    public GameObject CurrentTile { 
		get { return currentTile; } set { currentTile = value; } }


	/* AWAKE
	-------------------------------------------------------- */
    void Awake() {
		GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += new InteractableObjectEventHandler(ShowValidMovesOnGrab);
		GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += new InteractableObjectEventHandler(HideValidMovesOnUngrab);
	}


    /* TAKE TURN
	-------------------------------------------------------- */
    public void TakeTurn() {
		hasMoved = false;
		if (GameManager.instance.MovementType == GameManager.movementTypes.grab) {
			GetComponent<VRTK_InteractableObject>().isGrabbable = true;
		}
		
		validMoves = new List<GameObject>();
        validMoves = GetValidMoves(CurrentTile);
		ShowValidMoves();
	}


	/* SHOW VALID MOVES ON GRAB
	-------------------------------------------------------- */
	void ShowValidMovesOnGrab(object sender, InteractableObjectEventArgs e) {
		ShowValidMoves();
	}


	/* SHOW VALID MOVES
	-------------------------------------------------------- */
	private void ShowValidMoves() {
        foreach (GameObject tile in validMoves) {
            tile.GetComponent<Renderer>().enabled = true;
        }
    }


	/* HIDE VALID MOVES
	-------------------------------------------------------- */
	public void HideValidMoves() {
		foreach (GameObject tile in validMoves) {
            tile.GetComponent<Renderer>().enabled = false;
        }
	}


	/* HIDE VALID MOVES ON UNGRAB
	-------------------------------------------------------- */
	void HideValidMovesOnUngrab(object sender, InteractableObjectEventArgs e) {
		HideValidMoves();
	}


	/* GET VALID MOVES
	-------------------------------------------------------- */
	private List<GameObject> GetValidMoves(GameObject startTile)
    {
        validMoves.Add(startTile);
        List<GameObject> connectedTiles = startTile.GetComponent<Tile>().connected;
        foreach (GameObject tile in connectedTiles) {
            Tile currentTileScript = CurrentTile.GetComponent<Tile>();
            Tile targetTileScript = tile.GetComponent<Tile>();
            if ((Mathf.Abs(targetTileScript.X - currentTileScript.X) + Mathf.Abs(targetTileScript.Z - currentTileScript.Z) <= move) && !validMoves.Contains(tile)) {
                GetValidMoves(tile);
            }
        }

        return validMoves;
    }


	/* MOVE TO TILE
	-------------------------------------------------------- */
	public void MoveToTile(GameObject destinationTile, List<Tile> path) {
		if (!hasMoved) {
			// transform.position = tile.transform.position;
			StartCoroutine(AnimateMove(path));
			CurrentTile = destinationTile;
			hasMoved = true;
			HideValidMoves();
			GetComponent<VRTK_InteractableObject>().isGrabbable = false;
		}
	}


	/* ANIMATE MOVE
	-------------------------------------------------------- */
	IEnumerator AnimateMove(List<Tile> path) {
		foreach (Tile tile in path) {

			// face next tile

			Tile thisTile = currentTile.GetComponent<Tile>();
			int rotation = 0;

			if (tile.Z > thisTile.Z) {
				rotation = 0;
			}
			else if (tile.Z < thisTile.Z) {
				rotation = 180;
			}
			else if (tile.X > thisTile.X) {
				rotation = 90;
			}
			else{
				rotation = 270;
			}

			gameObject.transform.eulerAngles = new Vector3(0, rotation, 0);

			float time = moveTime;
			float elapsedTime = 0;
			Vector3 startingPos = transform.position;
			while (elapsedTime < time)
			{
				transform.position = Vector3.Lerp(
					startingPos,
					tile.gameObject.transform.position,
					(elapsedTime / time)
				);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			currentTile = tile.gameObject;
		}
	}
}
