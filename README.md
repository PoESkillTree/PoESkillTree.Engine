# PoESkillTree.Engine

[![Nuget](https://img.shields.io/nuget/v/PoESkillTree.Engine.svg)](https://www.nuget.org/packages/PoESkillTree.Engine/) [![Build status](https://ci.appveyor.com/api/projects/status/knmsvn7oqrh5l6ou/branch/master?svg=true)](https://ci.appveyor.com/project/PoESkillTree/poeskilltree-engine/branch/master) [![Discord](https://b.thumbs.redditmedia.com/YzI6TxCJcacCZw1sx1Z5tyy6YskyNiA84hn4WfPXaRM.png)](https://discord.gg/sC7cUHV)

This repository contains the game model and computation engine used for [PoESkillTree](https://github.com/PoESkillTree/PoESkillTree). For example usage, see [PoESkillTree/WPFSKillTree/Computation](https://github.com/PoESkillTree/PoESkillTree/tree/master/WPFSKillTree/Computation).

The goal is to develop this into a general .NET Standard library for Path of Exile build calculations (e.g. DPS) every interested developer can use and contribute to. Currently, the library is optimized for a GUI application with small incremental build changes. I will later add benchmarks for the one-off scenario where you want to input a build, get statistics as outputs and then input the next build. Those will serve as further example usages.

The libraries target .NET Standard 2.1, Console and the test projects .NET Core 3.1.
