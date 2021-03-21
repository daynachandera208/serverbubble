using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectLevels : MonoBehaviour
{
    int latestFile;
    public GameObject levelPrefab;
    public Vector3 startPosition;
    public Vector2 offset;
    public int countInRow = 4;
    public int countInColumn = 4;
    public Button backButton;
    public Button nextButton;
    int firstShownLevelInGrid;
    // Use this for initialization
    void Start()
    {
        GenerateGrid(0);
    }


    void GenerateGrid(int genfrom = 0)
    {
        int l = 0;
        
        ClearLevels();
        firstShownLevelInGrid = genfrom;
        latestFile = GetLastLevel();
        l = genfrom; 
        
        for(int i=0;i<10;i++)
        {
            GameObject level = gameObject.transform.GetChild(i).gameObject;
            level.GetComponent<Level>().number = l+1;
            level.GetComponent<Level>().GetData();
            l++;
        }
        if (genfrom == 0) backButton.gameObject.SetActive(false);
        else if (genfrom > 0) backButton.gameObject.SetActive(true);
        if (l + 1 >= latestFile) nextButton.gameObject.SetActive(false);
        else nextButton.gameObject.SetActive(true);

    }

    void ClearLevels()
    {
        foreach (Transform item in transform)
        {
            item.GetChild(4).gameObject.SetActive(false);
            item.GetChild(5).gameObject.SetActive(false);
            item.GetChild(6).gameObject.SetActive(false);
            item.GetChild(7).gameObject.SetActive(false);
        }
    }

    public void Next()
    {
        print("next");
        GenerateGrid(firstShownLevelInGrid +10);
    }

    public void Back()
    {
        GenerateGrid(firstShownLevelInGrid - 10);
        print("back");

    }

    int GetLastLevel()
    {
        TextAsset mapText = null;
        for (int i = 1; i < 50000; i++)
        {
            mapText = Resources.Load("Levels/" + i) as TextAsset;
            if (mapText == null)
            {
                return i - 1;
            }
        }
        return 0;
    }
}
