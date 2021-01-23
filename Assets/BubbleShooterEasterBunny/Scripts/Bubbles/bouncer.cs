using UnityEngine;
using System.Collections;

public class bouncer : MonoBehaviour
{
    Vector3 tempPosition;
    Vector3 targetPrepare;
    bool isPaused;
    public bool isEaterBubble;
    public int chainNo;
    public int chainLevel;
    public bool isShooter;
    //public int DoubleColor;
    public int bGumNo;
    public bool isMultiColor;
    public bool doesExecuteChangeColor;
    public ArrayList ballcolors = new ArrayList();
    public ArrayList colorForNearsetMulticolor = new ArrayList(); // for sroring multicolor nearest color
  // to store candidates where bgum can be spread
    public int noOfColorForMultiColor;
    public int multicolorChangeTracker;
    public bool doesChangeColor;
    public bool callChangeColor;
    //public bool isShooterBubbleDisabled;
    public bool startBounce;
    float startTime;
    public float offset;
    public ArrayList nearBalls = new ArrayList();
    //	private OTSpriteBatch spriteBatch = null;  
    GameObject Meshes;
    public int countNEarBalls;
    float gameOverBorder;

    /**********************************/

    /* private int chickenRoteteInMoves {
         get { return chickenRoteteInMoves; }
         set {  }
     }*/

    // Use this for initialization
    /** public void ChickenLevelRotator() {
         if (gameObject.tag == "chicken")
         {
             var dir = rigidbody2D.velocity;
             var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
             rigidbody2D.MoveRotation(angle);
         }
     }*/
    void Start()
    {
        this.multicolorChangeTracker = 0;
        gameObject.GetComponent<bouncer>().colorForNearsetMulticolor.Clear();
        isPaused = Camera.main.GetComponent<mainscript>().isPaused;
        gameOverBorder = Camera.main.GetComponent<mainscript>().gameOverBorder;
        targetPrepare = transform.position;
        isShooter = false;
    }

    // Update is called once per frame
    /*void Update()

    {
        if (gameObject.GetComponent<bouncer>().isMultiColor ) {
          
//           Debug.Log("is multicolor----");
            // if(multicolorChangeTracker ==0)
            //  StartCoroutine(  ChangeMulticolorColor());
            if (gameObject.GetComponent<bouncer>().doesChangeColor== true) {
                Debug.Log("Calling Change Color"+ GetComponent<bouncer>().doesChangeColor+" ball:" + GetComponent<bouncer>().name);
                gameObject.GetComponent<bouncer>().CheckNearestColorForMulticolor();
                gameObject.GetComponent<bouncer>().ChangeMulticolorColor();
                gameObject.GetComponent<bouncer>().doesChangeColor = false;
                Debug.Log("falsed :" + GetComponent<bouncer>().name);
                Debug.Log(colorForNearsetMulticolor.Count+"-----"+gameObject.name);
            }

        }


    }*/


    public void ChangeMulticolorColor() {

        // if (gameObject.GetComponent<bouncer>().doesExecuteChangeColor)
        {
            foreach (BallColor b in colorForNearsetMulticolor) {
                Debug.Log("------/" + b);
            }
            if (this.multicolorChangeTracker >= this.noOfColorForMultiColor)
            {
                this.multicolorChangeTracker = 0;
            }

            //       Debug.Log( colorForNearsetMulticolor[multicolorChangeTracker].ToString());
            if (this.noOfColorForMultiColor > 1)
            {
               // Debug.Log("Index for changiong multicolor -" + multicolorChangeTracker + "-Limit should be -" + this.colorForNearsetMulticolor.Count);
                //Debug.Log((BallColor)System.Enum.Parse(typeof(BallColor), this.colorForNearsetMulticolor[this.multicolorChangeTracker].ToString()));
                gameObject.GetComponent<ColorBallScript>().SetColor((BallColor)System.Enum.Parse(typeof(BallColor), this.colorForNearsetMulticolor[this.multicolorChangeTracker].ToString()));

//                Debug.Log("Index for changiong multicolor -"+multicolorChangeTracker+"-Limit should be -"+this.colorForNearsetMulticolor.Count);
            }
            // if (gameObject.GetComponent<bouncer>().doesExecuteChangeColor)
            //{
            multicolorChangeTracker += 1;
            //     gameObject.GetComponent<bouncer>().doesExecuteChangeColor = false;
            // }
            // else
            //    gameObject.GetComponent<bouncer>().doesExecuteChangeColor = true;
           

            Debug.Log("Does Single" + " ball:" + gameObject.name);
            //gameObject.GetComponent<bouncer>().doesExecuteChangeColor = false;
        }
        //  else
        // gameObject.GetComponent<bouncer>().doesExecuteChangeColor = true;

    }

    public void CheckNearestForBubbleGum()
    {
//        Debug.Log("Call To connect near for bubble");
      
        int layerMask = 1 << LayerMask.NameToLayer("Ball");
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, layerMask);
       
            foreach (Collider2D col in fixedBalls)
            {
            if (col.GetComponentInParent<bouncer>().bGumNo == 0 && col.GetComponentInParent < ball>().Destroyed==false  && col.GetComponentInParent<bouncer>().chainNo == 0 && !System.Array.Exists(GamePlay.Instance.avoidToChangeInMulticolor, element => element == col.tag))
                {
                    bool flag = true;
                    foreach (GameObject obj in GamePlay.Instance.candidatesForSpreadingBGum)
                    {
                        if (obj != null)
                        {
                            if (obj.name == col.name) { flag = false; break; }
                        }
                    }
                    if (flag)
                    {
                    //  Debug.Log("Locho in loop");
                    for (int i = 0; i < 100; i++) { if (GamePlay.Instance.candidatesForSpreadingBGum[i] == null) { GamePlay.Instance.candidatesForSpreadingBGum[i] = GameObject.Find(col.name);break; } }// Debug.Log(GamePlay.Instance.candidatesForSpreadingBGum[i]); }
                    }
                }

            }
        
        
        
    }
    public static void SpreadBubbleGum() {
        //for (int i = 0; i < 100 && GamePlay.Instance.candidatesForSpreadingBGum[i]!=null && GamePlay.Instance.candidatesForSpreadingBGum[i].GetComponent<ball>().Destroyed==false; i++) {  Debug.Log(GamePlay.Instance.candidatesForSpreadingBGum[i]+"-----"+i); GamePlay.Instance.candidatesForSpreadingBGum[i].transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = true; }
         GameObject[] fixedBallsForAddingNoOFBGum = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        LevelData.allBGumNo.Clear();
        foreach (GameObject obj in fixedBallsForAddingNoOFBGum) {

            if ( obj.layer == 9 && obj.GetComponent<bouncer>().bGumNo != 0)
            {
                if (!LevelData.allBGumNo.Contains(obj.GetComponent<bouncer>().bGumNo)) { LevelData.allBGumNo.Add(obj.GetComponent<bouncer>().bGumNo); }
            }
        }
        foreach (int bgno in LevelData.allBGumNo) {
            ArrayList temp = new ArrayList();
            for(int j = 0; j < 100; j++) {
                if (GamePlay.Instance.candidatesForSpreadingBGum[j] != null) {
                    // if (GamePlay.Instance.candidatesForSpreadingBGum[i].GetComponent<bouncer>().bGumNo == i)
                    int layerMask = 1 << LayerMask.NameToLayer("Ball");
                    Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(GamePlay.Instance.candidatesForSpreadingBGum[j].transform.position, 0.5f, layerMask);
                    foreach(Collider2D col in fixedBalls)
                    {
                        if (col.GetComponentInParent<bouncer>().bGumNo == bgno)
                        {
                            temp.Add(GamePlay.Instance.candidatesForSpreadingBGum[j]);
                          //  Debug.Log("Added Bubble -"+ GamePlay.Instance.candidatesForSpreadingBGum[j].name+"-- and bgumno="+ bgno + "-----"+ col.GetComponentInParent<bouncer>().bGumNo);
                            break;
                        }
                    }
                }
            }

            //            Debug.Log("Random and actual values are::" + temp.Count + "-----" + Random.Range(0, temp.Count));
            if (temp.Count!=0) {
                GameObject objToSpread = (GameObject)temp[Random.Range(0, temp.Count)];
                objToSpread.GetComponent<bouncer>().bGumNo = bgno;
                objToSpread.transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = true; }
        }


    }
    public void CheckNearestColorForMulticolor()
    {
        //        Debug.Log("workingg...1");
        // gameObject.GetComponent<bouncer>().colorForNearsetMulticolor.Clear();
        ArrayList toremove = new ArrayList();
        int counter = 0;
        int layerMask = 1 << LayerMask.NameToLayer("Ball");
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, layerMask);

       /* for (int i = 0; i < colorForNearsetMulticolor.Count; i++)
        {
            if (colorForNearsetMulticolor[i] == null)
            {
                colorForNearsetMulticolor.Remove(colorForNearsetMulticolor[i]);
                print("-=-=-=-=->null");
            }
        }*/
        //colorForNearsetMulticolor.Add(gameObject);
        Vector3 distEtalon = transform.localScale;
        //GameObject[] meshes = GameObject.FindGameObjectsWithTag( tag );
        foreach (BallColor colorToRemove in colorForNearsetMulticolor) {
            bool RemoveFlag = true ;
            //if(System.Array.Exists(fixedBalls, element => (BallColor)System.Enum.Parse(typeof(BallColor), element.tag.ToString())) == colorToRemove))
            foreach (Collider2D col in fixedBalls) {

                if ((BallColor)System.Enum.Parse(typeof(BallColor), col.gameObject.tag) == colorToRemove)
                {

                    RemoveFlag = false; Debug.Log("Gatting True"+col.gameObject.name);
                }
                else if (col.GetComponentInParent<bouncer>().ballcolors.Contains(colorToRemove)) {
                    RemoveFlag = false; Debug.Log("Gatting True"+col.gameObject.name);
                }
               
            }
            if (RemoveFlag) {
               toremove.Add(colorToRemove);

                // colorForNearsetMulticolor.Remove(colorToRemove);
               
            }

        }
        foreach (BallColor color in toremove) {
            colorForNearsetMulticolor.Remove(color);
            noOfColorForMultiColor -= 1;
            Debug.Log("Gatting Removed from arraylist");
        }
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
                if (!gameObject.GetComponent<bouncer>().colorForNearsetMulticolor.Contains((BallColor)System.Enum.Parse(typeof(BallColor), obj.gameObject.tag)) && obj.GetComponentInParent<bouncer>().chainNo == 0 && obj.GetComponentInParent<bouncer>().bGumNo == 0 && !System.Array.Exists(GamePlay.Instance.avoidToChangeInMulticolor, element => element == obj.tag))
                {
                 //   Debug.Log("-*-**-/*-/-*/-*/-*/-/-*/-/-/-/"+ obj.GetComponentInParent<bouncer>().DoubleColor);
                   //   Debug.Log(tag + "==from inside bouncer" + obj.gameObject.GetComponent<bouncer>().DoubleColor + "------------obj value" + obj.gameObject.tag+"000"+ (BallColor)System.Enum.Parse(typeof(BallColor), obj.gameObject.tag));
                    gameObject.GetComponent<bouncer>().colorForNearsetMulticolor.Add((BallColor)System.Enum.Parse( typeof(BallColor),obj.gameObject.tag));
                    if (obj.GetComponentInParent<bouncer>().ballcolors.Count != 0)
                    {
                        //Debug.Log("Entered 2 Colors");
                         BallColor b;
//                        b = BallColor.blue;
                        /*switch (obj.GetComponentInParent<bouncer>().DoubleColor)
                        {
                            case 1:
                                b = BallColor.blue;
                                break;
                            case 2:
                                b= BallColor.green;
                                break;
                            case 3:
                                b = BallColor.red;
                                break;
                            case 4:
                                b = BallColor.violet;
                                break;
                            case 5:
                                b= BallColor.yellow;
                                break;
                                Debug.Log("-------"+b);

                        }*/

                        foreach (BallColor bColor in obj.GetComponentInParent<bouncer>().ballcolors)
                        {
                            b = bColor;
                            if (!gameObject.GetComponent<bouncer>().colorForNearsetMulticolor.Contains(b))
                            {
                                // Debug.Log("Adding 2 secondary"+b);
                                gameObject.GetComponent<bouncer>().colorForNearsetMulticolor.Add(b);

                            }
                        }
                    }

                }
                }
            

        }

        gameObject.GetComponent<bouncer>().noOfColorForMultiColor = gameObject.GetComponent<bouncer>().colorForNearsetMulticolor.Count;
        if (1 == this.noOfColorForMultiColor)
        {
            this.multicolorChangeTracker = 0;
        }


    }

        IEnumerator bonceCoroutine()
    {

        while (Vector3.Distance(transform.position, targetPrepare) > 1 && !isPaused && !GetComponent<ball>().setTarget)
        {
            //transform.position  += targetPrepare * Time.deltaTime;
            transform.position = Vector3.Lerp(tempPosition, targetPrepare, (Time.time - startTime) * 2f);
            //	transform.position  = targetPrepare ;
            yield return new WaitForSeconds(1f / 30f);
        }

    }
     /*void Update()
    {
       /* if (Camera.main.GetComponent<mainscript>().chickenRoteteInMoves == 0) {

            ChickenLevelRotator();
        } */


  /*  }*/
  /*  private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.tag == "eater")
        {
            Debug.Log("Eater is working..!"+collision.gameObject.tag.ToString());
            GameObject eatingBubble = collision.gameObject;
            eatingBubble.
            

            
        }
    }*/
    IEnumerator bonceToCatapultCoroutine()
    {

        /*	while (Vector3.Distance(transform.position, targetPrepare)>1 && !isPaused && !GetComponent<ball>().setTarget ){
                //transform.position  += targetPrepare * Time.deltaTime;
                transform.position = Vector3.Lerp(tempPosition, targetPrepare,  (Time.time - startTime)*2);
                //	transform.position  = targetPrepare ;
                yield return new WaitForSeconds(1f/5f);
            }
            if(!isPaused)*/
        Invoke("delayedBonceToCatapultCoroutine", 0.5f);
        yield return new WaitForSeconds(1f / 5f);
    }

    void delayedBonceToCatapultCoroutine()
    {
        transform.position = targetPrepare;
        GetComponent<ball>().newBall = true;

    }

    void newBall()
    {
        GetComponent<ball>().newBall = true;
        Grid.waitForAnim = false;
    }

    public void bounceToCatapult(Vector3 vector3)
    {
        vector3 = new Vector3(vector3.x, vector3.y, gameObject.transform.position.z);
        tempPosition = transform.position;
        targetPrepare = vector3;
        startBounce = true;
        startTime = Time.time;
        iTween.MoveTo(gameObject, iTween.Hash("position", vector3, "time", 0.3, "easetype", iTween.EaseType.linear, "onComplete", "newBall"));
        //		StartCoroutine(bonceToCatapultCoroutine());
        //transform.position = vector3;
        Grid.waitForAnim = false;

    }

    public void bounceTo(Vector3 vector3)
    {
        vector3 = new Vector3(vector3.x, vector3.y, gameObject.transform.position.z);
        tempPosition = transform.position;
        targetPrepare = vector3;
        startBounce = true;
        startTime = Time.time;
        if( GamePlay.Instance.GameStatus == GameState.Playing )
            iTween.MoveTo(gameObject, iTween.Hash("position", vector3, "time", 0.3, "easetype", iTween.EaseType.linear));
        else if( GamePlay.Instance.GameStatus == GameState.Win )
            iTween.MoveTo(gameObject, iTween.Hash("position", vector3, "time", 0.00001, "easetype", iTween.EaseType.linear));
        //StartCoroutine(bonceCoroutine());
        //transform.position = vector3;
    }

    public void dropDown()
    {
        Vector3 v;

        //		GameObject[] meshes = GameObject.FindGameObjectsWithTag("Mesh");
        //		foreach(GameObject obj in meshes) {
        int layerMask = 1 << LayerMask.NameToLayer("Mesh");
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, layerMask);
        foreach (Collider2D obj in fixedBalls)
        {
            float distTemp = Vector3.Distance(new Vector3(transform.position.x - offset, transform.position.y, transform.position.z), obj.transform.position);
            if (distTemp <= 0.9f && obj.transform.position.y + 0.1f < transform.position.y)
            {
                if (obj.GetComponent<Grid>().offset > 0)
                {
                    v = new Vector3(transform.position.x + obj.GetComponent<Grid>().offset, obj.transform.position.y, transform.position.z);
                }
                else
                {
                    v = new Vector3(obj.transform.position.x, obj.transform.position.y, transform.position.z);
                }
                bounceTo(v);
                //	transform.position = v;
                return;
            }
        }

    }

    public bool checkNearestBall(ArrayList b)
    {
        if (transform.position.y >= 530f / 640f * Camera.main.orthographicSize)
        {
            Camera.main.GetComponent<mainscript>().controlArray = addFrom(b, Camera.main.GetComponent<mainscript>().controlArray);
            b.Clear();
            return true;    /// don't destroy
        }
        if (findInArray(Camera.main.GetComponent<mainscript>().controlArray, gameObject)) { b.Clear(); return true; } /// don't destroy
        b.Add(gameObject);
        foreach (GameObject obj in nearBalls)
        {
            if (obj.gameObject.layer == 9 && obj != gameObject)
            {
                //	if(findInArray(Camera.main.GetComponent<mainscript>().controlArray, obj.gameObject)){b.Clear(); return true;} /// don't destroy
                //	else{
                float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                if (distTemp <= 0.8f && distTemp > 0)
                {
                    if (!findInArray(b, obj.gameObject))
                    {
                        Camera.main.GetComponent<mainscript>().arraycounter++;
                        if (obj.GetComponent<bouncer>().checkNearestBall(b))
                            return true;
                    }
                }
                //		}
            }
        }
        return false;

    }

    public bool findInArray(ArrayList b, GameObject destObj)
    {
        foreach (GameObject obj in b)
        {

            if (obj == destObj) return true;
        }
        return false;
    }

    public ArrayList addFrom(ArrayList b, ArrayList b2)
    {
        foreach (GameObject obj in b)
        {
            if (!findInArray(b2, obj))
            {
                b2.Add(obj);
            }
        }
        return b2;
    }

    public void connectNearBalls()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ball");
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, layerMask);
        nearBalls.Clear();
        foreach (Collider2D obj in fixedBalls)
        {
            if (nearBalls.Count <= 7)
                nearBalls.Add(obj.gameObject);
        }
        countNEarBalls = nearBalls.Count;
    }

    public void checkNextNearestColor(ArrayList b, int counter , string tag1)
    {
        //		Debug.Log(b.Count);
        Vector3 distEtalon = transform.localScale;
        //		GameObject[] meshes = GameObject.FindGameObjectsWithTag(tag);
        //		foreach(GameObject obj in meshes) {
        int layerMask = 1 << LayerMask.NameToLayer("Ball");
        Collider2D[] meshes = Physics2D.OverlapCircleAll(transform.position, 1f, layerMask);

        foreach (Collider2D obj1 in meshes)
        {
            if ((obj1.gameObject.tag == tag1 || /*(BallColor)obj1.gameObject.GetComponent<bouncer>().DoubleColor == (BallColor)System.Enum.Parse(typeof(BallColor), tag1) */ obj1.GetComponent<bouncer>().ballcolors.Contains((BallColor)System.Enum.Parse(typeof(BallColor), tag1))) &&  obj1.GetComponentInParent<bouncer>().bGumNo == 0)///* && !obj1.gameObject.GetComponent<bouncer>().isShooterBubbleDisabled */ && obj1.gameObject.tag != "stone")
            {
                //Debug.Log(obj1.gameObject.GetComponent<bouncer>().DoubleColor);
                GameObject obj = obj1.gameObject;
                float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                if (distTemp <= 1f)
                {
                    if (!findInArray(b, obj) && obj.GetComponent<bouncer>().bGumNo == 0 && obj.GetComponent<bouncer>().chainNo == 0 && !System.Array.Exists(GamePlay.Instance.avoidToChangeInMulticolor, element => element == obj.tag.ToString()))
                    {
                        counter++;
                        b.Add(obj);
                        obj.GetComponent<ball>().checkNextNearestColor(b, counter,tag1);
                        //		destroy();
                        //obj.GetComponent<mesh>().checkNextNearestColor();
                        //		obj.GetComponent<mesh>().destroy();
                    }
                }
            }
        }
    }

}
