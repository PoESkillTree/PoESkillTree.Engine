using System;
using PoESkillTree.Engine.Computation.Common;

namespace PoESkillTree.Engine.Computation.Builders.Stats
{
    public static class RoundingBehaviors
    {
        public static NodeValue? Floor(NodeValue? value) =>
            value.Select(d => Math.Floor(d + 1e-5));

        public static NodeValue? Ceiling(NodeValue? value) =>
            value.Select(d => Math.Ceiling(d - 1e-5));
    }
}