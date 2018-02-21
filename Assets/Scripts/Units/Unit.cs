using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class Unit : MonoBehaviour {

	public int startingX;
	public int startingZ;
	public int startingRotation;
	public int moveRange;
	public int attackRange;
    public float moveTime = .5f;
	public float receiveAttackDelay = .7f;

    protected GameObject currentTile;
	protected List<GameObject> validMoves;
	protected List<GameObject> validAttacks;
	protected bool hasMoved = false;
	protected bool hasActed = false;
	protected Animator animator;
	protected Button moveButton;
	protected Button attackButton;
	protected bool moveActive = false;
	protected bool attackActive = false;
	protected GameManager gm;
	protected GameObject targetUnit;
	protected bool moveComplete;
	protected float attackAnimationLength;
	protected float receiveAttackAnimationLength;

    public GameObject CurrentTile { 
		get { return currentTile; } set { currentTile = value; } }


    protected virtual void Awake() {
        gm = GameManager.instance;

        VRTK_InteractableObject interactable = GetComponent<VRTK_InteractableObject>();

        interactable.InteractableObjectUsed += new InteractableObjectEventHandler(ReturnThisUnit);

        animator = GetComponent<Animator>();
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++) {
            if (ac.animationClips[i].name == "WAIT04") {
                attackAnimationLength = ac.animationClips[i].length / 1.5f;
            }
			if (ac.animationClips[i].name == "DAMAGED00") {
				receiveAttackAnimationLength = ac.animationClips[i].length;
			}
        }
		// gm.ActionCanvas.SetActive(false);
	} 

    public virtual void TakeTurn() {
		hasMoved = false;
		hasActed = false;
		moveActive = false;
		attackActive = false;

		gameObject.transform.Find("Highlight").gameObject.SetActive(true);
	}

	public virtual void ActivateMove() {
		validMoves = new List<GameObject>();
		validMoves = GetValidMoves(CurrentTile);
		moveActive = true;
	}

	public virtual void ActivateAttack() {
		validAttacks = new List<GameObject>();
		validAttacks = GetValidAttacks(CurrentTile);
		attackActive = true;
	}

	protected List<GameObject> GetValidMoves(GameObject startTile) {
		validMoves = CheckNextTile(startTile);
		foreach (GameObject unit in gm.Units) {
			validMoves.Remove(unit.GetComponent<Unit>().CurrentTile);
		}
		return validMoves;
	}

	protected List<GameObject> CheckNextTile(GameObject startTile) {
        validMoves.Add(startTile);
        List<GameObject> connectedTiles = startTile.GetComponent<Tile>().connected;
        foreach (GameObject tile in connectedTiles) {
            Tile currentTileScript = CurrentTile.GetComponent<Tile>();
            Tile targetTileScript = tile.GetComponent<Tile>();
            if ((Mathf.Abs(targetTileScript.X - currentTileScript.X) + Mathf.Abs(targetTileScript.Z - currentTileScript.Z) <= moveRange) && !validMoves.Contains(tile)) {
                CheckNextTile(tile);
            }
        }

        return validMoves;
    }

    protected List<GameObject> GetValidAttacks(GameObject startTile)
    {
        validAttacks.Add(startTile);
        List<GameObject> connectedTiles = startTile.GetComponent<Tile>().connected;
        foreach (GameObject tile in connectedTiles)
        {
            Tile currentTileScript = CurrentTile.GetComponent<Tile>();
            Tile targetTileScript = tile.GetComponent<Tile>();
            if ((Mathf.Abs(targetTileScript.X - currentTileScript.X) + Mathf.Abs(targetTileScript.Z - currentTileScript.Z) <= attackRange) && !validAttacks.Contains(tile)) {
                GetValidAttacks(tile);
            }
        }
        return validAttacks;
    }

    protected void ShowValidTiles(List<GameObject> tiles) {
        foreach (GameObject tile in tiles) {
            tile.GetComponent<Renderer>().enabled = true;
        }
    }

    public void HideValidTiles(List<GameObject> tiles) {
        foreach (GameObject tile in tiles) {
            tile.GetComponent<Renderer>().enabled = false;
        }
    }

	public virtual void MoveToTile(GameObject destinationTile, List<Tile> path) {
        CurrentTile.GetComponent<Tile>().CurrentUnit = null;
		StartCoroutine(AnimateMove(path));
        // CurrentTile = destinationTile;
		hasMoved = true;
		moveActive = false;
	}

	protected virtual IEnumerator AnimateMove(List<Tile> path) {
        animator.SetBool("moving", true);
		moveComplete = false;

        foreach (Tile tile in path) {
			Tile thisTile = currentTile.GetComponent<Tile>();
			int rotation = 0;

			if (tile.Z > thisTile.Z) 		rotation = 0;
			else if (tile.Z < thisTile.Z) 	rotation = 180;
			else if (tile.X > thisTile.X)	rotation = 90;
			else							rotation = 270;

			gameObject.transform.eulerAngles = new Vector3(0, rotation, 0);

			float time = moveTime;
			float elapsedTime = 0;
			Vector3 startingPos = transform.position;
            while (elapsedTime < time) {
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
		moveComplete = true;
        CurrentTile.GetComponent<Tile>().CurrentUnit = gameObject;
	}


	/* RETURN THIS UNIT
	-------------------------------------------------------- */
    // Event registered in Unit.Awake()
    // Handles the selection of the unit to attack, calls the active unit's Attack() function

    protected void ReturnThisUnit(object sender, InteractableObjectEventArgs e) {
		gm.Units[gm.ActiveUnit].GetComponent<Unit>().Attack(this.gameObject);
	}



	/* ATTACK
	-------------------------------------------------------- */
	// Turns toward the target unit and plays the attack animation
	// Calls other unit's ReceiveAttack() function

	public virtual void Attack(GameObject otherUnit) {
		Debug.Log("Attack called");
		targetUnit = otherUnit;
		Tile thisTile = CurrentTile.GetComponent<Tile>();
		Tile otherTile = targetUnit.GetComponent<Unit>().CurrentTile.GetComponent<Tile>();

		if (validAttacks.Contains(otherTile.gameObject)) {
			int rotation = 0;

            if (thisTile.Z > otherTile.Z) 		rotation = 180;
            else if (thisTile.Z < otherTile.Z) 	rotation = 0;
            else if (thisTile.X > otherTile.X) 	rotation = 270;
            else	 							rotation = 90;

            gameObject.transform.eulerAngles = new Vector3(0, rotation, 0);

			animator.SetTrigger("attack");
			otherUnit.GetComponent<Unit>().ReceiveAttack();
			hasActed = true;
			// StartCoroutine(AnimateAttack(otherUnit));
		}
	}

	// protected IEnumerator AnimateAttack(GameObject otherUnit) {
	// 	yield return null;
	// }

	public void ReceiveAttack() {
		StartCoroutine(AnimateReceiveAttack());
	}

	IEnumerator AnimateReceiveAttack() {
		yield return new WaitForSeconds(receiveAttackDelay);
		animator.SetTrigger("receiveAttack");
	}
}

