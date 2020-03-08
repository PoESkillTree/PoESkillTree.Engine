using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.StatTranslation;

namespace PoESkillTree.Engine.Computation.Console
{
    internal class RePoEUpdater
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private static readonly string[] Files =
        {
            "mods", "crafting_bench_options", "default_monster_stats", "characters",
            "gems", "gem_tags", "gem_tooltips", "base_items"
        };

        private readonly HttpClient _httpClient;
        private readonly string _savePath;

        public RePoEUpdater(HttpClient httpClient, string basePath)
        {
            _httpClient = httpClient;
            _savePath = basePath + "PoESkillTree.Engine.GameModel/Data/RePoE";
        }

        public async Task UpdateAsync()
        {
            Directory.CreateDirectory(Path.Combine(_savePath, "stat_translations"));
            var files = Files.Concat(StatTranslationFileNames.AllFromRePoE);
            await Task.WhenAll(files.Select(LoadAsync));
        }

        private async Task LoadAsync(string file)
        {
            var fileName = file + DataUtils.RePoEFileSuffix;
            var response = await _httpClient.GetAsync(DataUtils.RePoEDataUrl + fileName);
            if (!response.IsSuccessStatusCode)
            {
                Log.Error($"Failed to load {file}: {response.StatusCode}");
                return;
            }
            using (var writer = File.Create(Path.Combine(_savePath, fileName)))
            {
                await response.Content.CopyToAsync(writer).ConfigureAwait(false);
                Log.Info($"Loaded {file}");
            }
        }
    }
}