using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectAction : State {

    public override void Enter(PlayerUnit unit) {
        GameManager.instance.actionCanvas.SetActive(true);
    }

    public override void Exit(PlayerUnit unit) {
        GameManager.instance.actionCanvas.SetActive(false);
    }

}
