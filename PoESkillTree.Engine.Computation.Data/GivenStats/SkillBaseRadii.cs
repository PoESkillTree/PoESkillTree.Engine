using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Data.Base;
using PoESkillTree.Engine.Computation.Data.Collections;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Data.GivenStats
{
    public class SkillBaseRadii : UsesStatBuilders, IGivenStats
    {
        private static readonly IReadOnlyList<(string skillId, int radius)> PrimaryRadii = new[]
        {
            ("AccuracyAndCritsAura", 40),
            ("Anger", 40),
            ("ArcticBreath", 12),
            ("AssassinsMark", 22),
            ("BallLightning", 22),
            ("BladeVortex", 15),
            ("Bladestorm", 24),
            ("BlastRain", 24),
            ("Blight", 26),
            ("BloodSandArmour", 28),
            ("BloodstainedBanner", 46),
            ("CataclysmSigil", 18),
            ("ChargedAttack", 14),
            ("Clarity", 40),
            ("Cleave", 20),
            ("ClusterBurst", 14),
            ("ColdResistAura", 40),
            ("ColdSnap", 16),
            ("Conductivity", 22),
            ("Contagion", 20),
            ("CorrosiveShroud", 25),
            ("Cyclone", 11),
            ("DamageOverTimeAura", 40),
            ("DarkPact", 24),
            ("DarkRitual", 21),
            ("DecoyTotem", 60),
            ("Despair", 22),
            ("Determination", 40),
            ("DetonateDead", 22),
            ("Discharge", 30),
            ("Discipline", 40),
            ("DivineTempest", 38),
            ("DoubleSlash", 38),
            ("ElementalWeakness", 22),
            ("EnduringCry", 60),
            ("Enfeeble", 22),
            ("ExplosiveArrow", 15),
            ("FireResistAura", 40),
            ("FireTrap", 15),
            ("Fireball", 9),
            ("Firestorm", 25),
            ("FlameTotem", 16),
            ("FlameWhip", 30),
            ("Flammability", 22),
            ("FrostBoltNova", 20),
            ("FrostFury", 16),
            ("Frostbite", 22),
            ("GlacialCascade", 12),
            ("Grace", 40),
            ("GroundSlam", 35),
            ("Haste", 40),
            ("Hatred", 40),
            ("HeraldOfAsh", 10),
            ("HeraldOfIce", 12),
            ("HeraldOfThunder", 32),
            ("IceCrash", 26),
            ("IceNova", 30),
            ("IceShot", 23),
            ("InfernalBlow", 15),
            ("LeapSlam", 15),
            ("LightningExplosionMine", 20),
            ("LightningResistAura", 40),
            ("LightningTendrilsChannelled", 22),
            ("LightningWarp", 16),
            ("MambaStrike", 27),
            ("MoltenShell", 15),
            ("MortarBarrageMine", 20),
            ("PoachersMark", 22),
            ("PoisonArrow", 20),
            ("ProjectileWeakness", 22),
            ("Punishment", 22),
            ("PuresteelBanner", 46),
            ("Purity", 40),
            ("RainOfArrows", 24),
            ("Reave", 20),
            ("RejuvenationTotem", 10),
            ("RighteousFire", 18),
            ("Sanctify", 18),
            ("ShockNova", 26),
            ("ShockwaveTotem", 24),
            ("Soulrend", 10),
            ("SpellDamageAura", 40),
            ("StormBurstNew", 16),
            ("StormCall", 20),
            ("Sweep", 26),
            ("TemporalChains", 22),
            ("VaalBlight", 20),
            ("VaalClarity", 40),
            ("VaalCyclone", 24),
            ("VaalDiscipline", 40),
            ("VaalGrace", 40),
            ("VaalHaste", 40),
            ("VaalReave", 12),
            ("Vitality", 40),
            ("Vulnerability", 22),
            ("WarlordsMark", 22),
            ("Wither", 18),
            ("Wrath", 40),
        };

        private static readonly IReadOnlyList<(string skillId, int skillPart, int radius)> SkillPartSpecificPrimaryRadii = new[]
        {
            ("Earthquake", 0, 18),
            ("Earthquake", 1, 28),
            ("VaalEarthquake", 0, 18),
            ("VaalEarthquake", 1, 28),
        };

        private static readonly IReadOnlyList<(string skillId, int radius)> SecondaryRadii = new[]
        {
            ("PoisonArrow", 12),
            ("Bladestorm", 20),
            ("CataclysmSigil", 8),
            ("Firestorm", 10),
            ("Sanctify", 50),
            ("MortarBarrageMine", 26),
            ("StormBurstNew", 22),
        };

        private static readonly IReadOnlyList<(string skillId, int radius)> TertiaryRadii = new[]
        {
            ("MortarBarrageMine", 12)
        };

        private readonly IModifierBuilder _modifierBuilder;
        private readonly Lazy<IReadOnlyList<IIntermediateModifier>> _lazyGivenStats;

        public SkillBaseRadii(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories)
        {
            _modifierBuilder = modifierBuilder;
            _lazyGivenStats = new Lazy<IReadOnlyList<IIntermediateModifier>>(() => CreateCollection().ToList());
        }

        private IMetaStatBuilders MetaStats => BuilderFactories.MetaStatBuilders;

        public IReadOnlyList<Entity> AffectedEntities { get; } = new[] { GameModel.Entity.Character };

        public IReadOnlyList<string> GivenStatLines { get; } = new string[0];

        public IReadOnlyList<IIntermediateModifier> GivenModifiers => _lazyGivenStats.Value;

        private GivenStatCollection CreateCollection()
        {
            var collection = new GivenStatCollection(_modifierBuilder, ValueFactory);
            foreach (var (skillId, radius) in PrimaryRadii)
            {
                collection.Add(BaseSet, Stat.PrimaryRadius, radius, IsMainSkill(skillId));
            }

            foreach (var (skillId, skillPart, radius) in SkillPartSpecificPrimaryRadii)
            {
                collection.Add(BaseSet, Stat.PrimaryRadius, radius, IsMainSkill(skillId, skillPart));
            }

            foreach (var (skillId, radius) in SecondaryRadii)
            {
                collection.Add(BaseSet, Stat.SecondaryRadius, radius, IsMainSkill(skillId));
            }

            foreach (var (skillId, radius) in TertiaryRadii)
            {
                collection.Add(BaseSet, Stat.TertiaryRadius, radius, IsMainSkill(skillId));
            }

            return collection;
        }

        private IConditionBuilder IsMainSkill(string skillId, int skillPart)
            => IsMainSkill(skillId).And(Stat.MainSkillPart.Value.Eq(skillPart));

        private IConditionBuilder IsMainSkill(string skillId) =>
            MetaStats.MainSkillId.Value.Eq(Skills.FromId(skillId).SkillId);
    }
}