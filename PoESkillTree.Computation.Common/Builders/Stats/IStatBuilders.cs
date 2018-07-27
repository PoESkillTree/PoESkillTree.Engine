namespace PoESkillTree.Computation.Common.Builders.Stats
{
    /// <summary>
    /// Factory interface for stats.
    /// </summary>
    public interface IStatBuilders
    {
        IStatBuilder Level { get; }

        IStatBuilder Armour { get; }

        IEvasionStatBuilder Evasion { get; }


        IDamageRelatedStatBuilder Accuracy { get; }


        /// <summary>
        /// Gets a stat representing the multiplier to Self's movement speed.
        /// </summary>
        IStatBuilder MovementSpeed { get; }

        /// <summary>
        /// Gets a stat representing the multiplier to Self's animation speed. This acts like a more modifier to all
        /// kinds of speeds (movement, cast, trap throwing, ...).
        /// </summary>
        IStatBuilder AnimationSpeed { get; }


        /// <summary>
        /// The speed with which to cast (or attack with) the main skill (casts per second)
        /// </summary>
        IDamageRelatedStatBuilder CastSpeed { get; }

        IStatBuilder EffectivenessOfAddedDamage { get; }

        /// <summary>
        /// Gets a stat representing the main skill's area of effect. The square root of this stat is used as a
        /// modifier to <see cref="Range"/>.
        /// </summary>
        IStatBuilder AreaOfEffect { get; }

        /// <summary>
        /// Gets a stat representing the main skill's range
        /// </summary>
        IStatBuilder Range { get; }

        /// <summary>
        /// The main skill's cooldown recovery speed
        /// </summary>
        IStatBuilder CooldownRecoverySpeed { get; }

        /// <summary>
        /// The main skill's duration
        /// </summary>
        IStatBuilder Duration { get; }


        ITrapStatBuilders Trap { get; }

        IMineStatBuilders Mine { get; }

        ISkillEntityStatBuilders Totem { get; }


        IStatBuilder ItemQuantity { get; }

        IStatBuilder ItemRarity { get; }


        /// <summary>
        /// Gets a stat representing the number of primordial jewels socketed into the skill tree.
        /// </summary>
        IStatBuilder PrimordialJewelsSocketed { get; }

        /// <summary>
        /// Gets a stat representing the number of grand spectrum jewels socketed into the skill tree.
        /// </summary>
        IStatBuilder GrandSpectrumJewelsSocketed { get; }

        /// <summary>
        /// Gets a stat representing the number of currently active rampage stacks.
        /// </summary>
        IStatBuilder RampageStacks { get; }

        IStatBuilder CharacterSize { get; }

        /// <summary>
        /// The percentage of damage taken gained as mana over 4 seconds.
        /// </summary>
        IStatBuilder DamageTakenGainedAsMana { get; }

        IStatBuilder LightRadius { get; }


        // Stats from sub factories

        IAttributeStatBuilders Attribute { get; }

        IPoolStatBuilders Pool { get; }

        IDodgeStatBuilders Dodge { get; }

        IFlaskStatBuilders Flask { get; }

        IProjectileStatBuilders Projectile { get; }

        IFlagStatBuilders Flag { get; }

        IGemStatBuilders Gem { get; }
    }
}