using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectAttack : State {

    public override void Enter(PlayerUnit unit) {
        GameManager.instance.cancelCanvas.SetActive(true);
        // unit.ShowAttackTiles();
    }

    public override void Exit(PlayerUnit unit) {
        GameManager.instance.cancelCanvas.SetActive(false);
        // unit.HideAttackTiles();
    }

}
