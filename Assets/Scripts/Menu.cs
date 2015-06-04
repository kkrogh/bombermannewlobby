using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class Menu : MonoBehaviour {

	////////////////////////////////////////////////////////////////////////	
	public string IP = "192.168.0.2";//// Local IP here  //////////////////////////////
	////////////////////////////////////////////////////////////////////////	
	public int Port = 25001;


	public string username = "";
	public string password = "";
	bool RegisterUI = false;
	bool LoginUI = false;
	

	void OnGUI()
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			if (GUI.Button (new Rect (100, 100, 100, 25), "Start Client")) {
				Network.Connect (IP, Port);
			}
			if (GUI.Button (new Rect (100, 125, 100, 25), "Start Server")) {
				Network.InitializeServer (4, Port);
			}
		}
			
		else
		{
			if(Network.peerType==NetworkPeerType.Client)
			{
				if(RegisterUI == true && LoginUI == false)
				{
					GUI.Label(new Rect(100, 125, 100, 25), "User Name: ");
					username = GUI.TextArea (new Rect(200,125,110,25),username);
					GUI.Label(new Rect(100, 150, 100, 25), "Password: ");
					password = GUI.TextArea(new Rect(200,150,110,25),password);
					if(GUI.Button (new Rect(100,175,110,25),"Register"))
					{
						GetComponent<NetworkView>().RPC ("Register",RPCMode.Server,username,password);
						RegisterUI = false;
						username = "";
						password = "";
					}if(GUI.Button (new Rect(100,200,110,25),"Back"))
					{
						RegisterUI = false;
						username = "";
						password = "";
					}
				}
				else if(RegisterUI == false && LoginUI == true)
				{
					GUI.Label(new Rect(100, 125, 100, 25), "User Name: ");
					username = GUI.TextArea (new Rect(200,125,110,25),username);
					GUI.Label(new Rect(100, 150, 100, 25), "Password: ");
					password = GUI.TextArea(new Rect(200,150,110,25),password);
					
					if(GUI.Button (new Rect(100,175,110,25),"Login"))
					{
						Debug.Log( "Attempting login with username ="+username+"  password="+  password);

						GetComponent<NetworkView>().RPC ("Login",RPCMode.Server,username,password);
						Debug.Log("Here1");
						username = "";
						password = "";
					}if(GUI.Button (new Rect(100,200,110,25),"Back"))
					{
						LoginUI = false;
						username = "";
						password = "";
					}
				}
				else{

					GUI.Label(new Rect(100,100,100,25),"Client");

					if(GUI.Button (new Rect(100,125,110,25),"Login"))
					{
						LoginUI = true;
					}
					if(GUI.Button (new Rect(100,150,110,25),"Register"))
					{
						RegisterUI = true;
					}
					if(GUI.Button (new Rect(100,175,110,25),"Logout"))
					{
						Network.Disconnect(250);
					}
				}
			}
			if(Network.peerType == NetworkPeerType.Server)
			{				
				GUI.Label(new Rect(100,100,100,25),"Server");
				GUI.Label(new Rect(100,125,100,25),"Connections: " + Network.connections.Length);
				if(GUI.Button (new Rect(100,150,100,25),"Logout"))
				{
					Network.Disconnect(250);
				}
			}
		}
	}


	[RPC]
	void Login(string username, string password, NetworkMessageInfo info)
	{
		Debug.Log("login requested");

		if (username == "" || password == "") {Debug.Log("no blanks allowed");}
		else if (Network.isServer) {
			bool loginok = false;
			string conn = "URI=file:" + Application.dataPath + "/BombermanDB.s3db"; //Path to database.
			IDbConnection dbconn;
			dbconn = (IDbConnection) new SqliteConnection(conn);
			dbconn.Open(); //Open connection to the database.
			IDbCommand dbcmd = dbconn.CreateCommand();
			
			
			string inusername = username;
			string inpassword = password;
			
			string sqlQuery = "SELECT ID, username, password " + "FROM Users " + "WHERE username == '" + inusername + "' AND password == '" + inpassword + "'";
			dbcmd.CommandText = sqlQuery;
			IDataReader reader = dbcmd.ExecuteReader();
			
			
			while (reader.Read())
			{
				int ID = reader.GetInt32(0);
				string checkname = reader.GetString(1);
				string checkpassword = reader.GetString(2);
				
				Debug.Log( "Database ID= "+ID+" username ="+checkname+"  password="+  checkpassword);
				
				if(checkname == inusername && checkpassword == inpassword){
					Debug.Log ("Found a match");
					loginok = true;
				}
				
			}
				
	
			
			reader.Close();
			reader = null;
			dbcmd.Dispose();
			dbcmd = null;
			dbconn.Close();
			dbconn = null;
			GC.Collect();
			Debug.Log("Disconnected from DB");

			if(loginok)
			{GetComponent<NetworkView> ().RPC ("LoadLevel", info.sender);}
			else{
				Debug.Log("Failed Login");
			}




		}
		else if (Network.isClient)
		{
			Debug.Log ("Attempting to log in");
		}

	}


//	[RPC]
//	void LoadLevel()
//	{
//		if (Network.isClient) {
//			if(Application.loadedLevel == 0)
//			{
//				Application.LoadLevel ("MainGame");
//			}
//		}
//	}


	[RPC]
	void Register(string username, string password)
	{
		Debug.Log(username + " + " + password);
		if (Network.isServer) 
		{
			bool checkUsername = isTaken(username);
			
			if(!checkUsername)
			{
				string conn = "URI=file:" + Application.dataPath + "/BombermanDB.s3db"; //Path to database.
				IDbConnection dbconn;
				dbconn = (IDbConnection) new SqliteConnection(conn);
				dbconn.Open(); //Open connection to the database.
				IDbCommand dbcmd = dbconn.CreateCommand();
				
				
				int nextuserid = getNextUserID();
				string sqlQuery = "INSERT INTO USERS (ID, username, password) VALUES ("+nextuserid+", '"+username+"', '"+password+"')";
				dbcmd.CommandText = sqlQuery;
				IDataReader reader = dbcmd.ExecuteReader();
				
				
				Debug.Log( "user inserted into database" );
				
				reader.Close();
				reader = null;
				dbcmd.Dispose();
				dbcmd = null;
				dbconn.Close();
				dbconn = null;
				GC.Collect();
				Debug.Log("Disconnected from DB");
			}
			else
			{
				Debug.Log("User name already exists");
			}
		}
	}

	bool isTaken(string username)
	{
		bool returnval = false;
		string conn = "URI=file:" + Application.dataPath + "/BombermanDB.s3db"; //Path to database.
		IDbConnection dbconn;
		dbconn = (IDbConnection)new SqliteConnection (conn);
		dbconn.Open (); //Open connection to the database.
		IDbCommand dbcmd = dbconn.CreateCommand ();
		
		
		int countamount = 0;
		string sqlQuery = "SELECT COUNT(*) FROM Users WHERE username =='"+username+"'";
		dbcmd.CommandText = sqlQuery;
		IDataReader reader = dbcmd.ExecuteReader ();
		
		while (reader.Read()) {
			countamount = reader.GetInt32 (0);
			Debug.Log ("users found is "+countamount);
		}
		
		
		reader.Close ();
		reader = null;
		dbcmd.Dispose ();
		dbcmd = null;
		dbconn.Close ();
		dbconn = null;
		GC.Collect ();
		Debug.Log ("Disconnected from DB");

		if (countamount > 0) {
			returnval = true;
		}
		return returnval;

	}

	int getNextUserID()
	{
		string conn = "URI=file:" + Application.dataPath + "/BombermanDB.s3db"; //Path to database.
		IDbConnection dbconn;
		dbconn = (IDbConnection)new SqliteConnection (conn);
		dbconn.Open (); //Open connection to the database.
		IDbCommand dbcmd = dbconn.CreateCommand ();
		
		
		int nextID = 999999;
		string sqlQuery = "SELECT COUNT(*) FROM Users";
		dbcmd.CommandText = sqlQuery;
		IDataReader reader = dbcmd.ExecuteReader ();
		
		while (reader.Read()) {
			nextID = reader.GetInt32 (0);
			nextID++;
			Debug.Log ("nextID is "+nextID);
		}
		
		
		reader.Close ();
		reader = null;
		dbcmd.Dispose ();
		dbcmd = null;
		dbconn.Close ();
		dbconn = null;
		GC.Collect ();
		Debug.Log ("Disconnected from DB");
		return nextID;
	}
	
	
}