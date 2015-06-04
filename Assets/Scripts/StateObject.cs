using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

// State object for reading client data asynchronously
public class StateObject {
	// Client socket.
	public Socket workSocket = null;
	// Size of receive buffer.
	public const int BufferSize = 256;
	// Receive buffer.
	public byte[] buffer = new byte[BufferSize];
	// Received data string.
	public StringBuilder sb = new StringBuilder();
	// ManualResetEvent instances signal completion.
	public ManualResetEvent connectDone =
		new ManualResetEvent(false);
	
	public ManualResetEvent receiveDone =
		new ManualResetEvent(false);
	
	public ManualResetEvent sendDone =
		new ManualResetEvent(false);
	
	// The response from the remote device.
	public String response = String.Empty;
}