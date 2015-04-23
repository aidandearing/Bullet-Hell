
namespace ActionGame
{
    class FactionManager
    {
        public enum Faction { Adventurer, Bandit, Goblin, Treefolk, Mechanical, Spirit };
        public enum Hostility { Friendly, Neutral, Enemy };
        
        /// <summary>
        /// Returns the hostility of faction1 to faction2
        /// </summary>
        /// <param name="faction1">The faction to check the hostility of</param>
        /// <param name="faction2">The faction to check the hostility against</param>
        /// <returns></returns>
        public static Hostility GetHostility(Faction faction1, Faction faction2)
        {
            switch (faction1)
            {
                case Faction.Adventurer:
                    switch (faction2)
                    {
                        case Faction.Adventurer:
                            return Hostility.Friendly;
                        case Faction.Bandit:
                            return Hostility.Enemy;
                        case Faction.Goblin:
                            return Hostility.Enemy;
                        case Faction.Mechanical:
                            return Hostility.Neutral;
                        case Faction.Spirit:
                            return Hostility.Neutral;
                        case Faction.Treefolk:
                            return Hostility.Neutral;
                        default:
                            return Hostility.Neutral;
                    }
                case Faction.Bandit:
                    switch (faction2)
                    {
                        case Faction.Adventurer:
                            return Hostility.Enemy;
                        case Faction.Bandit:
                            return Hostility.Friendly;
                        case Faction.Goblin:
                            return Hostility.Enemy;
                        case Faction.Mechanical:
                            return Hostility.Neutral;
                        case Faction.Spirit:
                            return Hostility.Neutral;
                        case Faction.Treefolk:
                            return Hostility.Enemy;
                        default:
                            return Hostility.Enemy;
                    }
                case Faction.Goblin:
                    switch (faction2)
                    {
                        case Faction.Adventurer:
                            return Hostility.Enemy;
                        case Faction.Bandit:
                            return Hostility.Enemy;
                        case Faction.Goblin:
                            return Hostility.Friendly;
                        case Faction.Mechanical:
                            return Hostility.Friendly;
                        case Faction.Spirit:
                            return Hostility.Enemy;
                        case Faction.Treefolk:
                            return Hostility.Enemy;
                        default:
                            return Hostility.Enemy;
                    }
                case Faction.Mechanical:
                    switch (faction2)
                    {
                        case Faction.Mechanical:
                            return Hostility.Friendly;
                        default:
                            return Hostility.Neutral;
                    }
                case Faction.Spirit:
                    switch (faction2)
                    {
                        case Faction.Spirit:
                            return Hostility.Friendly;
                        case Faction.Treefolk:
                            return Hostility.Friendly;
                        default:
                            return Hostility.Neutral;
                    }
                case Faction.Treefolk:
                    switch (faction2)
                    {
                        case Faction.Bandit:
                            return Hostility.Enemy;
                        case Faction.Goblin:
                            return Hostility.Enemy;
                        case Faction.Mechanical:
                            return Hostility.Enemy;
                        case Faction.Spirit:
                            return Hostility.Friendly;
                        case Faction.Treefolk:
                            return Hostility.Friendly;
                        default:
                            return Hostility.Neutral;
                    }
                default:
                    return Hostility.Neutral;
            }
        }
    }
}
