using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.GameModel
{
    public class UntranslatedStat : ValueObject
    {
        public UntranslatedStat(string statId, int value) => (StatId, Value) = (statId, value);

        public string StatId { get; }
        public int Value { get; }

        protected override object ToTuple() => (StatId, Value);
    }
}