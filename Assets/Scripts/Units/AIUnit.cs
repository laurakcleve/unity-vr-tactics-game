using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnit : Unit {

	protected override void Awake() {
        base.Awake();
    }

    public override void TakeTurn() {
        Debug.Log("AI Unit taking turn");
        base.TakeTurn();
        base.ActivateMove();

        // ShowValidTiles(validMoves);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("PlayerUnit");
        Tile currentTileScript = CurrentTile.GetComponent<Tile>();
        Pathfinding pathfinder = CurrentTile.GetComponent<Pathfinding>();
        List<Tile> path = new List<Tile>();
        int shortestDistance = -1;
        int closestEnemy = -1;

        for (int i = 0; i < enemies.Length; i++) {
            List<Tile> tempPath = pathfinder.FindPath(
                currentTileScript,
                enemies[i].GetComponent<PlayerUnit>().CurrentTile.GetComponent<Tile>());

            if (shortestDistance < 0 || shortestDistance > tempPath.Count) {
                shortestDistance = tempPath.Count;
                closestEnemy = i;
                path = tempPath;
            }
        }

        GameObject destinationTile = null;
        List<Tile> movePath = new List<Tile>();

        for (int i = path.Count-1; i >= 0; i--) {
            if (validMoves.Contains(path[i].gameObject)) {
                destinationTile = path[i].gameObject;
                movePath = path.GetRange(0, i+1);
                break;
            }
        }

        MoveToTile(destinationTile, movePath);

        
    }

    protected override IEnumerator AnimateMove(List<Tile> path) {
        animator.SetBool("moving", true);

        foreach (Tile tile in path)
        {

            // face next tile

            Tile thisTile = currentTile.GetComponent<Tile>();
            int rotation = 0;

            if (tile.Z > thisTile.Z)
            {
                rotation = 0;
            }
            else if (tile.Z < thisTile.Z)
            {
                rotation = 180;
            }
            else if (tile.X > thisTile.X)
            {
                rotation = 90;
            }
            else
            {
                rotation = 270;
            }

            gameObject.transform.eulerAngles = new Vector3(0, rotation, 0);

            if (tile.Height > thisTile.Height + 0.5f || tile.Height < thisTile.Height - 0.5f)
            {
                animator.SetTrigger("jump");
            }


            float time = moveTime;
            float elapsedTime = 0;
            Vector3 startingPos = transform.position;
            while (elapsedTime < time)
            {
                transform.position = Vector3.Lerp(
                    startingPos,
                    tile.gameObject.transform.position,
                    (elapsedTime / time)
                );
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            currentTile = tile.gameObject;

        }
        animator.SetBool("moving", false);
        // HideValidTiles(validMoves);
        gm.EndTurn();
    }

}
