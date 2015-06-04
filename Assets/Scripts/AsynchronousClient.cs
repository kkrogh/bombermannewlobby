using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// State object for receiving data from remote device.
//public class StateObject {
//	// Client socket.
//	public Socket workSocket = null;
//	// Size of receive buffer.
//	public const int BufferSize = 256;
//	// Receive buffer.
//	public byte[] buffer = new byte[BufferSize];
//	// Received data string.
//	public StringBuilder sb = new StringBuilder();
//	// ManualResetEvent instances signal completion.
//	public ManualResetEvent connectDone =
//		new ManualResetEvent(false);
//	
//	public ManualResetEvent receiveDone =
//		new ManualResetEvent(false);
//	
//	public ManualResetEvent sendDone =
//		new ManualResetEvent(false);
//	
//	// The response from the remote device.
//	public String response = String.Empty;
//}

public class PlayerInfo
{
	public int playerNum;
	public float x;
	public float y;
	public bool disconnected = false;
	public bool death = false;
	public int score = 0;
	public bool updated = false;
}

public class BombInfo
{
	public int playerNum;
	public float x;
	public float y;
	public bool droppedBomb = false;
}

public class LobbyInfo
{
	public string userstextstring = "";
	public string chattextstring = "";

	public List<string> usersstrings = new List<string>();
	public Queue<string> chatstrings = new Queue<string>();

	//private static List<string> usersstrings = new List<string>();
	//private static List<string> chatstrings = new List<string>();
}
public class LobbyInfo2
{
	public string userstextstring = "";
	public string statustextstring = "Room status 1/4";
	public List<string> usersstrings = new List<string>();
}


public class ClientAction
{
	public static bool received = true;
	public static bool loadLevel = false;
	public static bool loadLobby = false;
	public static bool InLobby = false;
	public static bool loadLobby2 = false;
	public static int playerNum = 0;
	public static Queue<int> enemyNumQueue = new Queue<int>();
	public static PlayerInfo playerInfo = new PlayerInfo();
	public static LobbyInfo lobbyInfo = new LobbyInfo();
	public static LobbyInfo2 lobbyInfo2 = new LobbyInfo2();
	public static BombInfo bombInfo = new BombInfo();
}

public class AsynchronousClient : MonoBehaviour{

	public static string guiDebugStr = "";
	// The port number for the remote device.
	public static AsynchronousClient instance;
	public static Socket client;
	
	public string ipStr = "127.0.0.1";
	private const int port = 11000;
	private string message;
	
	private static string[] stringSeparators = new string[] { "<EOF>" };
	private static bool received = true;

	public string playername;
	public int session = 0;
	public static string[] roomstrings = new string[4];

	
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
	
	void Start()
	{
	//	StartClient();

	}
	
	void Update()
	{
		if(ClientAction.loadLevel)
		{
			Application.LoadLevel("MainGame");
			ClientAction.loadLevel = false;
		}
		if(ClientAction.loadLobby)
		{
			//Debug.Log("loading level lobby");
			Application.LoadLevel("LobbyScene");
			ClientAction.loadLobby = false;
		}
		if(ClientAction.loadLobby2)
		{
			//Debug.Log("loading level lobby2");
			Application.LoadLevel("LobbyScene2");
			ClientAction.loadLobby2 = false;
		}

		if(ClientAction.playerNum > 0)
		{
			Debug.Log("Loading Player " + ClientAction.playerNum);
			ClientLevelManager.instance.LoadPlayer(ClientAction.playerNum);
			ClientAction.playerNum = 0;
		}
		
		while(ClientAction.enemyNumQueue.Count > 0)
		{
			int enemyNum = ClientAction.enemyNumQueue.Dequeue();
			if(enemyNum > 0)
			{
				ClientLevelManager.instance.AddEnemy(enemyNum);
			}
		}
		
		if(ClientAction.playerInfo.updated)
		{			
			if(ClientAction.playerInfo.disconnected)
			{
				ClientLevelManager.instance.RemovePlayer(ClientAction.playerInfo.playerNum);
				ClientAction.playerInfo.disconnected = false;
			}
			else if(ClientAction.playerInfo.death)
			{
				ClientLevelManager.instance.UpdateScore(ClientAction.playerInfo.playerNum, ClientAction.playerInfo.score);
				ClientAction.playerInfo.death = false;
			}
			else
			{
				ClientLevelManager.instance.SetPlayerPos(ClientAction.playerInfo.playerNum,
				                                         ClientAction.playerInfo.x,
				                                         ClientAction.playerInfo.y);
			}
		
			
													 
			ClientAction.playerInfo.updated = false;
		}
		
		if(ClientAction.bombInfo.droppedBomb)
		{
			ClientLevelManager.instance.ClientDropBomb(ClientAction.bombInfo.playerNum,
													   ClientAction.bombInfo.x,
													   ClientAction.bombInfo.y);
			ClientAction.bombInfo.droppedBomb = false;
		}
		
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(50, 50, 200, 100),  "Thread: " + guiDebugStr);
	}
	
	public void MessageHandler(string message)
	{
		try
		{
		guiDebugStr = message;
		Debug.Log("ClientMessageHandler : " + message);
		string[] token = message.Split(new Char[]{'|'});
		
		if(token[0] == "LoadLevel")
		{
			session = int.Parse(token[1]);
			ClientAction.loadLevel = true;

		}
		if(token[0] == "LoadLobby")
		{
			ClientAction.loadLobby = true;
			ClientAction.InLobby = true;
				for(int i = 1; i < token.Length-1;i++)
				{
					ClientAction.lobbyInfo.usersstrings.Add(token[i]);
				}
		}
		if(token[0] == "LoadLobby2")
		{
			if(ClientAction.InLobby)
				{ClientAction.loadLobby2 = true;
					ClientAction.InLobby=false;
					session = int.Parse(token[1]);
					if(ClientAction.lobbyInfo2.usersstrings.Count==0)
					{ClientAction.lobbyInfo2.usersstrings.Add(playername);}
				}

			
			//for(int i = 1; i < token.Length-1;i++)
			//{
			//	ClientAction.lobbyInfo2.usersstrings.Add(token[i]);
				
			//}
		}
		if(token[0] == "ChangeMaxPlayers")
		{
			ClientAction.lobbyInfo2.statustextstring = "Room status:"+token[2]+"/"+token[3];
			
		}
		if(token[0] == "PlayerNum")
		{
			ClientAction.playerNum = int.Parse(token[1]);
			//ClientLevelManager.instance.SetPlayerStartPosition(1);
		}
		if(token[0] == "NewPlayer")
		{
			ClientAction.enemyNumQueue.Enqueue(int.Parse(token[1]));
		}
		if(token[0] == "EnemyPos")
		{
			ClientAction.playerInfo.playerNum = int.Parse(token[1]);
			ClientAction.playerInfo.x = float.Parse(token[2]);
			ClientAction.playerInfo.y = float.Parse(token[3]);
			ClientAction.playerInfo.updated = true;
		}
		if(token[0] == "BombDropped")
		{
			ClientAction.bombInfo.playerNum = int.Parse(token[1]);
			ClientAction.bombInfo.x = float.Parse(token[2]);
			ClientAction.bombInfo.y = float.Parse(token[3]);
			ClientAction.bombInfo.droppedBomb = true;
		}
		if(token[0] == "Chat")
		{
			Debug.Log("adding chat to list");
			ClientAction.lobbyInfo.chatstrings.Enqueue(token[1]);
		
			if(ClientAction.lobbyInfo.chatstrings.Count > 5)
			{ClientAction.lobbyInfo.chatstrings.Dequeue();}
		}
		if(token[0] == "LobbyRoomText")
		{
			roomstrings[int.Parse(token[1])-1] = token[2];
			//	Debug.Log("lobby room should say"+ClientAction.lobbyInfo.roomstrings[int.Parse (token[1])-1]);
		}
		if(token[0] == "Disconnect")
		{
			ClientAction.playerInfo.playerNum = int.Parse(token[1]);
			ClientAction.playerInfo.disconnected = true;
			ClientAction.playerInfo.updated = true;
		}
		if(token[0] == "Death")
		{
			ClientAction.playerInfo.death = true;
			ClientAction.playerInfo.playerNum = int.Parse(token[1]);
			ClientAction.playerInfo.score = int.Parse(token[2]);
			ClientAction.playerInfo.updated = true;
		}
		if(token[0] == "PlayerLoggedIn")
		{
			ClientAction.lobbyInfo.usersstrings.Add(token[1]);
		}
		if(token[0] == "PlayerLoggedInRoom")
		{
			ClientAction.lobbyInfo2.usersstrings.Add(token[1]);
		}
		//yo
		}
		catch(Exception e)
		{
			guiDebugStr = e.ToString();;
		}
	}
//	void OnGUI()
//	{
//		if(GUI.Button(new Rect(100,100,100,25), "Start Client"))
//		{
//			StartClient();
//			Debug.Log("client connected: " + client.Connected);
//		}
//		if(GUI.Button(new Rect(100,125,100,25), "Send 'Test'"))
//		{
//			StateObject send_so = new StateObject();
//			send_so.workSocket = client;
//			Send(client,"This is a test<EOF>", send_so);
//			send_so.sendDone.WaitOne(5000);
//			
//			StateObject recv_so = new StateObject();
//			recv_so.workSocket = client;
//			
//			Receive(recv_so);
//			recv_so.receiveDone.WaitOne(5000);
//			Debug.Log("Response received : " + recv_so.response);
//		}
//		if(GUI.Button(new Rect(100,150,100,25), "Send 'Login'"))
//		{
//			StateObject send_so = new StateObject();
//			send_so.workSocket = client;
//			Send(client,"Login myname mypassword<EOF>", send_so);
//			send_so.sendDone.WaitOne(5000);
//			
//			StateObject recv_so = new StateObject();
//			recv_so.workSocket = client;
//			
//			Receive(recv_so);
//			recv_so.receiveDone.WaitOne(5000);
//			Debug.Log("Response received : " + recv_so.response);
//		}
//		
//	}
	
	public void StartClient(string ipAdd) {
		// Connect to a remote device.
		try {
		Debug.Log("Client Connect Attempt");
			// Establish the remote endpoint for the socket.
			// The name of the 
			// remote device is "host.contoso.com".
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			//IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPAddress ipAddress = IPAddress.Parse(ipAdd);
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
			
			// Create a TCP/IP socket.
			client = new Socket(AddressFamily.InterNetwork,
			                           SocketType.Stream, ProtocolType.Tcp);
			
			StateObject send_so = new StateObject();
			send_so.workSocket = client;
			// Connect to the remote endpoint.
			client.BeginConnect( remoteEP, 
			                    new AsyncCallback(ConnectCallback), send_so);
			
			// Waits for 5 seconds for connection to be done
			send_so.connectDone.WaitOne(5000);
			
			// Send test data to the remote device.
//			Send(client,"This is a test<EOF>", send_so);
//			send_so.sendDone.WaitOne(5000);
			
			// Receive the response from the remote device.
			// Create the state object for receiving.
//			StateObject recv_so = new StateObject();
//			recv_so.workSocket = client;
//			
//			Receive(recv_so);
//			recv_so.receiveDone.WaitOne(5000);
			
			// Write the response to the console.
			//Console.WriteLine("Response received : {0}", recv_so.response);
//			Debug.Log("Response received : " + recv_so.response);
			
		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
	}
	
	private static void ConnectCallback(IAsyncResult ar) {
		try {
			// Create the state object.
			StateObject state = (StateObject)ar.AsyncState;
			// Retrieve the socket from the state object.
			Socket client = state.workSocket;
			
			// Complete the connection.
			client.EndConnect(ar);
			
			Console.WriteLine("Socket connected to {0}",
			                  client.RemoteEndPoint.ToString());
			Debug.Log("Socket connected to " + 
			                  client.RemoteEndPoint.ToString());
			// Signal that the connection has been made.
			state.connectDone.Set();
		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
	}
	
	public static void Receive(StateObject state) {
		try
		{
			Socket client = state.workSocket;
			
			// Begin receiving the data from the remote device.
			client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
			                    new AsyncCallback(ReceiveCallback), state);
		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
	}
	
	private static void ReceiveCallback( IAsyncResult ar ) {
		try {
			// Retrieve the state object and the client socket 
			// from the asynchronous state object.
			ClientAction.received = true;
			StateObject state = (StateObject) ar.AsyncState;
			Socket client = state.workSocket;
			
			// Read data from the remote device.
			int bytesRead = client.EndReceive(ar); 
			
			if (bytesRead > 0) {
				// Found a 
				state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
				string content = state.sb.ToString();
				
				String[] message = content.Split(stringSeparators, StringSplitOptions.None);
				if (message.Length > 1)
				{
					state.receiveDone.Set();
					state.response = message[0];
					
					for(int i = 0; i < message.Length - 1; i++)
					{
						instance.MessageHandler(message[i]);
						
					}
					
					
				//	state.workSocket.Shutdown(SocketShutdown.Both);
				//	state.workSocket.Close();
					
					StateObject newstate = new StateObject();
					newstate.workSocket = client;
					// Call BeginReceive with a new state object
					client.BeginReceive(newstate.buffer, 0, StateObject.BufferSize, 0,
					                     new AsyncCallback(ReceiveCallback), newstate);
				}
				else
				{
					// Get the rest of the data.
					client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
					                    new AsyncCallback(ReceiveCallback), state);
				}
			} 
			else 
			{
				guiDebugStr = "Connection close has been requested";
				//Console.WriteLine("Connection close has been requested.");
				// Signal that all bytes have been received.
				
			}
		} catch (Exception e) {
			//Console.WriteLine(e.ToString());
			guiDebugStr = e.ToString();
		}
	}
	
	public static void Send(Socket client, String data, StateObject so) {
		// Convert the string data to byte data using ASCII encoding.
		byte[] byteData = Encoding.ASCII.GetBytes(data);
		
		// Begin sending the data to the remote device.
		client.BeginSend(byteData, 0, byteData.Length, 0,
		                 new AsyncCallback(SendCallback), so);
	}
	
	private static void SendCallback(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			StateObject so = (StateObject) ar.AsyncState;
			Socket client = so.workSocket;
			
			// Complete sending the data to the remote device.
			int bytesSent = client.EndSend(ar);
			Console.WriteLine("Sent {0} bytes to server.", bytesSent);
			
			// Signal that all bytes have been sent.
			so.sendDone.Set();
		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
	}
	
	
	
	public bool isConnected()
	{
		return client.Connected;
	}
	
	void OnDestroy()
	{
		StateObject send_so = new StateObject();
		send_so.workSocket = AsynchronousClient.client;
		
		if(ClientLevelManager.instance != null)
		{
			AsynchronousClient.Send(AsynchronousClient.client,"Disconnect|" + AsynchronousClient.instance.session + "|"
								 +  ClientLevelManager.instance.playerNum + "|<EOF>", send_so);
		}
		else
		{
			AsynchronousClient.Send(AsynchronousClient.client,"Disconnect|" + 0 + "|" +0 + "|<EOF>", send_so);
		}
		send_so.sendDone.WaitOne(5000);

		
		//client.Disconnect(false);
	}

}

