using UnityEngine;
using System.Collections;

public enum BallColor
{
    
    blue  = 1 ,
    green ,
    red ,
    violet ,
    yellow ,
    random ,
    chicken ,
    eater ,
    frozen ,
    stone ,
    rowRemover ,
    rowAdd
}

public class ColorBallScript : MonoBehaviour {
	public Sprite[] sprites;
    public BallColor mainColor;
	// Use this for initialization
	void Start () {
        
	}

    public void SetColor(BallColor color, int chainLevel=0)
    {
        /*if (chainLevel == 1) {
            foreach (Sprite item in sprites)
            {
                if (item.name == "Chain_LVL_1")
                {
                    GetComponent<SpriteRenderer>().sprite = item;
                    switch (color)
                    {
                        case BallColor.blue:
                            GetComponent<SpriteRenderer>().color = Color.blue;
                            break;
                        case BallColor.green:
                            GetComponent<SpriteRenderer>().color = Color.green;
                            break;
                        case BallColor.red:
                            GetComponent<SpriteRenderer>().color = Color.red;
                            break;
                        case BallColor.violet:
                            GetComponent<SpriteRenderer>().color = Color.black;
                            break;
                        case BallColor.yellow:
                            GetComponent<SpriteRenderer>().color = Color.yellow;
                            break;

                    }
                    
                    
                    SetSettings(color);
                    gameObject.tag = "" + color;

                }
            }
        }
        else if (chainLevel == 2) {
            foreach (Sprite item in sprites)
            {
                if (item.name == "Chain_LVL_2")
                {
                    GetComponent<SpriteRenderer>().sprite = item;
                    switch (color)
                    {
                        case BallColor.blue:
                            GetComponent<SpriteRenderer>().color = Color.blue;
                            break;
                        case BallColor.green:
                            GetComponent<SpriteRenderer>().color = Color.green;
                            break;
                        case BallColor.red:
                            GetComponent<SpriteRenderer>().color = Color.red;
                            break;
                        case BallColor.violet:
                            GetComponent<SpriteRenderer>().color = Color.black;
                            break;
                        case BallColor.yellow:
                            GetComponent<SpriteRenderer>().color = Color.yellow;
                            break;
                        

                    }


                    SetSettings(color);
                    gameObject.tag = "" + color;

                }
            }
        }
        else*/
        {
            mainColor = color;
            foreach (Sprite item in sprites)
            {
                if (item.name == "ball_" + color)
                {
                    GetComponent<SpriteRenderer>().sprite = item;
                    SetSettings(color);
                    gameObject.tag = "" + color;
                  //  gameObject.GetComponent<bouncer>().ballcolors.Add(mainColor);
                }
            }
        }
	}

    private void SetSettings( BallColor color )
    {
        if( color == BallColor.chicken )
        {
            if( LevelData.mode == ModeGame.Rounded )
            {

            }
        }
    }

    public void SetColor( int color )
    {
        mainColor = (BallColor)color;
        GetComponent<SpriteRenderer>().sprite = sprites[color];
      //  gameObject.GetComponent<bouncer>().ballcolors.Add(mainColor);
    }

    public void ChangeRandomColor()
    {
        mainscript.Instance.GetColorsInGame();
        SetColor( (BallColor)mainscript.colorsDict[Random.Range( 0, mainscript.colorsDict.Count )]);
        GetComponent<Animation>().Stop();
    }

	// Update is called once per frame
	void Update () {
        if (transform.position.y <= -16 && transform.parent == null) {  Destroy(gameObject); }
	}
}
