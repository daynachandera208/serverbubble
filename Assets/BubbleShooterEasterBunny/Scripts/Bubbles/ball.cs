using UnityEngine;
using System.Collections;
using System.Threading;
using InitScriptName;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ball : MonoBehaviour
{
    public Sprite[] sprites;
    public Sprite[] boosts;
    public bool isTarget;
    bool isMoveMinus1;
    bool isMoveMinus; // used to decrese moves on Eater's Eating Event
    //	 public OTSprite sprite;                    // This star's sprite class
    Vector2 speed =                     // Star movement speed / second
        new Vector2( 250, 250 );
    public Vector3 target;
    Vector2 worldPos;
    Vector3 forceVect;
    public bool setTarget;
    public float startTime;
    float duration = 1.0f;
    public GameObject mesh;
    Vector2[] meshArray;
    public bool findMesh;
    Vector3 dropTarget;
    float row;
    string str;
    public bool newBall;
    float mTouchOffsetX;
    float mTouchOffsetY;
    float xOffset;
    float yOffset;
    public Vector3 targetPosition;
    bool stopedBall;
    private bool destroyed;
    public bool NotSorting;
    ArrayList fireballArray = new ArrayList();

    public bool Destroyed
    {
        get { return destroyed; }
        set
        {
            if( value )
            {
                GetComponent<BoxCollider2D>().enabled = false;
                GetComponent<SpriteRenderer>().enabled = false;

            }
            destroyed = value;
        }
    }
    public ArrayList nearBalls = new ArrayList();
    //	private OTSpriteBatch spriteBatch = null;  
    GameObject Meshes;
    public int countNEarBalls;
    float bottomBorder;
    float topBorder;
    float leftBorder;
    float rightBorder;
    float gameOverBorder;
    bool gameOver;
    bool isPaused;
    public AudioClip swish;
    public AudioClip pops;
    public AudioClip join;
    Vector3 meshPos;
    bool dropedDown;
    bool rayTarget;
    RaycastHit2D[] bugHits;
    RaycastHit2D[] bugHits2;
    RaycastHit2D[] bugHits3;
    public bool falling;
   // Animation rabbit;
    private int HitBug;
    private bool fireBall;
    private static int fireworks;
    private bool touchedTop;
    private bool touchedSide;
    private int fireBallLimit = 10;
    private bool launched;
    private bool animStarted;

    public int HitBug1
    {
        get { return HitBug; }
        set
        {
            if( value < 3 )
                HitBug = value;
        }
    }
 
    // Use this for initialization
    void Start()
    {
//        rabbit = GameObject.Find( "Rabbit" ).gameObject.GetComponent<Animation>();
        meshPos = new Vector3( -1000, -1000, -10 );
        //  sprite = GetComponent<OTSprite>();
        //sprite.passive = true;
        //	sprite.onCollision = OnCollision;
        dropTarget = transform.position;
        //		spriteBatch = GameObject.Find("SpriteBatch").GetComponent<OTSpriteBatch>();    
        Meshes = GameObject.Find( "-Ball" );
        // Add the custom tile action controller to this tile
        //      sprite.AddController(new MyActions(this));  

        bottomBorder = Camera.main.GetComponent<mainscript>().bottomBorder;
        topBorder = Camera.main.GetComponent<mainscript>().topBorder;
        leftBorder = Camera.main.GetComponent<mainscript>().leftBorder;
        rightBorder = Camera.main.GetComponent<mainscript>().rightBorder;
        gameOverBorder = Camera.main.GetComponent<mainscript>().gameOverBorder;
        gameOver = Camera.main.GetComponent<mainscript>().gameOver;
        isPaused = Camera.main.GetComponent<mainscript>().isPaused;
        dropedDown = Camera.main.GetComponent<mainscript>().dropingDown;
    }

    IEnumerator AllowLaunchBall()
    {
        yield return new WaitForSeconds( 2 );
        mainscript.StopControl = false;

    }

    public void PushBallAFterWin()
    {
		GetComponent<BoxCollider2D>().offset = Vector2.zero;
        GetComponent<BoxCollider2D>().size = new Vector2( 0.5f, 0.5f );

        setTarget = true;
        startTime = Time.time;
        target = Vector3.zero;
        Invoke( "StartFall", 0.4f );
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetMouseButtonUp( 0 ) )//shooting action code
        {
            GameObject ball = gameObject;

            

            if ( !ClickOnGUI(Input.mousePosition) && !launched && !ball.GetComponent<ball>().setTarget && mainscript.Instance.newBall2 == null /*&& mainscript.Instance.newBall == null*/ && newBall && !Camera.main.GetComponent<mainscript>().gameOver && ( GamePlay.Instance.GameStatus == GameState.Playing || GamePlay.Instance.GameStatus == GameState.WaitForChicken ) )
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint( Input.mousePosition );
                worldPos = pos;
//                print( " World pos Y ::"+ worldPos.y);
                if( worldPos.y > -1.5f && !mainscript.StopControl )
                {
                    launched = true;
                 //   rabbit.Play( "rabbit_move" );
                    SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot( SoundBase.Instance.swish[0] );
                    mTouchOffsetX = ( worldPos.x - ball.transform.position.x ); //+ MathUtils.random(-10, 10);
                    mTouchOffsetY = ( worldPos.y - ball.transform.position.y );
                    xOffset = (float)Mathf.Cos( Mathf.Atan2( mTouchOffsetY, mTouchOffsetX ) );
                    yOffset = (float)Mathf.Sin( Mathf.Atan2( mTouchOffsetY, mTouchOffsetX ) );
                    speed = new Vector2( xOffset, yOffset );
                    if(!fireBall)
                        GetComponent<CircleCollider2D>().enabled = true;
                    target = worldPos;

                    setTarget = true;
                    startTime = Time.time;
                    dropTarget = transform.position;
                    InitScript.Instance.BoostActivated = false;
                    mainscript.Instance.newBall = gameObject;
                    mainscript.Instance.newBall2 = gameObject;
                    GetComponent<Rigidbody2D>().AddForce( target - dropTarget, ForceMode2D.Force );
                  //  print(" Force is ==="+ (target - dropTarget)+ ForceMode2D.Force);
                    GetComponent<bouncer>().isShooter = true;
                    //Debug.DrawLine( DrawLine.waypoints[0], target );
                    //Debug.Break();
                }
            }
        }
        if( transform.position != target && setTarget && !stopedBall && !isPaused && Camera.main.GetComponent<mainscript>().dropDownTime < Time.time )//Ball is in air towards target
        {
            float totalVelocity = Vector3.Magnitude( GetComponent<Rigidbody2D>().velocity );
           // print(" Adding force to ball ...........!111 valocity"+ totalVelocity);

            if ( totalVelocity > 20 )
            {
                float tooHard = totalVelocity / ( 20 );
                GetComponent<Rigidbody2D>().velocity /= tooHard;
               // print(" Adding force to ball ...........!222");
//
            }
            else if( totalVelocity < 15 )
            {
                float tooSlowRate = totalVelocity / ( 15 );
                if( tooSlowRate != 0 )
                    GetComponent<Rigidbody2D>().velocity /= tooSlowRate;
             //   print(" Adding force to ball ...........!333");


            }

            if (GetComponent<Rigidbody2D>().velocity.y < 1.5f && GetComponent<Rigidbody2D>().velocity.y > 0)
            {
               // print(" Adding force to ball ...........!333");

                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 1.7f);
            }
        }
        if (setTarget)
        {
          //  print("Entering trigger ");
            triggerEnter();
            }
		if( (transform.position.y <= -10 || transform.position.y >= 5) && fireBall && !Destroyed  )
		{
          //  print("destroying here trigger ");

            mainscript.Instance.CheckFreeChicken();
			setTarget = false;
			launched = false;
			DestroySingle(gameObject, 0.00001f);
			mainscript.Instance.checkBall = gameObject;
		}
    }

    bool ClickOnGUI(Vector3 mousePos )
    {
        UnityEngine.EventSystems.EventSystem ct
           = UnityEngine.EventSystems.EventSystem.current;
       
     /*   Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - GameObject.Find("Cannon").transform.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
      //  Cannon.transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + (-90));
         print(" y=<><><><><><><><><><><><<>><<<>>><> "+ rotation_z + (-90)); */
        if (ct.IsPointerOverGameObject() )
        {
        //    print("true <<>><<<>>><<<<>>>><>");
            return true;
           
        }
        else
        {
          //  print("false <<>><<<>>><<<<>>>><>");
            return false;
           

        }
    }

    public void SetBoost( BoostType boostType )//setting up boost in game sapwn and inapp purchases
    {
        tag = "Ball";
        GetComponent<SpriteRenderer>().sprite = boosts[(int)boostType - 1];
        if( boostType == BoostType.ColorBallBoost )
        {
        }
        if( boostType == BoostType.FireBallBoost )
        {
            GetComponent<SpriteRenderer>().sortingOrder = 10;
            GetComponent<CircleCollider2D>().enabled = false; 
            fireBall = true;
            fireballArray.Add( gameObject );
        }
    }

    void FixedUpdate()
    {
        if( Camera.main.GetComponent<mainscript>().gameOver ) return;

        if( stopedBall )
        {

            transform.position = meshPos;
            stopedBall = false;
            if( newBall )
            {
                newBall = false;
                gameObject.layer = 9;
                Camera.main.GetComponent<mainscript>().checkBall = gameObject;
                this.enabled = false;
            }

        }

    }

    public GameObject findInArrayGameObject( ArrayList b, GameObject destObj )
    {
        foreach( GameObject obj in b )
        {

            if( obj == destObj ) return obj;
        }
        return null;
    }


    public bool findInArray( ArrayList b, GameObject destObj )
    {
        foreach( GameObject obj in b )
        {

            if( obj == destObj ) return true;
        }
        return false;
    }

    public ArrayList addFrom( ArrayList b, ArrayList b2 )
    {
        foreach( GameObject obj in b )
        {
            if( !findInArray( b2, obj ) )
            {
                b2.Add( obj );
            }
        }
        return b2;
    }

    public void changeNearestColor()
    {
        GameObject gm = GameObject.Find( "Creator" );
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll( transform.position, 0.5f, 1 << 9 );
        foreach( Collider2D obj in fixedBalls )
        {
            gm.GetComponent<creatorBall>().createBall( obj.transform.position );
            Destroy( obj.gameObject );
        }

    }


    public void checkNextNearestColor( ArrayList b, int counter ,string tag1) // pair of 2 asme methods to get nearest connected balls form connected balls 
    {
        //		Debug.Log(b.Count);
        Vector3 distEtalon = transform.localScale;
        //		GameObject[] meshes = GameObject.FindGameObjectsWithTag(tag);
        //		foreach(GameObject obj in meshes) {
        int layerMask = 1 << LayerMask.NameToLayer( "Ball" );
        Collider2D[] meshes = Physics2D.OverlapCircleAll( transform.position, 1.0f, layerMask );
        foreach( Collider2D obj1 in meshes )
        { if (tag1 != "Ball")
            {
                //Debug.Log(tag + "-----" + (BallColor)obj1.gameObject.GetComponent<bouncer>().DoubleColor + "00000000" + (BallColor)System.Enum.Parse(typeof(BallColor), tag));
                if (obj1.gameObject.tag == tag1 || /*(BallColor)obj1.gameObject.GetComponent<bouncer>().DoubleColor== (BallColor)System.Enum.Parse(typeof(BallColor),tag1)*/ (obj1.GetComponent<bouncer>().ballcolors.Contains((BallColor)System.Enum.Parse(typeof(BallColor), tag1))))
                {
                    // Debug.Log(tag+"-----"+ (BallColor)obj1.gameObject.GetComponent<bouncer>().DoubleColor+ "00000000"+(BallColor)System.Enum.Parse(typeof(BallColor),tag));
                    GameObject obj = obj1.gameObject;
                    float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                    if (distTemp <= 1.0f)
                    {
                        if (!findInArray(b, obj) && obj.GetComponent<bouncer>().bGumNo == 0 && obj.GetComponent<bouncer>().chainNo == 0 && !System.Array.Exists(GamePlay.Instance.avoidToChangeInMulticolor, element => element == obj.tag.ToString()))
                        {
                            counter++;
                            b.Add(obj);
                            obj.GetComponent<bouncer>().checkNextNearestColor(b, counter, tag1);
                            //		destroy();
                            //obj.GetComponent<mesh>().checkNextNearestColor();
                            //		obj.GetComponent<mesh>().destroy();
                        }
                    }
                }
            }
        }
    }

    public void checkNearestColor() // calls checkNextNearestColor on conneted balls in short this method is used to connect all same colored balls in array b using checkNextNearestColor Method and destroyes balls on hit if found same balls no >=3 
    {
        int counter = 0;
        GameObject[] fixedBalls = GameObject.FindObjectsOfType( typeof( GameObject ) ) as GameObject[];			// change color tag of the rainbow
        foreach( GameObject obj in fixedBalls )
        {
            if( obj.layer == 9 && ( obj.name.IndexOf( "Rainbow" ) > -1 ) /* && !obj.gameObject.GetComponent<bouncer>().isShooterBubbleDisabled */  && obj.GetComponent<bouncer>().bGumNo==0)//&& obj.gameObject.tag != "stone")

            {
                obj.tag = tag;
                Debug.Log("Raimbo..!");
            }
        }

        ArrayList b = new ArrayList();
        //if (gameObject.GetComponent<bouncer>().bGumNo==0) {
            b.Add(gameObject); 
        Vector3 distEtalon = transform.localScale;
        //GameObject[] meshes = GameObject.FindGameObjectsWithTag( tag );
        foreach (GameObject obj in fixedBalls)
        {
            /*if ( obj.gameObject.tag==tag)
            {
                Debug.Log(tag+"==double="+ obj.gameObject.GetComponent<bouncer>().DoubleColor+"------------obj value"+ obj.gameObject.tag);
                float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                if (distTemp <= 0.9f && distTemp > 0)
                {
                    Debug.Log(tag + "==from inside doyble=" + obj.gameObject.GetComponent<bouncer>().DoubleColor + "------------obj value" + obj.gameObject.tag);
                    b.Add(obj);
                    obj.GetComponent<bouncer>().checkNextNearestColor(b, counter);
                }
            }
           else */
            if (tag != "Ball")
            {
                if ((obj.layer == 9 && /*(BallColor)obj.GetComponent<bouncer>().DoubleColor==(BallColor)System.Enum.Parse(typeof(BallColor),tag)*/ (obj.GetComponent<bouncer>().ballcolors.Contains((BallColor)System.Enum.Parse(typeof(BallColor), tag))) && obj.GetComponent<bouncer>().bGumNo == 0) || obj.gameObject.tag == tag)
                {
                    //                Debug.Log(tag + "==doyble=" + obj.gameObject.GetComponent<bouncer>().DoubleColor + "------------obj value" + obj.gameObject.tag);
                    float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                    if (distTemp <= 0.9f && distTemp > 0 && obj.GetComponent<bouncer>().bGumNo == 0 && obj.GetComponent<bouncer>().chainNo == 0 && !System.Array.Exists(GamePlay.Instance.avoidToChangeInMulticolor, element => element == obj.tag.ToString()))
                    {
                        //Debug.Log(tag + "==from inside doyble=" + obj.gameObject.GetComponent<bouncer>().DoubleColor + "------------obj value" + obj.gameObject.tag);
                        b.Add(obj);
                        //Debug.Log("");
                        obj.GetComponent<bouncer>().checkNextNearestColor(b, counter, gameObject.tag);
                    }
                }
            }
               
        }

         /* foreach ( GameObject obj in meshes )
        {
            Debug.Log(tag + "==doyble=" + obj.gameObject.GetComponent<bouncer>().DoubleColor + "------------obj value" + obj.gameObject.tag);										// detect the same color balls
            float distTemp = Vector3.Distance( transform.position, obj.transform.position );
            if( distTemp <= 0.9f && distTemp > 0 )
            {
                Debug.Log(tag + "==doyble=" + obj.gameObject.GetComponent<bouncer>().DoubleColor + "------------obj value" + obj.gameObject.tag);
                b.Add( obj );
                obj.GetComponent<bouncer>().checkNextNearestColor( b, counter );
            }
        }*/
        mainscript.Instance.countOfPreparedToDestroy = b.Count;
        if( b.Count >= 3 )
        {
            mainscript.Instance.ComboCount++;
            destroy( b, 0.00001f );
            mainscript.Instance.CheckFreeChicken();
        }
        if( b.Count < 3 )
        {
            Camera.main.GetComponent<mainscript>().bounceCounter++;
            mainscript.Instance.ComboCount = 0;
        }

        b.Clear();
        Camera.main.GetComponent<mainscript>().dropingDown = false;
           FindLight( gameObject );

    }


    public void StartFall() // logic for falling of balls from mesh
    {
        enabled = false;

        if(mesh != null)
            mesh.GetComponent<Grid>().Busy = null;
        if( gameObject == null ) return;
        if( LevelData.mode == ModeGame.Vertical && isTarget )
        {
            Instantiate( Resources.Load( "Prefabs/TargetStar" ), gameObject.transform.position, Quaternion.identity );
        }
        else if( LevelData.mode == ModeGame.Animals && isTarget )
        {
            StartCoroutine( FlyToTarget() );
        }
        setTarget = false;
        transform.SetParent( null );
        gameObject.layer = 13;
        if (gameObject.name.Contains("ball") && !System.Array.Exists(GamePlay.Instance.avoidToChangeInMulticolor, element => element == gameObject.tag.ToString()))

            gameObject.tag = "Ball";
        if( gameObject.GetComponent<Rigidbody2D>() == null ) gameObject.AddComponent<Rigidbody2D>();
        /*for (int i = 0; i < 100; i++)
        {
            if (GamePlay.Instance.candidatesForSpreadingBGum[i] == gameObject) { GamePlay.Instance.candidatesForSpreadingBGum[i] = null; Debug.Log("b contains our candidate::" + i + "-----" + GamePlay.Instance.candidatesForSpreadingBGum[i]); };
        }*/
        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        gameObject.GetComponent<Rigidbody2D>().fixedAngle = false;
        gameObject.GetComponent<Rigidbody2D>().velocity = gameObject.GetComponent<Rigidbody2D>().velocity + new Vector2( Random.Range( -2, 2 ), 0 );
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
        gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
        gameObject.GetComponent<CircleCollider2D>().radius = 0.3f;
        GetComponent<ball>().falling = true;

    }

    IEnumerator FlyToTarget()//used to animate target from mesh to ui
    {
        Vector3 targetPos = new Vector3( 2.3f, 6, 0 );
        if (mainscript.Instance.TargetCounter1 < mainscript.Instance.TotalTargets)
        {
            mainscript.Instance.TargetCounter1++;//print("target plus from ball.cs");
        }
        AnimationCurve curveX = new AnimationCurve( new Keyframe( 0, transform.position.x ), new Keyframe( 0.5f, targetPos.x ) );
        AnimationCurve curveY = new AnimationCurve( new Keyframe( 0, transform.position.y ), new Keyframe( 0.5f, targetPos.y ) );
        curveY.AddKey( 0.2f, transform.position.y - 1 );
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        float speed = 0.2f;
        float distCovered = 0;
        while( distCovered < 0.6f )
        {
            distCovered = ( Time.time - startTime );
            transform.position = new Vector3( curveX.Evaluate( distCovered ), curveY.Evaluate( distCovered ), 0 );
            transform.Rotate( Vector3.back * 10 );
            yield return new WaitForEndOfFrame();
        }
        Destroy( gameObject );

    }

    public bool checkNearestBall( ArrayList b )//same functionality as above checkNextNearestColor group but used for different purpouses than destroying balls on shooter ball hit
    {//
        if(( mainscript.Instance.TopBorder.transform.position.y - transform.position.y <= 0.5f && LevelData.mode != ModeGame.Rounded ) || ( LevelData.mode == ModeGame.Rounded && tag == "chicken" ) )
        {
            Camera.main.GetComponent<mainscript>().controlArray = addFrom( b, Camera.main.GetComponent<mainscript>().controlArray );
            b.Clear();
            return true;    /// don't destroy
        }
        if( findInArray( Camera.main.GetComponent<mainscript>().controlArray, gameObject ) ) { b.Clear(); return true; } /// don't destroy
        b.Add( gameObject );
        foreach( GameObject obj in nearBalls )
        {
            if( obj != gameObject && obj != null )///* && !obj.gameObject.GetComponent<bouncer>().isShooterBubbleDisabled */&& obj.GetComponent<bouncer>().bGumNo == 0)//&& obj.gameObject.tag != "stone")
            {
                if( obj.gameObject.layer == 9 )//Don't apply it here it will cuse faling problems && obj.GetComponent<bouncer>().bGumNo == 0 && obj.GetComponent<bouncer>().chainNo == 0 && obj.GetComponent<bouncer>().isMultiColor == false && !System.Array.Exists(GamePlay.Instance.avoidToChangeInMulticolor, element => element == obj.tag.ToString()))
                {
                    //if(findInArray(Camera.main.GetComponent<mainscript>().controlArray, obj.gameObject)){b.Clear(); return true;} /// don't destroy
                    //else{
                    float distTemp = Vector3.Distance( transform.position, obj.transform.position );
                    if( distTemp <= 0.9f && distTemp > 0 )
                    {
                        if( !findInArray( b, obj.gameObject ) )
                        {
                            //print( gameObject + " " + distTemp );
                            Camera.main.GetComponent<mainscript>().arraycounter++;
                            if( obj.GetComponent<ball>().checkNearestBall( b ) )
                                return true;
                        }
                    }
                    //}
                }
            }
        }
        return false;

    }
    public void connectNearBalls()//same functionality as above checkNextNearestColor group but used for different purpouses than destroying balls on shooter ball hit
    {//stores nearballs for given ball in nearball arraylist and sets countnearballs
        int layerMask = 1 << LayerMask.NameToLayer( "Ball" );
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll( transform.position, 0.5f, layerMask );
        nearBalls.Clear();

        foreach( Collider2D obj in fixedBalls )
        {
            if( nearBalls.Count <= 7 )///* && obj.gameObject.GetComponent<bouncer>().isShooterBubbleDisabled==false */ && obj.GetComponentInParent<bouncer>().bGumNo == 0)// && obj.gameObject.tag!= "stone")
            {
                
                    nearBalls.Add( obj.gameObject );
            }
        }
        /*foreach(GameObject gameObject in nearBalls)
        {
            if (gameObject.GetComponent<bouncer>().isShooterBubbleDisabled ||  gameObject.tag == "stone")
                nearBalls.Remove(gameObject);
        }*/
        countNEarBalls = nearBalls.Count;
    }

    IEnumerator pullToMesh( Transform otherBall = null )// logic to  adjust shooter ball on mesh of balls
    {
        //	AudioSource.PlayClipAtPoint(join, new Vector3(5, 1, 2));
        GameObject busyMesh = null;
        float searchRadius = 0.2f;
        while( findMesh )
        {
            Vector3 centerPoint = transform.position;
            Collider2D[] fixedBalls1 = Physics2D.OverlapCircleAll( centerPoint, 0.1f, 1 << 10 );  //meshes

                foreach( Collider2D obj1 in fixedBalls1 )
                {
                    if( obj1.gameObject.GetComponent<Grid>() == null ) DestroySingle( gameObject, 0.00001f );
                    else if( obj1.gameObject.GetComponent<Grid>().Busy == null )
                    {
                        findMesh = false;
                        stopedBall = true;
                        if( meshPos.y <= obj1.gameObject.transform.position.y )
                        {
                            meshPos = obj1.gameObject.transform.position;
                            busyMesh = obj1.gameObject;
                        }
                    }
                }
                if( findMesh )
                {
                    Collider2D[] fixedBalls = Physics2D.OverlapCircleAll( centerPoint, searchRadius, 1 << 10 );  //meshes
                    foreach( Collider2D obj in fixedBalls )
                    {
                        if( obj.gameObject.GetComponent<Grid>() == null ) DestroySingle( gameObject, 0.00001f );
                        else if( obj.gameObject.GetComponent<Grid>().Busy == null )
                        {
                            findMesh = false;
                            stopedBall = true;


                            if( meshPos.y <= obj.gameObject.transform.position.y )
                            {
                                meshPos = obj.gameObject.transform.position;
                                busyMesh = obj.gameObject;
                            }

                            //yield return new WaitForSeconds(1f/10f);
                        }
                    }
                }
                if( busyMesh != null )
                {
//                print("gameObject==========" + gameObject);not here eater prob
                    busyMesh.GetComponent<Grid>().Busy = gameObject;
                    gameObject.GetComponent<bouncer>().offset = busyMesh.GetComponent<Grid>().offset;
                    if( LevelData.mode == ModeGame.Rounded )
                        LockLevelRounded.Instance.Rotate( target, transform.position );

                }
                transform.parent = Meshes.transform;
           //
                Destroy( GetComponent<Rigidbody2D>() );
            GetComponent<bouncer>().isShooter = false;
            //
            /* GameObject[] fixedBallsForMulticolor = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];         // change color tag of the rainbow
             foreach (GameObject obj in fixedBallsForMulticolor)
             {
                 if (obj.layer == 9 && obj.GetComponent<bouncer>().isMultiColor == true)
                 {
                     obj.GetComponent<bouncer>().doesChangeColor = true;
                     Debug.Log("trued :"+obj.name);
                     obj.GetComponent<bouncer>().CheckNearestColorForMulticolor();
                     obj.GetComponent<bouncer>().ChangeMulticolorColor();
                 }
             }*/
            if (!gameObject.GetComponent<bouncer>().doesChangeColor && gameObject.GetComponent<bouncer>().isMultiColor) {

                gameObject.GetComponent<bouncer>().doesChangeColor=true;
            }
         //   GamePlay.Instance.ChangeMultiColorBallColor();
            
            //  rigidbody2D.isKinematic = true;
            yield return new WaitForFixedUpdate();
                // StopCoroutine( "pullToMesh" );
                dropTarget = transform.position;

                if( findMesh ) searchRadius += 0.2f;

            yield return new WaitForFixedUpdate();
        }
        mainscript.Instance.connectNearBallsGlobal();
     //   FindLight( gameObject );

        if( busyMesh != null )
        {
            Hashtable animTable = mainscript.Instance.animTable;
            animTable.Clear();
            PlayHitAnim( transform.position, animTable );
        }
        creatorBall.Instance.OffGridColliders();

        yield return new WaitForSeconds( 0.5f );

        // StartCoroutine( mainscript.Instance.destroyAloneBall() );
    }

    public void PlayHitAnim( Vector3 newBallPos, Hashtable animTable )
    {

        int layerMask = 1 << LayerMask.NameToLayer( "Ball" );
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll( transform.position, 0.5f, layerMask );
        float force = 0.15f;
        foreach( Collider2D obj in fixedBalls )
        {
            if( !animTable.ContainsKey( obj.gameObject ) && obj.gameObject != gameObject && animTable.Count < 50 )
                obj.GetComponent<ball>().PlayHitAnimCorStart( newBallPos, force, animTable );
        }
        if( fixedBalls.Length > 0 && !animTable.ContainsKey( gameObject ) )
            PlayHitAnimCorStart( fixedBalls[0].gameObject.transform.position, 0, animTable );
    }

    public void PlayHitAnimCorStart( Vector3 newBallPos, float force, Hashtable animTable )
    {
        if( !animStarted )
        {
            StartCoroutine( PlayHitAnimCor( newBallPos, force, animTable ) );
            PlayHitAnim( newBallPos, animTable );
        }
    }

    public IEnumerator PlayHitAnimCor( Vector3 newBallPos, float force, Hashtable animTable )
    {
        animStarted = true;
        animTable.Add( gameObject, gameObject );
        if( tag == "chicken" ) yield break;
        yield return new WaitForFixedUpdate();
        float dist = Vector3.Distance( transform.position, newBallPos );
        force = 1 / dist + force;
        newBallPos = transform.position - newBallPos;
        if( transform.parent == null )
        {
            animStarted = false;
            yield break;
        }
        newBallPos = Quaternion.AngleAxis( transform.parent.parent.rotation.eulerAngles.z, Vector3.back ) * newBallPos;
        newBallPos = newBallPos.normalized;
        newBallPos = transform.localPosition + ( newBallPos * force / 10 );

        float startTime = Time.time;
        Vector3 startPos = transform.localPosition;
        float speed = force * 5;
        float distCovered = 0;
        while( distCovered < 1 && !float.IsNaN( newBallPos.x ) )
        {
            distCovered = ( Time.time - startTime ) * speed;
            if( this == null ) yield break;
            //   if( destroyed ) yield break;
            if( falling )
            {
     //           transform.localPosition = startPos;
                yield break;
            }
            transform.localPosition = Vector3.Lerp( startPos, newBallPos, distCovered );
            yield return new WaitForEndOfFrame();
        }
        Vector3 lastPos = transform.localPosition;
        startTime = Time.time;
        distCovered = 0;
        while( distCovered < 1 && !float.IsNaN( newBallPos.x ) )
        {
            distCovered = ( Time.time - startTime ) * speed;
            if( this == null ) yield break;
            if( falling )
            {
          //      transform.localPosition = startPos;
                yield break;
            }
            transform.localPosition = Vector3.Lerp( lastPos, startPos, distCovered );
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = startPos;
        animStarted = false;
    }
    void OnTriggerStay2D( Collider2D other )
    {
       // if( findMesh && other.gameObject.layer == 9 )
        {
            //	StartCoroutine(pullToMesh());
        }
    }

    public void FindLight(GameObject activatedByBall)
    {
        int layerMask = 1 << LayerMask.NameToLayer( "Ball" );
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll( transform.position, 0.5f, layerMask );
        int i = 0;
        foreach( Collider2D obj in fixedBalls )
        {
            i++;
            if( i <= 10 )
            {
                if( ( obj.gameObject.tag == "light"  ) && GamePlay.Instance.GameStatus == GameState.Playing ) {
                    DestroySingle( obj.gameObject );
                    DestroySingle( activatedByBall );
                }
                else if( ( obj.gameObject.tag == "cloud"  ) && GamePlay.Instance.GameStatus == GameState.Playing ) 
                {
                    obj.GetComponent<ColorBallScript>().ChangeRandomColor();
                }

            }
        }
    }


    void OnCollisionEnter2D( Collision2D coll )
    {
        OnTriggerEnter2D( coll.collider );
    }

    void OnTriggerEnter2D( Collider2D other )//main logic for operations performed when shooting ball hits the balls of mesh most powerups logic is called here
    {
        
        // stop
       // if(other.gameObject.)

        if( other.gameObject.name.Contains( "ball" ) && setTarget && name.IndexOf( "bug" ) < 0 ) // if collided with any bubble  
        {
            if( !other.gameObject.GetComponent<ball>().enabled )
            {
                if (tag != "Ball")
                {
                    if ((!isMoveMinus1) && other.GetComponent<bouncer>().chainNo != 0 && other.GetComponent<bouncer>().chainLevel != 0 && other.tag == gameObject.tag)// for breaking chains
                    {
                        // Debug.Log("Chain Breaker--"+other.gameObject);
                        GameObject[] fixedBalls = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
                        ArrayList ChainsToRemove = new ArrayList();
                        foreach (GameObject obj in fixedBalls)
                        {

                            if (obj.layer == 9 && obj.GetComponent<bouncer>().chainNo == other.GetComponent<bouncer>().chainNo && obj.gameObject.tag == other.gameObject.tag)
                            {
                                if (obj.GetComponent<bouncer>().chainLevel == 1)
                                {
                                    obj.GetComponent<bouncer>().chainLevel = 0;


                                    obj.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                                    ChainsToRemove.Add(obj);
                                }
                                if (obj.GetComponent<bouncer>().chainLevel == 2)
                                {
                                    obj.GetComponent<bouncer>().chainLevel = 1;

                                    obj.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
                                }
                            }
                        }

                        {
                            mainscript.Instance.LimitAmmountMinusBy1();
                            Camera.main.GetComponent<mainscript>().PowerUPCall();
                            isMoveMinus1 = true;
                        }
                        foreach (GameObject obj in ChainsToRemove)
                        {
                            obj.GetComponent<bouncer>().chainNo = 0;
                        }
                        Destroy(gameObject);
                        return;
                    }
                    else
                        isMoveMinus1 = false;
                    if ((other.gameObject.tag == "black_hole") && GamePlay.Instance.GameStatus == GameState.Playing)
                    {
                        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.black_hole);
                        DestroySingle(gameObject);
                    }
                    if (other.tag == "rowAdd") //Code to add row when shooter hits rowAdd bubble 
                    {

                        float rowToAdd = other.gameObject.GetComponent<Transform>().position.y;
                        GameObject[] fixedBalls = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
                        //Debug.Log("Adding Row operation Started-------on rowY" + rowToAdd.ToString());
                        //MoveBubbles1RowDown(rowToAdd);
                        // AddRowToY(rowToAdd);
                        //  AddRowToY(rowToAdd - 0.58f);
                        FillCavity(rowToAdd);
                        if (!isMoveMinus)
                        {
                            mainscript.Instance.LimitAmmountMinusBy1();
                            Camera.main.GetComponent<mainscript>().PowerUPCall();
                            isMoveMinus = true;
                        }
                        else
                            isMoveMinus = false;

                       GamePlay.Instance.shiftMesh.Clear();
                       GamePlay.Instance.shiftball.Clear();
                        other.GetComponent<ColorBallScript>().ChangeRandomColor();
                        Destroy(gameObject);
                        // Destroy(other.gameObject);

                    }
                    if (other.gameObject.tag == "rowRemover") // code for removing row of row remover bubble
                    {
                       
                        float rowToRemove = other.gameObject.GetComponent<Transform>().position.y;
                        int index=0;
                        GameObject[] fixedBalls = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
                        int[] b =new int[20];
                        foreach (GameObject obj in fixedBalls)
                        {
                            if (obj.layer == 9 && obj.GetComponent<Transform>().position.y == rowToRemove && obj.gameObject.GetComponent<bouncer>().chainNo==0 )//&& obj.gameObject.GetComponent<bouncer>().bGumNo==0 )
                            {
                                DestroySingle(obj);
                            }
                            else if (obj.layer == 9 && obj.GetComponent<Transform>().position.y == rowToRemove && obj.gameObject.GetComponent<bouncer>().chainNo != 0)
                            {
                                if (index != 0) {
                                    for(int i=0;i<index;i++) {
                                        if (b[i] == obj.GetComponent<bouncer>().chainNo)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            b[index] = obj.GetComponent<bouncer>().chainNo;
                                            index++;
                                        }
                                    }
                                }
                                else
                                {
                                    b[index] = obj.GetComponent<bouncer>().chainNo;
                                    index++;
                                }
                                    
                            }
                           

                        }
                        ArrayList ChainsToRemove = new ArrayList();
                        for (int i = 0; i < index; i++)
                        {
                            
                            foreach (GameObject obj in fixedBalls)
                            {

                                if (obj.layer == 9 && obj.GetComponent<bouncer>().chainNo == b[i] )
                                {
                                    if (obj.GetComponent<bouncer>().chainLevel == 1)
                                    {
                                        obj.GetComponent<bouncer>().chainLevel = 0;


                                        obj.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                                        ChainsToRemove.Add(obj);
                                    }
                                    if (obj.GetComponent<bouncer>().chainLevel == 2)
                                    {
                                        obj.GetComponent<bouncer>().chainLevel = 0;

                                        obj.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
                                        obj.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                                        ChainsToRemove.Add(obj);

                                       
                                    }
                                }
                            }
                        }
                        foreach (GameObject obj in ChainsToRemove)
                        {
                            obj.GetComponent<bouncer>().chainNo = 0;
                            if (obj.transform.position.y == rowToRemove)
                            {
                                DestroySingle(obj);
                            }

                        }
                        mainscript.Instance.destroyhanngingballs();
                        //   Debug.Log("Row Destroyimg code ........!");

                        if (!isMoveMinus)
                        {
                            mainscript.Instance.LimitAmmountMinusBy1();
                            Camera.main.GetComponent<mainscript>().PowerUPCall();
                            isMoveMinus = true;
                        }
                        else
                            isMoveMinus = false;
                        Destroy(gameObject);

                    }
                    if (other.GetComponent<bouncer>().bGumNo != 0)//for bubblegum effect
                    {
                        GamePlay.Instance.doesBubbleSpread = false;
                        other.transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = false;
                        other.GetComponent<bouncer>().bGumNo = 0;
                        if (!isMoveMinus)
                        {
                            mainscript.Instance.LimitAmmountMinusBy1();
                            Camera.main.GetComponent<mainscript>().PowerUPCall();
                            isMoveMinus = true;
                        }
                        else
                            isMoveMinus = false;
                        Destroy(gameObject);

                    }
                    else
                    {
                        GamePlay.Instance.doesBubbleSpread = true;
                    }
                    if (other.gameObject.tag == "frozen")
                    {// Code for changing random color and destroying shooter bubble
                        other.GetComponent<SpriteRenderer>().color = Color.white;
                        other.GetComponent<ColorBallScript>().ChangeRandomColor();
                        if (!isMoveMinus)
                        {
                            mainscript.Instance.LimitAmmountMinusBy1();
                            Camera.main.GetComponent<mainscript>().PowerUPCall();
                            isMoveMinus = true;
                        }
                        else
                            isMoveMinus = false;
                        Destroy(gameObject);


                        return;
                        //Debug.Log("Frozen Bubbe hit......>!");
                    }
                }
                if (other.gameObject.tag == "eater" && tag!="Ball") //logic for eating bubble...
                { //Debug.Log("Entered To Eater");
                    if (gameObject.GetComponent<bouncer>().isShooter== true)
                    {
                        Vector3 centerPoint = transform.position;
                        Collider2D[] fixedBalls1 = Physics2D.OverlapCircleAll(centerPoint, 0.1f, 1 << 10);  //meshes

                        foreach (Collider2D obj1 in fixedBalls1)
                        {
                            if (obj1.gameObject.GetComponent<Grid>() == null) DestroySingle(gameObject, 0.00001f);
                            else if (obj1.gameObject.GetComponent<Grid>().Busy == other)
                            {
                                /*findMesh = false;
                                stopedBall = true;
                                if (meshPos.y <= obj1.gameObject.transform.position.y)
                                {
                                    meshPos = obj1.gameObject.transform.position;
                                   // busyMesh = obj1.gameObject;
                                }*/
                                // Destroy(other.gameObject);
                               // print("gameObject=====" + gameObject);//not here eater prob.
                                obj1.gameObject.GetComponent<Grid>().Busy = gameObject;
                                //gameObject.GetComponent<bouncer>().isShooterBubbleDisabled = true;
                                launched = true;
                                mainscript.lastBall = gameObject.transform.position;
                                creatorBall.Instance.EnableGridColliders();
                                target = Vector2.zero;
                                setTarget = false;
                                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                                findMesh = true;
                                GetComponent<BoxCollider2D>().offset = Vector2.zero;
                                GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.5f);
                                /*if (GetComponent<SpriteRenderer>().sprite == boosts[0])  //color ball boost
                                {
                                    DestroyAround();
                                }
                                else*/
                                {
                                    gameObject.GetComponent<bouncer>().offset = obj1.GetComponent<Grid>().offset;
                                    if (LevelData.mode == ModeGame.Rounded)
                                        LockLevelRounded.Instance.Rotate(target, transform.position);
                                    transform.parent = obj1.transform;
                                    Destroy(GetComponent<Rigidbody2D>());
                                    //  rigidbody2D.isKinematic = true;

                                    // StopCoroutine( "pullToMesh" );
                                    dropTarget = transform.position;
                                }


                            }
                        }
                        //Destroy(other.gameObject);
                      //  Debug.Log("From outside- move eater" + isMoveMinus);
                        if (!isMoveMinus)
                        {
                            mainscript.Instance.LimitAmmountMinusBy1();
                            // Debug.Log("09090- move eater"+isMoveMinus);
                            Camera.main.GetComponent<mainscript>().PowerUPCall();
                            isMoveMinus = true;
                            //Debug.Log("Flag Trued as"+isMoveMinus);
                        }
                        else
                            isMoveMinus = false;
                        Debug.Log(" Call to destroy.........! ");
                        Destroy(gameObject);

                        //   grid.B
                        return;
                       // Debug.Log("09090- move eater from return");
                    }
                }
                else if( !fireBall )
                    StopBall( true, other.transform );
                else
                {
					if( other.gameObject.tag.Contains( "animal" ) || other.gameObject.tag.Contains( "empty" ) || other.gameObject.tag.Contains( "chicken" ) ) return;
                    fireBallLimit--;
                    if( fireBallLimit > 0)
                        DestroySingle( other.gameObject, 0.000000000001f );
                    else
                    {
                        StopBall();
                        destroy( fireballArray, 0.000000000001f );

                    }


                }
      //           FindLight(gameObject);
           }
            //          }
        }
        else if( other.gameObject.name.IndexOf( "ball" ) == 0 && setTarget && name.IndexOf( "bug" ) == 0 )
        {
            if( other.gameObject.tag == gameObject.tag )
            {
                Destroy( other.gameObject );
                //                Score.Instance.addScore(3);
            }
        }

        else if( other.gameObject.name == "TopBorder" && setTarget )
        {
            if( LevelData.mode == ModeGame.Vertical || LevelData.mode == ModeGame.Animals )
            {
                if( !findMesh )
                {
                    transform.position = new Vector3( transform.position.x, transform.position.y, transform.position.z );
                    StopBall();

                    if( fireBall )
                    {
                        destroy( fireballArray, 0.000000000001f );
                    }
                    mainscript.Instance.destroyhanngingballs();
                }

            }
        }
       // mainscript.Instance.destroyhanngingballs();

    }

    public void FillCavity(float topRow) //used for rowadder logic
    {

        float lastRow = 100f;
        
        
        GameObject[] fixedBalls = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
       
        foreach (GameObject obj in fixedBalls)
        {
            if (obj.layer == 9 && obj.GetComponent<Transform>().position.y <= topRow)
            {
              
                if (lastRow > obj.GetComponent<Transform>().position.y)
                {
                    lastRow = obj.GetComponent<Transform>().position.y; // getting lastrow of balls
                }
            }
        }
        foreach (GameObject obj in fixedBalls)
        {
            if (obj.layer == 10 && obj.GetComponent<Transform>().position.y <= topRow && obj.GetComponent<Transform>().position.y >= lastRow && obj.GetComponent<Grid>().Busy == null)//- 0.68f)
            {
               
                GameObject gm = GameObject.Find("Creator").GetComponent<creatorBall>().createBall(obj.GetComponent<Transform>().position);
             //   print("gm===" + gm);not here eater prob.
                obj.GetComponent<Grid>().Busy = gm;
            }
        }

    }

    void AddRowToY(float rowY)//older logic  for row adder working correctly but don't found usefull because it's below balls sometimes falls as pattern in zigzag mesh not able to shift correctly
    {
        GameObject[] fixedBalls = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        GamePlay.Instance.shiftball.Clear();
        GamePlay.Instance.shiftMesh.Clear();
        Debug.Log("Row To add=" + rowY.ToString());
        foreach (GameObject obj in fixedBalls)
        {
            if (obj.layer == 10 && obj.GetComponent<Transform>().position.y == rowY && obj.GetComponent<Grid>().Busy==null)
            {
                
                GamePlay.Instance.shiftMesh.Add(obj);
             //   Debug.Log("Selected meshes---" + obj);
            }
           
        }
        foreach(GameObject obj in GamePlay.Instance.shiftMesh)
        {
           GameObject gm= GameObject.Find("Creator").GetComponent<creatorBall>().createBall(obj.GetComponent<Transform>().position);
           // Debug.Log("---"+gm);
        }

    }

    void MoveBubbles1RowDown(float topRow)//older logic  for row adder working not correctly and not found usefull because it's below balls sometimes falls as pattern in zigzag mesh not able to shift correctly
    {//here multiple attempts with different ways to achive row shifting without problem failed untill moving 2 rows but causes problems when any immovable(i.e chain or stone)comes in way
        float lastRow=100f;
        //float[][] meshPositions={ { },{ } };
        float minXOfBall = 100f,offset=-0.33f;
        GameObject[] fixedBalls = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        GamePlay.Instance.shiftball.Clear();
        GamePlay.Instance.shiftMesh.Clear();
       // Debug.Log("TopRow="+topRow.ToString());
        foreach (GameObject obj in fixedBalls)
        {
            if (obj.layer == 9 && obj.GetComponent<Transform>().position.y <= topRow)
            {
                //Debug.Log("...................."+ obj.GetComponent<Transform>().position.y);
                //if(obj.GetComponent<Transform>().position.y )
                GamePlay.Instance.shiftball.Add(obj);
                if (lastRow > obj.GetComponent<Transform>().position.y)
                {
                    lastRow = obj.GetComponent<Transform>().position.y; // getting lastrow of balls
                }
            }
        }
        //Debug.Log("LastRowY="+lastRow.ToString()+"------/");

        foreach (GameObject obj in GamePlay.Instance.shiftball) {
           
           if (obj.GetComponent<Transform>().position.x== -3.34f && obj.GetComponent<Transform>().position.y == lastRow)
            {
                offset = 0.33f;
            }
        }
        foreach (GameObject obj in fixedBalls)
        {
            if (obj.layer == 10 && obj.GetComponent<Transform>().position.y <= topRow && obj.GetComponent<Transform>().position.y >= lastRow )//- 0.68f)
            {
                //Debug.Log("...................."+ obj.GetComponent<Transform>().position.y+"///"+obj.name);
                //if(obj.GetComponent<Transform>().position.y )
                GamePlay.Instance.shiftMesh.Add(obj);
               
            }
        }
        GameObject goBorder = GameObject.Find("GameOverBorder");

        ////// lastRow = lastRow - 0.58f;
        // Code to store the patterns of balls in arraylist
        /* Debug.Log("GO Border Y=" + goBorder.transform.position.y + "// Last Row y Position=" + lastRow);
         GameObject[][] connectedBallsInRow = new GameObject[6][];
         GameObject[] temp = new GameObject[11];
         GamePlay.Instance.allRowsTomoveDown.Clear();
         foreach (GameObject mesh in GamePlay.Instance.shiftMesh)
         {
             while (lastRow <= topRow)
             {
                 if (mesh.transform.position.y >= lastRow - 0.60f && mesh.transform.position.y < lastRow - 0.57f)
                 {
                     Debug.Log("Last Row Meshes are as:=" + mesh.name + "--////===" + mesh.transform.position.x);
                     if (mesh.GetComponent<Grid>().Busy != null)
                     {
                         if (temp.Length == 0)
                         {

                             temp[0] = mesh;

                         }

                         else if (temp[temp.Length - 1].transform.position.x > mesh.transform.position.x + 0.65 && temp[temp.Length - 1].transform.position.x <= mesh.transform.position.x + 0.69)
                         { temp[temp.Length] = mesh; }
                         else
                         {

                             connectedBallsInRow[connectedBallsInRow.Length] = temp;
                             temp = new GameObject[11];
                         }
                     }
                 }
                 GamePlay.Instance.allRowsTomoveDown.Add(connectedBallsInRow);
                 connectedBallsInRow = new GameObject[6][];
                 lastRow += 0.58f;
             }

         }
         Debug.Log("Total Rows to move Down"+GamePlay.Instance.allRowsTomoveDown.Count);
         foreach (GameObject[][] row in GamePlay.Instance.allRowsTomoveDown)
         {
             Debug.Log("");
         }*/



        //Tryimg to store in 2-d array(Uselessssss)
       /* List<int[]> patternoMove = new List<int[]>();
        int[] rowPattern = { 0,0,0,0,0,0,0,0,0,0,0};int i = 0;
        foreach (GameObject mesh in GamePlay.Instance.shiftMesh)
        {
           // Debug.Log("988979878979798----"+mesh.name);
            if (mesh.GetComponent<Grid>().Busy != null && mesh.GetComponent<Grid>().Busy.GetComponent<bouncer>().bGumNo == 0 && mesh.GetComponent<Grid>().Busy.GetComponent<bouncer>().chainNo == 0 && !System.Array.Exists(GamePlay.Instance.avoidToChangeInMulticolor, element => element == mesh.GetComponent<Grid>().Busy.gameObject.tag.ToString()))
            {
                rowPattern[i] =(int)System.Enum.Parse(typeof(BallColor), mesh.GetComponent<Grid>().Busy.tag);

            }
            i++;
            if (i == 11)
            {
                i = 0;
                patternoMove.Add(rowPattern);
                for (int k=0;k<11;k++)
                {
                    rowPattern[i] = 0;
                }

            }
        }
        foreach (int[] list in patternoMove)
        {
            Debug.Log("Row Started-0-0-0-0-0-0-0-0-0-");
            for (int index = 0; index < 11; index++)
            {
                Debug.Log("Elements :====="+list[index]);
            }
        }*/










        //2 RowAdding Ball because of immovable balls not working perfectly
         Debug.Log("Inside code ---"+GamePlay.Instance.shiftMesh.Count);
         foreach (GameObject mesh in GamePlay.Instance.shiftMesh)
         {
             //Debug.Log("MEsh-------------------------------"+mesh.name);
             if ( mesh.GetComponent<Grid>().Busy!=null)// && mesh.GetComponent<Grid>().Busy.GetComponent<bouncer>().bGumNo == 0 && mesh.GetComponent<Grid>().Busy.GetComponent<bouncer>().chainNo == 0  && !System.Array.Exists(GamePlay.Instance.avoidToChangeInMulticolor, element => element == mesh.GetComponent<Grid>().Busy.gameObject.tag.ToString()))
             {
                 Debug.Log("mesh------+-+-+-+--+-+++++++-+-+-+-+-+------"+mesh.name);
                 string[] ST = mesh.name.Split(')');
                 GameObject newMesh = GameObject.Find("box(Clone)" +(int.Parse(ST[1])+22));
                 newMesh.GetComponent<Grid>().Busy = mesh.GetComponent<Grid>().Busy;
                 newMesh.GetComponent<Grid>().Busy.transform.position = newMesh.transform.position;
                 mesh.GetComponent<Grid>().Busy = null;
             }
         }







        /*Debug.Log("Inside code ---" + GamePlay.Instance.shiftMesh.Count);
        int ofsetadd, zeroadd;
        if (GamePlay.Instance.shiftMesh[GamePlay.Instance.shiftMesh.Count - 1].GetComponent<Grid>().offset == 0.33f)
        {
            print("move left.///.......................!");
            ofsetadd = 11;
            zeroadd = 10;
        }
        else
        {
            zeroadd = 11;
            ofsetadd = 12;
        }
        foreach (GameObject mesh in GamePlay.Instance.shiftMesh)
        {
            //Debug.Log("MEsh-------------------------------"+mesh.name);
            if (mesh.GetComponent<Grid>().Busy != null && mesh.GetComponent<Grid>().Busy.GetComponent<bouncer>().bGumNo == 0 && mesh.GetComponent<Grid>().Busy.GetComponent<bouncer>().chainNo == 0 && !System.Array.Exists(GamePlay.Instance.avoidToChangeInMulticolor, element => element == mesh.GetComponent<Grid>().Busy.gameObject.tag.ToString()))
            {
                Debug.Log("mesh------+-+-+-+--+-+++++++-+-+-+-+-+------" + mesh.name);
                string[] ST = mesh.name.Split(')');
                GameObject newMesh = null;
                if (mesh.GetComponent<Grid>().offset==0.33)
                  newMesh = GameObject.Find("box(Clone)" + (int.Parse(ST[1]) + ofsetadd));
                else
                     newMesh = GameObject.Find("box(Clone)" + (int.Parse(ST[1]) + zeroadd));
                newMesh.GetComponent<Grid>().Busy = mesh.GetComponent<Grid>().Busy;
                newMesh.GetComponent<Grid>().Busy.transform.position = newMesh.transform.position;
                mesh.GetComponent<Grid>().Busy = null;
            }
        }*/


        //trying to move whole structure 11 box ahade not correct for all
        /* bool flag = true;
         if (offset > 0)
             flag = true;
         else
             flag = false;
         float rowField=0.121f;
         GameObject newMesh;
         foreach (GameObject mesh in GamePlay.Instance.shiftMesh)
         {
             //Debug.Log("MEsh-------------------------------"+mesh.name);
             if (mesh.GetComponent<Grid>().Busy != null && mesh.GetComponent<Grid>().Busy.GetComponent<bouncer>().bGumNo == 0 && mesh.GetComponent<Grid>().Busy.GetComponent<bouncer>().chainNo == 0 && !System.Array.Exists(GamePlay.Instance.avoidToChangeInMulticolor, element => element == mesh.GetComponent<Grid>().Busy.gameObject.tag.ToString()))
             {
                 if (rowField == 0.121f)
                 {
                     rowField = mesh.transform.position.y;
                 }
                 if (mesh.transform.position.y == rowField)
                 {
                     Debug.Log("mesh------+-+-+-+--+-+++++++-+-+-+-+-+------" + mesh.name);
                     string[] ST = mesh.name.Split(')');
                     if (flag)
                     {
                         newMesh = GameObject.Find("box(Clone)" + (int.Parse(ST[1]) + 11));
                     }
                     else
                     {
                         newMesh = GameObject.Find("box(Clone)" + (int.Parse(ST[1]) + 10));
                     }
                     if (newMesh.GetComponent<Grid>().Busy == null)
                     {
                         newMesh.GetComponent<Grid>().Busy = mesh.GetComponent<Grid>().Busy;
                         newMesh.GetComponent<Grid>().Busy.transform.position = newMesh.transform.position;
                         mesh.GetComponent<Grid>().Busy = null;
                     }
                 }
                 else
                 {
                     rowField = mesh.transform.position.y;
                     if (flag) flag = false;
                     else flag = true;
                     Debug.Log("mesh------+-+-+-+--+-+++++++-+-+-+-+-+------" + mesh.name);
                     string[] ST = mesh.name.Split(')');
                     if (flag)
                     {
                         newMesh = GameObject.Find("box(Clone)" + (int.Parse(ST[1]) + 11));
                     }
                     else
                     {
                         newMesh = GameObject.Find("box(Clone)" + (int.Parse(ST[1]) + 10));
                     }
                     if (newMesh.GetComponent<Grid>().Busy == null)
                     {
                         newMesh.GetComponent<Grid>().Busy = mesh.GetComponent<Grid>().Busy;
                         newMesh.GetComponent<Grid>().Busy.transform.position = newMesh.transform.position;
                         mesh.GetComponent<Grid>().Busy = null;
                     }
                 }
             }
         }*/










        //Original Code Working withs hangging ball glitch
        // Debug.Log("==**********=" + (GamePlay.Instance.shiftMesh[32].GetComponent<Transform>().position.x).ToString() + "==" + (GamePlay.Instance.shiftMesh[21].GetComponent<Transform>().position.x).ToString());
        /* while (lastRow <= topRow)
         {
             Debug.Log("Moving rows y" + lastRow);
             int cout1 = 0, count2 = 0;
             foreach (GameObject obj in GamePlay.Instance.shiftMesh)
             {// removing lastrow 1 down



                 //float tmp1 = lastRow - 0.580001f;
                 // Debug.Log("*out loop**" + (lastRow - 0.58f).ToString() + "******" + obj.GetComponent<Transform>().position.y + "lastrow val:" + lastRow+"upper bound:"+ (lastRow - 0.60f));
                 if ((obj.GetComponent<Transform>().position.y < lastRow - 0.57f && obj.GetComponent<Transform>().position.y > lastRow - 0.60f))
                 {
                     // Debug.Log("*in loop**" + (lastRow - 0.58f).ToString() + "******" + obj.GetComponent<Transform>().position.y+"lastrow val:"+lastRow);
                     foreach (GameObject objParent in GamePlay.Instance.shiftMesh)
                     {
                         //Debug.Log("LastRowY=" + lastRow.ToString() + "---**336---/");
                         //if (offset > 0)
                         // {
                         if (objParent.GetComponent<Transform>().position.y >= lastRow - 0.10f && objParent.GetComponent<Transform>().position.y < lastRow + 0.10f && objParent.GetComponent<Grid>().Busy != null)
                         {
                             cout1++;
                             if (obj.GetComponent<Grid>().Busy == null && objParent.GetComponent<Grid>().Busy.GetComponent<bouncer>().bGumNo == 0 && objParent.GetComponent<Grid>().Busy.GetComponent<bouncer>().chainNo == 0 && (obj.GetComponent<Transform>().position.x > objParent.GetComponent<Transform>().position.x - offset - 0.01f && obj.GetComponent<Transform>().position.x < objParent.GetComponent<Transform>().position.x - offset + 0.01f) && !System.Array.Exists(GamePlay.Instance.avoidToChangeInMulticolor, element => element == objParent.GetComponent<Grid>().Busy.tag.ToString()))// && obj.GetComponent<Transform>().position.x > (objParent.GetComponent<Transform>().position.x + offset) - 0.010)
                             {
                                 obj.GetComponent<Grid>().Busy = objParent.GetComponent<Grid>().Busy;
                                 objParent.GetComponent<Grid>().Busy.GetComponent<Transform>().position = obj.GetComponent<Transform>().position;
                                 objParent.GetComponent<Grid>().Busy = null;
                             }
                         }

                     }

                 }
             }
             // Debug.Log("Last Row Y value in while =:" + lastRow + "-- Toprow =:" + topRow + "-=-=-=-=-=-cout1=-=-=-=-> " + cout1 + "-=-=-=-=-=-cout2=-=-=-=-> " + count2 + "---':::"+offset);
             lastRow = lastRow + 0.58f;
             offset = offset * -1;
         }*/

    }

   /* public void MoveChildsOFRow(GameObject obj,bool Temp, int meshNo) not useful attempt for row adder
    {
        foreach (GameObject objToMove in GamePlay.Instance.shiftMesh)
        {
            if (objToMove.GetComponent<Grid>().Busy != null  )
            {
                string[] ST = objToMove.name.Split(')');
                if (int.Parse(ST[1]) > meshNo) { }
            }
        }
    }*/
   /* public void MoveChildsForRowAdd(float offset,GameObject obj,GameObject objParent,bool Temp)
    {
        if (Temp)
        {
            string[] ST = obj.name.Split(')');
            GameObject newObj = GameObject.Find("box(Clone)" + (int.Parse(ST[1]) + 1).ToString());

            newObj.GetComponent<Grid>().Busy = objParent.GetComponent<Grid>().Busy;
            objParent.GetComponent<Grid>().Busy.GetComponent<Transform>().position = newObj.GetComponent<Transform>().position;
            objParent.GetComponent<Grid>().Busy = null;
        }
        else
        {
            string[] ST = obj.name.Split(')');
            GameObject newObj = GameObject.Find("box(Clone)" + (int.Parse(ST[1]) - 1).ToString());

            newObj.GetComponent<Grid>().Busy = objParent.GetComponent<Grid>().Busy;
            objParent.GetComponent<Grid>().Busy.GetComponent<Transform>().position = newObj.GetComponent<Transform>().position;
            objParent.GetComponent<Grid>().Busy = null;
        }
    }*/
    void StopBall( bool pulltoMesh = true, Transform otherBall = null )//called when ontriggerenter2D logic is over and shooting ball is going to add to mesh 
    {
        launched = true;
        mainscript.lastBall = gameObject.transform.position;
        creatorBall.Instance.EnableGridColliders();
        target = Vector2.zero;
        setTarget = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        findMesh = true;
		GetComponent<BoxCollider2D>().offset = Vector2.zero;
        GetComponent<BoxCollider2D>().size = new Vector2( 0.5f, 0.5f );

        if( GetComponent<SpriteRenderer>().sprite == boosts[0] )  //color ball boost
        {
            DestroyAround();
            
        }
        if (pulltoMesh)
        {
        //    Debug.Log("Pull To Mesh---------------*********");
            StartCoroutine(pullToMesh(otherBall));
        }

        }

    
    void DestroyAround()
    {
        ArrayList b = new ArrayList();
        b.Add( gameObject );
        int layerMask = 1 << LayerMask.NameToLayer( "Ball" );
        Collider2D[] meshes = Physics2D.OverlapCircleAll( transform.position, 1f, layerMask );
        foreach( Collider2D obj1 in meshes )
        {
            GameObject obj = obj1.gameObject;
            if( !findInArray( b, obj ) && obj.tag != "chicken" && !obj.tag.Contains( "animal" ) && !obj.tag.Contains( "empty" ) )
            {
                b.Add( obj );
            }
        }
        if( b.Count >= 0 )
        {
            mainscript.Instance.ComboCount++;
            destroy( b, 0.001f );
        }

    }

    void DestroyLine()
    {

        ArrayList b = new ArrayList();
        int layerMask = 1 << LayerMask.NameToLayer( "Ball" );
        RaycastHit2D[] fixedBalls = Physics2D.LinecastAll( transform.position + Vector3.left * 10, transform.position + Vector3.right * 10, layerMask );
        int i = 0;
        foreach( RaycastHit2D item in fixedBalls )
        {
                if( !findInArray( b, item.collider.gameObject ) )
                {
                    b.Add( item.collider.gameObject );
                }
        }


        if( b.Count >= 0 )
        {
            mainscript.Instance.ComboCount++;
            mainscript.Instance.destroy( b);
        }

        mainscript.Instance.StartCoroutine( mainscript.Instance.destroyAloneBall() );
    }


    public void CheckBallCrossedBorder()
    {
        if( Physics2D.OverlapCircle( transform.position, 0.1f, 1 << 14 ) != null || Physics2D.OverlapCircle( transform.position, 0.1f, 1 << 17 ) != null )
        {
            DestroySingle( gameObject, 0.00001f );
        }

    }

    void triggerEnter()//direction changes of shooted ball
    {
      //  print(" trigger  0"+ "  form .position " + transform.position.y + " Bottom border  " + bottomBorder + " target y" + target.y);
     

        // check if we collided with a bottom block and adjust our speed and rotation accordingly
        if ( transform.position.y <= bottomBorder && target.y < 0 )
        {

          //  print(" trans form .position "+ transform.position.y+" Bottom border  "+ bottomBorder+" target y"+ target.y);
           // print(" trigger  1");

            growUp();
            StopBall( false );

           // target = new Vector2( target.x, target.y * -1 );
        }
        else
        {

            //// check if we collided with a left block and adjust our speed and rotation accordingly
            if( transform.position.x <= leftBorder && target.x < 0 && !touchedSide && fireBall )
            {
             //   print(" trigger  2");

                //  touchedSide = true;
                Invoke( "CanceltouchedSide", 0.1f );
                 target = new Vector2( target.x * -1, target.y );
                GetComponent<Rigidbody2D>().velocity = new Vector2( GetComponent<Rigidbody2D>().velocity.x * -1, GetComponent<Rigidbody2D>().velocity.y );
            }
            // check if we collided with a right block and adjust our speed and rotation accordingly
            if( transform.position.x >= rightBorder && target.x > 0 && !touchedSide && fireBall )
            {
              //  print(" trigger  3");

                //  touchedSide = true;
                Invoke( "CanceltouchedSide", 0.1f );
                 target = new Vector2( target.x * -1, target.y );
                GetComponent<Rigidbody2D>().velocity = new Vector2( GetComponent<Rigidbody2D>().velocity.x * -1, GetComponent<Rigidbody2D>().velocity.y );
            }
//             check if we collided with a right block and adjust our speed and rotation accordingly
            if( transform.position.y >= topBorder && target.y > 0 && LevelData.mode == ModeGame.Rounded && !touchedTop )
            {
               // print(" trigger  4");

                touchedTop = true;
                // target = new Vector2( target.x, -target.y );
                GetComponent<Rigidbody2D>().velocity = new Vector2( GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y * -1 );
                //         print( target.y );
            }

        }



    }

    void CanceltouchedSide()
    {
        touchedSide = false;

    }

    public void destroy( ArrayList b, float speed = 0.1f )
    {
        StartCoroutine( DestroyCor( b, speed ) );//used fdor destruction of balls
    }

    IEnumerator DestroyCor( ArrayList b, float speed = 0.1f )
    {
        
        ArrayList l = new ArrayList();
        foreach( GameObject obj in b )
        {
            l.Add( obj );
           
        }
        Debug.Log("destructions..!");
        Camera.main.GetComponent<mainscript>().bounceCounter = 0;
        int scoreCounter = 0;
        int rate = 0;
        int soundPool = 0;
        foreach( GameObject obj in l )
        {
            if( obj == null ) continue;
            if( obj.name.IndexOf( "ball" ) == 0 ) obj.layer = 0;
            //GameObject obj2 = findInArrayGameObject( b, obj );
            //if(obj2 != null)
            obj.GetComponent<ball>().growUp();
            soundPool++;
            GetComponent<Collider2D>().enabled = false;
            if( scoreCounter > 3 )
            {
                rate += 10;
                scoreCounter += rate;
            }
            scoreCounter += 10;
            if( b.Count > 10 && Random.Range( 0, 10 ) > 5 ) mainscript.Instance.perfect.SetActive( true );
            obj.GetComponent<ball>().Destroyed = true;
            //		Destroy(obj);

            //  Camera.main.GetComponent<mainscript>().explode( obj.gameObject );
            if(b.Count<10 || soundPool % 20 == 0)
                yield return new WaitForSeconds( speed );

            //			Destroy(obj);
        }
        //if (name.IndexOf("bug") < 0)
        //    Score.Instance.addScore(scoreCounter);
        mainscript.Instance.PopupScore( scoreCounter, transform.position );
     //   StartCoroutine( mainscript.Instance.destroyAloneBall() );

    }

    void DestroySingle( GameObject obj, float speed = 0.1f )//used to destroy single ball
    {
        Camera.main.GetComponent<mainscript>().bounceCounter = 0;
        int scoreCounter = 0;
        int rate = 0;
        int soundPool = 0;
        if( obj.name.IndexOf( "ball" ) == 0 ) obj.layer = 0;
        obj.GetComponent<ball>().growUp();
        soundPool++;

        if( obj.tag == "light" )
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot( SoundBase.Instance.spark );
            obj.GetComponent<ball>().DestroyLine();
        }

        if( scoreCounter > 3 )
        {
            rate += 10;
            scoreCounter += rate;
        }
        scoreCounter += 10;
        obj.GetComponent<ball>().Destroyed = true;
        mainscript.Instance.PopupScore( scoreCounter, transform.position );

    }

    public void SplashDestroy()
    {
        if (setTarget) mainscript.Instance.newBall2 = null;
        Destroy( gameObject );
    }

    public void destroy()
    {
        growUpPlaySound();
        destroy( gameObject );
    }

    public void destroy( GameObject obj )
    {
        if( obj.name.IndexOf( "ball" ) == 0 ) obj.layer = 0;

        Camera.main.GetComponent<mainscript>().bounceCounter = 0;
        //	collider.enabled = false;
        obj.GetComponent<ball>().destroyed = true;
        //	Destroy(obj);
        //obj.GetComponent<ball>().growUpPlaySound();
        obj.GetComponent<ball>().growUp();
        //	Invoke("playPop",1/(float)Random.Range(2,10));
        Camera.main.GetComponent<mainscript>().explode( obj.gameObject );
        //     if (name.IndexOf("bug") < 0)
        //       Score.Instance.addScore(3);

    }

    public void growUp()
    {
        StartCoroutine( explode() );
    }

    public void growUpPlaySound()
    {
        Invoke( "growUpDelayed", 1 / (float)Random.Range( 2, 10 ) );
    }

    public void growUpDelayed()
    {
        StartCoroutine( explode() );
    }

    void playPop()
    {
    }


    IEnumerator explode()
    {

        float startTime = Time.time;
        float endTime = Time.time + 0.1f;
        Vector3 tempPosition = transform.localScale;
        Vector3 targetPrepare = transform.localScale * 1.2f;

        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;


        while( !isPaused && endTime > Time.time )
        {
            //transform.position  += targetPrepare * Time.deltaTime;
            transform.localScale = Vector3.Lerp( tempPosition, targetPrepare, ( Time.time - startTime ) * 10 );
            //	transform.position  = targetPrepare ;
            yield return new WaitForEndOfFrame();
        }
        //      yield return new WaitForSeconds(0.01f );
        GameObject prefab = Resources.Load( "Particles/BubbleExplosion" ) as GameObject;

        GameObject explosion = (GameObject)Instantiate( prefab, gameObject.transform.position + Vector3.back * 20f, Quaternion.identity );
        if( mesh != null )
            explosion.transform.parent = mesh.transform;
        //   if( !isPaused )
        CheckNearCloud();

        if( LevelData.mode == ModeGame.Vertical && isTarget )
        {
            Instantiate( Resources.Load( "Prefabs/TargetStar" ), gameObject.transform.position, Quaternion.identity );
        }
        else if( LevelData.mode == ModeGame.Animals && isTarget )
        {
           // Instantiate( Resources.Load( "Prefabs/TargetStar" ), gameObject.transform.position, Quaternion.identity );
        }
        Destroy( gameObject, 1 );
       for (int i = 0; i < 100; i++)
        {
            if (GamePlay.Instance.candidatesForSpreadingBGum[i]==gameObject) { GamePlay.Instance.candidatesForSpreadingBGum[i] = null;/* Debug.Log("b contains our candidate::" + i + "-----" + GamePlay.Instance.candidatesForSpreadingBGum[i]);*/ };
        }
    }
    void CheckNearCloud()
    {
        int layerMask = 1 << LayerMask.NameToLayer( "Ball" );
        Collider2D[] meshes = Physics2D.OverlapCircleAll( transform.position, 1f, layerMask );
        foreach( Collider2D obj1 in meshes )
        {
            if( obj1.gameObject.tag == "cloud" )
            {
                GameObject obj = obj1.gameObject;
                float distTemp = Vector3.Distance( transform.position, obj.transform.position );
                if( distTemp <= 1f )
                {
                    obj.GetComponent<ColorBallScript>().ChangeRandomColor();
                }
            }
        }

    }

    public void ShowFirework()
    {
        fireworks++;
        if( fireworks <= 2 )
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot( SoundBase.Instance.hit );

    }




}
