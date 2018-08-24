﻿namespace PoESkillTree.GameModel.Skills
{
    /// <summary>
    /// Constants for the ActiveSkillTypes that need to be referenced.
    /// See https://github.com/brather1ng/RePoE/blob/master/RePoE/constants.py.
    /// (using an enum would not work because it would need all types and the don't fit into an enum)
    /// </summary>
    public static class ActiveSkillType
    {
        public const string Attack = "attack";
        public const string Spell = "spell";
        public const string Projectile = "projectile";
        public const string Buff = "buff";
        public const string Minion = "minion";
        public const string AreaOfEffect = "aoe";
        public const string ExplicitProjectileDamage = "explicit_deals_projectile_damage";
        public const string Melee = "melee";
        public const string Totem = "totem";
        public const string ExplicitProvidesBuff = "unknown_30";
        public const string Curse = "curse";
        public const string Fire = "fire";
        public const string Cold = "cold";
        public const string Lightning = "lightning";
        public const string Trap = "trap";
        public const string Movement = "movement";
        public const string Mine = "mine";
        public const string Vaal = "vaal";
        public const string Aura = "aura";
        public const string TriggerAttack = "trigger_attack";
        public const string Chaos = "chaos";
        public const string Golem = "golem";
        public const string Herald = "herald";
    }
}