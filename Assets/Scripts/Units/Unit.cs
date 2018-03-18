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
    public GameObject currentTile;
	public int maxHealth = 20;
	public int currentHealth;
	public int damage = 8;
	public bool isDead = false;

	protected GameObject tempCurrentTile;
	protected List<GameObject> validMoves;
	protected List<GameObject> validAttacks;
	protected bool hasMoved = false;
	protected bool hasActed = false;
	protected Animator animator;
	protected Button moveButton;
	protected Button attackButton;
	protected GameManager gm;
	protected GameObject targetUnit;
	protected bool moveAnimationComplete;
	protected float attackAnimationLength;
	protected float receiveAttackAnimationLength;
    protected int currentRotation;
	protected int tempCurrentRotation;
	protected float attackWaitTime;
	protected Slider healthSlider;



    protected virtual void Awake() {
        gm = GameManager.instance;

		healthSlider = transform.Find("Canvas/Slider").gameObject.GetComponent<Slider>();
		healthSlider.maxValue = maxHealth;
		healthSlider.value = maxHealth;

        VRTK_InteractableObject interactable = GetComponent<VRTK_InteractableObject>();
        interactable.InteractableObjectUsed += new InteractableObjectEventHandler(ReturnThisUnit);

        animator = GetComponent<Animator>();
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++) {
            if (ac.animationClips[i].name == "Attack") {
                attackAnimationLength = ac.animationClips[i].length / 1.5f;
            }
			if (ac.animationClips[i].name == "BeAttacked") {
				receiveAttackAnimationLength = ac.animationClips[i].length;
			}
        }
		
        if (attackAnimationLength > receiveAttackAnimationLength + receiveAttackDelay)
            attackWaitTime = attackAnimationLength;
        else
            attackWaitTime = receiveAttackAnimationLength + receiveAttackDelay;

        currentRotation = startingRotation;
		currentHealth = maxHealth;

		Debug.Log(gameObject.GetInstanceID());
	} 

    public virtual void TakeTurn() {
		hasMoved = false;
		hasActed = false;
		gameObject.transform.Find("Highlight").gameObject.SetActive(true);
	}

	public virtual void ActivateMove() {
		validMoves = new List<GameObject>();
		validMoves = GetValidMoves(currentTile);
	}

	public virtual void ActivateAttack() {
		validAttacks = new List<GameObject>();
		validAttacks = GetValidAttacks(currentTile);
	}

	protected List<GameObject> GetValidMoves(GameObject startTile) {
		validMoves = CheckNextTile(startTile);
		foreach (GameObject unit in gm.Units) {
			validMoves.Remove(unit.GetComponent<Unit>().currentTile);
		}
		return validMoves;
	}

	protected List<GameObject> CheckNextTile(GameObject startTile) {
        validMoves.Add(startTile);
        List<GameObject> connectedTiles = startTile.GetComponent<Tile>().connected;
        foreach (GameObject tile in connectedTiles) {
            Tile currentTileScript = currentTile.GetComponent<Tile>();
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
            Tile currentTileScript = currentTile.GetComponent<Tile>();
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
		StartCoroutine(AnimateMove(path));
	}

	protected virtual IEnumerator AnimateMove(List<Tile> path) {
        animator.SetBool("moving", true);
		moveAnimationComplete = false;
		Tile thisTile = currentTile.GetComponent<Tile>();
		int rotation = 0;

        foreach (Tile nextTile in path) {

			if (nextTile.Z > thisTile.Z) 		rotation = 0;
			else if (nextTile.Z < thisTile.Z) 	rotation = 180;
			else if (nextTile.X > thisTile.X)	rotation = 90;
			else								rotation = 270;

			gameObject.transform.eulerAngles = new Vector3(0, rotation, 0);

			float time = moveTime;
			float elapsedTime = 0;
			Vector3 startingPos = transform.position;
            while (elapsedTime < time) {
                transform.position = Vector3.Lerp(
                    startingPos,
                    nextTile.gameObject.transform.position,
                    (elapsedTime / time)
                );
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            thisTile = nextTile;
        }
		tempCurrentRotation = rotation;
		tempCurrentTile = thisTile.gameObject;
		animator.SetBool("moving", false);
		moveAnimationComplete = true;
		FinishMove();
	}

	protected virtual void FinishMove() {}



	/* RETURN THIS UNIT
	-------------------------------------------------------- */
    // Event registered in Unit.Awake()
    // Handles the selection of the unit to attack, calls the active unit's Attack() function

    protected void ReturnThisUnit(object sender, InteractableObjectEventArgs e) {
		// gm.Units[gm.ActiveUnit].GetComponent<Unit>().Attack(this.gameObject);
		gm.Units[gm.ActiveUnit].GetComponent<Unit>().AskConfirmAttack(this.gameObject);
	}

	public virtual void AskConfirmAttack(GameObject otherUnit) {}


	/* ATTACK
	-------------------------------------------------------- */
	// Turns toward the target unit and plays the attack animation
	// Calls other unit's ReceiveAttack() function

	public virtual void Attack(GameObject otherUnit) {
		Debug.Log(gameObject.name + " attacking " + otherUnit.name);
		targetUnit = otherUnit;
		Tile thisTile = currentTile.GetComponent<Tile>();
		Tile otherTile = targetUnit.GetComponent<Unit>().currentTile.GetComponent<Tile>();

		if (validAttacks.Contains(otherTile.gameObject)) {
			int rotation = 0;

            if (thisTile.Z > otherTile.Z) 		rotation = 180;
            else if (thisTile.Z < otherTile.Z) 	rotation = 0;
            else if (thisTile.X > otherTile.X) 	rotation = 270;
            else	 							rotation = 90;

            gameObject.transform.eulerAngles = new Vector3(0, rotation, 0);

			animator.SetTrigger("attack");
			otherUnit.GetComponent<Unit>().ReceiveAttack(this);
			hasActed = true;
			// StartCoroutine(AnimateAttack(otherUnit));
		}
	}

	// protected IEnumerator AnimateAttack(GameObject otherUnit) {
	// 	yield return null;
	// }

	public void ReceiveAttack(Unit otherUnit) {
		StartCoroutine(AnimateReceiveAttack(otherUnit));
	}

	IEnumerator AnimateReceiveAttack(Unit otherUnit) {
		yield return new WaitForSeconds(receiveAttackDelay);
		currentHealth -= otherUnit.damage;
		Debug.Log(gameObject.name + " hit for " + otherUnit.damage + " damage, current health: " + currentHealth);
		if (currentHealth <= 0) {
			Debug.Log("Unit dies");
			isDead = true;
			currentHealth = 0;
			healthSlider.value = 0;
			gm.RemoveUnit(GetUnitNumber());
		}
		else {
			healthSlider.value = currentHealth;
		}
		animator.SetTrigger("receiveAttack");
	}

	private int GetUnitNumber() {
		for (int i = 0; i < gm.Units.Length; i++) {
			if (gm.Units[i].GetInstanceID() == gameObject.GetInstanceID()) {
				Debug.Log("Found unit to be removed: Unit at index " + i + " (" + gameObject.name + ")");
				return i;
			}
		}
		return -1;
	}
}

