using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {

	public int startingX;
	public int startingZ;
	public int rotation;

	private GameObject unitButton;


    /* TAKE TURN
	-------------------------------------------------------- */
    public void TakeTurn() {
		Debug.Log(gameObject.name + " taking turn");
		SetButtonColor(Color.black);
	}


    /* SET BUTTON
	-------------------------------------------------------- */
    public void SetButton(GameObject button) {
		unitButton = button;
    }


    /* GET BUTTON
	-------------------------------------------------------- */
	public GameObject GetButton() {
		return unitButton;
	}


    /* SET BUTTON COLOR
	-------------------------------------------------------- */
    public void SetButtonColor(Color newColor) {
		ColorBlock cb = unitButton.GetComponent<Button>().colors;
        cb.normalColor = newColor;
        unitButton.GetComponent<Button>().colors = cb;
	}

}
