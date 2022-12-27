using UnityEngine;
using System.Collections;

public class PieceScript : MonoBehaviour {

    public bool isBlue;
    public int[] Values; //top bottom Left Right
    public int[] Specials;
    public bool inHand;

    public void Fight(PieceScript P, int index)
    {
        if(Values[index]>=P.Values[GameControl.singleton.FightIndex[index]])
        {
                P.Flip();
            GameControl.singleton.HandlePlacement(P);
        }
    }

    public void UpdateValues(int[] vals)
    {
        for (int i = 0; i < Values.Length; i++)
        {
            Values[i] = vals[i];
            transform.GetChild(1).GetChild(i).GetComponent<UnityEngine.UI.Text>().text = Values[i].ToString() + GameControl.singleton.SpecialChar(Specials[i]);
         }
    }

    public void Flip()
    {
        isBlue = !isBlue;
        if (isBlue)
        {
            gameObject.AddComponent<ColorChange>().ColorIndex = new int[2] { 1, 0 };
            transform.GetChild(0).gameObject.AddComponent<RotateIt>();
        }
        else
        {
            gameObject.AddComponent<ColorChange>().ColorIndex = new int[2] { 0, 1 };
            transform.GetChild(0).gameObject.AddComponent<RotateIt>();
        }

    }

    private void OnMouseDown()
    {
        if(inHand && GameControl.singleton.CurrentState==GameControl.GameState.PlayerTurn)
        {
            GameControl.singleton.SelectedPiece = this;
            GameControl.singleton.CurrentState = GameControl.GameState.MovingPiece;
            GetComponent<BoxCollider2D>().enabled = false;
        }

    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
