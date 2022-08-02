using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfirmMove : State {

    public override void Enter(PlayerUnit unit) {
        GameManager.instance.confirmCanvas.SetActive(true);
        GameManager.instance.cancelCanvas.SetActive(true);
    }

    public override void Exit(PlayerUnit unit) {
        GameManager.instance.confirmCanvas.SetActive(false);
        GameManager.instance.cancelCanvas.SetActive(false);
    }

}
