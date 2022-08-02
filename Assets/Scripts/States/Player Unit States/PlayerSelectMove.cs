using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectMove : State {

    public override void Enter(PlayerUnit unit) {
        GameManager.instance.cancelCanvas.SetActive(true);
        // unit.ShowMoveTiles();
    }

    public override void Exit(PlayerUnit unit) {
        GameManager.instance.cancelCanvas.SetActive(false);
        // unit.HideMoveTiles();
    }

}
