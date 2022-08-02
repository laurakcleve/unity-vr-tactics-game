using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class PlayerUnit : Unit {

    /*
    
        playerselectaction (menus)
        playerselectdestination
        playerselecttarget
        animate
            move
            action
        (waiting)
    
     */




    private State currentState;
    public PlayerInactive playerInactive;
    public PlayerSelectAction playerSelectAction;
    // public PlayerSelectMove playerSelectMove;
    // public PlayerAnimateMove playerAnimateMove;
    // public PlayerConfirmMove playerConfirmMove;
    // public PlayerSelectAttack playerSelectAttack;
    // public PlayerConfirmAttack playerConfirmAttack;
    // public PlayerAnimateAttack playerAnimateAttack;
    


    protected override void Awake() {
        base.Awake();

        playerInactive = new PlayerInactive();
        playerSelectAction = new PlayerSelectAction();
        // playerSelectMove = new PlayerSelectMove();
        // playerAnimateMove = new PlayerAnimateMove();
        // playerConfirmMove = new PlayerConfirmMove();
        // playerSelectAttack = new PlayerSelectAttack();
        // playerConfirmAttack = new PlayerConfirmAttack();
        // playerAnimateAttack = new PlayerAnimateAttack();

        currentState = playerInactive;

        // Grab mode not updated
        // VRTK_InteractableObject interactable = GetComponent<VRTK_InteractableObject>();
        // interactable.InteractableObjectGrabbed += new InteractableObjectEventHandler(ShowValidTilesOnGrab);
        // interactable.InteractableObjectUngrabbed += new InteractableObjectEventHandler(HideValidTilesOnUngrab);
    }

    public void ChangeState(State newState) {
        if (currentState != null) {
            currentState.Exit(this);
        }
        currentState = newState;
        currentState.Enter(this);
    }
    
    public override void TakeTurn() {
        ChangeState(playerSelectAction);
    }

    // public void SelectMove() {
    //     ChangeState(playerSelectMove);        
    // }

    // public void ShowMoveTiles() {
    //     validMoves = new List<GameObject>();
    //     validMoves = GetValidMoves(currentTile);
    //     ShowTiles(validMoves);
    //     ActivateMoveTiles();
    // }

    // public void HideMoveTiles() {
    //     HideTiles(validMoves);
    //     DeactivateMoveTiles();
    // }

    // public override IEnumerator AnimateMove() {
    //     base.AnimateMove();
    //     ChangeState(playerConfirmMove);
    //     yield break;
    // }

    // public void SelectAttack() {
    //     ChangeState(playerSelectAttack);
    // }

    // public void ShowAttackTiles() {
    //     validAttacks = new List<GameObject>();
    //     validAttacks = GetValidAttacks(currentTile);
    //     ShowTiles(validAttacks);
    //     foreach (GameObject tile in validAttacks) {
    //         GameObject unit = tile.GetComponent<Tile>().CurrentUnit;
    //         if (unit != null && unit.CompareTag("AIUnit")) {
    //             unit.GetComponent<VRTK_InteractableObject>().isUsable = true;
    //         }
    //     }
    // }

    // public void HideAttackTiles() {
    //     HideTiles(validAttacks);
    //     foreach (GameObject tile in validAttacks) {
    //         GameObject unit = tile.GetComponent<Tile>().CurrentUnit;
    //         if (unit != null) {
    //             unit.GetComponent<VRTK_InteractableObject>().isUsable = false;
    //         }
    //     }
    // }

    // public void Attack() {
    //     Tile thisTile = currentTile.GetComponent<Tile>();
    //     Tile otherTile = targetUnit.GetComponent<Unit>().currentTile.GetComponent<Tile>();

    //     int rotation = 0;

    //     if (thisTile.Z > otherTile.Z)       rotation = 180;
    //     else if (thisTile.Z < otherTile.Z)  rotation = 0;
    //     else if (thisTile.X > otherTile.X)  rotation = 270;
    //     else                                rotation = 90;

    //     gameObject.transform.eulerAngles = new Vector3(0, rotation, 0);

    //     animator.SetTrigger("attack");
    //     targetUnit.GetComponent<Unit>().ReceiveAttack(this);
    // }

    // public void Confirm() {
    //     if (currentState == playerConfirmMove) {
    //         currentRotation = tempCurrentRotation;
    //         currentTile = tempCurrentTile;
    //         GameManager.instance.moveButton.interactable = false;
    //         ChangeState(playerSelectAction);
    //     }
    //     else if (currentState == playerConfirmAttack) {
    //         ChangeState(playerAnimateAttack);
    //     }
    // }

    // public void Cancel() {
    //     ChangeState(playerSelectAction);
    // }

    
    // public override void ActivateMove() {
    //     if (!hasMoved) {
    //         base.ActivateMove();
    //         ShowValidTiles(validMoves);
    //         ActivateMoveTiles();
    //         gm.actionCanvas.SetActive(false);
    //         gm.cancelCanvas.SetActive(true);
    //         gm.confirmButton.onClick.AddListener(ConfirmMove);
    //         gm.cancelButton.onClick.AddListener(CancelMove);

    //         // Grab mode not updated
    //         if (GameManager.instance.movementType == GameManager.movementTypes.grab) {
    //             GetComponent<VRTK_InteractableObject>().isGrabbable = true;
    //         }
    //     }
    // }

    // public override void MoveToTile(GameObject destinationTile, List<Tile> path) {
    //     if (!hasMoved) {
    //         HideValidTiles(validMoves);
    //         gm.confirmCanvas.SetActive(false);
    //         gm.cancelCanvas.SetActive(false);
    //         base.MoveToTile(destinationTile, path);
    //     }
    // }

    // protected override void FinishMove() {
    //     gm.confirmCanvas.SetActive(true);
    //     gm.cancelCanvas.SetActive(true);
    // }

    // private void ConfirmMove() {
    //     gm.confirmCanvas.SetActive(false);
    //     gm.cancelCanvas.SetActive(false);
    //     gm.actionCanvas.SetActive(true);
    //     DeactivateMoveTiles();
    //     gm.moveButton.interactable = false;
    //     currentRotation = tempCurrentRotation;
    //     currentTile.GetComponent<Tile>().CurrentUnit = null;
    //     currentTile = tempCurrentTile;
    //     currentTile.GetComponent<Tile>().CurrentUnit = gameObject;
    //     gm.confirmButton.onClick.RemoveAllListeners();
    //     gm.cancelButton.onClick.RemoveAllListeners();

    //     // Grab mode not updated
    //     GetComponent<VRTK_InteractableObject>().isGrabbable = false;
    // }

    // private void CancelMove() {
    //     transform.position = currentTile.transform.position;
    //     transform.eulerAngles = new Vector3(0, currentRotation, 0);
    //     gm.confirmCanvas.SetActive(false);
    //     gm.cancelCanvas.SetActive(false);
    //     gm.actionCanvas.SetActive(true);
    //     HideTiles(validMoves);
    //     DeactivateMoveTiles();
    // }

    // public override void ActivateAttack() {
    //     if (!hasActed) {
    //         base.ActivateAttack();
            
    //         ShowValidTiles(validAttacks);
    //         gm.actionCanvas.SetActive(false);
    //         gm.cancelCanvas.SetActive(true);
    //         gm.cancelButton.onClick.AddListener(CancelAttack);
    //         gm.confirmButton.onClick.AddListener(ConfirmAttack);
    //     }
    // }

    // private void CancelAttack() {
    //     gm.cancelCanvas.SetActive(false);
    //     gm.confirmCanvas.SetActive(false);
    //     gm.actionCanvas.SetActive(true);
    //     HideTiles(validAttacks);
    // }

    // public override void AskConfirmAttack(GameObject otherUnit) {
    //     gm.confirmCanvas.SetActive(true);
    //     targetUnit = otherUnit;
    // }

    // private void ConfirmAttack() {
    //     gm.confirmCanvas.SetActive(false);
    //     gm.cancelCanvas.SetActive(false);
    //     gm.confirmButton.onClick.RemoveAllListeners();
    //     gm.cancelButton.onClick.RemoveAllListeners();
    //     Attack(targetUnit);
    //     StartCoroutine(AnimateAttack());
    // }

    // private IEnumerator AnimateAttack() {
    //     yield return new WaitForSeconds(attackWaitTime);
    //     gm.actionCanvas.SetActive(true);
    // }

    // private void ActivateMoveTiles() {
    //     foreach (GameObject tile in validMoves) {
    //         tile.GetComponent<VRTK_InteractableObject>().isUsable = true;
    //     }
    // }

    // public void DeactivateMoveTiles() {
    //     foreach (GameObject tile in validMoves) {
    //         tile.GetComponent<VRTK_InteractableObject>().isUsable = false;
    //     }
    // }

    // private void ShowValidTilesOnGrab(object sender, InteractableObjectEventArgs e) {
    //     ShowTiles(validMoves);
    // }

    // private void HideValidTilesOnUngrab(object sender, InteractableObjectEventArgs e) {
    //     HideTiles(validMoves);
    // }

    // public override void Attack(GameObject otherUnit) {
    //     base.Attack(otherUnit);
    //     HideTiles(validAttacks);
    //     gm.attackButton.interactable = false;
    // }

}
