using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	private int x;
	private int z;
	private float height;

	public int getX() { return x; }

	public int getZ() {	return z; }

	public float getHeight() { return height; }

	public void setX(int newX) { x = newX; }

	public void setZ(int newZ) { z = newZ; }

	public void setHeight(float newHeight) { height = newHeight; }
}
