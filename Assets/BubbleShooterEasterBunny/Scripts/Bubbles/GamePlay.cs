using UnityEngine;
using System.Collections;
using InitScriptName;
using System.Collections.Generic;

public enum GameState
{
    Playing,
    Highscore,
    GameOver,
    Pause,
    Win,
    WaitForPopup,
    WaitAfterClose,
    BlockedGame,
    Tutorial,
    PreTutorial,
    WaitForChicken
}


public class GamePlay : MonoBehaviour {
    public static GamePlay Instance;
    public List<GameObject> shiftball = new List<GameObject>();
    public ArrayList allRowsTomoveDown = new ArrayList();
    public List<GameObject> shiftMesh = new List<GameObject>();
    private GameState gameStatus;
    bool winStarted;
    public string[] avoidToChangeInMulticolor;
    public GameObject[] allMultiColorBalls;
    public  GameObject[] candidatesForSpreadingBGum = new GameObject[100];
    public bool doesBubbleSpread;
    public int totalMultiColorBall;
    public GameState GameStatus
    {
        get { return GamePlay.Instance.gameStatus; }
        set 
        {
            if( GamePlay.Instance.gameStatus != value )
            {
                if( value == GameState.Win )
                {
                    if( !winStarted )
                        StartCoroutine( WinAction ());
                }
                else if( value == GameState.GameOver )
                {
                    StartCoroutine( LoseAction() );
                }
                else if( value == GameState.Tutorial && gameStatus != GameState.Playing )
                {
                    value = GameState.Playing;
                    gameStatus = value;
                  //  ShowTutorial();
                }
                else if( value == GameState.PreTutorial && gameStatus != GameState.Playing )
                {
                    ShowPreTutorial();
                }

            }
            if( value == GameState.WaitAfterClose )
                StartCoroutine( WaitAfterClose() );

            if( value == GameState.Tutorial )
            {
                if( gameStatus != GameState.Playing )
                    GamePlay.Instance.gameStatus = value;

            }
          
                    GamePlay.Instance.gameStatus = value;

        }
    }

	// Use this for initialization
	void Start () {
        Instance = this;
        Instance.allMultiColorBalls = new GameObject[10];
       
        Instance.totalMultiColorBall = 0;
        

    }
    
    public void AddMultiColorBall(GameObject gm) {
        for (int i = 0; i < 10; i++) {
            if (Instance.allMultiColorBalls[i] == null) {
                Instance.allMultiColorBalls[i] = gm;
                Instance.totalMultiColorBall += 1;
            }
        }
       
        
        Instance.totalMultiColorBall += 1;

    }
    public  void RemoveMultiColorBall(string name) {
        for(int i=0; i<10;i++)
        { if (Instance.allMultiColorBalls[i].name == name) {
                Instance.allMultiColorBalls[i].name = null;
                Instance.totalMultiColorBall -= 1;
            }
        }
    }
    public void ChangeMultiColorBallColor() {

        Debug.Log("Call To Change Color");
        for (int i = 0; i < 10 && Instance.allMultiColorBalls[i]!=null && Instance.allMultiColorBalls[i].GetComponent<bouncer>().doesChangeColor; i++)
        {

            Debug.Log("Processing for ball:"+ Instance.allMultiColorBalls[i].name);

            Instance.allMultiColorBalls[i].GetComponent<bouncer>().colorForNearsetMulticolor.Clear();


            int layerMask = 1 << LayerMask.NameToLayer("Ball");
            Collider2D[] fixedBalls = Physics2D.OverlapCircleAll( Instance.allMultiColorBalls[i].GetComponent<Transform>().position, 0.5f, layerMask);


            //colorForNearsetMulticolor.Add(gameObject);
            Vector3 distEtalon = Instance.allMultiColorBalls[i].GetComponent<Transform>().localScale;
            //GameObject[] meshes = GameObject.FindGameObjectsWithTag( tag );
            foreach (Collider2D obj in fixedBalls)
            {
                /*if ( obj.gameObject.tag==tag)
                {
                    Debug.Log(tag+"==doyble="+ obj.gameObject.GetComponent<bouncer>().DoubleColor+"------------obj value"+ obj.gameObject.tag);
                    float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                    if (distTemp <= 0.9f && distTemp > 0)
                    {
                        Debug.Log(tag + "==from inside doyble=" + obj.gameObject.GetComponent<bouncer>().DoubleColor + "------------obj value" + obj.gameObject.tag);
                        b.Add(obj);
                        obj.GetComponent<bouncer>().checkNextNearestColor(b, counter);
                    }
                }
               else */

                //  Debug.Log(tag + "==doyble=" + obj.gameObject.GetComponent<bouncer>().DoubleColor + "------------obj value" + obj.gameObject.tag);
                // Debug.Log(tag + "==from inside bouncer" + obj.gameObject.GetComponent<bouncer>().DoubleColor + "------------obj value" + obj.gameObject.tag);
                float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                if (distTemp <= 0.9f && distTemp > 0)
                {
                    //Debug.Log(tag + "==from inside bouncer" + obj.gameObject.GetComponent<bouncer>().DoubleColor + "------------obj value" + obj.gameObject.tag);
                    if (!Instance.allMultiColorBalls[i].GetComponent<bouncer>().colorForNearsetMulticolor.Contains(obj.gameObject.tag) && !System.Array.Exists(GamePlay.Instance.avoidToChangeInMulticolor, element => element == obj.tag))
                    {
                        //  Debug.Log(tag + "==from inside bouncer" + obj.gameObject.GetComponent<bouncer>().DoubleColor + "------------obj value" + obj.gameObject.tag+"000"+ (BallColor)System.Enum.Parse(typeof(BallColor), obj.gameObject.tag));
                        gameObject.GetComponent<bouncer>().colorForNearsetMulticolor.Add((BallColor)System.Enum.Parse(typeof(BallColor), obj.gameObject.tag));
                    }
                }


            }

            Instance.allMultiColorBalls[i].GetComponent<bouncer>().noOfColorForMultiColor = Instance.allMultiColorBalls[i].GetComponent<bouncer>().colorForNearsetMulticolor.Count;

            
                foreach (BallColor b in Instance.allMultiColorBalls[i].GetComponent<bouncer>().colorForNearsetMulticolor)
                {
                    Debug.Log("------/" + b);
                }
            //       Debug.Log( colorForNearsetMulticolor[multicolorChangeTracker].ToString());
            //      Debug.Log(gameObject.tag);
            Instance.allMultiColorBalls[i].GetComponent<ColorBallScript>().SetColor((BallColor)System.Enum.Parse(typeof(BallColor), Instance.allMultiColorBalls[i].GetComponent<bouncer>().colorForNearsetMulticolor[Instance.allMultiColorBalls[i].GetComponent<bouncer>().multicolorChangeTracker].ToString()));
            // if (gameObject.GetComponent<bouncer>().doesExecuteChangeColor)
            //{
            Instance.allMultiColorBalls[i].GetComponent<bouncer>().multicolorChangeTracker += 1;
                //     gameObject.GetComponent<bouncer>().doesExecuteChangeColor = false;
                // }
                // else
                //    gameObject.GetComponent<bouncer>().doesExecuteChangeColor = true;
                if (Instance.allMultiColorBalls[i].GetComponent<bouncer>().multicolorChangeTracker == Instance.allMultiColorBalls[i].GetComponent<bouncer>().noOfColorForMultiColor)
                {
                Instance.allMultiColorBalls[i].GetComponent<bouncer>().multicolorChangeTracker = 0;
                }


                Debug.Log("Does Single" + " ball:" + gameObject.name);
                //gameObject.GetComponent<bouncer>().doesExecuteChangeColor = false;
            
            //  else
            // gameObject.GetComponent<bouncer>().doesExecuteChangeColor = true;


        }

    }

  /*  void ChangeAllMultiColorBallColor() {

        for (int i = 0; i < 10; i++)
        {
            GameObject currentBall = Instance.allMultiColorBalls[i][0];

            int layerMask = 1 << LayerMask.NameToLayer("Ball");
            Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(currentBall.GetComponent<Transform>().position, 0.5f, layerMask);


            //colorForNearsetMulticolor.Add(gameObject);
            Vector3 distEtalon = transform.localScale;
            //GameObject[] meshes = GameObject.FindGameObjectsWithTag( tag );
            foreach (Collider2D obj in fixedBalls)
            {

                float distTemp = Vector3.Distance(currentBall.GetComponent<Transform>().position, obj.transform.position);
                if (distTemp <= 0.9f && distTemp > 0)
                { bool flag = false;
                    for (int j = 0; j < 9; j++) {

                        GameObject[] CurrentArray = Instance.allMultiColorBalls[i];
                        foreach (GameObject obj1 in CurrentArray) {
                            if (obj1.tag == obj.tag) {
                                flag = true;
                                break;
                            }

                        }
                        if()
                        //Debug.Log(tag + "==from inside bouncer" + obj.gameObject.GetComponent<bouncer>().DoubleColor + "------------obj value" + obj.gameObject.tag);
                        if (gameObject[] && !System.Array.Exists(GamePlay.Instance.avoidToChangeInMultivolor, element => element == obj.tag))
                        {
                            //  Debug.Log(tag + "==from inside bouncer" + obj.gameObject.GetComponent<bouncer>().DoubleColor + "------------obj value" + obj.gameObject.tag+"000"+ (BallColor)System.Enum.Parse(typeof(BallColor), obj.gameObject.tag));
                            gameObject.GetComponent<bouncer>().colorForNearsetMulticolor.Add((BallColor)System.Enum.Parse(typeof(BallColor), obj.gameObject.tag));
                        }
                    }
                }


            }

            gameObject.GetComponent<bouncer>().noOfColorForMultiColor = gameObject.GetComponent<bouncer>().colorForNearsetMulticolor.Count;

        }
    }*/

    void Update()
    {
        if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            if( Input.GetKey( KeyCode.W ) ) GamePlay.Instance.GameStatus = GameState.Win;
            if( Input.GetKey( KeyCode.L ) ) { LevelData.LimitAmount = 0; GamePlay.Instance.GameStatus = GameState.GameOver; }
            if( Input.GetKey( KeyCode.D ) ) mainscript.Instance.destroyAllballs() ;
            if( Input.GetKey( KeyCode.M ) ) LevelData.LimitAmount = 1;

        }
    }
	
	// Update is called once per frame
	IEnumerator WinAction () 
    {
        winStarted = true;
        InitScript.Instance.AddLife( 1 );
        GameObject.Find( "Canvas" ).transform.Find( "LevelCleared" ).gameObject.SetActive( true );
  //       yield return new WaitForSeconds( 1f );
        //if( GameObject.Find( "Music" ) != null)
        //    GameObject.Find( "Music" ).SetActive( false );
        //    GameObject.Find( "CanvasPots" ).transform.Find( "Black" ).gameObject.SetActive( true );
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot( SoundBase.Instance.winSound );
         yield return new WaitForSeconds( 1f );
         if( LevelData.mode == ModeGame.Vertical )
         {
           //  SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.swish[0] );
           //  GameObject.Find( "Canvas" ).transform.Find( "PreComplete" ).gameObject.SetActive( true );
            yield return new WaitForSeconds( 1f );
            GameObject.Find( "CanvasPots" ).transform.Find( "Black" ).gameObject.SetActive( false );
            //     SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.swish[0] );
          //  yield return new WaitForSeconds( 1.5f );
            yield return new WaitForSeconds( 0.5f );
         }

        foreach( GameObject item in GameObject.FindGameObjectsWithTag("Ball") )
        {
            item.GetComponent<ball>().StartFall();

                                   
        }
       // StartCoroutine( PushRestBalls() );
        Transform b = GameObject.Find( "-Ball" ).transform;
        ball[] balls = GameObject.Find( "-Ball" ).GetComponentsInChildren<ball>();
        foreach( ball item in balls )
        {
            item.StartFall();
        }

        while( LevelData.LimitAmount > 0 )
        {
            if( mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy != null )
            {
                LevelData.LimitAmount--;
                ball ball = mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy.GetComponent<ball>();
                mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy = null;
                ball.transform.parent = mainscript.Instance.Balls;
                ball.tag = "Ball";
                ball.PushBallAFterWin();

            }
            yield return new WaitForEndOfFrame();
        }
        foreach( ball item in balls )
        {
            if(item != null)
                item.StartFall();
        }
        yield return new WaitForSeconds( 2f );
        while( GameObject.FindGameObjectsWithTag( "Ball" ).Length > 0  )
        {
            yield return new WaitForSeconds( 0.1f );
        }
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot( SoundBase.Instance.aplauds );
        if( PlayerPrefs.GetInt( string.Format( "Level.{0:000}.StarsCount", mainscript.Instance.currentLevel ),0 ) < mainscript.Instance.stars )
            PlayerPrefs.SetInt( string.Format( "Level.{0:000}.StarsCount", mainscript.Instance.currentLevel ), mainscript.Instance.stars );


        if( PlayerPrefs.GetInt( "Score" + mainscript.Instance.currentLevel ) < mainscript.Score )
        {
            PlayerPrefs.SetInt( "Score" + mainscript.Instance.currentLevel, mainscript.Score );

        }

        GameObject.Find( "Canvas" ).transform.Find( "LevelCleared" ).gameObject.SetActive( false );
        GameObject.Find("Canvas").transform.Find("MenuLevelComplete").gameObject.SetActive(true);
        ColyseusClient.Instance.SendMessage("NewScore:-" + mainscript.Score);
        ColyseusClient.Instance.SendMessage("NewStar:-" + mainscript.Instance.stars);
        yield return new WaitForSeconds(2.5f);
        ColyseusClient.Instance.SendMessage("GameLevelWin");

        
    }

    //IEnumerator PushRestBalls()
    //{

    //    while( LevelData.limitAmount  > 0)
    //    {
    //        if( mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy != null )
    //        {
    //            LevelData.limitAmount--;
    //            ball b = mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy.GetComponent<ball>();
    //            mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy = null;
    //            b.transform.parent = mainscript.Instance.Balls;
    //            b.tag = "Ball";
    //            b.PushBallAFterWin();

    //        }
    //        yield return new WaitForEndOfFrame();
    //    }

    //}

    void ShowTutorial()
    {
        //GameObject.Find( "Canvas" ).transform.Find( "Tutorial" ).gameObject.SetActive( true );
        

    }
    void ShowPreTutorial()
    {
        GameObject.Find( "Canvas" ).transform.Find( "PreTutorial" ).gameObject.SetActive( true );

    }

    IEnumerator LoseAction()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot( SoundBase.Instance.OutOfMoves );
        GameObject.Find( "Canvas" ).transform.Find( "OutOfMoves" ).gameObject.SetActive( true );
        yield return new WaitForSeconds( 1.5f );
        GameObject.Find( "Canvas" ).transform.Find( "OutOfMoves" ).gameObject.SetActive( false );
        
            //  GameObject.Find( "Canvas" ).transform.Find( "MenuPreGameOver" ).gameObject.SetActive( true );
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.gameOver);
        //Debug.Log("-/-//-//-//45454545454424545----"+GameObject.Find("Canvas"));
    
        GameObject.Find("Canvas").transform.Find("MenuLavelOver").gameObject.SetActive(true);

        yield return new WaitForSeconds( 1.5f );
        ColyseusClient.Instance.SendMessage("GameLevelOver");// to Indicate currrent level is failed t complete G.O. for this level
        //GameObject.Find("Canvas").transform.Find("MenuLavelOver").gameObject.SetActive(false);


    }

    IEnumerator WaitAfterClose()
    {
        yield return new WaitForSeconds( 1 );
        GameStatus = GameState.Playing;
    }
}
