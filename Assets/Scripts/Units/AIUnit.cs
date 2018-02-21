using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnit : Unit {

    public override void TakeTurn() {
        base.TakeTurn();

        StartCoroutine(TakeTurnAI());

        // StartCoroutine(DelayedMove());

        // // If enemy is within attack range, attack
        // StartCoroutine(CheckEnemiesAndAttack());
        // // Else move to closest enemy
        // StartCoroutine(Move());
        // // If enemy is within attack range, attack
        // StartCoroutine(CheckEnemiesAndAttack());

        // StartCoroutine(WaitAndEndTurn());
    }

    private IEnumerator TakeTurnAI() {

        float waitTime;
        if (attackAnimationLength > receiveAttackAnimationLength + receiveAttackDelay)
            waitTime = attackAnimationLength;
        else
            waitTime = receiveAttackAnimationLength + receiveAttackDelay;

        base.ActivateAttack();
        foreach (GameObject tile in validAttacks) {
            GameObject otherUnit = tile.GetComponent<Tile>().CurrentUnit;
            if (otherUnit != null && otherUnit.tag == "PlayerUnit") {
                Attack(otherUnit);
                yield return new WaitForSeconds(waitTime);
                break;
            }
        }
        attackActive = false;

        base.ActivateMove();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("PlayerUnit");
        Tile currentTileScript = CurrentTile.GetComponent<Tile>();
        Pathfinding pathfinder = CurrentTile.GetComponent<Pathfinding>();
        List<Tile> path = new List<Tile>();
        int shortestDistance = -1;

        for (int i = 0; i < enemies.Length; i++) {
            List<Tile> tempPath = pathfinder.FindPath(
                currentTileScript,
                enemies[i].GetComponent<PlayerUnit>().CurrentTile.GetComponent<Tile>());

            if (shortestDistance < 0 || shortestDistance > tempPath.Count) {
                shortestDistance = tempPath.Count;
                targetUnit = gm.Units[i];
                path = tempPath;
            }
        }

        GameObject destinationTile = null;
        List<Tile> movePath = new List<Tile>();

        for (int i = path.Count - 1; i >= 0; i--) {
            if (validMoves.Contains(path[i].gameObject)) {
                destinationTile = path[i].gameObject;
                movePath = path.GetRange(0, i + 1);
                break;
            }
            attackActive = false;
        }
        MoveToTile(destinationTile, movePath);

        yield return new WaitUntil(() => moveComplete);

        if (!hasActed) {
            base.ActivateAttack();
            foreach (GameObject tile in validAttacks) {
                GameObject otherUnit = tile.GetComponent<Tile>().CurrentUnit;
                if (otherUnit != null && otherUnit.tag == "PlayerUnit") {
                    Attack(otherUnit);
                    yield return new WaitForSeconds(waitTime);
                    break;
                }
            }
        }
        attackActive = false;

        gm.EndTurn();
    }





    // private IEnumerator CheckEnemiesAndAttack() {
    //     yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
    //     if (!hasActed) {
    //         base.ActivateAttack();
    //         foreach (GameObject tile in validAttacks) {
    //             GameObject otherUnit = tile.GetComponent<Tile>().CurrentUnit;
    //             if (otherUnit != null && otherUnit.tag == "PlayerUnit") {
    //                 Attack(otherUnit);
    //                 break;
    //             } 
    //         }
    //     }
    // }

    // private IEnumerator Move() {
    //     yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
    //     if (!hasMoved) {
    //         base.ActivateMove();
    //         GameObject[] enemies = GameObject.FindGameObjectsWithTag("PlayerUnit");
    //         Tile currentTileScript = CurrentTile.GetComponent<Tile>();
    //         Pathfinding pathfinder = CurrentTile.GetComponent<Pathfinding>();
    //         List<Tile> path = new List<Tile>();
    //         int shortestDistance = -1;

    //         for (int i = 0; i < enemies.Length; i++) {
    //             List<Tile> tempPath = pathfinder.FindPath(
    //                 currentTileScript,
    //                 enemies[i].GetComponent<PlayerUnit>().CurrentTile.GetComponent<Tile>());

    //             if (shortestDistance < 0 || shortestDistance > tempPath.Count) {
    //                 shortestDistance = tempPath.Count;
    //                 targetUnit = gm.Units[i];
    //                 path = tempPath;
    //             }
    //         }

    //         GameObject destinationTile = null;
    //         List<Tile> movePath = new List<Tile>();

    //         for (int i = path.Count - 1; i >= 0; i--) {
    //             if (validMoves.Contains(path[i].gameObject)) {
    //                 destinationTile = path[i].gameObject;
    //                 movePath = path.GetRange(0, i + 1);
    //                 break;
    //             }
    //         }

    //         MoveToTile(destinationTile, movePath);
    //     }
    // }

    // private IEnumerator WaitAndEndTurn() {
    //     yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
        
    //     gm.EndTurn();
    // }




    // protected override IEnumerator AnimateMove(List<Tile> path) {
    //     animator.SetBool("moving", true);

    //     ongoingActions++;

    //     foreach (Tile tile in path) {
    //         Tile thisTile = currentTile.GetComponent<Tile>();
    //         int rotation = 0;

    //         if (tile.Z > thisTile.Z) {
    //             rotation = 0; }
    //         else if (tile.Z < thisTile.Z) {
    //             rotation = 180; }
    //         else if (tile.X > thisTile.X) {
    //             rotation = 90; }
    //         else {
    //             rotation = 270; }

    //         gameObject.transform.eulerAngles = new Vector3(0, rotation, 0);

    //         // if (tile.Height > thisTile.Height + 0.5f || tile.Height < thisTile.Height - 0.5f) {
    //         //     animator.SetTrigger("jump");
    //         // }

    //         float time = moveTime;
    //         float elapsedTime = 0;
    //         Vector3 startingPos = transform.position;
    //         while (elapsedTime < time)
    //         {
    //             transform.position = Vector3.Lerp(
    //                 startingPos,
    //                 tile.gameObject.transform.position,
    //                 (elapsedTime / time)
    //             );
    //             elapsedTime += Time.deltaTime;
    //             yield return null;
    //         }
    //         currentTile = tile.gameObject;

    //     }
    //     animator.SetBool("moving", false);
    //     // HideValidTiles(validMoves);
    //     StartCoroutine(DelayedAttack());
    // }

    // private IEnumerator DelayedMove() {
    //     ongoingActions++;
    //     yield return new WaitForSeconds(1);

    //     base.ActivateMove();

    //     // ShowValidTiles(validMoves);

    //     GameObject[] enemies = GameObject.FindGameObjectsWithTag("PlayerUnit");
    //     Tile currentTileScript = CurrentTile.GetComponent<Tile>();
    //     Pathfinding pathfinder = CurrentTile.GetComponent<Pathfinding>();
    //     List<Tile> path = new List<Tile>();
    //     int shortestDistance = -1;

    //     for (int i = 0; i < enemies.Length; i++) {
    //         List<Tile> tempPath = pathfinder.FindPath(
    //             currentTileScript,
    //             enemies[i].GetComponent<PlayerUnit>().CurrentTile.GetComponent<Tile>());

    //         if (shortestDistance < 0 || shortestDistance > tempPath.Count) {
    //             shortestDistance = tempPath.Count;
    //             targetUnit = gm.Units[i];
    //             path = tempPath;
    //         }
    //     }

    //     GameObject destinationTile = null;
    //     List<Tile> movePath = new List<Tile>();

    //     for (int i = path.Count - 1; i >= 0; i--) {
    //         if (validMoves.Contains(path[i].gameObject)) {
    //             destinationTile = path[i].gameObject;
    //             movePath = path.GetRange(0, i + 1);
    //             break;
    //         }
    //     }

    //     MoveToTile(destinationTile, movePath);

    //     StartCoroutine(DelayedAttack());
    // }

    // private IEnumerator DelayedAttack() {
    //     ongoingActions++;
    //     yield return new WaitForSeconds(1);
    //     base.ActivateAttack();

    //     if (validAttacks.Contains(targetUnit.GetComponent<Unit>().CurrentTile)) {
    //         base.Attack(targetUnit);
    //     }
    //     StartCoroutine(DelayedEndTurn());

    // }

    // private IEnumerator DelayedEndTurn() {
    //     ongoingActions++;
    //     yield return new WaitForSeconds(1);
    //     gm.EndTurn();
    // }

}
