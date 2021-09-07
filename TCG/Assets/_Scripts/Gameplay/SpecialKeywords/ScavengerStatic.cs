using System.Collections.Generic;
using MLAPI;

public static class ScavengerStatic
{
    public static void Trigger (MatchManager matchManager) {
        if (!NetworkManager.Singleton.IsServer) return; 

        matchManager.AddEffectToStack (new ScavengerEffect (matchManager));
    }
}
