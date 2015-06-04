using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class Login : MonoBehaviour 
{
	SocketListener serverInstance;
	AsynchronousClient clientInstance;
	
	public string username = "";
	public string password = "";
	bool registerUI = false;
	bool loginUI = false;
	public string ipAdd = "192.168.0.16";
	// Use this for initialization
	void Start () 
	{
		serverInstance = SocketListener.instance;
		clientInstance = AsynchronousClient.instance;
	}
	
	// Update is called once per frame
	void OnGUI()
	{
		if(AsynchronousClient.client == null || !AsynchronousClient.client.Connected)
		{
			GUI.Label(new Rect(125, 260, 100, 25),  "IP Address: ");
			ipAdd = GUI.TextArea (new Rect(225,260,110,25),ipAdd);
			if(GUI.Button(new Rect(225,290,150,25),"Connect to Server"))
			{
				clientInstance.StartClient(ipAdd);
			}
		}
		else
		{
			if(loginUI == true && registerUI == false)
			{
				GUI.Label(new Rect(125, 230, 500, 25),  "Please enter your user name and password");
				GUI.Label(new Rect(125, 275, 100, 25),  "User Name: ");
				username = GUI.TextArea (new Rect(225,275,110,25),username);
				GUI.Label(new Rect(125, 325, 100, 25), "Password: ");
				password = GUI.TextArea(new Rect(225,325,110,25),password);
				
				if(GUI.Button (new Rect(225,365,110,25),"Login"))
				{
					string hashed = Md5Sum(password);
					string content = "Login|" + username + "|" + hashed + "|<EOF>";
					Debug.Log( "Attempting login with username ="+username+"  password="+  hashed);
					
					StateObject send_so = new StateObject();
					send_so.workSocket = AsynchronousClient.client;
					AsynchronousClient.Send(AsynchronousClient.client,content, send_so);
					send_so.sendDone.WaitOne(3000);
					
					StateObject recv_so = new StateObject();
					recv_so.workSocket = AsynchronousClient.client;
					
					AsynchronousClient.Receive(recv_so);
					recv_so.receiveDone.WaitOne(3000);
					
					clientInstance.playername = username;
					username = "";
					password = "";
					
					
					
				}
				
				if(GUI.Button (new Rect(225,390,110,25),"Back"))
				{
					loginUI = false;
					username = "";
					password = "";
				}
			}
			else if(loginUI == false && registerUI == true)
			{
				GUI.Label(new Rect(125, 230, 500, 25),  "Please enter your user name and password");
				GUI.Label(new Rect(125, 275, 100, 25),  "User Name: ");
				username = GUI.TextArea (new Rect(225,275,110,25),username);
				GUI.Label(new Rect(125, 325, 100, 25), "Password: ");
				password = GUI.TextArea(new Rect(225,325,110,25),password);
				
				if(GUI.Button (new Rect(225,365,110,25),"Register"))
				{
					string hashed = Md5Sum(password);
					string content = "Register|" + username + "|" + hashed + "|<EOF>";
					Debug.Log( "Attempting Register with username ="+username+"  password="+  hashed);
					
					StateObject send_so = new StateObject();
					send_so.workSocket = AsynchronousClient.client;
					AsynchronousClient.Send(AsynchronousClient.client,content, send_so);
					send_so.sendDone.WaitOne(5000);
					
					StateObject recv_so = new StateObject();
					recv_so.workSocket = AsynchronousClient.client;
					
					AsynchronousClient.Receive(recv_so);
					recv_so.receiveDone.WaitOne(5000);
					Debug.Log("Response received : " + recv_so.response);
					
					username = "";
					password = "";
				}
				
				if(GUI.Button (new Rect(225,390,110,25),"Back"))
				{
					registerUI = false;
					username = "";
					password = "";
				}
			}
			else
			{
				if(GUI.Button(new Rect(225,250,100,25),"Login"))
				{
					loginUI =true;
				}
				if(GUI.Button(new Rect(225,275,100,25),"Regiser"))
				{
					registerUI = true;
				}
			}

		}
	}
	
	public string Md5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
		
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}
}
