using UnityEngine;
using System.Collections.Generic;

public class GameControl : MonoBehaviour {

    public static GameControl singleton;
    public GameObject Piece;
    public GameObject Tile;
    public Color[] SideColors;
    public Sprite[] ArtworkSprites;
    public int MaxWidth;
    public int MaxHeight;
    public int TileOffsetCoefficient;
    public TileScript SelectedTile;
    public PieceScript SelectedPiece;
    public enum GameState { PlayerTurn, MovingPiece, FX};
    public GameState CurrentState;
    public int[] FightIndex;
    Vector2[] ReferenceVectors = new Vector2[8] { Vector2.up*2, Vector2.down*2, Vector2.left*2, Vector2.right*2,
    Vector2.left*2+Vector2.up*2, Vector2.right*2+Vector2.up*2, Vector2.left*2+Vector2.down*2, Vector2.right*2+Vector2.down*2 };
    public System.Random RNG;
    public List<PieceScript> Pieces;

    private void Awake()
    {
        singleton = this;
        RNG = new System.Random();
        FightIndex = new int[8] { 1, 0, 3, 2, 7, 6, 5, 4};
    }

    // Use this for initialization
    void Start () {
        for(int x=0;x<MaxWidth;x++)
        {
            for(int y=0;y<MaxHeight;y++)
            {
                Instantiate(Tile, (Vector2)transform.position+new Vector2(x * TileOffsetCoefficient, y * TileOffsetCoefficient), Quaternion.identity);
                MakePiece(RNG.Next(ArtworkSprites.Length), (Vector2)transform.position+new Vector2(-8, 0) + new Vector2(x * TileOffsetCoefficient, y * TileOffsetCoefficient));
            }
        }
	
	}

    public void MakePiece(int i, Vector2 v)
    {
        PieceScript p = (Instantiate(Piece, v, Quaternion.identity) as GameObject).GetComponent<PieceScript>();
        p.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = ArtworkSprites[i];
        int[] ia = new int[8];
        for (int j = 0; j < 8; j++)
        {
            ia[j] = 1+RNG.Next(9);
            if (RNG.Next(10) == 4)
                p.Specials[j] = RNG.Next(3);
        }
        p.UpdateValues(ia);
        Pieces.Add(p);
    }

    public void NextTurn()
    {
        BotScript.singleton.TakeTurn();
        Invoke("SetPlayerTurn", .5f);
    }

    public void SetPlayerTurn()
    {
        CurrentState = GameState.PlayerTurn;
    }

    public void HandlePlacement(PieceScript P)
    {
        RaycastHit2D hit;
        for (int i=0;i<8;i++)
        {
            hit = Physics2D.Raycast((Vector2)P.transform.position + ReferenceVectors[i], Vector2.zero);
            if(hit.collider!=null && hit.collider.CompareTag("Player"))
            {
                PieceScript HitPiece = hit.collider.GetComponent<PieceScript>();
                if (!HitPiece.inHand)
                {
                    if (P.isBlue != HitPiece.isBlue)
                    {
                        P.Fight(HitPiece, i);
                    }
                    else if (P.Specials[i] != 0)
                    {
                        switch (P.Specials[i])
                        {
                            case 1:
                                int[] ia = new int[8];
                                for (int j = 0; j < 8; j++)
                                    ia[j] = HitPiece.Values[j] + P.Values[i];
                                HitPiece.UpdateValues(ia);
                                break;
                            case 2:
                                hit = Physics2D.Raycast((Vector2)HitPiece.transform.position + ReferenceVectors[i], Vector2.zero);
                                if (hit.collider != null && !hit.collider.CompareTag("Player"))
                                {
                                    HitPiece.transform.position = hit.collider.transform.position;
                                    HandlePlacement(HitPiece);
                                }
                                break;
                        }
                    }
                }
            }
        }
    }

    public string SpecialChar(int s)
    {
        switch(s)
        {
            case 1:
                return "+";
            case 2:
                return "P";
            default:return "";
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(CurrentState==GameState.MovingPiece)
        {
            SelectedPiece.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if(Input.GetMouseButtonUp(0) && CurrentState==GameState.MovingPiece)
        {
            if(SelectedTile != null)
            {

                CurrentState = GameState.FX;
                SelectedTile.GetComponent<BoxCollider2D>().enabled = false;
                SelectedPiece.transform.position = SelectedTile.transform.position;
                SelectedPiece.GetComponent<BoxCollider2D>().enabled = true;
                HandlePlacement(SelectedPiece);
                SelectedPiece.inHand = false;
                SelectedPiece.isBlue = true;
                SelectedPiece.GetComponent<SpriteRenderer>().color=SideColors[0];
                Invoke("NextTurn", .5f);
                SelectedTile = null;
                SelectedPiece = null;
            }
        }
	
	}
}
