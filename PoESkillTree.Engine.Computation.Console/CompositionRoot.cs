using System;
using System.Threading.Tasks;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Core;
using PoESkillTree.Engine.Computation.Data;
using PoESkillTree.Engine.Computation.Data.Steps;
using PoESkillTree.Engine.Computation.Parsing;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.PassiveTree;

namespace PoESkillTree.Engine.Computation.Console
{
    public class CompositionRoot
    {
        private readonly Lazy<GameData> _gameData;
        private readonly Lazy<ICalculator> _calculator;
        private readonly Lazy<Task<IBuilderFactories>> _builderFactories;
        private readonly Lazy<Task<IParser>> _parser;

        public CompositionRoot()
        {
            _gameData = new Lazy<GameData>(
                () => new GameData(PassiveTreeDefinition.CreateKeystoneDefinitions(), true));
            _calculator = new Lazy<ICalculator>(Core.Calculator.Create);
            _builderFactories = new Lazy<Task<IBuilderFactories>>(
                () => Builders.BuilderFactories.CreateAsync(_gameData.Value));
            _parser = new Lazy<Task<IParser>>(
                async () => await Parser<ParsingStep>.CreateAsync(_gameData.Value, _builderFactories.Value,
                    ParsingData.CreateAsync(_gameData.Value, _builderFactories.Value)).ConfigureAwait(false));
        }

        public GameData GameData => _gameData.Value;
        public ICalculator Calculator => _calculator.Value;
        public Task<IBuilderFactories> BuilderFactories => _builderFactories.Value;
        public Task<IParser> Parser => _parser.Value;
    }
}