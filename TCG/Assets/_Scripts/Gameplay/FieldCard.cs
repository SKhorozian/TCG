using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public abstract class FieldCard : NetworkBehaviour, ITargetable
{
    [SerializeField] protected CardInstance card;
    [SerializeField] protected Player player;

    //Position, this is used for networking
    [SerializeField] public NetworkVariableVector3 position = new NetworkVariableVector3 (new NetworkVariableSettings {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    [SerializeField] protected HexagonCell cell;
    [SerializeField] protected SpriteRenderer icon;


    public CardInstance Card {get {return card;}}
    public Player Player {get {return player;}}
    public HexagonCell Cell {get {return cell;} set {if (IsServer) cell = value;}}

    public abstract void TurnStart ();
    public abstract void TurnEnd ();

}
