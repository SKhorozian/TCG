using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventTracker
{
    List <GameEvent> allEvents;

    public GameEventTracker () {
        allEvents = new List<GameEvent> ();
    }

    public void AddEvent (GameEvent gameEvent) {
        allEvents.Add (gameEvent);
    }

    public List<GameEvent> AllEvents {get {return allEvents;}}
}
