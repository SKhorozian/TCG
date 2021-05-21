using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonCell : MonoBehaviour
{
    Vector2 position;
    FieldCard fieldCard;

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
}
