[System.Flags]
public enum Target {
    Card = (1 << 0),
    FriendlyUnit = (1 << 1),
    EnemyUnit = (1 << 2),
    FriendlyHero = (1 << 3),
    EnemyHero = (1 << 4),
    FriendlyStructure = (1 << 5),
    EnemyStructure = (1 << 6),
    FriendlyTrap = (1 << 7),
    EnemyTrap = (1 << 8),
}
