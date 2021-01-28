using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static BotManager Instance;
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
        
    }


    public float[] GetBotLevelData(int levelNo)
    {
        if (levelNo == 0)
            levelNo = 1;
        
        TextAsset mapText = Resources.Load("Levels/" + levelNo) as TextAsset;
        if (mapText == null)
        {
            mapText = Resources.Load("Levels/" + levelNo) as TextAsset;
        }
      return   ProcessBotDataFromString(mapText.text);
        //return null;
    }



    float[] ProcessBotDataFromString(string mapText)
    {
        string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
       
        int mapLine = 0;
        int key = 0;
        float[] data = new float[13];
        foreach (string line in lines)
        {
            if (line.StartsWith("BotData"))
            {
                
                string botST = line.Replace("BotData", string.Empty).Trim();
                string[] botData = botST.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                // using format as BotData mintime-maxtime-minscore-maxscore-server_minTime-server_maxtime-server_minscore-server_maxscore-lelvl_hardness-level_moves
                for(int i=0;i<9;i++)
                data[i] = float.Parse(botData[i].ToString());
                
            }
            else if (line.StartsWith("LIMIT "))
            {
                string blocksString = line.Replace("LIMIT", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                data[9] = int.Parse(sizes[1]);

            }
            else if (line.StartsWith("STARS "))
            {
                string blocksString = line.Replace("STARS", string.Empty).Trim();
                string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                data[10] = int.Parse(blocksNumbers[0]);
                data[11] = int.Parse(blocksNumbers[1]);
                data[12] = int.Parse(blocksNumbers[2]);
            }

        }

        return data;
      
    }
}
