using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerCharacter : MonoBehaviour {

	public int life;
	public int bombLimit;
	public int liveBombs;
	public int bombPower;
	public float speed;
	public bool kickItem;
	private Animator animator;
	public int deathCount;
	public bool dying;
	public GameObject bomb;
	public int deaths;
	public int playerNum;
	public string playername;
	public int session;
	
	void Awake()
	{
		animator = GetComponent<Animator> ();
		bomb = Resources.Load("ServerBomb") as GameObject;
		deathCount = 0;
		dying = false;
		deaths = 5;
		playerNum = 0;
	}
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		//when the player dies, give them few frames of break with death animation
		
		if (deathCount > 1) {
			deathCount--;
		} else if(deathCount==1){
			
			float offset = (session-1)*13;
			this.transform.position = new Vector2 (1+offset, 1);
			deathCount=0;
			dying=false;
		}
		
	}
	
	
	void OnTriggerEnter2D(Collider2D col)
	{
		//when the player dies
		if (col.tag == "Explosion" && !dying) 
		{
			life--;
			deathCount=90;
			//animator.SetTrigger ("Death");
			dying=true;
			
			deaths--;
			
			ServerCharacter owner = col.gameObject.GetComponent<ServerExplosion>().owner;
			
			if(owner != this)
			{
				SocketListener.instance.DataBaseAddKillScore(owner.playername, 1);
			}
			
			SocketListener.instance.DataBaseAddDeathScore(playername, 1);
			
			foreach(ServerPlayer player in ServerLevelManager.instance.sessionMap[session])
			{
				if(player != null && player.client != null)
				{
					SocketListener.Send(player.client,  "Death|" + playerNum + "|"
					                    + deaths + "|<EOF>");
				}
				
			}
			
		}
		if (col.tag == "Item_BombPower") {
			this.bombPower++;
			Destroy(col.gameObject);
		}
	}
	
	public void DropBomb(float x, float y)
	{
		Debug.Log("DropBomb function");		
		Vector2 bombPos = new Vector2(x,y);
		
		GameObject obj = Instantiate(bomb, bombPos,transform.rotation) as GameObject;
		ServerBomb bombObj = obj.GetComponent<ServerBomb> ();
		bombObj.owner = this;
	}
}
