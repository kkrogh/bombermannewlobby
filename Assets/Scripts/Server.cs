using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class Server : MonoBehaviour
 {
	public string ip = "127.0.0.1";
	public const int port = 25001;
	
	private static Server instance;
	private static List<Socket> clients;
	private Socket listener;
	
	private bool connected = false;
	
	void Awake()
	{
		if(instance != null && instance != this)
		{
			DestroyImmediate(gameObject);
			return;
		} 
		
		instance = this;
		clients = new List<Socket>();
	}
	
	void Start () 
	{
		StartCoroutine("Listen");
		
	}
	
	void Update()
	{
		
	}
	
	void OnGUI()
	{
		if(!connected)
		{
			if (GUI.Button (new Rect (100, 100, 100, 25), "Start Server"))
			{
				
			}
		}
	}
	
	IEnumerator Listen()
	{
		listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		
		IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
		IPAddress ipAddress = ipHostInfo.AddressList[0];
		IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
		
		IPEndPoint any = new IPEndPoint(IPAddress.Any, port);
		
		listener.Bind(any);
		listener.Listen(100);
		
		Debug.Log("listening");
		Socket socket = listener.Accept();
		
		yield return null;
	}
}
