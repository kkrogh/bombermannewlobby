Milestone 4 instructions

You can find all the relevant files in the Assets folder

Scenes are in Assets/Scenes
Scripts are in Assets/Scripts

--Relevant Scripts--
AsynchronousClient
SocketListener
ServerLevelManager
ClientLevelManager
Lobbyscript
Login


--Setting Up the Game--
Client:
You must create a new Unity build with the following scenes:
LoginScreen, LobbyScene, and MainGame
Make sure that the first scene is the login screen

Server:
Run the ServerMainScene in the editor.
The server cannot be run as a build because the datapath cannot find the 
database on build.


--Playing the game--
-Run the ServerMainScene in the editor
-Run the executable client build(s)
-On client type in the server's ip address
-You can register a new username or login with an existing one
-If login was succesful you will be taken to a lobby screen
	Existing usernames:
	username:mario
	password:luigi

	username:zelda
	password:link

-Players in the lobby screen can chat with one another
-By pressing one of the StartSession buttons the game will start
-A player can enter a game session alone
-Other players can join his or the other game session

Database:
Scores are updated and kept in Assets/BombermanDB.s3db