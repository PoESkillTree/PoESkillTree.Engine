﻿using System;
using System.Collections.Generic;

namespace PoESkillTree.Utils
{
    public static class CollectionChangedEventArgs
    {
        public static CollectionChangedEventArgs<T> AddedSingle<T>(T addedItem)
            => Added(new[] { addedItem });

        public static CollectionChangedEventArgs<T> Added<T>(IReadOnlyCollection<T> addedItems)
            => Replaced(addedItems, new T[0]);

        public static CollectionChangedEventArgs<T> RemovedSingle<T>(T removedItem)
            => Removed(new[] { removedItem });

        public static CollectionChangedEventArgs<T> Removed<T>(IReadOnlyCollection<T> removedItems)
            => Replaced(new T[0], removedItems);

        public static CollectionChangedEventArgs<T> ReplacedSingle<T>(T addedItem, T removedItem)
            => Replaced(new[] { addedItem }, new[] { removedItem });

        public static CollectionChangedEventArgs<T> Replaced<T>(
            IReadOnlyCollection<T> addedItems, IReadOnlyCollection<T> removedItems)
            => new CollectionChangedEventArgs<T>(addedItems, removedItems);
    }

    public class CollectionChangedEventArgs<T> : EventArgs
    {
        public CollectionChangedEventArgs(IReadOnlyCollection<T> addedItems, IReadOnlyCollection<T> removedItems)
        {
            AddedItems = addedItems;
            RemovedItems = removedItems;
        }

        public IReadOnlyCollection<T> AddedItems { get; }
        public IReadOnlyCollection<T> RemovedItems { get; }
    }
}