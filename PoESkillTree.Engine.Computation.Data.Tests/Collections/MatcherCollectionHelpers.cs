using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Data;

namespace PoESkillTree.Engine.Computation.Data.Collections
{
    internal static class MatcherCollectionHelpers
    {
        internal static ModifierBuilderStub AssertSingle(this IEnumerable<MatcherData> sut, 
            string regex, string substitution = "")
        {
            var data = sut.Single();
            Assert.AreEqual(regex, data.Regex);
            Assert.IsInstanceOf<ModifierBuilderStub>(data.Modifier);
            Assert.AreEqual(substitution, data.MatchSubstitution);
            return (ModifierBuilderStub) data.Modifier;
        }

        internal static ModifierBuilderStub AssertSingle(this IEnumerable<IIntermediateModifier> sut)
        {
            var mod = sut.Single();
            Assert.IsInstanceOf<ModifierBuilderStub>(mod);
            return (ModifierBuilderStub) mod;
        }
    }
}