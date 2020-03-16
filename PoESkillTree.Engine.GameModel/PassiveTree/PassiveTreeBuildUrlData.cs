using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PoESkillTree.Engine.GameModel.PassiveTree
{
    public class PassiveTreeBuildUrlData
    {
        private readonly Regex _urlRegex = new Regex(@".*\/(?<build>[\w-=]+)");

        public Uri BasePassiveTreeUri => new Uri("https://www.pathofexile.com/passive-skill-tree/");

        public int HeaderSize => Version > 3 ? 7 : 6;

        public int Version { get; set; } = 4;

        public bool Fullscreen { get; set; } = false;

        public CharacterClass CharacterClass { get; set; } = CharacterClass.Witch;

        public int AscendancyClass { get; set; } = 0;

        public HashSet<ushort> PassiveNodeIds { get; } = new HashSet<ushort>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PassiveTreeBuildUrlData"/> class whose elements have default values.
        /// </summary>
        public PassiveTreeBuildUrlData() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PassiveTreeBuildUrlData"/> class whose elements have the specified values.
        /// </summary>
        /// <param name="characterClass"></param>
        /// <param name="ascendancyClass"></param>
        /// <param name="passiveNodeIds"></param>
        /// <param name="version"></param>
        /// <param name="fullscreen"></param>
        public PassiveTreeBuildUrlData(CharacterClass characterClass, int ascendancyClass, ICollection<ushort> passiveNodeIds, int version = 4, bool fullscreen = false)
            => (CharacterClass, AscendancyClass, PassiveNodeIds, Version, Fullscreen) = (characterClass, ascendancyClass, new HashSet<ushort>(passiveNodeIds), version, fullscreen);

        /// <summary>
        /// Initializes a new instance of the <see cref="PassiveTreeBuildUrlData"/> class whose elements have the decoded values from the specified value.
        /// <seealso cref="Decode(string)"/>
        /// </summary>
        /// <param name="url"></param>
        public PassiveTreeBuildUrlData(string url) => Decode(url);

        /// <summary>
        /// Combines <see cref="BasePassiveTreeUri"/> with the output of <see cref="Encode"/> to produce a valid official url.
        /// </summary>
        /// <returns></returns>
        public string EncodeUrl() => new Uri(BasePassiveTreeUri, Encode()).AbsoluteUri;

        /// <summary>
        /// Creates a Base64 String based on the output of <see cref="EncodeBytes"/> with a web-safe encoding
        /// </summary>
        /// <returns></returns>
        public string Encode() => Convert.ToBase64String(EncodeBytes()).Replace("+", "-").Replace("/", "_").TrimEnd('=');

        /// <summary>
        /// Creates a byte array bases on the elements of the object. The array's format is dictated by the official website
        /// </summary>
        /// <returns></returns>
        public byte[] EncodeBytes()
        {
            var target = new byte[HeaderSize + PassiveNodeIds.Count * 2];
            target[0] = Convert.ToByte(Version >> 24 & 0xFF);
            target[1] = Convert.ToByte(Version >> 16 & 0xFF);
            target[2] = Convert.ToByte(Version >> 8 & 0xFF);
            target[3] = Convert.ToByte(Version >> 0 & 0xFF);
            target[4] = Convert.ToByte(CharacterClass);

            switch (HeaderSize)
            {
                case 6:
                    target[5] = Convert.ToByte(Fullscreen);
                    break;
                case 7:
                    target[5] = Convert.ToByte(AscendancyClass);
                    target[6] = Convert.ToByte(Fullscreen);
                    break;
            }

            var i = HeaderSize;
            foreach (var id in PassiveNodeIds)
            {
                target[i++] = Convert.ToByte(id >> 8 & 0xFF);
                target[i++] = Convert.ToByte(id >> 0 & 0xFF);
            }

            return target;
        }

        /// <summary>
        /// Decodes an official url style build string 
        /// </summary>
        /// <example>
        /// http://www.pathofexile.com/passive-skill-tree/AAAABAMAAA==
        /// http://www.pathofexile.com/fullscreen-passive-skill-tree/AAAABAMAAQ==
        /// </example>
        /// <param name="url"></param>
        public void Decode(string url) => Decode(GetRawData(url));

        private void Decode(byte[] bytes)
        {
            if (bytes.Length >= 4)
            {
                Version = bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3];
            }

            if (bytes.Length >= 5)
            {
                CharacterClass = (CharacterClass)bytes[4];
            }

            switch (HeaderSize)
            {
                case 6 when bytes.Length >= 6:
                    Fullscreen = Convert.ToBoolean(bytes[5]);
                    break;
                case 7 when bytes.Length >= 7:
                    AscendancyClass = bytes[5];
                    Fullscreen = Convert.ToBoolean(bytes[6]);
                    break;
            }

            for (var i = HeaderSize; i < bytes.Length; i += 2)
            {
                var id = Convert.ToUInt16(bytes[i] << 8 | bytes[i + 1]);
                if (!PassiveNodeIds.Contains(id))
                {
                    PassiveNodeIds.Add(id);
                }
            }
        }

        /// <summary>
        /// Converts build data from the specified url to an array of bytes.
        /// </summary>
        private byte[] GetRawData(string url)
        {
            var match = _urlRegex.Match(url);

            if (!match.Success)
            {
                return new byte[0];
            }

            var data = match.Groups["build"].Value.Replace("-", "+").Replace("_", "/");
            switch (data.Length % 4)
            {
                case 2:
                    data += "==";
                    break;
                case 3:
                    data += "=";
                    break;
            }

            return Convert.FromBase64String(data); ;
        }
    }
}
