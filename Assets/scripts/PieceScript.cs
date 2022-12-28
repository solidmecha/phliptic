using UnityEngine;
using System.Collections;

public class PieceScript : MonoBehaviour {

    public bool isBlue;
    public int[] Values; //top bottom Left Right
    public int[] Specials;
    public bool inHand;
    public TileScript Tile;

    public void Fight(PieceScript P, int index)
    {
        if (Values[index] != -1 && P.Values[GameControl.singleton.FightIndex[index]] != -1)
        {
            if (Values[index] == P.Values[GameControl.singleton.FightIndex[index]] && GameControl.singleton.AtkTiebreak)
            {
                P.Flip();
                if (GameControl.singleton.ComboChain)
                    GameControl.singleton.HandlePlacement(P);
            }
            else if (Values[index] > P.Values[GameControl.singleton.FightIndex[index]])
            {
                P.Flip();
                if (GameControl.singleton.ComboChain)
                    GameControl.singleton.HandlePlacement(P);
            }
        }
    }

    public void UpdateValues(int[] vals)
    {
        for (int i = 0; i < Values.Length; i++)
        {
            Values[i] = vals[i];
            if(Values[i]>0)
                transform.GetChild(1).GetChild(i).GetComponent<UnityEngine.UI.Text>().text = Values[i].ToString() + GameControl.singleton.SpecialChar(Specials[i]);
         }
    }

    public void ReplaceTile(TileScript T)
    {
        Tile.GetComponent<BoxCollider2D>().enabled = true;
        Tile = T;
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
