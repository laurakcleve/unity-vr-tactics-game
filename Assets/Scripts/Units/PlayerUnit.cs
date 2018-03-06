using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class PlayerUnit : Unit {
    
    protected override void Awake() {
        base.Awake();

        // Grab mode not updated
        VRTK_InteractableObject interactable = GetComponent<VRTK_InteractableObject>();
        interactable.InteractableObjectGrabbed += new InteractableObjectEventHandler(ShowValidTilesOnGrab);
        interactable.InteractableObjectUngrabbed += new InteractableObjectEventHandler(HideValidTilesOnUngrab);
    }

    public override void TakeTurn() {
        base.TakeTurn();
        gm.actionCanvas.SetActive(true);
        gm.moveButton.onClick.AddListener(ActivateMove);
        gm.attackButton.onClick.AddListener(ActivateAttack);
        gm.moveButton.interactable = true;
        gm.attackButton.interactable = true;
    }

    public override void ActivateMove() {
        if (!hasMoved) {
            base.ActivateMove();
            ShowValidTiles(validMoves);
            ActivateMoveTiles();
            gm.actionCanvas.SetActive(false);
            gm.cancelCanvas.SetActive(true);
            gm.confirmButton.onClick.AddListener(ConfirmMove);
            gm.cancelButton.onClick.AddListener(CancelMove);

            // Grab mode not updated
            if (GameManager.instance.movementType == GameManager.movementTypes.grab) {
                GetComponent<VRTK_InteractableObject>().isGrabbable = true;
            }
        }
    }

    public override void MoveToTile(GameObject destinationTile, List<Tile> path) {
        if (!hasMoved) {
            HideValidTiles(validMoves);
            gm.confirmCanvas.SetActive(false);
            gm.cancelCanvas.SetActive(false);
            base.MoveToTile(destinationTile, path);
        }
    }

    protected override void FinishMove() {
        gm.confirmCanvas.SetActive(true);
        gm.cancelCanvas.SetActive(true);
    }

    private void ConfirmMove() {
        gm.confirmCanvas.SetActive(false);
        gm.cancelCanvas.SetActive(false);
        gm.actionCanvas.SetActive(true);
        DeactivateMoveTiles();
        gm.moveButton.interactable = false;
        hasMoved = true;
        currentRotation = tempCurrentRotation;
        currentTile.GetComponent<Tile>().CurrentUnit = null;
        currentTile = tempCurrentTile;
        currentTile.GetComponent<Tile>().CurrentUnit = gameObject;
        gm.confirmButton.onClick.RemoveAllListeners();
        gm.cancelButton.onClick.RemoveAllListeners();

        // Grab mode not updated
        GetComponent<VRTK_InteractableObject>().isGrabbable = false;
    }

    private void CancelMove() {
        transform.position = currentTile.transform.position;
        transform.eulerAngles = new Vector3(0, currentRotation, 0);
        gm.confirmCanvas.SetActive(false);
        gm.cancelCanvas.SetActive(false);
        gm.actionCanvas.SetActive(true);
        HideValidTiles(validMoves);
        DeactivateMoveTiles();
    }

    public override void ActivateAttack() {
        if (!hasActed) {
            base.ActivateAttack();
            
            ShowValidTiles(validAttacks);
            gm.actionCanvas.SetActive(false);
            gm.cancelCanvas.SetActive(true);
            gm.cancelButton.onClick.AddListener(CancelAttack);
            gm.confirmButton.onClick.AddListener(ConfirmAttack);
        }
    }

    private void CancelAttack() {
        gm.cancelCanvas.SetActive(false);
        gm.confirmCanvas.SetActive(false);
        gm.actionCanvas.SetActive(true);
        HideValidTiles(validAttacks);
    }

    public override void AskConfirmAttack(GameObject otherUnit) {
        gm.confirmCanvas.SetActive(true);
        targetUnit = otherUnit;
    }

    private void ConfirmAttack() {
        gm.confirmCanvas.SetActive(false);
        gm.cancelCanvas.SetActive(false);
        gm.confirmButton.onClick.RemoveAllListeners();
        gm.cancelButton.onClick.RemoveAllListeners();
        Attack(targetUnit);
        StartCoroutine(AnimateAttack());
    }

    private IEnumerator AnimateAttack() {
        yield return new WaitForSeconds(attackWaitTime);
        gm.actionCanvas.SetActive(true);
    }

    private void ActivateMoveTiles() {
        foreach (GameObject tile in validMoves) {
            tile.GetComponent<VRTK_InteractableObject>().isUsable = true;
        }
    }

    private void DeactivateMoveTiles() {
        foreach (GameObject tile in validMoves) {
            tile.GetComponent<VRTK_InteractableObject>().isUsable = false;
        }
    }

    private void ShowValidTilesOnGrab(object sender, InteractableObjectEventArgs e) {
        ShowValidTiles(validMoves);
    }

    private void HideValidTilesOnUngrab(object sender, InteractableObjectEventArgs e) {
        HideValidTiles(validMoves);
    }

    public override void Attack(GameObject otherUnit) {
        base.Attack(otherUnit);
        HideValidTiles(validAttacks);
        gm.attackButton.interactable = false;
    }

}
