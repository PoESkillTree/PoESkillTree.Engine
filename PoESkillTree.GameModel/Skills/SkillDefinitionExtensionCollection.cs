﻿using System.Collections;
using System.Collections.Generic;
using MoreLinq;

namespace PoESkillTree.GameModel.Skills
{
    public class SkillDefinitionExtensionCollection : IEnumerable<(string, SkillDefinitionExtension)>
    {
        private readonly List<(string, SkillDefinitionExtension)> _collection =
            new List<(string, SkillDefinitionExtension)>();

        public IEnumerator<(string, SkillDefinitionExtension)> GetEnumerator()
            => throw new System.NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IReadOnlyDictionary<string, SkillDefinitionExtension> ToDictionary()
            => _collection.ToDictionary();

        public void Add(string skillId,
            params (string name, SkillPartDefinitionExtension extension)[] parts)
            => Add(skillId, new SkillPartDefinitionExtension(), parts);

        public void Add(string skillId,
            SkillPartDefinitionExtension commonExtension,
            params (string name, SkillPartDefinitionExtension extension)[] parts)
            => Add(skillId, commonExtension, new Dictionary<string, IEnumerable<Entity>>(), parts);

        public void Add(string skillId,
            IReadOnlyDictionary<string, IEnumerable<Entity>> buffStats,
            params (string name, SkillPartDefinitionExtension extension)[] parts)
            => Add(skillId, new SkillPartDefinitionExtension(), buffStats, parts);

        public void Add(string skillId,
            IEnumerable<string> passiveStats,
            params (string name, SkillPartDefinitionExtension extension)[] parts)
            => Add(skillId, new SkillPartDefinitionExtension(), passiveStats, parts);

        public void Add(string skillId,
            SkillPartDefinitionExtension commonExtension, IReadOnlyDictionary<string, IEnumerable<Entity>> buffStats,
            params (string name, SkillPartDefinitionExtension extension)[] parts)
            => Add(skillId, commonExtension, buffStats, new string[0], parts);

        public void Add(string skillId,
            SkillPartDefinitionExtension commonExtension, IEnumerable<string> passiveStats,
            params (string name, SkillPartDefinitionExtension extension)[] parts)
            => Add(skillId, commonExtension, new Dictionary<string, IEnumerable<Entity>>(), passiveStats, parts);

        public void Add(string skillId,
            IReadOnlyDictionary<string, IEnumerable<Entity>> buffStats, IEnumerable<string> passiveStats,
            params (string name, SkillPartDefinitionExtension extension)[] parts)
            => Add(skillId, new SkillPartDefinitionExtension(), buffStats, passiveStats, parts);

        public void Add(string skillId,
            SkillPartDefinitionExtension commonExtension,
            IReadOnlyDictionary<string, IEnumerable<Entity>> buffStats, IEnumerable<string> passiveStats,
            params (string name, SkillPartDefinitionExtension extension)[] parts)
            => _collection.Add((skillId,
                new SkillDefinitionExtension(commonExtension, buffStats, passiveStats, parts)));
    }
}