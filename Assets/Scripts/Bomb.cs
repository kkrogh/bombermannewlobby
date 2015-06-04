using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour 
{
	public Character owner;
	public GameObject explosion;
	public float deathTime = 3.0f;


	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		deathTime = deathTime - Time.deltaTime;

		if (deathTime <= 0)
		{
			BombDeath();
		}
	}

	void BombDeath()
	{
		owner.liveBombs--;
		Destroy (this.gameObject);

		Instantiate(explosion, this.transform.position, this.transform.rotation);

		Vector2 up = (Vector2)this.transform.position + new Vector2 (0, 1);
		Vector2 down = (Vector2)this.transform.position + new Vector2 (0, -1);
		Vector2 right = (Vector2)this.transform.position + new Vector2 (1, 0);
		Vector2 left = (Vector2)this.transform.position + new Vector2 (-1, 0);
		
		Collider2D colObj;

		colObj = Physics2D.OverlapCircle (up, 0.3f);

		if(colObj == null || colObj.tag != "UnDestroyable" )
		{
			Instantiate(explosion, up, this.transform.rotation);
		}

		colObj = Physics2D.OverlapCircle (down, 0.3f);

		if(colObj == null || colObj.tag != "UnDestroyable" )
		{
			Instantiate(explosion, down, this.transform.rotation);
		}

		colObj = Physics2D.OverlapCircle (right, 0.3f);
		
		if(colObj == null || colObj.tag != "UnDestroyable" )
		{
			Instantiate(explosion, right, this.transform.rotation);
		}

		colObj = Physics2D.OverlapCircle (left, 0.3f);
		
		if(colObj == null || colObj.tag != "UnDestroyable" )
		{
			Instantiate(explosion, left, this.transform.rotation);
		}

	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			this.GetComponent<Collider2D>().isTrigger = false;
		}
		
	}
	
}
