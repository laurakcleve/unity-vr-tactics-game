using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInactive : State {

    public override void Enter(PlayerUnit unit) {

    }

    public override void Exit(PlayerUnit unit) {
        unit.gameObject.transform.Find("Highlight").gameObject.SetActive(true);
        
        GameManager gm = GameManager.instance;
        gm.moveButton.interactable = true;
        // gm.attackButton.interactable = true;
        // gm.moveButton.onClick.AddListener(unit.SelectMove);
        // gm.attackButton.onClick.AddListener(unit.SelectAttack);
        // gm.confirmButton.onClick.AddListener(unit.Confirm);
        // gm.cancelButton.onClick.AddListener(unit.Cancel);
    }

}
