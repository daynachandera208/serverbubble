using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkCoordination : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
  
    public void LoadTest() {
        Application.LoadLevel("ServerTesting");
    }
    public void LogActiveRooms() {
        ColyseusClient.Instance.ConnectToServer();
        //ColyseusClient.Instance.JoinOrCreateRoom();

        ColyseusClient.Instance.GetAvailableRooms();
       // Debug.Log("Correct untill here let's see what's problem printing my id:" + ColyseusClient.Instance.room.SessionId);
    }
    public void StartGame()
    {
        if (GameObject.Find("MenuMessage").GetComponent<TextMeshProUGUI>().enabled==false || GameObject.Find("MenuMessage").GetComponent<TextMeshProUGUI>().text == "Tap Play to Start Game.")//ColyseusClient.Instance.client==null || ColyseusClient.Instance.room==null)
        {
            ColyseusClient.Instance.ConnectToServer();
            ColyseusClient.Instance.JoinOrCreateRoom();
           // ColyseusClient.Instance.OnGameStateChangeMenuHandler += GameStateChangeHandler;
           // Debug.Log("Correct untill here let's see what's problem printing my id:"+ColyseusClient.Instance.room.SessionId);
            // if (ColyseusClient.Instance.room.State.roomStatus == "waiting")
            {
                //Debug.Log("-/-////-/-/-/-/-/-/-/-/-*****-"+ GameObject.Find("MenuMessage").GetComponent<TextMeshProUGUI>().text);
                 GameObject.Find("MenuMessage").GetComponent<TextMeshProUGUI>().enabled = true;
                 GameObject.Find("MenuMessage").GetComponent<TextMeshProUGUI>().text = "Waiting For Opponent...!";
                ColyseusClient.Instance.isoffline = false;
                GameObject.Find("Canvas").transform.Find("PlayOffline").gameObject.SetActive(false);
                 GameObject.Find("StatusDialog").GetComponent<Image>().enabled=true;
                GameObject.Find("PlayMain").transform.GetChild(0).gameObject.SetActive(false);
                GameObject.Find("PlayMain").transform.GetChild(1).gameObject.SetActive(true);
                GameObject.Find("PlayMain").GetComponent<RectTransform>().anchoredPosition = new Vector3(0.5f,-38f,0f);


            }
        }
        else
        {
           // ColyseusClient.Instance.OnGameStateChangeMenuHandler -= GameStateChangeHandler;
            ColyseusClient.Instance.LeaveRoom();
            GameObject.Find("MenuMessage").GetComponent<TextMeshProUGUI>().enabled = true;
            GameObject.Find("PlayMain").GetComponent<RectTransform>().anchoredPosition = new Vector3(0.5f, 136f, 0f);


            GameObject.Find("MenuMessage").GetComponent<TextMeshProUGUI>().text = "Tap Play to Start Game.";
            GameObject.Find("MenuMessage").GetComponent<TextMeshProUGUI>().enabled = false;
            GameObject.Find("StatusDialog").GetComponent<Image>().enabled = false;
            GameObject.Find("Canvas").transform.Find("PlayOffline").gameObject.SetActive(true);
            GameObject.Find("PlayMain").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("PlayMain").transform.GetChild(1).gameObject.SetActive(false);
            print("Left The game");
        }
    }

   /* private void GameStateChangeHandler(object sender, State state)
    {
       // Debug.Log("Satte is -/-/-//-/-/-/-/-/--5646545645646456456435-//-/-/-/"+roomState);
        switch (state.roomStatus)
        {
            case "waiting":
                GameObject.Find("MenuMessage").GetComponent<TextMeshProUGUI>().enabled = true;
                GameObject.Find("MenuMessage").GetComponent<TextMeshProUGUI>().text = "Waiting For Opponent...!";
                break;
            case "startplaying":
               
                    if (state.player1.sessionId == ColyseusClient.Instance.room.SessionId)
                        LoadGame(state.player1.currentLevel);
                    else
                        LoadGame(state.player2.currentLevel);
                
                break;
        }
    }*/

    public void LoadGame(int levelNo)
    {
        
        {
            PlayerPrefs.SetInt("OpenLevel", levelNo);
            Application.LoadLevel("game");
        }
    }

    
}
