using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class Unit : MonoBehaviour {

	public int startingX;
	public int startingZ;
	public int rotation;
	public int moveRange;
	public int attackRange;
    public float moveTime = .7f;
	public float receiveAttackDelay = .7f;

    private GameObject currentTile;
	private List<GameObject> validMoves;
	private List<GameObject> validAttacks;
	private bool hasMoved = false;
	private bool hasActed = false;
	private Animator animator;
	private Button moveButton;
	private Button attackButton;
	private bool moveActive = false;
	private bool attackActive = false;
	private GameManager gm;
	private GameObject targetUnit;

    public GameObject CurrentTile { 
		get { return currentTile; } set { currentTile = value; } }


	/* AWAKE
	-------------------------------------------------------- */
    void Awake() {
		GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += new InteractableObjectEventHandler(ShowValidTilesOnGrab);
		GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += new InteractableObjectEventHandler(HideValidTilesOnUngrab);
		GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += new InteractableObjectEventHandler(ReturnThisUnit);

		animator = GetComponent<Animator>();
		moveButton = GameObject.Find("MoveButton").GetComponent<Button>();
		attackButton = GameObject.Find("AttackButton").GetComponent<Button>();
		gm = GameManager.instance;
	}


    /* TAKE TURN
	-------------------------------------------------------- */
    public void TakeTurn() {
		hasMoved = false;
		hasActed = false;
		moveActive = false;
		attackActive = false;

		moveButton.onClick.AddListener(ActivateMove);
		attackButton.onClick.AddListener(ActivateAttack);
		
		moveButton.interactable = true;
		attackButton.interactable = true;

		gameObject.transform.Find("Highlight").gameObject.SetActive(true);
	}


	/* ACTIVATE MOVE
	-------------------------------------------------------- */
	public void ActivateMove() {
		if(!hasMoved) {

			if (GameManager.instance.MovementType == GameManager.movementTypes.grab) {
				GetComponent<VRTK_InteractableObject>().isGrabbable = true;
			}

			validMoves = new List<GameObject>();
			validMoves = GetValidMoves(CurrentTile);
			ShowValidTiles(validMoves);
			ActivateMoveTiles();
			moveActive = true;
		}
	}


	/* ACTIVATE MOVE TILES
	-------------------------------------------------------- */
	void ActivateMoveTiles() {
		foreach (GameObject tile in validMoves) {
			tile.GetComponent<VRTK_InteractableObject>().isUsable = true;
		}
	}


	/* DEACTIVATE MOVE TILES
	-------------------------------------------------------- */
	void DeactivateMoveTiles() {
		foreach (GameObject tile in validMoves) {
            tile.GetComponent<VRTK_InteractableObject>().isUsable = false;
        }
	}


	/* ACTIVATE ATTACK
	-------------------------------------------------------- */
	public void ActivateAttack() {
		if(!hasActed) {
			validAttacks = new List<GameObject>();
			validAttacks = GetValidAttacks(CurrentTile);
			ShowValidTiles(validAttacks);
			attackActive = true;
		}
	}


	/* SHOW VALID TILES ON GRAB
	-------------------------------------------------------- */
	void ShowValidTilesOnGrab(object sender, InteractableObjectEventArgs e) {
		ShowValidTiles(validMoves);
	}


	/* SHOW VALID TILES
	-------------------------------------------------------- */
	private void ShowValidTiles(List<GameObject> tiles) {
        foreach (GameObject tile in tiles) {
            tile.GetComponent<Renderer>().enabled = true;
        }
    }


	/* HIDE VALID TILES
	-------------------------------------------------------- */
	public void HideValidTiles(List<GameObject> tiles) {
		foreach (GameObject tile in tiles) {
            tile.GetComponent<Renderer>().enabled = false;
        }
	}


	/* HIDE VALID TILES ON UNGRAB
	-------------------------------------------------------- */
	void HideValidTilesOnUngrab(object sender, InteractableObjectEventArgs e) {
		HideValidTiles(validMoves);
	}


	/* GET VALID MOVES
	-------------------------------------------------------- */
	private List<GameObject> GetValidMoves(GameObject startTile) {
        validMoves.Add(startTile);
        List<GameObject> connectedTiles = startTile.GetComponent<Tile>().connected;
        foreach (GameObject tile in connectedTiles) {
            Tile currentTileScript = CurrentTile.GetComponent<Tile>();
            Tile targetTileScript = tile.GetComponent<Tile>();
            if ((Mathf.Abs(targetTileScript.X - currentTileScript.X) + Mathf.Abs(targetTileScript.Z - currentTileScript.Z) <= moveRange) && !validMoves.Contains(tile)) {
                GetValidMoves(tile);
            }
        }

        return validMoves;
    }


    /* GET VALID ATTACKS
	-------------------------------------------------------- */
    private List<GameObject> GetValidAttacks(GameObject startTile)
    {
        validAttacks.Add(startTile);
        List<GameObject> connectedTiles = startTile.GetComponent<Tile>().connected;
        foreach (GameObject tile in connectedTiles)
        {
            Tile currentTileScript = CurrentTile.GetComponent<Tile>();
            Tile targetTileScript = tile.GetComponent<Tile>();
            if ((Mathf.Abs(targetTileScript.X - currentTileScript.X) + Mathf.Abs(targetTileScript.Z - currentTileScript.Z) <= attackRange) && !validAttacks.Contains(tile))
            {
                GetValidAttacks(tile);
            }
        }

        return validAttacks;
    }


	/* MOVE TO TILE
	-------------------------------------------------------- */
	public void MoveToTile(GameObject destinationTile, List<Tile> path) {
		if (!hasMoved) {
            StartCoroutine(AnimateMove(path));
			CurrentTile = destinationTile;
			hasMoved = true;
			HideValidTiles(validMoves);
			DeactivateMoveTiles();
			moveActive = false;
			moveButton.interactable = false;
			GetComponent<VRTK_InteractableObject>().isGrabbable = false;
		}
	}


	/* ANIMATE MOVE
	-------------------------------------------------------- */
	IEnumerator AnimateMove(List<Tile> path) {
        animator.SetBool("moving", true);

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

			if (tile.Height > thisTile.Height + 0.5f || tile.Height < thisTile.Height - 0.5f) {
				animator.SetTrigger("jump");
			}


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
		animator.SetBool("moving", false);
	}


	/* RETURN THIS UNIT
	-------------------------------------------------------- */
	void ReturnThisUnit(object sender, InteractableObjectEventArgs e) {
		gm.Units[gm.ActiveUnit].GetComponent<Unit>().GetTargetUnit(this.gameObject);
	}

	void GetTargetUnit(GameObject otherUnit) {
		targetUnit = otherUnit;
		if (validAttacks.Contains(targetUnit.GetComponent<Unit>().CurrentTile)) {
			int rotation = 0;

            if (CurrentTile.GetComponent<Tile>().Z > targetUnit.GetComponent<Unit>().CurrentTile.GetComponent<Tile>().Z)
            {
                rotation = 180;
            }
            else if (CurrentTile.GetComponent<Tile>().Z < targetUnit.GetComponent<Unit>().CurrentTile.GetComponent<Tile>().Z)
            {
                rotation = 0;
            }
            else if (CurrentTile.GetComponent<Tile>().X > targetUnit.GetComponent<Unit>().CurrentTile.GetComponent<Tile>().X)
            {
                rotation = 270;
            }
            else
            {
                rotation = 90;
            }

            gameObject.transform.eulerAngles = new Vector3(0, rotation, 0);

			animator.SetTrigger("attack");
			otherUnit.GetComponent<Unit>().ReceiveAttack();

			HideValidTiles(validAttacks);
			attackActive = false;
			attackButton.interactable = false;
		}
	}

	public void ReceiveAttack() {
		StartCoroutine(AnimateReceiveAttack());
	}

	IEnumerator AnimateReceiveAttack() {
		yield return new WaitForSeconds(receiveAttackDelay);
		animator.SetTrigger("receiveAttack");
	}
}

