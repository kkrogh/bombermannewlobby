using UnityEngine;
using System.Collections;

public class ServerBomb : MonoBehaviour {

	public ServerCharacter owner;
	public GameObject serverExplosionObj;
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
		
		GameObject expObj = Instantiate(serverExplosionObj, this.transform.position, this.transform.rotation) as GameObject;
		ServerExplosion explosion = expObj.GetComponent<ServerExplosion>();
		explosion.owner = this.owner;
		
		Vector2 up = (Vector2)this.transform.position + new Vector2 (0, 1);
		Vector2 down = (Vector2)this.transform.position + new Vector2 (0, -1);
		Vector2 right = (Vector2)this.transform.position + new Vector2 (1, 0);
		Vector2 left = (Vector2)this.transform.position + new Vector2 (-1, 0);
		
		Collider2D colObj;
		
		colObj = Physics2D.OverlapCircle (up, 0.3f);
		
		if(colObj == null || colObj.tag != "UnDestroyable" )
		{
			expObj = Instantiate(serverExplosionObj, up, this.transform.rotation) as GameObject;
			explosion = expObj.GetComponent<ServerExplosion>();
			explosion.owner = this.owner;
		}
		
		colObj = Physics2D.OverlapCircle (down, 0.3f);
		
		if(colObj == null || colObj.tag != "UnDestroyable" )
		{
			expObj = Instantiate(serverExplosionObj, down, this.transform.rotation) as GameObject;
			explosion = expObj.GetComponent<ServerExplosion>();
			explosion.owner = this.owner;
		}
		
		colObj = Physics2D.OverlapCircle (right, 0.3f);
		
		if(colObj == null || colObj.tag != "UnDestroyable" )
		{
			expObj = Instantiate(serverExplosionObj, right, this.transform.rotation) as GameObject;
			explosion = expObj.GetComponent<ServerExplosion>();
			explosion.owner = this.owner;
		}
		
		colObj = Physics2D.OverlapCircle (left, 0.3f);
		
		if(colObj == null || colObj.tag != "UnDestroyable" )
		{
			expObj = Instantiate(serverExplosionObj, left, this.transform.rotation) as GameObject;
			explosion = expObj.GetComponent<ServerExplosion>();
			explosion.owner = this.owner;
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
