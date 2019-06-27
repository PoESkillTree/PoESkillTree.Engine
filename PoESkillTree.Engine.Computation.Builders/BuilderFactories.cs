using System.Threading.Tasks;
using PoESkillTree.Engine.Computation.Builders.Actions;
using PoESkillTree.Engine.Computation.Builders.Buffs;
using PoESkillTree.Engine.Computation.Builders.Charges;
using PoESkillTree.Engine.Computation.Builders.Conditions;
using PoESkillTree.Engine.Computation.Builders.Damage;
using PoESkillTree.Engine.Computation.Builders.Effects;
using PoESkillTree.Engine.Computation.Builders.Entities;
using PoESkillTree.Engine.Computation.Builders.Equipment;
using PoESkillTree.Engine.Computation.Builders.Forms;
using PoESkillTree.Engine.Computation.Builders.Resolving;
using PoESkillTree.Engine.Computation.Builders.Skills;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Builders.Values;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Actions;
using PoESkillTree.Engine.Computation.Common.Builders.Buffs;
using PoESkillTree.Engine.Computation.Common.Builders.Charges;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Effects;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.Computation.Common.Builders.Equipment;
using PoESkillTree.Engine.Computation.Common.Builders.Forms;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.Computation.Common.Builders.Skills;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.PassiveTree;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Builders
{
    public class BuilderFactories : IBuilderFactories
    {
        public BuilderFactories(PassiveTreeDefinition tree, SkillDefinitions skills)
        {
            var statFactory = new StatFactory();
            ActionBuilders = new ActionBuilders(statFactory);
            BuffBuilders = new BuffBuilders(statFactory, skills);
            ChargeTypeBuilders = new ChargeTypeBuilders(statFactory);
            ConditionBuilders = new ConditionBuilders(statFactory);
            DamageTypeBuilders = new DamageTypeBuilders(statFactory);
            EffectBuilders = new EffectBuilders(statFactory);
            EntityBuilders = new EntityBuilders(statFactory);
            EquipmentBuilders = new EquipmentBuilders(statFactory);
            FormBuilders = new FormBuilders();
            KeywordBuilders = new KeywordBuilders();
            PassiveTreeBuilders = new PassiveTreeBuilders(statFactory, tree);
            SkillBuilders = new SkillBuilders(statFactory, skills);
            StatBuilders = new StatBuilders(statFactory);
            ValueBuilders = new ValueBuilders();
            MetaStatBuilders = new MetaStatBuilders(statFactory);
            MatchContexts = new MatchContexts(statFactory);
        }

        public static async Task<IBuilderFactories> CreateAsync(GameData gameData)
            => new BuilderFactories(
                await gameData.PassiveTree.ConfigureAwait(false),
                await gameData.Skills.ConfigureAwait(false));

        public IActionBuilders ActionBuilders { get; }
        public IBuffBuilders BuffBuilders { get; }
        public IChargeTypeBuilders ChargeTypeBuilders { get; }
        public IConditionBuilders ConditionBuilders { get; }
        public IDamageTypeBuilders DamageTypeBuilders { get; }
        public IEffectBuilders EffectBuilders { get; }
        public IEntityBuilders EntityBuilders { get; }
        public IEquipmentBuilders EquipmentBuilders { get; }
        public IFormBuilders FormBuilders { get; }
        public IKeywordBuilders KeywordBuilders { get; }
        public IPassiveTreeBuilders PassiveTreeBuilders { get; }
        public ISkillBuilders SkillBuilders { get; }
        public IStatBuilders StatBuilders { get; }
        public IValueBuilders ValueBuilders { get; }
        public IMetaStatBuilders MetaStatBuilders { get; }
        public IMatchContexts MatchContexts { get; }
    }
}