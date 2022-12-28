using UnityEngine;
using UnityEngine.UI;
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
    public bool hasBot;
    public bool blueTurn;
    public int PlayerColor;
    public bool AtkTiebreak;
    public bool ComboChain;
    public bool hasPush;
    public bool hasPlus;
    public int NumberFreq;
    public int SpecialFreq;
    public Toggle[] Toggles;
    public Slider[] Sliders;
     
    private void Awake()
    {
        singleton = this;
        RNG = new System.Random();
        FightIndex = new int[8] { 1, 0, 3, 2, 7, 6, 5, 4};
    }

    // Use this for initialization
    void Start () {
	
	}

    public void BeginGame()
    {
        transform.GetChild(0).position = new Vector3(100, 100, -100);
        AtkTiebreak = Toggles[0].isOn;
        ComboChain = Toggles[1].isOn;
        hasPush = Toggles[2].isOn;
        hasPlus = Toggles[3].isOn;
        hasBot = Toggles[4].isOn;
        NumberFreq = (int)Sliders[0].value;
        SpecialFreq = (int)Sliders[1].value;
        List<int> SpriteIndex = new List<int> {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15};
        for (int x = 0; x < MaxWidth; x++)
        {
            for (int y = 0; y < MaxHeight; y++)
            {
                Instantiate(Tile, (Vector2)transform.position + new Vector2(x * TileOffsetCoefficient, y * TileOffsetCoefficient), Quaternion.identity);
                int r = RNG.Next(SpriteIndex.Count);
                MakePiece(SpriteIndex[r], (Vector2)transform.position + new Vector2(-8, 0) + new Vector2(x * TileOffsetCoefficient, y * TileOffsetCoefficient));
                SpriteIndex.RemoveAt(r);
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
            ia[j] = -1;
            if(RNG.Next(10)<NumberFreq)
                ia[j] = 1+RNG.Next(9);
            if (ia[j]!=-1 && RNG.Next(10) < SpecialFreq)
            {
                int r = RNG.Next(1, 3);
                if (r == 1 && hasPlus)
                    p.Specials[j] = r;
                else if (r == 2 && hasPush)
                    p.Specials[j] = r;
            }
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
                                {
                                    if (HitPiece.Values[j] > 0)
                                    {
                                        ia[j] = HitPiece.Values[j] + P.Values[i];
                                    }
                                    else
                                    {
                                        ia[j] = -1;
                                    }
                                }
                                HitPiece.UpdateValues(ia);
                                break;
                            case 2:
                                hit = Physics2D.Raycast((Vector2)HitPiece.transform.position + ReferenceVectors[i], Vector2.zero);
                                if (hit.collider != null && !hit.collider.CompareTag("Player"))
                                {
                                    HitPiece.transform.position = hit.collider.transform.position;
                                    HitPiece.ReplaceTile(hit.collider.GetComponent<TileScript>());
                                    hit.collider.enabled = false;
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
#if UNITY_WEBGL
        if (Input.GetMouseButtonUp(0) && CurrentState==GameState.MovingPiece)
        {
            if(SelectedTile != null)
            {

                CurrentState = GameState.FX;
                SelectedPiece.Tile = SelectedTile;
                SelectedTile.GetComponent<BoxCollider2D>().enabled = false;
                SelectedPiece.transform.position = SelectedTile.transform.position;
                SelectedPiece.GetComponent<BoxCollider2D>().enabled = true;
                SelectedPiece.inHand = false;
                SelectedPiece.isBlue = blueTurn;
                HandlePlacement(SelectedPiece);
                SelectedPiece.GetComponent<SpriteRenderer>().color=SideColors[PlayerColor];
                if (hasBot)
                    Invoke("NextTurn", .5f);
                else
                {
                    blueTurn = !blueTurn;
                    CurrentState = GameState.PlayerTurn;
                    if (blueTurn)
                        PlayerColor = 0;
                    else
                        PlayerColor = 1;
                }
                SelectedTile = null;
                SelectedPiece = null;
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
            Application.LoadLevel(0);
#endif
#if UNITY_ANDROID
    if (Input.touchCount==0 && CurrentState==GameState.MovingPiece)
        {
            if (SelectedTile != null)
            {

                CurrentState = GameState.FX;
                SelectedPiece.Tile = SelectedTile;
                SelectedTile.GetComponent<BoxCollider2D>().enabled = false;
                SelectedPiece.transform.position = SelectedTile.transform.position;
                SelectedPiece.GetComponent<BoxCollider2D>().enabled = true;
                SelectedPiece.inHand = false;
                SelectedPiece.isBlue = blueTurn;
                HandlePlacement(SelectedPiece);
                SelectedPiece.GetComponent<SpriteRenderer>().color = SideColors[PlayerColor];
                if (hasBot)
                    Invoke("NextTurn", .5f);
                else
                {
                    blueTurn = !blueTurn;
                    CurrentState = GameState.PlayerTurn;
                    if (blueTurn)
                        PlayerColor = 0;
                    else
                        PlayerColor = 1;
                }
                SelectedTile = null;
                SelectedPiece = null;
            }
        }
#endif

    }
}
