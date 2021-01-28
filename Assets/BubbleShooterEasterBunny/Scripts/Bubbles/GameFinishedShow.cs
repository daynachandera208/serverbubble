using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFinishedShow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("ShowDetails", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void  ShowDetails()
    {
        if (ColyseusClient.Instance.room.State.player1.sessionId == ColyseusClient.Instance.room.SessionId)
        {
            if(ColyseusClient.Instance.room.State.player1.secondsLeft!=0 && ColyseusClient.Instance.room.State.winnerPlayerIndex==1)
                GameObject.Find("TitleText").GetComponent<Text>().text = "Game Won (opponrnt Left The Game)";
            else if(ColyseusClient.Instance.room.State.player1.secondsLeft != 0 && ColyseusClient.Instance.room.State.winnerPlayerIndex == 2)
                GameObject.Find("TitleText").GetComponent<Text>().text = "Game Lost (You Left The Game)";
            else if(ColyseusClient.Instance.room.State.player1.secondsLeft == 0 && ColyseusClient.Instance.room.State.winnerPlayerIndex == 2)
                GameObject.Find("TitleText").GetComponent<Text>().text = "Game Lost";
            else if(ColyseusClient.Instance.room.State.player1.secondsLeft == 0 && ColyseusClient.Instance.room.State.winnerPlayerIndex == 1)
                GameObject.Find("TitleText").GetComponent<Text>().text = "Game Won";

            GameObject.Find("OwnStatus").GetComponent<Text>().text += ColyseusClient.Instance.room.State.player1.sessionId + ")";
            GameObject.Find("MyScores").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player1.TotalScore.ToString();
            GameObject.Find("MyStars").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player1.TotalStars.ToString();
            GameObject.Find("MyLvlFinished").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player1.FinishedLevels.ToString();
            GameObject.Find("MyLvlLost").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player1.totalGameOvers.ToString();
            if (ColyseusClient.Instance.room.State.isBot)
            {
                GameObject.Find("OpponentStatus").GetComponent<Text>().text += ColyseusClient.Instance.room.State.player2.sessionId + ")";
                GameObject.Find("OpoScores").GetComponent<Text>().text = (ColyseusClient.Instance.room.State.player2.TotalScore+ ColyseusClient.Instance.room.State.player2.currentLevelScore).ToString();
                GameObject.Find("OpoStars").GetComponent<Text>().text = (ColyseusClient.Instance.room.State.player2.TotalStars+ ColyseusClient.Instance.room.State.player2.currentStars).ToString();
                GameObject.Find("OpoLvlFinished").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player2.FinishedLevels.ToString();
                GameObject.Find("OpoLvlLost").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player2.totalGameOvers.ToString();
            }
            else
            {

                GameObject.Find("OpponentStatus").GetComponent<Text>().text += ColyseusClient.Instance.room.State.player2.sessionId + ")";
                GameObject.Find("OpoScores").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player2.TotalScore.ToString();
                GameObject.Find("OpoStars").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player2.TotalStars.ToString();
                GameObject.Find("OpoLvlFinished").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player2.FinishedLevels.ToString();
                GameObject.Find("OpoLvlLost").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player2.totalGameOvers.ToString();
            }
        }
        else
        {
            if (ColyseusClient.Instance.room.State.player2.secondsLeft != 0 && ColyseusClient.Instance.room.State.winnerPlayerIndex == 2)
                GameObject.Find("TitleText").GetComponent<Text>().text = "Game Won (opponrnt Left The Game)";
            else if (ColyseusClient.Instance.room.State.player2.secondsLeft != 0 && ColyseusClient.Instance.room.State.winnerPlayerIndex == 1)
                GameObject.Find("TitleText").GetComponent<Text>().text = "Game Lost (You Left The Game)";
            else if (ColyseusClient.Instance.room.State.player2.secondsLeft == 0 && ColyseusClient.Instance.room.State.winnerPlayerIndex == 1)
                GameObject.Find("TitleText").GetComponent<Text>().text = "Game Lost";
            else if (ColyseusClient.Instance.room.State.player2.secondsLeft == 0 && ColyseusClient.Instance.room.State.winnerPlayerIndex == 2)
                GameObject.Find("TitleText").GetComponent<Text>().text = "Game Won";


            GameObject.Find("OwnStatus").GetComponent<Text>().text += ColyseusClient.Instance.room.State.player2.sessionId + ")";
            GameObject.Find("MyScores").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player2.TotalScore.ToString();
            GameObject.Find("MyStars").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player2.TotalStars.ToString();
            GameObject.Find("MyLvlFinished").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player2.FinishedLevels.ToString();
            GameObject.Find("MyLvlLost").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player2.totalGameOvers.ToString();

            if (ColyseusClient.Instance.room.State.isBot)
            {
                GameObject.Find("OpponentStatus").GetComponent<Text>().text += ColyseusClient.Instance.room.State.player1.sessionId + ")";
                GameObject.Find("OpoScores").GetComponent<Text>().text = (ColyseusClient.Instance.room.State.player1.TotalScore + ColyseusClient.Instance.room.State.player1.currentLevelScore).ToString();
                GameObject.Find("OpoStars").GetComponent<Text>().text = (ColyseusClient.Instance.room.State.player1.TotalStars + ColyseusClient.Instance.room.State.player1.currentStars).ToString();
                GameObject.Find("OpoLvlFinished").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player1.FinishedLevels.ToString();
                GameObject.Find("OpoLvlLost").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player1.totalGameOvers.ToString();
            }
            else
            {
                GameObject.Find("OpponentStatus").GetComponent<Text>().text += ColyseusClient.Instance.room.State.player1.sessionId + ")";
                GameObject.Find("OpoScores").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player1.TotalScore.ToString();
                GameObject.Find("OpoStars").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player1.TotalStars.ToString();
                GameObject.Find("OpoLvlFinished").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player1.FinishedLevels.ToString();
                GameObject.Find("OpoLvlLost").GetComponent<Text>().text = ColyseusClient.Instance.room.State.player1.totalGameOvers.ToString();

            }
        }
        ColyseusClient.Instance.LeaveRoom();
    }
    public void ContinueOnClick()
    {
        //logic to leave server...-\|/-\|/-
        ColyseusClient.Instance.LeaveRoom();
        Application.LoadLevel("menu");
    }
    
}
