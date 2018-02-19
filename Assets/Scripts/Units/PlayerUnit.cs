using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class PlayerUnit : Unit {

    protected override void Awake() {
        base.Awake();

        VRTK_InteractableObject interactable = GetComponent<VRTK_InteractableObject>();

        interactable.InteractableObjectGrabbed += new InteractableObjectEventHandler(ShowValidTilesOnGrab);
        interactable.InteractableObjectUngrabbed += new InteractableObjectEventHandler(HideValidTilesOnUngrab);
        interactable.InteractableObjectUsed += new InteractableObjectEventHandler(ReturnThisUnit);
        
        
    }

    public override void TakeTurn() {
        // gm.ActionCanvas.SetActive(true);
        // gm.ActionCanvas.GetComponent<GraphicRaycaster>().enabled = true;
        moveButton = GameObject.Find("MoveButton").GetComponent<Button>();
        attackButton = GameObject.Find("AttackButton").GetComponent<Button>();
        base.TakeTurn();


        moveButton.onClick.AddListener(ActivateMove);
        attackButton.onClick.AddListener(ActivateAttack);

        moveButton.interactable = true;
        attackButton.interactable = true;
    }

    public override void ActivateMove() {
        if (!hasMoved) {
            base.ActivateMove();

            if (GameManager.instance.MovementType == GameManager.movementTypes.grab) {
                GetComponent<VRTK_InteractableObject>().isGrabbable = true;
            }

            ShowValidTiles(validMoves);
            ActivateMoveTiles();
            moveActive = true;
        }
    }

    public override void ActivateAttack() {
        if (!hasActed) {
            base.ActivateAttack();
            
            ShowValidTiles(validAttacks);
        }
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

    public override void MoveToTile(GameObject destinationTile, List<Tile> path) {
        if (!hasMoved) {
            base.MoveToTile(destinationTile, path);

            HideValidTiles(validMoves);
            DeactivateMoveTiles();
            moveButton.interactable = false;
            GetComponent<VRTK_InteractableObject>().isGrabbable = false;
        }
    }

    protected override void Attack(GameObject otherUnit) {
        base.Attack(otherUnit);
        HideValidTiles(validAttacks);
        attackButton.interactable = false;
    }

}
