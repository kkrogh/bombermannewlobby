using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public Character owner;
	public float deathTime = 1.0f;


	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		deathTime = deathTime - Time.deltaTime;
		if (deathTime <= 0)
		{
			Destroy (this.gameObject);
		}
	}
}
