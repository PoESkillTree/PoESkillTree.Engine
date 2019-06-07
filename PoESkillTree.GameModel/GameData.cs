﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using PoESkillTree.GameModel.Items;
using PoESkillTree.GameModel.Modifiers;
using PoESkillTree.GameModel.PassiveTree;
using PoESkillTree.GameModel.Skills;
using PoESkillTree.GameModel.StatTranslation;

namespace PoESkillTree.GameModel
{
    public class GameData
    {
        private readonly AsyncLazy<PassiveTreeDefinition> _passiveTree;
        private readonly AsyncLazy<ModifierDefinitions> _modifiers;
        private readonly AsyncLazy<BaseItemDefinitions> _baseItems;
        private readonly AsyncLazy<SkillDefinitions> _skills;
        private readonly AsyncLazy<StatTranslators> _statTranslators;
        private readonly AsyncLazy<CharacterBaseStats> _characterBaseStats;
        private readonly AsyncLazy<MonsterBaseStats> _monsterBaseStats;

        /// <param name="passiveNodeDefinitions">The passive skill tree definition</param>
        /// <param name="runOnThreadPool">If true, all tasks are run on the thread pool instead of on the calling
        /// thread. I/O operations will always run asynchronously regardless of this parameter.</param>
        public GameData(
            IEnumerable<PassiveNodeDefinition> passiveNodeDefinitions, bool runOnThreadPool = false)
        {
            var asyncLazyFlags = runOnThreadPool ? AsyncLazyFlags.None : AsyncLazyFlags.ExecuteOnCallingThread;
            _passiveTree = new AsyncLazy<PassiveTreeDefinition>(
                () => Task.FromResult(new PassiveTreeDefinition(passiveNodeDefinitions.ToList())), asyncLazyFlags);
            _modifiers = new AsyncLazy<ModifierDefinitions>(
                () => ModifierJsonDeserializer.DeserializeAsync(runOnThreadPool), asyncLazyFlags);
            _baseItems = new AsyncLazy<BaseItemDefinitions>(
                () => BaseItemJsonDeserializer.DeserializeAsync(runOnThreadPool, _modifiers.Task),
                asyncLazyFlags);
            _skills = new AsyncLazy<SkillDefinitions>(
                () => SkillJsonDeserializer.DeserializeAsync(runOnThreadPool), asyncLazyFlags);
            _statTranslators = new AsyncLazy<StatTranslators>(
                () => StatTranslation.StatTranslators.CreateAsync(runOnThreadPool), asyncLazyFlags);
            _characterBaseStats = new AsyncLazy<CharacterBaseStats>(
                GameModel.CharacterBaseStats.CreateAsync, asyncLazyFlags);
            _monsterBaseStats = new AsyncLazy<MonsterBaseStats>(GameModel.MonsterBaseStats.CreateAsync, asyncLazyFlags);
        }

        public Task<PassiveTreeDefinition> PassiveTree => _passiveTree.Task;
        public Task<ModifierDefinitions> Modifiers => _modifiers.Task;
        public Task<BaseItemDefinitions> BaseItems => _baseItems.Task;
        public Task<SkillDefinitions> Skills => _skills.Task;
        public Task<StatTranslators> StatTranslators => _statTranslators.Task;
        public Task<CharacterBaseStats> CharacterBaseStats => _characterBaseStats.Task;
        public Task<MonsterBaseStats> MonsterBaseStats => _monsterBaseStats.Task;

        public void StartAllTasks()
        {
            _modifiers.Start();
            _baseItems.Start();
            _skills.Start();
            _statTranslators.Start();
            _characterBaseStats.Start();
            _monsterBaseStats.Start();
        }
    }
}