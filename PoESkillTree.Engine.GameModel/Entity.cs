using System;
using System.Collections.Generic;
using System.Linq;

namespace PoESkillTree.Engine.GameModel
{
    /// <summary>
    /// The types of entities relevant for stat calculation. The first entry is the default entity of stats.
    /// </summary>
    public enum Entity
    {
        Character,
        Totem,
        Minion,
        Enemy,
    }

    public static class EntityExtensions
    {
        public static IEnumerable<Entity> Opponents(this Entity @this) =>
            @this switch
            {
                Entity.Character => new[] {Entity.Enemy},
                Entity.Totem => new[] {Entity.Enemy},
                Entity.Minion => new[] {Entity.Enemy},
                Entity.Enemy => new[] {Entity.Character, Entity.Totem, Entity.Minion},
                _ => throw new ArgumentOutOfRangeException(nameof(@this), @this, null)
            };

        public static IEnumerable<Entity> Allies(this Entity @this) =>
            @this switch
            {
                Entity.Character => new[] {Entity.Totem, Entity.Minion},
                Entity.Totem => new[] {Entity.Character, Entity.Minion},
                Entity.Minion => new[] {Entity.Character, Entity.Totem},
                Entity.Enemy => Array.Empty<Entity>(),
                _ => throw new ArgumentOutOfRangeException(nameof(@this), @this, null)
            };

        public static IEnumerable<Entity> SelfAndAllies(this Entity @this) =>
            @this.Allies().Prepend(@this);

        public static IEnumerable<Entity> Minions(this Entity @this) =>
            @this == Entity.Character ? new[] {Entity.Minion} : Array.Empty<Entity>();
    }
}