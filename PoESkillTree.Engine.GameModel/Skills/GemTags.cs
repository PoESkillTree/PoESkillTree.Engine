using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoESkillTree.Engine.GameModel.Skills
{
    public class GemTags
    {
        public GemTags(IReadOnlyList<GemTag> tags)
        {
            Tags = tags;
        }

        public static async Task<GemTags> CreateAsync()
        {
            var dict = await DataUtils.LoadRePoEAsync<IReadOnlyDictionary<string, string>>("gem_tags").ConfigureAwait(false);
            return new GemTags(dict.Select(p => new GemTag(p.Key, p.Value)).ToList());
        }

        public IReadOnlyList<GemTag> Tags { get; }
    }

    public class GemTag
    {
        public GemTag(string internalId, string? translation)
        {
            InternalId = internalId;
            Translation = translation;
        }

        public string InternalId { get; }
        public string? Translation { get; }
    }
}