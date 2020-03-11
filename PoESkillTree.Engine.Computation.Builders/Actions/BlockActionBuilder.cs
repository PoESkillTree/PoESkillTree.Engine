using System.Runtime.CompilerServices;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Actions;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Builders.Actions
{
    internal class BlockActionBuilder : ActionBuilder, IBlockActionBuilder
    {
        public BlockActionBuilder(IStatFactory statFactory, IEntityBuilder entity)
            : base(statFactory, CoreBuilder.Create("Block"), entity)
        {
        }

        public IStatBuilder Recovery =>
            StatBuilderUtils.FromIdentity(StatFactory, "Block.Recovery", typeof(int));

        public IStatBuilder AttackChance =>
            StatBuilderUtils.FromIdentity(StatFactory, "Block.ChanceAgainstAttacks", typeof(uint));

        public IStatBuilder SpellChance =>
            StatBuilderUtils.FromIdentity(StatFactory, "Block.ChanceAgainstSpells", typeof(uint));

        public IActionBuilder Attack => Create();
        public IActionBuilder Spell => Create();

        private IActionBuilder Create([CallerMemberName] string identity = "") =>
            new ActionBuilder(StatFactory, CoreBuilder.Create("Block." + identity), Entity);
    }
}