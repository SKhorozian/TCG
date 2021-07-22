using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
using System.Collections.Generic;
using TMPro;

public abstract class FieldCard : NetworkBehaviour, ITargetable
{
    [SerializeField] protected CardInstance card;
    [SerializeField] protected Player player;

    [SerializeField] public NetworkVariableBool toBeRemoved = new NetworkVariableBool (new NetworkVariableSettings {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    //Position, this is used for networking
    [SerializeField] public NetworkVariableVector3 position = new NetworkVariableVector3 (new NetworkVariableSettings {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    public NetworkVariableInt tallies = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    [SerializeField] protected HexagonCell cell;
    [SerializeField] protected SpriteRenderer icon;

    [SerializeField] protected TextMeshPro tallyText;

    [SerializeField] protected CardEffectTrigger[] effectTriggers;
    [SerializeField] protected CardEffectListener[] effectListeners;
    [SerializeField] protected ActionAbility[] actions;

    public void PerformAction (int n, Vector2[] extraCostTargets, Vector2[] targets) {
        if (IsServer) {
            if (n >= actions.Length) return;
            if (n < 0) return;

            ActionAbility action = Instantiate <ActionAbility> (actions[n]);
            action.name = actions[n].name;
            action.FieldCard = this;
            action.Player = player;
            
            List<ITargetable> newTargets = Targeting.ConvertTargets (action.TargetTypes, targets, player);
            List<ITargetable> newExtraCostTargets = new List<ITargetable> ();
            if (action.ExtraCost)
                newExtraCostTargets = Targeting.ConvertTargets (action.ExtraCost.TargetTypes, extraCostTargets, player);

            player.MatchManage.FieldCardAct (this, newExtraCostTargets, newTargets, action);
        } else {
            PerformActionRequestServerRPC (n, extraCostTargets, targets);
        }
    }
    [ServerRpc]
    public void PerformActionRequestServerRPC (int n, Vector2[] extraCostTargets, Vector2[] targets) {
        PerformAction (n, extraCostTargets, targets);
    }

    //Tally n number of times.
    public void Tally (int n) {
        tallies.Value += n;

        tallies.Value = Mathf.Clamp (tallies.Value, 0, 100);
    }

    //Set tallies to a certain value.
    public void SetTallies (int n) {
        tallies.Value = n;

        tallies.Value = Mathf.Clamp (tallies.Value, 0, 100);
    }

    public CardInstance Card {get {return card;}}
    public Player Player {get {return player;}}
    public HexagonCell Cell {get {return cell;} set {if (IsServer) cell = value;}}
    
    public CardEffectTrigger[] EffectTriggers {get {return effectTriggers;}}

    public ActionAbility[] Actions {get {return actions;}}

    public abstract void TurnStart ();
    public abstract void TurnEnd ();
    public abstract void OnRemove ();

}
