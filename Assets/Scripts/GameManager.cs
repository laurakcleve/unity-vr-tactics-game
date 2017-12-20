using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject tilePrefab;
	public Transform stageCenter;
	public float tileStartHeight;
	public int stageSize;
	public float tileSize;
	public GameObject[] units;
	public int totalRounds;

	public static GameManager instance = null;
	private GameObject[,] tiles;
	private int activeUnit;
	private int round;

	void Awake () {
		if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this);
        }

		Debug.Log("Game manager instance: " + instance);
		tiles = CreateTiles();
		PlaceUnits();
		round = 0;	
	}
	
	GameObject[,] CreateTiles() {

		GameObject[,] tempTiles = new GameObject[stageSize, stageSize];

		for (int x = 0; x < stageSize; x++) {
			for (int z = 0; z < stageSize; z++) { 
				float tileX = -((stageSize/2 - z) * tileSize) + tileSize / 2;
				float tileY = -((stageSize/2 - x) * tileSize) + tileSize / 2;

				float height = 0;

				Vector3 tilePos = new Vector3(tileX, tileStartHeight, tileY);

				GameObject tileInstance = Instantiate(
					tilePrefab, 
					tilePos, 
					Quaternion.identity
				) as GameObject;
		
				RaycastHit hit;
				if (Physics.Raycast(
					tileInstance.transform.position, 
					-tileInstance.transform.up, 
					out hit, 
					Mathf.Infinity
				)) {
					height = hit.point.y;
					tileInstance.transform.position = new Vector3(
						tileInstance.transform.position.x,
						height + 0.001f,
						tileInstance.transform.position.z
					);
				}

				Tile tileScript = tileInstance.GetComponent<Tile>();
				tileScript.setX(x);
				tileScript.setZ(z);
				tileScript.setHeight(height);

				tileInstance.name = "Tile " + x + "," + z;

				tempTiles[x,z] = tileInstance;
			}
		}

		return tempTiles;

	}

	void PlaceUnits() {
		for (int i = 0; i < units.Length; i++) {
            int x = units[i].GetComponent<Unit>().startingX;
            int z = units[i].GetComponent<Unit>().startingZ;
			GameObject tile = tiles[x,z];
            GameObject newUnit = Instantiate(units[i], tile.transform.position, Quaternion.identity) as GameObject;
			newUnit.transform.eulerAngles = new Vector3(0, newUnit.GetComponent<Unit>().rotation, 0);
        }

		activeUnit = 0;
		Debug.Log("Round 1");
		units[activeUnit].GetComponent<Unit>().TakeTurn();
	}

	public void PassTurn()
    {
        while (round < totalRounds) {
            if (activeUnit >= units.Length - 1) {
                activeUnit = 0;
            }
            else {
                activeUnit++;
            }
            round++;
			Debug.Log("Round " + round);
            units[activeUnit].GetComponent<Unit>().TakeTurn();
        }
    }
}
