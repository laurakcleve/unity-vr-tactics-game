using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	
	public int totalRounds;

	[Header("Stage")]
	public Transform stageCenter;
	[Tooltip("Number of tiles on one side")]
	[Range(1,20)]
	public int stageSize;
	
	[Header("Tile")]
	public GameObject tilePrefab;
	[Tooltip("Tile instantiation height before raycasting down onto the stage")]
	[Range(0,10)]
	public float tileStartHeight;
	public float tileSize;
	
	[Header("Units")]
	public GameObject[] unitPrefabs;
	
	[Header("Unit buttons")]
	public GameObject unitButtonPrefab;
	public float unitButtonMargin;


	private GameObject[,] tiles;
	private GameObject[] unitInstances;
	private GameObject[] unitButtons;
	private GameObject unitListCanvas;
	private int round;
	private int activeUnit;


	/* AWAKE
	-------------------------------------------------------- */
	void Awake () {
		if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this);
        }

		unitListCanvas = GameObject.Find("UnitListCanvas");

		tiles = CreateTiles();
		PlaceUnits();

		activeUnit = 0;
        round = 1;
        unitInstances[activeUnit].GetComponent<Unit>().TakeTurn();
	}


    /* CREATE TILES
	-------------------------------------------------------- */
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

    /* PLACE UNITS
	-------------------------------------------------------- */
    void PlaceUnits() {

		unitInstances = new GameObject[unitPrefabs.Length];
		unitButtons = new GameObject[unitPrefabs.Length];

		for (int i = 0; i < unitPrefabs.Length; i++) {

			// Instantiate units
            
			Unit unitPrefabScript = unitPrefabs[i].GetComponent<Unit>();
			int x = unitPrefabScript.startingX;
            int z = unitPrefabScript.startingZ;
            
			GameObject newUnit = Instantiate(
				unitPrefabs[i], 
				tiles[x,z].transform.position, 
				Quaternion.identity
			) as GameObject;
			
			newUnit.transform.eulerAngles = new Vector3(
				0, 
				newUnit.GetComponent<Unit>().rotation, 
				0
			);

			newUnit.name = "Unit " + (i+1);

			unitInstances[i] = newUnit;


			//Instantiate unit buttons

            GameObject newUnitButton = Instantiate(unitButtonPrefab) as GameObject;
            newUnitButton.transform.SetParent(unitListCanvas.transform, false);

			RectTransform buttonRect = newUnitButton.GetComponent<RectTransform>();
			float buttonHeight = buttonRect.rect.height;
			float posX = buttonRect.anchoredPosition.x;
			float posY = buttonRect.anchoredPosition.y - (i * (buttonHeight + unitButtonMargin));
			buttonRect.anchoredPosition = new Vector2(posX, posY);


			// Set button text

			newUnitButton.transform.GetComponentInChildren<Text>().text = newUnit.name;
			newUnitButton.name = newUnit.name + " button";


			unitButtons[i] = newUnitButton;

			unitInstances[i].GetComponent<Unit>().SetButton(unitButtons[i]);

        }

	}


    /* END TURN
	-------------------------------------------------------- */
    public void EndTurn()
    {
		Debug.Log("Round: " + round);
        if (round < totalRounds) {
            if (activeUnit >= unitPrefabs.Length - 1) {
                activeUnit = 0;
				round++;
            }
            else {
                activeUnit++;
            }

			//Remove previous unit's highlight

			GameObject previousUnit;
			if (activeUnit == 0) {
				previousUnit = unitInstances[unitInstances.Length-1];
			}
			else {
				previousUnit = unitInstances[activeUnit-1];
			}
			previousUnit.GetComponent<Unit>().SetButtonColor(Color.cyan);

            Debug.Log("New active unit: " + activeUnit);
			Debug.Log("Round " + round);
			
            unitInstances[activeUnit].GetComponent<Unit>().TakeTurn();
        }
    }

}
