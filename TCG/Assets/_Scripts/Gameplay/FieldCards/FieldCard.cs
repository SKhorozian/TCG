using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
using System.Collections.Generic;

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

    [SerializeField] protected CardEffectTrigger[] effectTriggers;
    [SerializeField] protected ActionAbility[] actions;

    public void PerformAction (int n, Vector2[] targets) {
        if (IsServer) {
            if (n !< actions.Length) return;
            if (n > 0) return;

            ActionAbility action = Instantiate <ActionAbility> (actions[n]);
            
            List<ITargetable> newTargets = Targeting.ConvertTargets (action.TargetTypes, targets, player);

            if (action.TragetVaildity (newTargets, player)) {
                action.SetTargets (newTargets);
                action.DoEffect ();
            } else {Debug.Log("Action Ability failed targeting");};

        } else {
            PerformActionServerRPC (n, targets);
        }   
    }
    [ServerRpc]
    public void PerformActionServerRPC (int n, Vector2[] targets) {
        PerformAction (n, targets);
    }

    public CardInstance Card {get {return card;}}
    public Player Player {get {return player;}}
    public HexagonCell Cell {get {return cell;} set {if (IsServer) cell = value;}}
    
    public CardEffectTrigger[] EffectTriggers {get {return effectTriggers;}}

    public ActionAbility[] Actions {get {return actions;}}

    public abstract void TurnStart ();
    public abstract void TurnEnd ();

}
