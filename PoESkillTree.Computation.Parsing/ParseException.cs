﻿using System;

namespace PoESkillTree.Computation.Parsing
{
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message)
        {
        }
    }
}