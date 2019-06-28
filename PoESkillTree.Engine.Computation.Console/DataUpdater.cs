using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Console
{
    internal class DataUpdater
    {
        private readonly HttpClient _httpClient = new HttpClient
        {
            DefaultRequestHeaders = { { "User-Agent", "PoESkillTree.Engine.Console" } }
        };

        private readonly string _basePath
            = Regex.Replace(Directory.GetCurrentDirectory(),
                @"PoESkillTree\.Engine\.Computation\.Console(/|\\)?.*$", "");

        private readonly GameData _gameData;

        private readonly Lazy<RePoEUpdater> _rePoEUpdater;
        private readonly Lazy<UniquesUpdater> _uniquesUpdater;

        public DataUpdater(GameData gameData)
        {
            _gameData = gameData;
            _rePoEUpdater = new Lazy<RePoEUpdater>(() => new RePoEUpdater(_httpClient, _basePath));
            _uniquesUpdater = new Lazy<UniquesUpdater>(() => new UniquesUpdater(_httpClient, _basePath));
        }

        public Task UpdateRePoEAsync()
            => _rePoEUpdater.Value.UpdateAsync();

        public Task UpdateUniquesAsync()
            => _uniquesUpdater.Value.UpdateAsync();

        public void UpdateSkillTreeStatLines(string skillTreeTxtPath)
            => TestDataUpdater.UpdateSkillTreeStatLines(skillTreeTxtPath, _basePath);

        public async Task UpdateParseableBaseItemsAsync()
            => TestDataUpdater.UpdateParseableBaseItems(await _gameData.BaseItems, _basePath);

        public async Task UpdateItemAffixesAsync()
            => TestDataUpdater.UpdateItemAffixes(
                await _gameData.Modifiers, await _gameData.StatTranslators, _basePath);
    }
}