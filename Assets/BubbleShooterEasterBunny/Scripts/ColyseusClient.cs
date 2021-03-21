using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

using Colyseus;
using Colyseus.Schema;

using GameDevWare.Serialization;
//client script for connection and mgmt of server connectivity
[Serializable]
class Metadata
{
	public string str;
	public int number;
}

[Serializable]
class CustomRoomAvailable : RoomAvailable
{
	public Metadata metadata;
}

class CustomData
{
	public int integer;
	public string str;
}

class TypeMessage
{
	public bool hello;
}

enum MessageType {
	ONE = 0
};
class MessageByEnum
{
	public string str;
}

public class ColyseusClient : MonoBehaviour {

	// UI Buttons are attached through Unity Inspector
	bool isFirstCall;
	public string roomName = "game";
	public bool isoffline = false;
	public static ColyseusClient Instance;
	public Client client;
	public Room<State> room;
	public EventHandler<State> OnInitialState;
	public Room<IndexedDictionary<string, object>> roomFossilDelta;
	public Room<object> roomNoneSerializer;
	public EventHandler<State> OnGameStateChangeMenuHandler;
	public EventHandler<State> OnGameStateChangeGameHandler;
	private bool initialStateReceived = false;
	public bool isGameQuit;
	// Use this for initialization
	void Start () {//initialization
		/* Demo UI */
		isGameQuit = false;
		 isFirstCall = true;
		if (!Instance)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public async void ConnectToServer ()//method for connecting to server
	{
		/*
		 * Get Colyseus endpoint from InputField
		 */
		
		//string endpoint = "ws://floating-wave-93492.herokuapp.com";
		string endpoint = "ws://localhost:2567";

		Debug.Log("Connecting to " + endpoint);

		/*
		 * Connect into Colyeus Server
		 */
		client = ColyseusManager.Instance.CreateClient(endpoint);

		/*await client.Auth.Login();

		var friends = await client.Auth.GetFriends();

		// Update username
		client.Auth.Username = "Jake";
		await client.Auth.Save();*/
	}

	public async void CreateRoom()//for creation of room 
	{
		room = await client.Create<State>(roomName, new Dictionary<string, object>() { });
		//roomNoneSerializer = await client.Create("no_state", new Dictionary<string, object>() { });
		//roomFossilDelta = await client.Create<IndexedDictionary<string, object>>("fossildelta", new Dictionary<string, object>() { });
		print("----" + room);
		RegisterRoomHandlers();
	}

	public async void JoinOrCreateRoom()// for joining available room if not found any room than creates one mainly used 
	{
		room = await client.JoinOrCreate<State>(roomName, new Dictionary<string, object>() { });
		RegisterRoomHandlers();
	}

	public async void JoinRoom ()
	{
		room = await client.Join<State>(roomName, new Dictionary<string, object>() { });
		
		RegisterRoomHandlers();
	}

	async void ReconnectRoom ()
	{
		string roomId = PlayerPrefs.GetString("roomId");
		string sessionId = PlayerPrefs.GetString("sessionId");
		if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(roomId))
		{
			Debug.Log("Cannot Reconnect without having a roomId and sessionId");
			return;
		}

		room = await client.Reconnect<State>(roomId, sessionId);

		Debug.Log("Reconnected into room successfully.");
		RegisterRoomHandlers();
	}

	public void RegisterRoomHandlers()//used to assign properites and assign handlers for different state changing events
	{
		

		
		room.State.TriggerAll();

		PlayerPrefs.SetString("roomId", room.Id);
		PlayerPrefs.SetString("sessionId", room.SessionId);
		PlayerPrefs.Save();

		room.OnLeave += (code) => Debug.Log("ROOM: ON LEAVE");
		room.OnError += (code, message) => Debug.LogError("ERROR, code =>" + code + ", message => " + message);
		room.OnStateChange += OnStateChangeHandler;

		room.OnMessage((Message message) =>
		{
			Debug.Log("Received Schema message:");
			Debug.Log(message.num + ", " + message.str);
		});
		
		room.OnMessage<MessageByEnum>((byte) MessageType.ONE, (message) =>
		{
			Debug.Log(">> Received message by enum/number => " + message.str);
		});

		room.OnMessage<TypeMessage>("type", (message) =>
		{
			Debug.Log("Received 'type' message!");
			Debug.Log(message);
		});

		room.OnMessage<TestMSGClass>("TestBrodcast", (message) =>
        {
			Debug.Log("Brodcast from server -=-=-=-=-="+message);
		});
		
		room.OnMessage<PowerUpMessage>("powerup", (message) => {
			Debug.Log("message received from server Powerupppppppp");
			Debug.Log(message);
		});
		room.OnMessage<LoadLevelMsg>("LoadLevel", (message)=>
		{
			
			Debug.Log("Load Level -/-/-/-/-/-/-/-/"+message.lvl);
			PlayerPrefs.SetInt("OpenLevel", message.lvl);
			Application.LoadLevel("game");

		});
		room.OnMessage<GameFinishedMsg>("GameFinished", (message) =>
		{

			
		});
		room.OnMessage<GetBotLevelDataMSg>("GetBotLevelData", (message) =>
		{
			float[] botData = BotManager.Instance.GetBotLevelData(message.lvl);
			if (room != null)
			{
				//room.Send("schema");
				room.Send("BotCurrentLevelDataData", botData);
			}
			else
			{
				Debug.Log("Room is not connected!");
			}
		});

	}
	
	public void LeaveRoom()
	{
        room.Leave();

	}

	public async void GetAvailableRooms()
	{
		var roomsAvailable = await client.GetAvailableRooms<CustomRoomAvailable>(roomName);

		Debug.Log("Available rooms (" + roomsAvailable.Length + ")");
		for (var i = 0; i < roomsAvailable.Length; i++)
		{
			Debug.Log("roomId: " + roomsAvailable[i].roomId);
			Debug.Log("maxClients: " + roomsAvailable[i].maxClients);
			Debug.Log("clients: " + roomsAvailable[i].clients);
			Debug.Log("metadata.str: " + roomsAvailable[i].metadata.str);
			Debug.Log("metadata.number: " + roomsAvailable[i].metadata.number);
		}
	}

	public void SendMessage(String msg)//used to instruct server about scores level win loose etc.
	{
		if (room != null)
		{
			//room.Send("schema");
			room.Send("GameControl",msg);
		}
		else
		{
			Debug.Log("Room is not connected!");
		}
	}

	void OnStateChangeHandler(State state, bool isFirstState)//main code where state changes comes and called to different changes
	{
		// Setup room first state
		Debug.Log("State has been updated!-/-/-/-//-//-/--/--/-/-/" + state.roomStatus);
		if (state.roomStatus== "GameFinished")
		{
			//GameObject.Find("Canvas").transform.Find("GameFinished").gameObject.SetActive(true);
		//	yield return new WaitForSeconds(1.5f);
			Application.LoadLevel("GameFinished");
		}

		/*if (state.roomStatus == "waiting")
		{
//			Debug.Log("Waiting for oppponent:=" + isFirstState);
			//OnGameStateChangeMenuHandler?.Invoke(this, state);

		}*/
		if (state.roomStatus == "startplaying")
		{
			if (isFirstCall)
			{
				isFirstCall = false;

				//OnGameStateChangeMenuHandler?.Invoke(this, state);this logic is replaced with other no need fro this handlar
			}
			//Debug.Log("Game started for first time-/-/-/-"+isFirstState);
			//OnGameStateChange?.Invoke(this, room.State.roomStatus);
			//	if (this.room.State.player1.sessionId==room.SessionId) { }
			//NetworkCoordination.LoadGame(int.Parse(this.room.State.player1.currentLevel.ToString()));
		}
		

		if (state.roomStatus != "GameFinished" && state.roomStatus== "startplaying")
		{
			OnGameStateChangeGameHandler?.Invoke(this, state);

		}
		else if(state.roomStatus == "GameFinished" && isGameQuit!=true)
        {
			OnGameStateChangeGameHandler?.Invoke(this, state);
		}
	}



	
	void OnApplicationQuit()
	{
	}
}

class GameFinishedMsg
{
	public string msg;
	public int player;
}

class LoadLevelMsg
{
	public int lvl;
}
class GetBotLevelDataMSg
{
	public int lvl;
}
internal class TestMSGClass
{
 public string msg;
}

class PowerUpMessage
{
	public string type;
}
