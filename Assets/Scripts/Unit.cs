using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {

	public int startingX;
	public int startingZ;
	public int rotation;

	public GameObject unitButton;
	private int startingNumber;

	public void TakeTurn() {
		Debug.Log(gameObject.name + " taking turn");
		SetButtonColor(Color.black);
	}

	public void SetStartingNumber(int num) {
		startingNumber = num;
	}

	public void SetButton() {
        unitButton = GameManager.instance.GetUnitButton(startingNumber);
    }

	public void SetButtonColor(Color newColor) {
		ColorBlock cb = unitButton.GetComponent<Button>().colors;
        cb.normalColor = newColor;
        unitButton.GetComponent<Button>().colors = cb;
	}

}
