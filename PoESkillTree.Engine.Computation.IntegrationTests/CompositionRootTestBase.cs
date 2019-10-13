using System;
using System.Threading.Tasks;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Data.Steps;
using PoESkillTree.Engine.Computation.Parsing;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.PassiveTree;

namespace PoESkillTree.Engine.Computation.IntegrationTests
{
    public abstract class CompositionRootTestBase
    {
        private static readonly Lazy<GameData> LazyGameData = new Lazy<GameData>(
            () => new GameData(PassiveTreeDefinition.CreateKeystoneDefinitions()));

        private static readonly Lazy<Task<IBuilderFactories>> LazyBuilderFactories = new Lazy<Task<IBuilderFactories>>(
            () => Builders.BuilderFactories.CreateAsync(LazyGameData.Value));

        private static readonly Lazy<Task<IParsingData<ParsingStep>>> LazyParsingData =
            new Lazy<Task<IParsingData<ParsingStep>>>(
                () => Data.ParsingData.CreateAsync(LazyGameData.Value, LazyBuilderFactories.Value));

        private static readonly Lazy<Task<IParser>> LazyParser = new Lazy<Task<IParser>>(
            () => Parser<ParsingStep>.CreateAsync(LazyGameData.Value, LazyBuilderFactories.Value,
                LazyParsingData.Value));

        protected static GameData GameData => LazyGameData.Value;
        protected static Task<IBuilderFactories> BuilderFactoriesTask => LazyBuilderFactories.Value;
        protected static Task<IParsingData<ParsingStep>> ParsingDataTask => LazyParsingData.Value;
        protected static Task<IParser> ParserTask => LazyParser.Value;
    }
}