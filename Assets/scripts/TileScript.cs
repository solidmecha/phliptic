using UnityEngine;
using System.Collections;

public class TileScript : MonoBehaviour {

    private void OnMouseEnter()
    {
        GameControl.singleton.SelectedTile = this;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
