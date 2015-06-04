using UnityEngine;
using System.Collections;

public enum GameState
{	
	Loading,
	WaitingForPlayers,
	Playing
};

public class ClientLevelManager : MonoBehaviour 
{
	public static ClientLevelManager instance = null;
	
	public GameObject[] scoreBoard;
	public GameObject[] scoreGuys;
	
	public GameState gameState = GameState.Loading;
	public GameObject[] bManPrefabs;
	
	public GameObject player;
	public int playerNum;
	
	
	
	GameObject[] playerArray;
	
	private float timer = 0;
	
	void Awake()
	{
		
		if(instance != null && instance != this)
		{
			DestroyImmediate(gameObject);
			return;
		}
		
		instance = this;
		DontDestroyOnLoad(gameObject);
	}
	// Use this for initialization
	void Start () 
	{
		scoreBoard = new GameObject[4];
		scoreBoard[0] = GameObject.Find("Score1");
		scoreBoard[1] = GameObject.Find("Score2");
		scoreBoard[2] = GameObject.Find("Score3");
		scoreBoard[3] = GameObject.Find("Score4");
		
		scoreBoard[0].SetActive(false);
		scoreBoard[1].SetActive(false);
		scoreBoard[2].SetActive(false);
		scoreBoard[3].SetActive(false);
		
		scoreGuys = new GameObject[4];
		scoreGuys[0] = GameObject.Find("Player1");
		scoreGuys[1] = GameObject.Find("Player2");
		scoreGuys[2] = GameObject.Find("Player3");
		scoreGuys[3] = GameObject.Find("Player4");
		
		scoreGuys[0].SetActive(false);
		scoreGuys[1].SetActive(false);
		scoreGuys[2].SetActive(false);
		scoreGuys[3].SetActive(false);
	
		bManPrefabs = new GameObject[4];
		bManPrefabs[0] = Resources.Load("ClientMan") as GameObject;
		bManPrefabs[1] = Resources.Load("ClientMan") as GameObject;
		bManPrefabs[2] = Resources.Load("ClientMan") as GameObject;
		bManPrefabs[3] = Resources.Load("ClientMan") as GameObject;
		
		player = GameObject.Find("Bomberman");
		playerNum = 0;
		
		playerArray = new GameObject[4];
		
		StateObject send_so = new StateObject();
		send_so.workSocket = AsynchronousClient.client;
		AsynchronousClient.Send(AsynchronousClient.client,"Loaded|<EOF>", send_so);
		send_so.sendDone.WaitOne(5000);
		Debug.Log("sent loaded");
	}
	
	// Update is called once per frame
	void Update () 
	{
		
//		if(timer > 0.05)
//		{
//		
////			string content =  "PlayerPos " + ClientLevelManager.instance.playerNum + " " + player.transform.position.x.ToString () 
////				+ " " + player.transform.position.y.ToString () + " <EOF>";
////			StateObject send_so = new StateObject ();
////			send_so.workSocket = AsynchronousClient.client;
////			AsynchronousClient.Send (AsynchronousClient.client, content, send_so);
//			
//			timer = 0;
//		}
//		
//		timer = timer + Time.deltaTime;
	}
	
	
	
	void OnGUI()
	{
		if(gameState == GameState.WaitingForPlayers)
		{
			
		}
		
	}
	
	public void LoadPlayer(int playerNum)
	{

		this.playerNum = playerNum;
		int index = playerNum - 1;
		
		scoreGuys[index].SetActive(true);
		scoreBoard[index].SetActive(true);
		
		playerArray[index] = player;
		
		if(playerNum == 1)
		{
			player.transform.position = new Vector2(1,9);
		}
		else if(playerNum == 2)
		{
			player.transform.position = new Vector2(1,1);
		}
		else if(playerNum == 3)
		{
			player.transform.position = new Vector2(11,9);
		}
		else if(playerNum == 4)
		{
			player.transform.position = new Vector2(11,1);
		}
	
	}
	
	public void AddEnemy(int enemyNum)
	{
		Debug.Log("Adding Enemy " + enemyNum);
		int index = enemyNum - 1;
		
		scoreGuys[index].SetActive(true);
		scoreBoard[index].SetActive(true);
		
		playerArray[index] = Instantiate(bManPrefabs[enemyNum-1]) as GameObject;
		
		if(enemyNum == 1)
		{
			playerArray[index].transform.position = new Vector2(1,9);
		}
		else if(enemyNum == 2)
		{
			playerArray[index].transform.position = new Vector2(1,1);
		}
		else if(enemyNum == 3)
		{
			playerArray[index].transform.position = new Vector2(11,9);
		}
		else if(enemyNum == 4)
		{
			playerArray[index].transform.position = new Vector2(11,1);
		}
	}
	
	public void SetPlayerPos(int playerNum, float x, float y)
	{
		int index = playerNum - 1;
		
		if(index > -1 && index < 4 && playerArray[index] != null)
		{
			playerArray[index].transform.position = new Vector2(x,y);
		}
	}
	
	public void ClientDropBomb(int playerNum, float x, float y)
	{
		int index = playerNum - 1;
		
		if(index > -1 && index < 4 && playerArray[index] != null)
		{
			Character clientChar = playerArray[index].GetComponent<Character>();
			clientChar.DropBomb(x,y);
		}
		
	}
	
	public void RemovePlayer(int playerNum)
	{
		int index = playerNum - 1;
		
		if(index > -1 && index < 4 && playerArray[index] != null)
		{
			Destroy(playerArray[index]);
			playerArray[index] = null;
			
			scoreBoard[index].SetActive(false);
			scoreGuys[index].SetActive(false);
		}
	}
	
	public void UpdateScore(int playerNum, int score)
	{
		int index = playerNum - 1;
		scoreBoard[index].GetComponent<TextMesh>().text = "" + score;
	}
}
