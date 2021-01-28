using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkTestClient : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void JoinServer()
    {
        ColyseusClient.Instance.ConnectToServer();
    }
    public void GetRooms() {
        ColyseusClient.Instance.GetAvailableRooms();
    }
    public void JOCRoom() { ColyseusClient.Instance.JoinOrCreateRoom(); }

    public void TestRoom()
    {
       // ColyseusClient.Instance.ConnectToServer();
       // ColyseusClient.Instance.JoinOrCreateRoom();
        GameObject.Find("Message").GetComponent<Text>().text = "My Id=:"+ColyseusClient.Instance.room.SessionId+" & Connected to room "+ColyseusClient.Instance.room.Id;
    }
    public void SendMSG() {
        ColyseusClient.Instance.SendMessage(GameObject.Find("MessageToPAss").GetComponent<InputField>().text);
    }

}
