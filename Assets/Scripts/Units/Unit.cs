using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public abstract class Unit : MonoBehaviour {

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
    public List<GameObject> validMoves;
    public GameObject targetUnit;

    protected GameObject tempCurrentTile;
    protected List<GameObject> validAttacks;
    protected Animator animator;
    protected Button moveButton;
    protected Button attackButton;
    protected GameManager gm;
    protected bool moveAnimationComplete;
    protected float attackAnimationLength;
    protected float receiveAttackAnimationLength;
    protected int currentRotation;
    protected int tempCurrentRotation;
    protected float attackWaitTime;
    protected Slider healthSlider;
    protected GameObject targetTile;

    public GameObject TargetTile { get; set; }



    protected virtual void Awake() {
        healthSlider = transform.Find("Canvas/Slider").gameObject.GetComponent<Slider>();
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

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
    }

	public abstract void TakeTurn();

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

    // protected List<GameObject> GetValidAttacks(GameObject startTile) {
    //     validAttacks.Add(startTile);
    //     List<GameObject> connectedTiles = startTile.GetComponent<Tile>().connected;
    //     foreach (GameObject tile in connectedTiles) {
    //         Tile currentTileScript = currentTile.GetComponent<Tile>();
    //         Tile targetTileScript = tile.GetComponent<Tile>();
    //         if ((Mathf.Abs(targetTileScript.X - currentTileScript.X) + Mathf.Abs(targetTileScript.Z - currentTileScript.Z) <= attackRange) && !validAttacks.Contains(tile)) {
    //             GetValidAttacks(tile);
    //         }
    //     }
    //     return validAttacks;
    // }

    public void ShowTiles(List<GameObject> tiles) {
        foreach (GameObject tile in tiles) {
            tile.GetComponent<Renderer>().enabled = true;
        }
    }

    public void HideTiles(List<GameObject> tiles) {
        foreach (GameObject tile in tiles) {
            tile.GetComponent<Renderer>().enabled = false;
        }
    }

    // public virtual void MoveToTile() {
    //     StartCoroutine(AnimateMove());
    // }

    // public virtual IEnumerator AnimateMove() {
    //     Tile thisTile = currentTile.GetComponent<Tile>();
    //     List<Tile> path = TargetTile.GetComponent<Pathfinding>().FindPath(thisTile, TargetTile.GetComponent<Tile>());
    //     animator.SetBool("moving", true);
    //     // moveAnimationComplete = false;

    //     int rotation = 0;

    //     foreach (Tile nextTile in path) {

    //         if (nextTile.Z > thisTile.Z) rotation = 0;
    //         else if (nextTile.Z < thisTile.Z) rotation = 180;
    //         else if (nextTile.X > thisTile.X) rotation = 90;
    //         else rotation = 270;

    //         gameObject.transform.eulerAngles = new Vector3(0, rotation, 0);

    //         float time = moveTime;
    //         float elapsedTime = 0;
    //         Vector3 startingPos = transform.position;
    //         while (elapsedTime < time) {
    //             transform.position = Vector3.Lerp(
    //                 startingPos,
    //                 nextTile.gameObject.transform.position,
    //                 (elapsedTime / time)
    //             );
    //             elapsedTime += Time.deltaTime;
    //             yield return null;
    //         }
    //         thisTile = nextTile;
    //     }
    //     tempCurrentRotation = rotation;
    //     tempCurrentTile = thisTile.gameObject;
    //     animator.SetBool("moving", false);


    //     // moveAnimationComplete = true;
    //     // FinishMove();
    // }

    // public virtual void Attack() {
    //     Tile thisTile = currentTile.GetComponent<Tile>();
    //     Tile otherTile = targetUnit.GetComponent<Unit>().currentTile.GetComponent<Tile>();

    //     if (validAttacks.Contains(otherTile.gameObject)) {
    //         int rotation = 0;

    //         if (thisTile.Z > otherTile.Z) rotation = 180;
    //         else if (thisTile.Z < otherTile.Z) rotation = 0;
    //         else if (thisTile.X > otherTile.X) rotation = 270;
    //         else rotation = 90;

    //         gameObject.transform.eulerAngles = new Vector3(0, rotation, 0);

    //         animator.SetTrigger("attack");
    //         targetUnit.GetComponent<Unit>().ReceiveAttack(this);
    //         // StartCoroutine(AnimateAttack(otherUnit));
    //     }
    // }

    // // protected IEnumerator AnimateAttack(GameObject otherUnit) {
    // // 	yield return null;
    // // }

    // public void ReceiveAttack(Unit otherUnit) {
    //     StartCoroutine(AnimateReceiveAttack(otherUnit));
    // }

    // IEnumerator AnimateReceiveAttack(Unit otherUnit) {
    //     currentHealth -= otherUnit.damage;
    //     Debug.Log(gameObject.name + " hit for " + otherUnit.damage + " damage, current health: " + currentHealth);
    //     if (currentHealth <= 0) {
    //         Debug.Log("Unit dies");
    //         isDead = true;
    //         currentHealth = 0;
    //         healthSlider.value = 0;
    //         gm.RemoveUnit(GetUnitNumber());
    //     }
    //     else {
    //         healthSlider.value = currentHealth;
    //     }
        
	// 	yield return new WaitForSeconds(receiveAttackDelay);
    //     animator.SetTrigger("receiveAttack");
	// 	// EndAttack(otherUnit);
    // }

	// protected virtual void EndAttack(PlayerUnit otherUnit) {}

    // private int GetUnitNumber() {
    //     for (int i = 0; i < gm.Units.Length; i++) {
    //         if (gm.Units[i].GetInstanceID() == gameObject.GetInstanceID()) {
    //             Debug.Log("Found unit to be removed: Unit at index " + i + " (" + gameObject.name + ")");
    //             return i;
    //         }
    //     }
    //     return -1;
    // }
}

