using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    public uint TimerSeconds=600;
    public bool isGameisGoingON=true;
    void Start()
    {
       
        if (ColyseusClient.Instance.isoffline == false)
        {
            ColyseusClient.Instance.OnGameStateChangeGameHandler += GameStateChangeHandler;
            isGameisGoingON = true;
            Invoke("GetScores", 1f);
            // StartCoroutine("ClockCountdown");
        }
        else
        {
            GameObject.Find("MyID").SetActive(false);
            GameObject.Find("OponentID").SetActive(false);
            GameObject.Find("TotalScore").SetActive(false);
            GameObject.Find("GameTimer").SetActive(false);
            GameObject.Find("Canvas").transform.Find("5BallsBoost").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("ColorBallBoost").gameObject.SetActive(true);

            GameObject.Find("Canvas").transform.Find("FireBallBoost").gameObject.SetActive(true);

        }
    }

    public void GetScores()
    {
        GameObject.Find("MyID").GetComponent<Text>().text += ColyseusClient.Instance.room.SessionId;
        if (ColyseusClient.Instance.room.State.player1.sessionId == ColyseusClient.Instance.room.SessionId)
        {
            GameObject.Find("OponentID").GetComponent<Text>().text += ColyseusClient.Instance.room.State.player2.sessionId;
            TimerSeconds = ColyseusClient.Instance.room.State.player1.secondsLeft;
            // GameObject.Find("TotalScore").GetComponent<Text>().text=ColyseusClient.Instance.room.State.player1.TotalScore.ToString();
            GameObject.Find("TotalScore").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player1.TotalScore.ToString();
            // print("This mustbe repeted as much as levels are loaded.......!"+ ColyseusClient.Instance.room.State.player1.TotalScore);
        }
        else
        {
            GameObject.Find("OponentID").GetComponent<Text>().text += ColyseusClient.Instance.room.State.player1.sessionId;
            TimerSeconds = ColyseusClient.Instance.room.State.player2.secondsLeft;
            //    GameObject.Find("TotalScore").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player2.TotalScore.ToString();
            GameObject.Find("TotalScore").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player2.TotalScore.ToString();
            // print("This mustbe repeted as much as levels are loaded.......!" + ColyseusClient.Instance.room.State.player2.TotalScore);
        }
       
    }
    private void GameStateChangeHandler(object sender, State e)
    {
        if (ColyseusClient.Instance.room.State.player1.sessionId == ColyseusClient.Instance.room.SessionId)
        {
           
            TimerSeconds = ColyseusClient.Instance.room.State.player1.secondsLeft;
           // Debug.Log("Timer Seconds=" + TimerSeconds+"-/-/-/"+ GameObject.Find("GameTimer"));
            if (GameObject.Find("GameTimer") != null)
                GameObject.Find("GameTimer").GetComponent<Text>().text = (TimerSeconds / 60) + ":" + (TimerSeconds - ((TimerSeconds / 60) * 60));
        }
        else
        {
           
            TimerSeconds = ColyseusClient.Instance.room.State.player2.secondsLeft;
           // Debug.Log("Timer Seconds=" + TimerSeconds + "-/-/-/" + GameObject.Find("GameTimer"));
            if(GameObject.Find("GameTimer")!=null)
            GameObject.Find("GameTimer").GetComponent<Text>().text = (TimerSeconds / 60) + ":" + (TimerSeconds - ((TimerSeconds / 60) * 60));

        }

        if (e.roomStatus== "GameFinished")
        {
            if (isGameisGoingON == true)
            {
                isGameisGoingON = false;
              //  mainscript.Instance.CallGameFinished();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator ClockCountdown()
    {

        while (TimerSeconds != 0)
        {
            GameObject.Find("GameTimer").GetComponent<Text>().text = (TimerSeconds / 60) + ":" + (TimerSeconds - ((TimerSeconds / 60) * 60));
            yield return new WaitForSeconds(1f);
            TimerSeconds--;
        }
    }
    public IEnumerator GameFinishedAction()
    {
        GameObject.Find("Canvas").transform.Find("GameFinished").gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        Application.LoadLevel("GameFinished");
    }

}
