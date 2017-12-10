using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pulse : MonoBehaviour {

	public GameObject crystal;

	private Material mat;

	Color baseEmission;
	Color baseColor;

	float floor = 0.3f;
	float ceiling = 1.0f;

	float speed = 0.3f;
	float delay;
	bool running = false;

	// Use this for initialization
	void Start () {
		mat = crystal.GetComponent<Renderer>().material;
		baseEmission = mat.GetColor("_EmissionColor");
		baseColor = mat.GetColor("_Color");
		delay = Random.Range(0f, 5.0f);
		Debug.Log("delay: " + delay);
		StartCoroutine(Delay());
	}
	
	// Update is called once per frame
	void Update () {
		if (running) {
			float emission = floor + Mathf.PingPong(Time.time * speed + delay, ceiling - floor);
			// Color newBaseColor = baseColor; //Replace this with whatever you want for your base color at emission level '1'
			float color = floor + Mathf.PingPong(Time.time * speed + delay, ceiling - floor);

			Color newBaseEmission = baseEmission;
			Color newBaseColor = baseColor;

			Color finalEmission = newBaseEmission * Mathf.LinearToGammaSpace(emission);
			Color finalColor = newBaseColor * Mathf.LinearToGammaSpace(color);

			mat.SetColor("_Color", finalColor);
			// mat.SetColor("_EmissionColor", finalColor);
			DynamicGI.SetEmissive(crystal.GetComponent<Renderer>(), finalEmission);
		}
	}

	IEnumerator Delay() {
		yield return new WaitForSeconds(delay);
		Debug.Log(Time.time);
		running = true;
	}
}
