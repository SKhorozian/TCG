using MLAPI;
using MLAPI.Serialization;

public struct CardInstanceInfo : INetworkSerializable {
    public int costChange;
    
    //Unit Stuff
    public int bonusPower;
    public int bonusHealth;
    public int bonusRange;
    public int bonusSpeed;
    public StaticKeywords staticKeywords;

    //

    public CardInstanceInfo (int costChange, int bonusPower, int bonusHealth, int bonusRange, int bonusSpeed, StaticKeywords staticKeywords) {
        this.costChange = costChange;
        this.bonusPower = bonusPower;
        this.bonusHealth = bonusHealth;
        this.bonusRange = bonusRange;
        this.bonusSpeed = bonusSpeed;
        this.staticKeywords = staticKeywords;
    }

    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref costChange);
        serializer.Serialize(ref bonusPower);
        serializer.Serialize(ref bonusHealth);
        serializer.Serialize(ref bonusRange);
        serializer.Serialize(ref bonusSpeed);
        serializer.Serialize(ref staticKeywords);
    }

}