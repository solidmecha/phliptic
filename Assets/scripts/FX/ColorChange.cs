using UnityEngine;
using System.Collections;

public class ColorChange : MonoBehaviour {

    float Counter;
    public int[] ColorIndex;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Counter += Time.deltaTime;
        GetComponent<SpriteRenderer>().color = Color.Lerp(GameControl.singleton.SideColors[ColorIndex[0]], GameControl.singleton.SideColors[ColorIndex[1]], Counter * 2);
        if(Counter>=.5f)
        {
            GetComponent<SpriteRenderer>().color = GameControl.singleton.SideColors[ColorIndex[1]];
            Destroy(this);
        }
	
	}
}
