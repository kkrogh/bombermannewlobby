using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class Client : MonoBehaviour
 {
	
	private const int port = 25001;
	private Socket client;
	// Use this for initialization
	void Start ()
	{
		StartCoroutine("ConnectClient");
		//Debug.Log("connected to " + client.RemoteEndPoint.ToString());
		
	}
	
	// Update is called once per frame
	void OnGUI()
	{
		if (GUI.Button (new Rect (100, 100, 100, 25), "Start client"))
		{
			Debug.Log("connected to " + client.RemoteEndPoint.ToString());
		}
	}
	
	IEnumerator ConnectClient()
	{
		IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
		IPAddress ipAddress = ipHostInfo.AddressList[0];	//replace this adress with the desired server address
		IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
		
		client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		client.Connect(remoteEP);
		
		yield return null;
	}
}
