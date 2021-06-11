public class Damage 
{
    int damage;
    DamageSource source;
    Player playerSource;

    public Damage (int damage, DamageSource source, Player playerSource) {
        this.damage = damage;
        this.source = source;
        this.playerSource = playerSource;
    }

    public int DamageAmount   {get {return damage;}}
    public DamageSource Source  {get {return source;}}
    public Player PlayerSource  {get {return playerSource;}}
}

public enum DamageSource {
    Spell,
    Effect,
    Friendly,
    Attack
}
