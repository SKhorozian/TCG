using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexagonCell : MonoBehaviour
{
    Vector2 position;
    [SerializeField] FieldCard fieldCard;
    [SerializeField] TextMeshPro cellCoordiantes;

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

    public void SetCoordinates (Vector2Int position) {
        cellCoordiantes.text = position.ToString ();
    }
}
