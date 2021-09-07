using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MLAPI;

public class HexagonCell : MonoBehaviour, ITargetable
{
    Vector2 position;
    [SerializeField] FieldCard fieldCard;
    [SerializeField] TextMeshPro cellCoordiantes;

    [SerializeField] Player conqueredPlayer;
    [SerializeField] CardColor color = CardColor.Colorless;

    [SerializeField] Color[] colors;

    [Space (10)]
    [SerializeField] Renderer tileRenderer;
    [SerializeField] Material outlineMaterial;

    public Vector2 Position {
        get {
            return position;
        }
        set {
            position = value;
        }
    }

    public FieldCard FieldCard {
        get {
            return fieldCard;
        }
        set {
            fieldCard = value;
        }
    }

    public Player ConqueredPlayer {get {return conqueredPlayer;}}
    public CardColor Color {get {return color;}}

    public void ConquerHex (CardColor color, Player player) {
        this.conqueredPlayer = player;
        this.color = color;

        UpdateMaterials ();
    }

    public void UpdateMaterials () {

        if (conqueredPlayer == null) {
            tileRenderer.material.color = UnityEngine.Color.grey;
        } else if (conqueredPlayer.OwnerClientId.Equals (NetworkManager.Singleton.LocalClientId)) {
            tileRenderer.material.color = UnityEngine.Color.green;
        } else {
            tileRenderer.material.color = UnityEngine.Color.red;
        }

        // switch (color) {
        //     case CardColor.Red:
        //         tileRenderer.material.color = colors[0];
        //     break;
        //     case CardColor.Blue:
        //         tileRenderer.material.color = colors[1];
        //     break;
        //     case CardColor.Black:
        //         tileRenderer.material.color = colors[2];
        //     break;
        //     case CardColor.Green:
        //         tileRenderer.material.color = colors[3];
        //     break;
        //     case CardColor.Yellow:
        //         tileRenderer.material.color = colors[4];
        //     break;
        //     case CardColor.Pink:
        //         tileRenderer.material.color = colors[5];
        //     break;
        //     case CardColor.Colorless:
        //         tileRenderer.material.color = colors[6];
        //     break;
        // }   
    }
    
    public void SetCoordinates (Vector2Int position) {
        cellCoordiantes.text = position.ToString ();
    }
}
