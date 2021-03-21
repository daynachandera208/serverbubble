using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using InitScriptName;
//main script responsible for creation of balls and level generation reads data from .txt files
public class creatorBall : MonoBehaviour
{
    public static creatorBall Instance;
    public GameObject ball_hd;
    public GameObject ball_ld;
    public GameObject bug_hd;
    public GameObject bug_ld;
    public GameObject thePrefab;
    GameObject eaterBallToActive;
    GameObject ball;
    GameObject bug;
    string[] ballsForCatapult = new string[11];
    string[] ballsForMatrix = new string[11];
    string[] bugs = new string[11];
    public static int columns = 11;
    public static int rows = 70;
    public static List<Vector2> grid = new List<Vector2>();
    int lastRow;
    float offsetStep = 0.33f;
    //private OTSpriteBatch spriteBatch = null;  
    GameObject Meshes;
    [HideInInspector]
    public List<GameObject> squares = new List<GameObject>();
    int[] map;
    private int maxCols;
    private int maxRows;
    private LIMIT limitType;

    // Use this for initialization
    void Start()//instantiate and load map data
    {
        Instance = this;
        ball = ball_hd;
        bug = bug_hd;
        thePrefab.transform.localScale = new Vector3( 0.67f, 0.58f, 1 );
        Meshes = GameObject.Find( "-Ball" );
        // LevelData.LoadDataFromXML( mainscript.Instance.currentLevel );
        LoadLevel();
        //LevelData.LoadDataFromLocal(mainscript.Instance.currentLevel);
        if( LevelData.mode == ModeGame.Vertical || LevelData.mode == ModeGame.Animals )
            MoveLevelUp();
        else
        {
            // GameObject.Find( "TopBorder" ).transform.position += Vector3.down * 3.5f;
            GameObject.Find( "TopBorder" ).transform.parent = null;
            GameObject.Find( "TopBorder" ).GetComponent<SpriteRenderer>().enabled = false;
            GameObject ob = GameObject.Find( "-Meshes" );
            ob.transform.position += Vector3.up * 2f;
            LockLevelRounded slider = ob.AddComponent<LockLevelRounded>();
            GamePlay.Instance.GameStatus = GameState.PreTutorial;
        }
        createMesh();
        LoadMap( LevelData.map,LevelData.chainMap , LevelData.doubleColorMap,LevelData.bGumMap);
        Camera.main.GetComponent<mainscript>().connectNearBallsGlobal();
        StartCoroutine( getBallsForMesh() );
        ShowBugs();
    }

    public void LoadLevel()
    {
        mainscript.Instance.currentLevel = PlayerPrefs.GetInt("OpenLevel");// TargetHolder.level;
        if (mainscript.Instance.currentLevel == 0)
            mainscript.Instance.currentLevel = 1;
        LoadDataFromLocal(mainscript.Instance.currentLevel);

    }


    public bool LoadDataFromLocal(int currentLevel)
    {
        //Read data from text file
        TextAsset mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        if (mapText == null)
        {
            mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        }
        ProcessGameDataFromString(mapText.text);
        return true;
    }

    void ProcessGameDataFromString(string mapText)// main logic for map reading from .txt file
    {


        //
        //bot data structure is as - BotData 5-15-0-500-72-125-4500-30000-1 (args= Min_Time_For_shoot-max_time_for_shoot-min_score_pre_shoot-max_scores_for_shoot-min_time_to_stay_in_level-max_time_tofinish_level-min_level_score-max_level_score-hardnessscaler)
        //Fireball 7-5-2 (args- "Moves after it spans"-"Probability to span from 0-100"-"total no of spans" ) for fireball
        //Bomb 5 - 5 - 4 (args- "Moves after it spans"-"Probability to span from 0-100"-"total no of spans" ) for bomb
        //Multicolor 3 - 5 - 7 (args- "Moves after it spans"-"Probability to span from 0-100"-"total no of spans" ) for multicolor ball
        //Structure of text file like this:
        //1st: Line start with "GM". This is game mode line (0-Move Limit, 1-Time Limit)
        //2nd: Line start with "LMT" is limit amount of play time (time of move or seconds depend on game mode)
        //Ex: LMT 20  mean player can move 20 times or 20 seconds, depend on game mode
        //3rd: Line start with "MNS" is missions line. This is amount ofScore/Block/Ring/... 
        //Ex: MNS 10000/24/0' mean user need get 1000 points, 24 block, and not need to get rings.
        //4th:Map lines: This is an array of square types.
        //First thing is split text to get all in arrays of text
        //1-for blue
        //2- green
        //3- red
        //4- violate
        //5- yello
        //6- random
        //7- chicken
        //8- eater
        //9 - frozen
        //10- stone
        //11 - rowremover
        // 12 - rowadder
        //0 - no ball
        //color-Chain-chain_no-chain_lvl
        //color-BGum-bgumno
        //color-DoubleColor-second_color
        string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        LevelData.colorsDict.Clear();
        int mapLine = 0;
        int key = 0;
        foreach (string line in lines)
        {
            if (line.StartsWith("MODE "))
            {
                string modeString = line.Replace("MODE", string.Empty).Trim();
                LevelData.mode = (ModeGame)int.Parse(modeString);
            }
            else if (line.StartsWith("SIZE "))
            {
                string blocksString = line.Replace("SIZE", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                maxCols = int.Parse(sizes[0]);
                maxRows = int.Parse(sizes[1]);
            }
            else if (line.StartsWith("LIMIT "))
            {
                string blocksString = line.Replace("LIMIT", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                limitType = (LIMIT)int.Parse(sizes[0]);
                LevelData.LimitAmount = int.Parse(sizes[1]);

            }
            else if (line.StartsWith("COLOR LIMIT "))
            {
                string blocksString = line.Replace("COLOR LIMIT", string.Empty).Trim();
                LevelData.colors = int.Parse(blocksString);
            }
            else if (line.StartsWith("STARS "))
            {
                string blocksString = line.Replace("STARS", string.Empty).Trim();
                string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                LevelData.star1 = int.Parse(blocksNumbers[0]);
                LevelData.star2 = int.Parse(blocksNumbers[1]);
                LevelData.star3 = int.Parse(blocksNumbers[2]);
            }
            else if (line.StartsWith("Multicolor"))
            {

                string multiColorST = line.Replace("Multicolor", string.Empty).Trim();
                string[] multiColorData = multiColorST.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                Camera.main.GetComponent<mainscript>().multiColorAfterMoves = int.Parse(multiColorData[0]);
                Camera.main.GetComponent<mainscript>().multiColorProbability = int.Parse(multiColorData[1]);
                Camera.main.GetComponent<mainscript>().moveCountForMultiColorBall = 1;
                Camera.main.GetComponent<mainscript>().totalmulticolorspans = int.Parse(multiColorData[2]);

            }
            else if (line.StartsWith("Bomb"))
            {
                string bombST = line.Replace("Bomb", string.Empty).Trim();
                string[] bombData = bombST.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                Camera.main.GetComponent<mainscript>().bombAfterMoves = int.Parse(bombData[0]);
                Camera.main.GetComponent<mainscript>().bombProbability = int.Parse(bombData[1]);
                Camera.main.GetComponent<mainscript>().moveCountForbomb = 1;
                Camera.main.GetComponent<mainscript>().totalbombspans = int.Parse(bombData[2]);
            }
            else if (line.StartsWith("Fireball"))
            {
                string fireballST = line.Replace("Fireball", string.Empty).Trim();
                string[] fireballData = fireballST.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                Camera.main.GetComponent<mainscript>().fireballAfterMoves = int.Parse(fireballData[0]);
                Camera.main.GetComponent<mainscript>().fireballProbability = int.Parse(fireballData[1]);
                Camera.main.GetComponent<mainscript>().moveCountForfireball = 1;
                Camera.main.GetComponent<mainscript>().totalfireballspans = int.Parse(fireballData[2]);
            }
            else if (line.StartsWith("BotData") )
            {
                //do  nothing here inverse it to get got data.
            }
            else
            {




                //Maps
                //Split lines again to get map numbers

                string[] st = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < st.Length; i++)
                {
                    if (st[i].Contains("Chain"))
                    {
                        string[] chainST = st[i].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                        int value = int.Parse(chainST[0].ToString());  // int.Parse(st[i][0].ToString());
                                                                       //value = value * 10;
                                                                       //value = value + int.Parse(st[i][1].ToString());
                        if (!LevelData.colorsDict.ContainsValue((BallColor)value) && value > 0 && value < (int)BallColor.random)
                        {
                            LevelData.colorsDict.Add(key, (BallColor)value);
                            key++;

                        }

                        LevelData.map[mapLine * maxCols + i] = int.Parse(chainST[0].ToString());
                        LevelData.chainMap[mapLine * maxCols + i] = new int[] { int.Parse(chainST[2].ToString()), int.Parse(chainST[3].ToString()) };
                        LevelData.bGumMap[mapLine * maxCols + i] = 0;
                        LevelData.doubleColorMap[mapLine * maxCols + i] = 0;
                        //Debug.Log(LevelData.chainMap[mapLine * maxCols + i][1]);
                    }
                    else if (st[i].Contains("DoubleColor"))
                    {// logic for double color bubble
                        string[] doubleColorST = st[i].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                        int value = int.Parse(doubleColorST[0].ToString());  // int.Parse(st[i][0].ToString());
                                                                             //value = value * 10;
                                                                             //value = value + int.Parse(st[i][1].ToString());
                        if (!LevelData.colorsDict.ContainsValue((BallColor)value) && value > 0 && value < (int)BallColor.random)
                        {
                            LevelData.colorsDict.Add(key, (BallColor)value);
                            key++;

                        }

                        LevelData.map[mapLine * maxCols + i] = int.Parse(doubleColorST[0].ToString());
                        LevelData.doubleColorMap[mapLine * maxCols + i] = int.Parse(doubleColorST[2].ToString());
                        LevelData.chainMap[mapLine * maxCols + i] = new int[] { 0, 0 };
                        LevelData.bGumMap[mapLine * maxCols + i] = 0;
                        //Debug.Log(LevelData.chainMap[mapLine * maxCols + i][1]);
                    }
                    else if (st[i].Contains("BGum"))
                    {
                        string[] bGumST = st[i].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                        int value = int.Parse(bGumST[0].ToString());  // int.Parse(st[i][0].ToString());
                                                                      //value = value * 10;
                                                                      //value = value + int.Parse(st[i][1].ToString());
                        if (!LevelData.colorsDict.ContainsValue((BallColor)value) && value > 0 && value < (int)BallColor.random)
                        {
                            LevelData.colorsDict.Add(key, (BallColor)value);
                            key++;

                        }

                        LevelData.map[mapLine * maxCols + i] = int.Parse(bGumST[0].ToString());
                        LevelData.chainMap[mapLine * maxCols + i] = new int[] { 0, 0 };
                        LevelData.doubleColorMap[mapLine * maxCols + i] = 0;
                        LevelData.bGumMap[mapLine * maxCols + i] = int.Parse(bGumST[2].ToString());
                    }
                    else
                    {
                        int value = int.Parse(st[i].ToString());  // int.Parse(st[i][0].ToString());
                                                                  //value = value * 10;
                                                                  //value = value + int.Parse(st[i][1].ToString());
                        if (!LevelData.colorsDict.ContainsValue((BallColor)value) && value > 0 && value < (int)BallColor.random)
                        {
                            LevelData.colorsDict.Add(key, (BallColor)value);
                            key++;

                        }

                        LevelData.map[mapLine * maxCols + i] = int.Parse(st[i].ToString());
                        LevelData.chainMap[mapLine * maxCols + i] = new int[] { 0, 0 };
                        LevelData.doubleColorMap[mapLine * maxCols + i] = 0;
                        LevelData.bGumMap[mapLine * maxCols + i] = 0;
                    }
                }
                mapLine++;
            }

        }
        if (Camera.main.GetComponent<mainscript>().multiColorProbability <= 0)
        {
            Camera.main.GetComponent<mainscript>().multiColorAfterMoves = 10;
            Camera.main.GetComponent<mainscript>().multiColorProbability = 0;
            Camera.main.GetComponent<mainscript>().moveCountForMultiColorBall = 1;
            Camera.main.GetComponent<mainscript>().totalmulticolorspans = 0;
        }
        if (Camera.main.GetComponent<mainscript>().bombProbability <= 0)
        {
            Camera.main.GetComponent<mainscript>().bombAfterMoves = 10;
            Camera.main.GetComponent<mainscript>().bombProbability = 0;
            Camera.main.GetComponent<mainscript>().moveCountForbomb = 1;
            Camera.main.GetComponent<mainscript>().totalbombspans = 0;
        }
        if (Camera.main.GetComponent<mainscript>().fireballProbability <= 0)
        {
            Camera.main.GetComponent<mainscript>().fireballAfterMoves = 10;
            Camera.main.GetComponent<mainscript>().fireballProbability = 0;
            Camera.main.GetComponent<mainscript>().moveCountForfireball = 1;
            Camera.main.GetComponent<mainscript>().totalfireballspans = 0;
        }
        //random colors
        if (LevelData.colorsDict.Count == 0)
        {
            //add constant colors 
            LevelData.colorsDict.Add(0, BallColor.yellow);
            LevelData.colorsDict.Add(1, BallColor.red);

            //add random colors
            List<BallColor> randomList = new List<BallColor>();
            randomList.Add(BallColor.blue);
            randomList.Add(BallColor.green);
            //if (LevelData.mode != ModeGame.Rounded)
                randomList.Add(BallColor.violet);
            for (int i = 0; i < LevelData.colors - 3; i++)
            {
                BallColor randCol = BallColor.yellow;
                while (LevelData.colorsDict.ContainsValue(randCol))
                {
                    randCol = randomList[UnityEngine.Random.    Range(0, randomList.Count)];
                }
                LevelData.colorsDict.Add(2 + i, randCol);

            }

        }

       /* foreach (int i in LevelData.bGumMap)
        {
            if (!LevelData.allBGumNo.Contains(i)) { LevelData.allBGumNo.Add(i); }
        }*/

    }

    public void LoadMap( int[] pMap , int[][] cMap, int[] dCMap,int[] bGumMap)
    {
        map = pMap;
        int key = -1;
        int roww = 0;
        for( int i = 0; i < rows; i++ )
        {
            for( int j = 0; j < columns; j++ )
            {
                int mapValue = map[i * columns + j];
                if( mapValue > 0  )
                {
                    roww = i;
                    if (LevelData.mode == ModeGame.Rounded) roww = i +4;
                                      // Debug.Log("++chain map"+cMap[10][0].ToString());
                    createBall(GetSquare(roww, j).transform.position, (BallColor)mapValue, false, false, false, false, i, cMap[i * columns + j][0], cMap[i * columns + j][1], dCMap[i * columns + j],bGumMap[i * columns + j]);
                }
                else if( mapValue == 0 && LevelData.mode == ModeGame.Vertical && i == 0 )
                {
                    Instantiate( Resources.Load( "Prefabs/TargetStar" ), GetSquare( i, j ).transform.position, Quaternion.identity );
                }
            }
        }
    }

    private void MoveLevelUp()
    {
        StartCoroutine( MoveUpDownCor() );
    }

    IEnumerator MoveUpDownCor( bool inGameCheck = false )
    {
        yield return new WaitForSeconds( 0.1f );
        if( !inGameCheck )
            GamePlay.Instance.GameStatus = GameState.BlockedGame;
        bool up = false;
        List<float> table = new List<float>();
        float lineY = -1.3f;//GameObject.Find( "GameOverBorder" ).transform.position.y;
        Transform bubbles = GameObject.Find( "-Ball" ).transform;
        int i = 0;
        foreach( Transform item in bubbles )
        {
            if( !inGameCheck )
            {
                if( item.position.y < lineY )
                {
                    table.Add( item.position.y );
                }
            }
            else if( !item.GetComponent<ball>().Destroyed )
            {
                if( item.position.y > lineY && mainscript.Instance.TopBorder.transform.position.y > 5f )
                {
                    table.Add( item.position.y );
                }
                else if( item.position.y < lineY + 1f )
                {
                    table.Add( item.position.y );
                    up = true;
                }
            }
            i++;
        }


        if( table.Count > 0 )
        {
            if( up ) AddMesh();

            float targetY = 0;
            table.Sort();
            if( !inGameCheck ) targetY = lineY - table[0] + 2.5f;
            else targetY = lineY - table[0] + 1.5f;
            GameObject Meshes = GameObject.Find( "-Meshes" );
            Vector3 targetPos = Meshes.transform.position + Vector3.up * targetY;
            float startTime = Time.time;
            Vector3 startPos = Meshes.transform.position;
            float speed = 0.5f;
            float distCovered = 0;
            while( distCovered < 1 )
            {
           //                     print( table.Count );
                speed += Time.deltaTime / 1.5f;
                distCovered = ( Time.time - startTime ) / speed;
                Meshes.transform.position = Vector3.Lerp( startPos, targetPos, distCovered );
                yield return new WaitForEndOfFrame();
                if( startPos.y > targetPos.y )
                {
                    if( mainscript.Instance.TopBorder.transform.position.y <= 5 && inGameCheck ) break;
                }
            }
        }

        //        Debug.Log("lift finished");
        if( GamePlay.Instance.GameStatus == GameState.BlockedGame )
            GamePlay.Instance.GameStatus = GameState.PreTutorial;
        else if( GamePlay.Instance.GameStatus != GameState.GameOver && GamePlay.Instance.GameStatus != GameState.Win )
            GamePlay.Instance.GameStatus = GameState.Playing;


    }

    public void MoveLevelDown()
    {
        StartCoroutine( MoveUpDownCor( true ) );
    }

    private bool BubbleBelowLine()
    {
        throw new System.NotImplementedException();
    }

    void ShowBugs()
    {
        int effset = 1;
        for( int i = 0; i < 2; i++ )
        {
            effset *= -1;
            CreateBug( new Vector3( 10 * effset, -2, 0 ) );

        }

    }

    public void CreateBug( Vector3 pos, int value = 1 )
    {
        Transform spiders = GameObject.Find( "Spiders" ).transform;
        List<Bug> listFreePlaces = new List<Bug>();
        foreach( Transform item in spiders )
        {
            if( item.childCount > 0 ) listFreePlaces.Add( item.GetChild( 0 ).GetComponent<Bug>() );
        }

        if( listFreePlaces.Count < 6 )
            Instantiate( bug, pos, Quaternion.identity );
        else
        {
            listFreePlaces.Clear();
            foreach( Transform item in spiders )
            {
                if( item.childCount > 0 )
                {
                    if( item.GetChild( 0 ).GetComponent<Bug>().color == 0 ) listFreePlaces.Add( item.GetChild( 0 ).GetComponent<Bug>() );
                }
            }
            if( listFreePlaces.Count > 0 )
                listFreePlaces[UnityEngine.Random.Range( 0, listFreePlaces.Count )].ChangeColor( 1 );
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator getBallsForMesh()
    {
        GameObject[] meshes = GameObject.FindGameObjectsWithTag( "Mesh" );
        foreach( GameObject obj1 in meshes )
        {
            Collider2D[] fixedBalls = Physics2D.OverlapCircleAll( obj1.transform.position, 0.2f, 1 << 9 );  //balls
            foreach( Collider2D obj in fixedBalls )
            {
                obj1.GetComponent<Grid>().Busy = obj.gameObject;
                //	obj.GetComponent<bouncer>().offset = obj1.GetComponent<Grid>().offset;
            }
        }
        yield return new WaitForSeconds( 0.5f );
    }

    public void EnableGridColliders()
    {
        foreach( GameObject item in squares )
        {
            item.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
    public void OffGridColliders()
    {
        foreach( GameObject item in squares )
        {
            item.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void createRow( int j )
    {
        float offset = 0;
        GameObject gm = GameObject.Find( "Creator" );
        for( int i = 0; i < columns; i++ )
        {
            if( j % 2 == 0 ) offset = 0; else offset = offsetStep;
            Vector3 v = new Vector3( transform.position.x + i * thePrefab.transform.localScale.x + offset, transform.position.y - j * thePrefab.transform.localScale.y, transform.position.z );
            createBall( v );
        }
    }
    public void AddEater() {

        this.eaterBallToActive.GetComponent<bouncer>().isEaterBubble = true;
        // b.GetComponent<BoxCollider2D>().enabled = false;
        // gatting circlecollider2d which is disabled
        Component[] allComponents = this.eaterBallToActive.GetComponents(typeof(CircleCollider2D));
        foreach (CircleCollider2D currentComponent in allComponents)
        {

            if (currentComponent.isTrigger)
            {
                currentComponent.enabled = true;
               // Debug.Log("Adding Eater");
            }

        }
    }
    public GameObject createBall( Vector3 vec, BallColor color = BallColor.random, bool newball = false, bool multiColor = false, bool bomb = false, bool fireball = false, int row = 1 ,int chainNo=0 ,int chainLevel=0 ,int doubleColor=0,int bGumNo=0)
    {
        GameObject b = null;
        List<BallColor> colors = new List<BallColor>();

        for( int i = 1; i < System.Enum.GetValues( typeof( BallColor ) ).Length; i++ )
        {
            colors.Add( (BallColor)i );
        }

        if( color == BallColor.random )
            color = (BallColor)LevelData.colorsDict[UnityEngine.Random.Range( 0, LevelData.colorsDict.Count )];
		if( newball && mainscript.colorsDict.Count > 0 )
        {
            if( GamePlay.Instance.GameStatus == GameState.Playing )
            {
                mainscript.Instance.GetColorsInGame();
                color = (BallColor)mainscript.colorsDict[UnityEngine.Random.Range( 0, mainscript.colorsDict.Count )];
            }
            else
                color = (BallColor)LevelData.colorsDict[UnityEngine.Random.Range( 0, LevelData.colorsDict.Count )];

        }



        b = Instantiate( ball, transform.position, transform.rotation ) as GameObject;
        b.transform.position = new Vector3( vec.x, vec.y, ball.transform.position.z );
        // b.transform.Rotate( new Vector3( 0f, 180f, 0f ) );
        if (b.tag == "stone")
        {
            
        }
        // logic for adding chain levels and Double Color
        b.GetComponent<bouncer>().chainNo = chainNo;
        b.GetComponent<bouncer>().chainLevel = chainLevel;
       // b.GetComponent<bouncer>().DoubleColor = doubleColor;
        b.GetComponent<bouncer>().bGumNo = bGumNo;
        if (bGumNo!=0)
        {
            b.transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = true;
        }
        if (multiColor)
        {
            b.GetComponent<bouncer>().isMultiColor = multiColor;
            mainscript.Instance.totalmulticolorspans--;
            //GamePlay.Instance.AddMultiColorBall(b.gameObject);
            // Debug.Log("Created From Createro ball script");
        }
        else if (fireball)
        {
            InitScript.Instance.BoostActivated = true;
         StartCoroutine(   InitScript.Instance.SpawnBoost((BoostType)System.Enum.Parse(typeof(BoostType), "FireBallBoost")));
           // b.GetComponent<ball>().SetBoost((BoostType)System.Enum.Parse(typeof(BoostType), "FireBallBoost"));
            print("set boost");
            mainscript.Instance.totalfireballspans--;
        }
        else if (bomb)
        {
            InitScript.Instance.BoostActivated = true;
           StartCoroutine( InitScript.Instance.SpawnBoost((BoostType)System.Enum.Parse(typeof(BoostType), "ColorBallBoost")));
           // b.GetComponent<ball>().SetBoost((BoostType)System.Enum.Parse(typeof(BoostType), "ColorBallBoost"));
            print("set boost");
            mainscript.Instance.totalbombspans--;
        }
        b.GetComponent<bouncer>().doesChangeColor = false;
       
        b.GetComponent<ColorBallScript>().SetColor(color);//,chainLevel);
        if (b.GetComponent<bouncer>().isMultiColor) {
            b.transform.GetChild(3).GetComponent<SpriteRenderer>().enabled = true;

        }
        if (doubleColor != 0)
        {
            b.transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = true;
            switch (doubleColor)
            {
                case 1:
                    b.transform.GetChild(2).GetComponent<SpriteRenderer>().color = Color.blue;
                    b.GetComponent<bouncer>().ballcolors.Add((BallColor)doubleColor);
                    break;
                case 2:
                    b.transform.GetChild(2).GetComponent<SpriteRenderer>().color = Color.green;
                    b.GetComponent<bouncer>().ballcolors.Add((BallColor)doubleColor);
                    break;
                case 3:
                    b.transform.GetChild(2).GetComponent<SpriteRenderer>().color = Color.red;
                    b.GetComponent<bouncer>().ballcolors.Add((BallColor)doubleColor);
                    break;
                case 4:
                    Color col;
                    ColorUtility.TryParseHtmlString("#F648DC", out col);
                    b.transform.GetChild(2).GetComponent<SpriteRenderer>().color = col;
                    b.GetComponent<bouncer>().ballcolors.Add((BallColor)doubleColor);
                    break;
                case 5:
                    b.transform.GetChild(2).GetComponent<SpriteRenderer>().color = Color.yellow;
                    b.GetComponent<bouncer>().ballcolors.Add((BallColor)doubleColor);
                    break;


            }
        }
        if (chainLevel == 2)
        { b.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            b.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;
        }
        if (chainLevel == 1)
            b.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        b.transform.parent = Meshes.transform;
        b.tag = "" + color;
        if (b.tag == "frozen")//Giving ice tint to iceclube bubbles
        {
            //Color iceColor;
           // b.GetComponent<SpriteRenderer>().color= new Color(144f,205f,253f);
           // ColorUtility.TryParseHtmlString("#90CDFD", out iceColor);
           // b.GetComponent<SpriteRenderer>().color = iceColor;
            
            
        }
        if(b.tag == "eater")
        {
            this.eaterBallToActive = b;
            //Invoke("AddEater",0.2f);
            AddEater();
            /*CircleCollider2D bubbleColliderForEater = b.GetComponent<CircleCollider2D>();
            if (bubbleColliderForEater.isTrigger) {
                bubbleColliderForEater.enabled = true;
            }
            */
        }
       
        /*if (chainLevel==1)
        {
            b.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }else if (chainLevel == 2)
        {
            b.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }*/
        GameObject[] fixedBalls = GameObject.FindObjectsOfType( typeof( GameObject ) ) as GameObject[];
        b.name = b.name + fixedBalls.Length.ToString();
        if( newball )
        {

            b.gameObject.layer = 17;
            b.transform.parent = Camera.main.transform;
            Rigidbody2D rig = b.AddComponent<Rigidbody2D>();
            // b.collider2D.isTrigger = false;
      //      rig.fixedAngle = true;
            b.GetComponent<CircleCollider2D>().enabled = false;
            rig.gravityScale = 0;
            if( GamePlay.Instance.GameStatus == GameState.Playing )
                b.GetComponent<Animation>().Play();
        }
        else
        {
            b.GetComponent<ball>().enabled = false;
            if( LevelData.mode == ModeGame.Vertical && row == 0 )
                b.GetComponent<ball>().isTarget = true;
            b.GetComponent<BoxCollider2D>().offset = Vector2.zero;
            b.GetComponent<BoxCollider2D>().size = new Vector2( 0.5f, 0.5f );
            //Destroy( b.rigidbody2D );
            //b.rigidbody2D.isKinematic = true;
            //Destroy( b.GetComponent < BoxCollider2D>() );
            //b.AddComponent<BoxCollider2D>();
            //b.GetComponent<BoxCollider2D>().enabled = false;
            //b.GetComponent<BoxCollider2D>().enabled = true;
        }
        return b.gameObject;
    }

    void CreateEmptyBall( Vector3 vec )
    {
        GameObject b2 = Instantiate( ball, transform.position, transform.rotation ) as GameObject;
        b2.transform.position = new Vector3( vec.x, vec.y, ball.transform.position.z );
        // b.transform.Rotate( new Vector3( 0f, 180f, 0f ) );
        b2.GetComponent<ColorBallScript>().SetColor( 11 );
        b2.transform.parent = Meshes.transform;
        b2.tag = "empty";
        b2.GetComponent<ball>().enabled = false;
        b2.gameObject.layer = 9;
        b2.GetComponent<Animation>().Play( "cat_idle" );
        b2.GetComponent<SpriteRenderer>().sortingOrder = 20;
        b2.GetComponent<BoxCollider2D>().offset = Vector2.zero;
        b2.GetComponent<BoxCollider2D>().size = new Vector2( 0.5f, 0.5f );

    }

 
    int setColorFrame( string sTag )
    {
        int frame = 0;
        //		if(Camera.main.GetComponent<mainscript>().hd){
        if( sTag == "Orange" ) frame = 7;
        else if( sTag == "Red" ) frame = 3;
        else if( sTag == "Yellow" ) frame = 1;
        else if( sTag == "Rainbow" ) frame = 4;
        else if( sTag == "Pearl" ) frame = 6;
        else if( sTag == "Blue" ) frame = 11;
        else if( sTag == "DarkBlue" ) frame = 8;
        else if( sTag == "Green" ) frame = 10;
        else if( sTag == "Pink" ) frame = 5;
        else if( sTag == "Violet" ) frame = 2;
        else if( sTag == "Brown" ) frame = 9;
        else if( sTag == "Gray" ) frame = 12;
        return frame;
    }

    int setColorFrameBug( string sTag )
    {
        int frame = 0;
        if( sTag == "Orange" ) frame = 5;
        else if( sTag == "Red" ) frame = 3;
        else if( sTag == "Yellow" ) frame = 1;
        else if( sTag == "Rainbow" ) frame = 4;
        else if( sTag == "Pearl" ) frame = 10;
        else if( sTag == "Blue" ) frame = 10;
        else if( sTag == "DarkBlue" ) frame = 8;
        else if( sTag == "Green" ) frame = 7;
        else if( sTag == "Pink" ) frame = 4;
        else if( sTag == "Violet" ) frame = 2;
        else if( sTag == "Brown" ) frame = 9;
        else if( sTag == "Gray" ) frame = 6;
        return frame;
    }

    public string getRandomColorTag()
    {
        int color = 0;
        string sTag = "";
        if( mainscript.stage < 6 )
            color = UnityEngine.Random.Range( 0, 4 + mainscript.stage - 1 );
        else
            color = UnityEngine.Random.Range( 0, 4 + 6 );

        if( color == 0 ) sTag = "Orange";
        else if( color == 1 ) sTag = "Red";
        else if( color == 2 ) sTag = "Yellow";
        else if( color == 3 ) sTag = "Rainbow";
        else if( color == 4 ) sTag = "Blue";
        else if( color == 5 ) sTag = "Green";
        else if( color == 6 ) sTag = "Pink";
        else if( color == 7 ) sTag = "Violet";
        else if( color == 8 ) sTag = "Brown";
        else if( color == 9 ) sTag = "Gray";
        return sTag;
    }

    public void createMesh()
    {

        GameObject Meshes = GameObject.Find( "-Meshes" );
        float offset = 0;

        for( int j = 0; j < rows + 1; j++ )
        {
            for( int i = 0; i < columns; i++ )
            {
                if( j % 2 == 0 ) offset = 0; else offset = offsetStep;
                GameObject b = Instantiate( thePrefab, transform.position, transform.rotation ) as GameObject;
                Vector3 v = new Vector3( transform.position.x + i * b.transform.localScale.x + offset, transform.position.y - j * b.transform.localScale.y, transform.position.z );
                b.transform.parent = Meshes.transform;
                b.transform.localPosition = v;
                GameObject[] fixedBalls = GameObject.FindGameObjectsWithTag( "Mesh" );
                b.name = b.name + fixedBalls.Length.ToString();
                b.GetComponent<Grid>().offset = offset;
                squares.Add( b );
                lastRow = j;
            }
        }
        creatorBall.Instance.OffGridColliders();

    }

    public void AddMesh()
    {
        GameObject Meshes = GameObject.Find( "-Meshes" );
        float offset = 0;
        int j = lastRow + 1;
        for( int i = 0; i < columns; i++ )
        {
            if( j % 2 == 0 ) offset = 0; else offset = offsetStep;
            GameObject b = Instantiate( thePrefab, transform.position, transform.rotation ) as GameObject;
            Vector3 v = new Vector3( transform.position.x + i * b.transform.localScale.x + offset, transform.position.y - j * b.transform.localScale.y, transform.position.z );
            b.transform.parent = Meshes.transform;
            b.transform.position = v;
            GameObject[] fixedBalls = GameObject.FindGameObjectsWithTag( "Mesh" );
            b.name = b.name + fixedBalls.Length.ToString();
            b.GetComponent<Grid>().offset = offset;
            squares.Add( b );
        }
        lastRow = j;

    }

    public GameObject GetSquare( int row, int col )
    {
        return squares[row * columns + col];
    }


}
