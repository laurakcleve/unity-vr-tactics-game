using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject tilePrefab;
	public Transform stageCenter;
	public float tileStartHeight;
	public int stageSize;
	public float tileSize;

	
	void Start () {
		CreateTiles();	
	}
	
	void CreateTiles() {

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
					Debug.Log("Raycast hit");
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
			}
		}



	}
}
