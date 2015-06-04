using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LobbyScript : MonoBehaviour {

	SocketListener serverInstance;
	AsynchronousClient clientInstance;

	public Text userstext;
	public Text userstext2;
	public Text chattext;
	public Text statustext;

	public Text room1text;
	public Text room2text;
	public Text room3text;
	public Text room4text;



	public InputField inputfield;


	private string userstextstring = "";
	private string userstextstring2 = "";
	private string chattextstring = "";
	private string statustextstring = "Room Status 1/4";
	
		// Use this for initialization
	void Start () {

		serverInstance = SocketListener.instance;
		clientInstance = AsynchronousClient.instance;
			
		userstextstring = "";
		//userstext.text = userstextstring;
		chattextstring = "";
		statustextstring = "Room status 1/4";
	}
	
	// Update is called once per frame
	void Update () {

		userstext.text = userstextstring;
		userstext2.text = userstextstring2;
		chattext.text = chattextstring;
		statustext.text = statustextstring;
		room1text.text = AsynchronousClient.roomstrings[0];
		room2text.text = AsynchronousClient.roomstrings[1];
		room3text.text = AsynchronousClient.roomstrings[2];
		room4text.text = AsynchronousClient.roomstrings[3];
	}

	void OnGUI(){

		if(inputfield.isFocused && inputfield.text != "" && Input.GetKey(KeyCode.Return)) {
			sendChat();
			inputfield.text = "";
		}

		chattextstring = "";
		userstextstring = "";
		userstextstring2 = "";
		string[] stringy = new string[5];
		ClientAction.lobbyInfo.chatstrings.CopyTo (stringy, 0);

		foreach (string s in stringy) {
			chattextstring+= "\n"+s;

		}
		foreach (string s in ClientAction.lobbyInfo.usersstrings) {
			userstextstring+= "\n"+s;
		}
		foreach (string s in ClientAction.lobbyInfo2.usersstrings) {
			userstextstring2+= "\n"+s;
		}
		statustextstring = ClientAction.lobbyInfo2.statustextstring;
		userstext2.text = userstextstring2;
		userstext.text = userstextstring;
		chattext.text = chattextstring;
		statustext.text = statustextstring;

		room1text.text = AsynchronousClient.roomstrings[0];
		room2text.text = AsynchronousClient.roomstrings[1];
		room3text.text = AsynchronousClient.roomstrings[2];
		room4text.text = AsynchronousClient.roomstrings[3];
	}


	public void sendChat()
	{
		Debug.Log ("sending chat");
		string content = "Chat|" + clientInstance.playername +":" + inputfield.text + "|<EOF>";
		StateObject send_so = new StateObject ();
		send_so.workSocket = AsynchronousClient.client;
		AsynchronousClient.Send (AsynchronousClient.client, content, send_so);
		send_so.sendDone.WaitOne(5000);
		Debug.Log("chat sent");

		//chattextstring += inputfield.text;
	}

	public void sendLoadGame(int session)
	{
		AsynchronousClient.instance.session = session;
		StateObject send_so = new StateObject();
		send_so.workSocket = AsynchronousClient.client;
		AsynchronousClient.Send(AsynchronousClient.client,"StartSession|"+session+"|<EOF>", send_so);
		send_so.sendDone.WaitOne(5000);
	}
	public void sendBeginGame()
	{


		StateObject send_so = new StateObject();
		send_so.workSocket = AsynchronousClient.client;
		AsynchronousClient.Send(AsynchronousClient.client,"BeginGameSession|"+clientInstance.session+"|<EOF>", send_so);
		send_so.sendDone.WaitOne(5000);
	}
	public void sendJoinGame(int session)
	{
		AsynchronousClient.instance.session = session;
		StateObject send_so = new StateObject();
		send_so.workSocket = AsynchronousClient.client;
		AsynchronousClient.Send(AsynchronousClient.client,"JoinSession|"+session+"|<EOF>", send_so);
		send_so.sendDone.WaitOne(5000);
	}
	public void sendHostGame()
	{
		//AsynchronousClient.instance.session = getnextsession
		StateObject send_so = new StateObject();
		send_so.workSocket = AsynchronousClient.client;
		AsynchronousClient.Send(AsynchronousClient.client,"HostSession|<EOF>", send_so);
		send_so.sendDone.WaitOne(5000);
	}
	public void sendPlayers(int playernum)
	{
		//AsynchronousClient.instance.session = getnextsession
		StateObject send_so = new StateObject();
		send_so.workSocket = AsynchronousClient.client;
		AsynchronousClient.Send(AsynchronousClient.client,"ChangePlayers|"+AsynchronousClient.instance.session+"|"+playernum+"|<EOF>", send_so);
		send_so.sendDone.WaitOne(5000);
	}

	public void sendQuitGame()
	{
		//AsynchronousClient.instance.session = getnextsession
		StateObject send_so = new StateObject();
		send_so.workSocket = AsynchronousClient.client;
		AsynchronousClient.Send(AsynchronousClient.client,"QuitLobbySession|" +AsynchronousClient.instance.session +"|<EOF>", send_so);
		send_so.sendDone.WaitOne(5000);
	}




}
