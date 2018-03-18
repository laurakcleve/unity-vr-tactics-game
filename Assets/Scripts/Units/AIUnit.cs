using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnit : Unit {

    public override void TakeTurn() {
        base.TakeTurn();
        StartCoroutine(TakeTurnAI());
    }

    private IEnumerator TakeTurnAI() {
        Debug.Log(gameObject.name + " AI checking attack #1");
        base.ActivateAttack();
        GameObject enemy = EnemyInRange();
        if (enemy != null) {
            Attack(enemy);
            yield return new WaitForSeconds(attackWaitTime);
        }
        else {
            Debug.Log(gameObject.name + " AI no enemies in range, moving");
            base.ActivateMove();
            MoveToNearestEnemy();
            yield return new WaitUntil(() => moveAnimationComplete);
        }

        if (!hasActed) {
            Debug.Log(gameObject.name + " AI checking attack #2");
            base.ActivateAttack();
            enemy = EnemyInRange();
            if (enemy != null) {
                Attack(enemy);
                yield return new WaitForSeconds(attackWaitTime);
            }
            else {
                Debug.Log(gameObject.name + " AI no enemies in range, ending turn");
            }
        }

        gm.EndTurn();
    }

    private GameObject EnemyInRange() {
        foreach (GameObject tile in validAttacks) {
            GameObject otherUnit = tile.GetComponent<Tile>().CurrentUnit;
            if (otherUnit != null && otherUnit.tag == "PlayerUnit" && !otherUnit.GetComponent<Unit>().isDead) {
                return otherUnit;
            }
        }
        return null;
    }

    private void MoveToNearestEnemy() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("PlayerUnit");
        Tile currentTileScript = currentTile.GetComponent<Tile>();
        Pathfinding pathfinder = currentTile.GetComponent<Pathfinding>();
        List<Tile> path = new List<Tile>();
        
        int shortestDistance = -1;
        
        for (int i = 0; i < enemies.Length; i++) {
            List<Tile> tempPath = pathfinder.FindPath(
                currentTileScript,
                enemies[i].GetComponent<PlayerUnit>().currentTile.GetComponent<Tile>());

            if ((shortestDistance < 0 || shortestDistance > tempPath.Count) && !enemies[i].GetComponent<Unit>().isDead) {
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
        }

        MoveToTile(destinationTile, movePath);
    }

    public override void MoveToTile(GameObject destinationTile, List<Tile> path) {
        base.MoveToTile(destinationTile, path);
        currentTile.GetComponent<Tile>().CurrentUnit = null;
        currentTile = destinationTile;
        currentTile.GetComponent<Tile>().CurrentUnit = gameObject;
        currentRotation = tempCurrentRotation;
        hasMoved = true;
    }
    
}
