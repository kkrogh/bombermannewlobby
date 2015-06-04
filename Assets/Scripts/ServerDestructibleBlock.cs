using UnityEngine;
using System.Collections;

public class ServerDestructibleBlock : MonoBehaviour {

	public GameObject BombPower;
	// Use this for initialization
	void Start () {
		
	}
	
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Explosion") 
		{
			SpawnItem ();
			Destroy (this.gameObject);
		}
	}
	
	void SpawnItem()
	{
		if (Random.value > 0.5) {
			Instantiate(BombPower, this.gameObject.transform.position, this.gameObject.transform.rotation);
		}
	}
}
