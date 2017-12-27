﻿using PoESkillTree.Computation.Parsing.Builders.Forms;
using PoESkillTree.Computation.Parsing.Builders.Stats;

namespace PoESkillTree.Computation.Parsing.Data
{
    /// <summary>
    /// Data that specifies stats that are always active.
    /// </summary>
    public class GivenStatData
    {
        public GivenStatData(IFormBuilder form, IStatBuilder stat, double value)
        {
            Form = form;
            Stat = stat;
            Value = value;
        }

        public IFormBuilder Form { get; }

        public IStatBuilder Stat { get; }

        public double Value { get; }
    }
}