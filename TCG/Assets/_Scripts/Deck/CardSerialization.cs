using MLAPI;
using MLAPI.Serialization;

public struct CardInstanceInfo : INetworkSerializable {
    public int costChange;
    
    //Unit Stuff
    public int bonusStrength;
    public int bonusHealth;
    public int bonusRange;
    public int bonusSpeed;
    public UnitCardStaticKeywords unitStatics;

    //

    public CardInstanceInfo (int costChange, int bonusStrength, int bonusHealth, int bonusRange, int bonusSpeed, UnitCardStaticKeywords unitStatics) {
        this.costChange = costChange;
        this.bonusStrength = bonusStrength;
        this.bonusHealth = bonusHealth;
        this.bonusRange = bonusRange;
        this.bonusSpeed = bonusSpeed;
        this.unitStatics = unitStatics;
    }

    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref costChange);
        serializer.Serialize(ref bonusStrength);
        serializer.Serialize(ref bonusHealth);
        serializer.Serialize(ref bonusRange);
        serializer.Serialize(ref bonusSpeed);
        serializer.Serialize(ref unitStatics);
    }

}