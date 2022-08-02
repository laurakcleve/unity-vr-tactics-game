using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

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
	public enum movementTypes{
		walk, grab
	}
    public movementTypes movementType;

	[Header("Other buttons")]
	public GameObject actionCanvas;
	public Button moveButton;
	public Button attackButton;
	public GameObject confirmCanvas;
	public GameObject cancelCanvas;
	public Button confirmButton;
	public Button cancelButton;


	private GameObject[,] tiles;
	private GameObject[] units;
	private int round = 1;
	private int activeUnit;

    public int ActiveUnit { get; set; }

    public GameObject[] Units { get; set; }


    /* AWAKE
	-------------------------------------------------------- */
    void Awake () {
		if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this);
        }

		tiles = CreateTiles();
		CreateUnits();

		ActiveUnit = 0;
	}

	void Start() {
        Units[ActiveUnit].GetComponent<Unit>().TakeTurn();
	}


    /* CREATE TILES
	-------------------------------------------------------- */
    GameObject[,] CreateTiles() {

		GameObject[,] tempTiles = new GameObject[stageSize, stageSize];

		for (int z = 0; z < stageSize; z++) {
			for (int x = 0; x < stageSize; x++) {

				// Instantiate tiles above the stage

				float tileX = -((stageSize/2 - x) * tileSize) + tileSize / 2;
				float tileZ = -((stageSize/2 - z) * tileSize) + tileSize / 2;

				float height = 0;

				GameObject tileInstance = Instantiate(
					tilePrefab,
					new Vector3(tileX, tileStartHeight, tileZ),
					Quaternion.identity
				) as GameObject;

				tileInstance.GetComponent<Renderer>().enabled = false;
				
				tileInstance.GetComponent<VRTK_InteractableObject>().isUsable = false;


				// Shoot the tiles down onto the stage

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


				// Set some fields

				Tile tileScript = tileInstance.GetComponent<Tile>();
				tileScript.X = x;
				tileScript.Z = z;
				tileScript.Height = height;

				tileInstance.name = "Tile " + x + "," + z;

				tempTiles[x,z] = tileInstance;
			}
		}


		// Populate connected tiles

		for (int z = 0; z < stageSize; z++) {
            for (int x = 0; x < stageSize; x++) {
				List<GameObject> connectedTiles = tempTiles[z,x].GetComponent<Tile>().connected;
                if (z > 0)
                    connectedTiles.Add(tempTiles[z - 1, x]);
                if (x < stageSize-1)
                    connectedTiles.Add(tempTiles[z, x + 1]);
                if (z < stageSize-1)
                    connectedTiles.Add(tempTiles[z + 1, x]);
                if (x > 0)
                    connectedTiles.Add(tempTiles[z, x - 1]);
            }
        }

		return tempTiles;

	}

    /* CREATE UNITS
	-------------------------------------------------------- */
    void CreateUnits() {

		Units = new GameObject[unitPrefabs.Length];

		for (int i = 0; i < unitPrefabs.Length; i++) {

			// Instantiate units

			Unit unitPrefabScript = unitPrefabs[i].GetComponent<Unit>();
			int x = unitPrefabScript.startingX;
            int z = unitPrefabScript.startingZ;

			GameObject unitInstance = Instantiate(
				unitPrefabs[i],
				tiles[x,z].transform.position,
				Quaternion.identity
			) as GameObject;

			Unit unitInstanceScript = unitInstance.GetComponent<Unit>();

			unitInstance.transform.eulerAngles = new Vector3(
				0,
				unitInstanceScript.startingRotation,
				0
			);

			unitInstance.name = "Unit " + (i+1);

			unitInstanceScript.currentTile = tiles[x,z];
			tiles[x,z].GetComponent<Tile>().CurrentUnit = unitInstance;

			Units[i] = unitInstance;

        }

	}


    /* PASS TURN
	-------------------------------------------------------- */
    public void PassTurn() {
		moveButton.onClick.RemoveAllListeners();
		attackButton.onClick.RemoveAllListeners();
		actionCanvas.SetActive(false);
		Units[ActiveUnit].transform.Find("Highlight").gameObject.SetActive(false);


        if (round <= totalRounds) {
            activeUnit = (activeUnit + 1) % units.Length;
            Units[ActiveUnit].GetComponent<Unit>().TakeTurn();
        }
    }


	/* SET BUTTON COLOR
	-------------------------------------------------------- */
	void SetButtonColor(GameObject button, Color newColor) {
		ColorBlock cb = button.GetComponent<Button>().colors;
		cb.normalColor = newColor;
		button.GetComponent<Button>().colors = cb;
	}

	public void RemoveUnit(int unit) {
		if (unit >= 0) {
			List<GameObject> tempList = new List<GameObject>(units);
			tempList.RemoveAt(unit);
			units = tempList.ToArray();
		}
		else {
			throw new Exception("Unit to be removed was not found");
		}
	}

}
