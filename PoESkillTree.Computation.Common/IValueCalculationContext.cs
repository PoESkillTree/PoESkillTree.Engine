﻿using System.Collections.Generic;

namespace PoESkillTree.Computation.Common
{
    public interface IValueCalculationContext
    {
        // For NodeTypes occurring on paths, refers to the non-path node or the main path if always a path-node.
        NodeValue? GetValue(IStat stat, NodeType nodeType = NodeType.Total);
        // Refers to the given path. Not usable with Total, Subtotal and TotalOverride.
        //NodeValue? GetValue(IStat stat, NodeType nodeType, PathProperty);

        // Refers to the main path. Obsolete with below overload.
        IEnumerable<NodeValue?> GetValues(Form form, params IStat[] stats);
        //IEnumerable<NodeValue?> GetValues(Form, (IStat, PathProperty)[])

        // Returns the values of all paths.
        //IEnumerable<NodeValue?> GetValues(IStat, NodeType nodeType);

        // Total, Subtotal, TotalOverride: behavior unchanged
        //  (requires GetValue(IStat, NodeType), GetValues(Form, (IStat, PathProperty)))
        // UncappedSubtotal outside of paths: Combines UncappedSubtotal of all paths
        //  (requires GetValues(IStat, NodeType))
        // UncappedSubtotal in a path: same behavior but path-specific
        //  (requires GetValue(IStat, NodeType, PathProperty))
        // Base with conversion: value is the base value of the first stat in the conversion list with the same path
        //  except removing that from the conversion list.
        //  (requires GetValue(IStat, NodeType, PathProperty))
        // Base without conversion: same behavior but path-specific
        //  (requires GetValue(IStat, NodeType, PathProperty))
        // Increase, More: still form aggregating, but with multiple paths:
        //  For each source in currentPath.Source.InfluencingSources:
        //   For each stat in currentPath.Stats:
        //    paths.Add((source, stat))
        //  (requires GetValues(Form, (IStat, PathProperty)[]))
        // BaseOverride, BaseSet, BaseAdd: only used in paths without conversions. Same behavior except using the path's
        //  form node collection.
        //  (requires GetValues(Form, (IStat, PathProperty)))
        // -> Values need to know their path
    }
}