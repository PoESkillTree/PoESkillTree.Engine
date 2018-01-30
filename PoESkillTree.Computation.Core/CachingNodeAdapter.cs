﻿using System;
using PoESkillTree.Computation.Common;

namespace PoESkillTree.Computation.Core
{
    public class CachingNodeAdapter : SubscriberCountingNode
    {
        private readonly ICachingNode _adaptedNode;

        public CachingNodeAdapter(ICachingNode adaptedNode)
        {
            _adaptedNode = adaptedNode;
            _adaptedNode.ValueChangeReceived += AdaptedNodeOnValueChangeReceived;
        }

        public override NodeValue? Value => _adaptedNode.Value;

        public override void Dispose()
        {
            _adaptedNode.ValueChangeReceived -= AdaptedNodeOnValueChangeReceived;
        }

        private void AdaptedNodeOnValueChangeReceived(object sender, EventArgs args) => OnValueChanged();
    }
}