using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.GameModel.Items
{
    public class Requirements : ValueObject
    {
        public Requirements(int level, int dexterity, int intelligence, int strength)
            => (Level, Dexterity, Intelligence, Strength) = (level, dexterity, intelligence, strength);

        public int Level { get; }

        public int Dexterity { get; }
        public int Intelligence { get; }
        public int Strength { get; }

        protected override object ToTuple() => (Level, Dexterity, Intelligence, Strength);
    }
}