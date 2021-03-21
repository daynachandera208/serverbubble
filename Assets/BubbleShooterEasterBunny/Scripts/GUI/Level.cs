using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Level : MonoBehaviour {
    public int number;
    public Text label;
    public GameObject lockimage;

	
	public void GetData () {
        gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = number.ToString();
//        print(" Number ===+"+ number);
  //      print(" Score  ===+" + PlayerPrefs.GetInt("Score" + (number - 1)));

        if (PlayerPrefs.GetInt("Score" + (number - 1)) > 0 || number == 1)
        {
    //        print(" Score  ===+" + PlayerPrefs.GetInt("Score" + (number - 1)));

             lockimage.gameObject.SetActive( false );
           // gameObject.transform.GetChild(7).gameObject.SetActive(false);
            // label.text = "" + number;
        }
        else if (PlayerPrefs.GetInt("Score" + (number - 1)) == 0)
        {
            lockimage.gameObject.SetActive(true);
           // gameObject.transform.GetChild(7).gameObject.SetActive(false);

        }

        int stars = PlayerPrefs.GetInt( string.Format( "Level.{0:000}.StarsCount", number ), 0 );

        
        //Debug.Log(string.Format("Lavel.{0.000}.StarsCount", number));

        if ( stars > 0 )
        {
            for( int i = 1; i <= stars; i++ )
            {
                transform.Find( "Star" + i ).gameObject.SetActive( true );
            }

        }

	}
	
	

    public void StartLevel()
    {
        InitScriptName.InitScript.Instance.OnLevelClicked( number );
    }
}
