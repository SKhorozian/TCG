using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldState 
{
    CurrentFieldState state;

    public FieldState () {
        state = CurrentFieldState.Mulligan;
    }

    public void ChangeState (CurrentFieldState newState) {
        state = newState;
    }

    public CurrentFieldState State {get {return state;}}
}

public enum CurrentFieldState {
    Mulligan,                   //Both players are mulliganing.
    FreePlay,                   //Turn player can act freely. Can play Rituals, Structures, and Units.
    TurnPlayerPriority,         //Turn player can only play Arcanes and Chants.
    ReactingPlayerPriority,     //Opposing player reacting. Can only play Arcanes and Chants.
    None,                       //Neither players can play. For animations.
    Prompt                      //One of the players is given a prompt. Neither can act. Prompted player can only answer their prompt.
}