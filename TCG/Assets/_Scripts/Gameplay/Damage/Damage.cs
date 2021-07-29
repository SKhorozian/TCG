public class Damage 
{
    int baseDamage;
    int damage;
    DamageSource source;
    Player playerSource;

    public Damage (int damage, DamageSource source, Player playerSource) {
        baseDamage = damage;
        this.damage = baseDamage;
        this.source = source;
        this.playerSource = playerSource;
    }

    public int DamageAmount   {get {return damage;} set {damage = value;}}
    public int BaseDamage     {get {return baseDamage;}}
    public DamageSource Source  {get {return source;}}
    public Player PlayerSource  {get {return playerSource;}}
}

public enum DamageSource {
    Spell,
    Effect,
    Friendly,
    Attack
}
