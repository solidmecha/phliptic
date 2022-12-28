using UnityEngine;
using System.Collections.Generic;

public class BotScript : MonoBehaviour {
    public static BotScript singleton;

    public void TakeTurn()
    {
        List<PieceScript> AvailablePieces = new List<PieceScript> { };
        List<Vector2> Locs=new List<Vector2> { };
        foreach (PieceScript p in GameControl.singleton.Pieces)
            if (p.inHand)
                AvailablePieces.Add(p);
        RaycastHit2D hit;
        for (int x = 0; x < GameControl.singleton.MaxWidth; x++)
        {
            for (int y = 0; y < GameControl.singleton.MaxHeight; y++)
            {
                Vector2 v = (Vector2)transform.position + new Vector2(x * GameControl.singleton.TileOffsetCoefficient, y * GameControl.singleton.TileOffsetCoefficient);
                hit = Physics2D.Raycast(v, Vector2.zero);
                if (hit.collider !=null && !hit.collider.CompareTag("Player"))
                {
                    Locs.Add(v);
                }
            }
        }
        if (AvailablePieces.Count > 0)
        {
           int r= GameControl.singleton.RNG.Next(AvailablePieces.Count);
            Vector2 v = Locs[GameControl.singleton.RNG.Next(Locs.Count)];
            hit = Physics2D.Raycast(v, Vector2.zero);
            AvailablePieces[r].Tile = hit.collider.GetComponent<TileScript>();
            hit.collider.enabled = false;
            AvailablePieces[r].transform.position = v;
            AvailablePieces[r].inHand = false;
            AvailablePieces[r].Flip();
            GameControl.singleton.HandlePlacement(AvailablePieces[r]);
        }
    }

    private void Awake()
    {
        singleton = this;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
