using UnityEngine;
using System.Collections;

public class DestructibleBlock : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Explosion") 
		{
			Destroy (this.gameObject);
		}
	}
}
