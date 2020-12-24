using System;
using System.Collections;
using System.Collections.Generic;

namespace Roslyn.Tools
{
    public interface IDictionaryRef<TKey, TValue>:IEnumerable<IDictionaryRef<TKey,TValue>.EntryNode<TKey,TValue>>
    {
        TValue Add(TKey key, TValue value);

        TValue Remove(TKey key);

        ref TValue this[TKey key] { get;}

        ref TValue Last();

        bool ContainsKey(TKey key);

        bool ContainsValue(TValue value);

        int Count { get; set; }

        void Clear();

        public class EntryNode<TK, TV>
        {
            public TK Key;
            public TV Value;
            public EntryNode<TK, TV> Next;

            public EntryNode(TK key, TV value)
            {
                Key = key;
                Value = value;
            }

            public bool KeyIsEquals(TK key)
            {
                if (Key.Equals(key))
                    return true;
                return key != null && key.Equals(Key);
            }
        }
    }

    public class DictionaryRef<TK, TV> : IDictionaryRef<TK, TV>
    {
        private IDictionaryRef<TK, TV>.EntryNode<TK, TV>[] _elements;
        private IDictionaryRef<TK, TV>.EntryNode<TK, TV> _last;
        private readonly float _loadFactor;
        public const int DefaultCapacity = 16;
        public const int RehashBase = 2;
        public const float DefaultLoadFactor = 0.75f;

        public int Count { get; set; }

        public DictionaryRef()
        {
            Count = 0;
            _loadFactor = DefaultLoadFactor;
            _elements = new IDictionaryRef<TK, TV>.EntryNode<TK, TV>[DefaultCapacity];
        }

        public DictionaryRef(IDictionaryRef<TK, TV> other)
        {
            Count = 0;
            _loadFactor = DefaultLoadFactor;
            _elements = new IDictionaryRef<TK, TV>.EntryNode<TK, TV>[DefaultCapacity];
            foreach (var i in other)
                Add(i.Key, i.Value);
        }

        public TV Add(TK key, TV value)
        {
            if (NeedReHash())
                ReHash();
            var index = GetIndex(key);
            var firstEntryNode = _elements[index];
            if (firstEntryNode == null)
            {
                _elements[index] = new IDictionaryRef<TK, TV>.EntryNode<TK, TV>(key, value);
                Count++;
                _last = _elements[index];
                return default;
            }

            if (firstEntryNode.KeyIsEquals(key))
            {
                var oldValue = firstEntryNode.Value;
                firstEntryNode.Value = value;
                _last = firstEntryNode;
                return oldValue;
            }

            var targetPreviousNode = GetTargetPreviousEntryNode(firstEntryNode, key);
            var targetNode = targetPreviousNode.Next;
            if (targetNode != null)
            {
                var oldValue = targetNode.Value;
                targetNode.Value = value;
                _last = targetNode;
                return oldValue;
            }

            targetPreviousNode.Next = new IDictionaryRef<TK, TV>.EntryNode<TK, TV>(key, value);
            _last = targetPreviousNode.Next;
            Count++;
            return default;
        }

        public TV Remove(TK key)
        {
            var index = GetIndex(key);
            var firstEntryNode = _elements[index];

            if (firstEntryNode == null)
                return default;
            if (firstEntryNode.KeyIsEquals(key))
            {
                _elements[index] = firstEntryNode.Next;
                Count--;
                return firstEntryNode.Value;
            }

            var targetPreviousNode = GetTargetPreviousEntryNode(firstEntryNode, key);
            var targetNode = targetPreviousNode.Next;
            if (targetNode == null) return default;
            targetPreviousNode.Next = targetNode.Next;
            Count--;
            return targetNode.Value;
        }

        public ref TV this[TK key]
        {
            get
            {
                var index = GetIndex(key);
                ref var firstEntryNode = ref _elements[index];
                if (firstEntryNode == null)
                    throw new Exception("error：[" + key + "] element not found");
                if (firstEntryNode.KeyIsEquals(key))
                    return ref firstEntryNode.Value;
                var targetPreviousNode = GetTargetPreviousEntryNode(firstEntryNode, key);
                var targetNode = targetPreviousNode.Next;
                if (targetNode != null)
                    return ref targetNode.Value;
                throw new Exception("error：[" + key + "] element not found");
            }
        }

        public ref TV Last() => ref _last.Value;

        public bool ContainsKey(TK key) => throw new NotImplementedException();

        public bool ContainsValue(TV value)
        {
            foreach (var element in _elements)
            {
                var entryNode = element;
                while (entryNode != null)
                {
                    if (entryNode.Value.Equals(value))
                        return true;
                    entryNode = entryNode.Next;
                }
            }

            return false;
        }

        public void Clear()
        {
            for (var i = 0; i < _elements.Length; i++)
                _elements[i] = null;
            Count = 0;
        }

        public IEnumerator<IDictionaryRef<TK, TV>.EntryNode<TK, TV>> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Private

        private int GetIndex(TK key) => GetIndex(key, _elements);

        private static int GetIndex(TK key, IDictionaryRef<TK, TV>.EntryNode<TK, TV>[] elements)
        {
            if (key == null)
                return 0;
            var hashCode = key.GetHashCode();
            var finalHashCode = hashCode ^ (hashCode >> 16);
            return (elements.Length - 1) & finalHashCode;
        }

        private IDictionaryRef<TK, TV>.EntryNode<TK, TV> GetTargetPreviousEntryNode(IDictionaryRef<TK, TV>.EntryNode<TK, TV> currentNode, TK key)
        {
            var nextNode = currentNode.Next;
            while (nextNode != null)
            {
                if (nextNode.KeyIsEquals(key))
                    return currentNode;
                currentNode = nextNode;
                nextNode = nextNode.Next;
            }

            return currentNode;
        }

        private void ReHash()
        {
            var newElements = new IDictionaryRef<TK, TV>.EntryNode<TK, TV>[_elements.Length * RehashBase];

            for (var i = 0; i < _elements.Length; i++)
            {
                ReHashSlot(i, newElements);
            }

            _elements = newElements;
        }

        private void ReHashSlot(int index, IDictionaryRef<TK, TV>.EntryNode<TK, TV>[] newElements)
        {
            var currentEntryNode = _elements[index];
            if (currentEntryNode == null)
                return;

            IDictionaryRef<TK, TV>.EntryNode<TK, TV> lowListHead = null;
            IDictionaryRef<TK, TV>.EntryNode<TK, TV> lowListTail = null;
            IDictionaryRef<TK, TV>.EntryNode<TK, TV> highListHead = null;
            IDictionaryRef<TK, TV>.EntryNode<TK, TV> highListTail = null;

            while (currentEntryNode != null)
            {
                var entryNodeIndex = GetIndex(currentEntryNode.Key, newElements);
                if (entryNodeIndex == index)
                {
                    if (lowListHead == null)
                    {
                        lowListHead = currentEntryNode;
                        lowListTail = currentEntryNode;
                    }
                    else
                    {
                        lowListTail.Next = currentEntryNode;
                        lowListTail = lowListTail.Next;
                    }
                }
                else
                {
                    if (highListHead == null)
                    {
                        highListHead = currentEntryNode;
                        highListTail = currentEntryNode;
                    }
                    else
                    {
                        highListTail.Next = currentEntryNode;
                        highListTail = highListTail.Next;
                    }
                }

                currentEntryNode = currentEntryNode.Next;
            }

            newElements[index] = lowListHead;
            if (lowListTail != null)
            {
                lowListTail.Next = null;
            }

            newElements[index + _elements.Length] = highListHead;
            if (highListTail != null)
            {
                highListTail.Next = null;
            }
        }

        private bool NeedReHash()
        {
            // ReSharper disable once PossibleLossOfFraction
            return (Count / _elements.Length) > _loadFactor;
        }

        #endregion

        public class Enumerator : IEnumerator<IDictionaryRef<TK, TV>.EntryNode<TK, TV>>
        {
            public DictionaryRef<TK, TV> DictionaryRef { get; set; }

            public IDictionaryRef<TK, TV>.EntryNode<TK, TV> CurrentNode { get; set; }

            public IDictionaryRef<TK, TV>.EntryNode<TK, TV> NextNode { get; set; }

            public int CurrentIndex { get; set; }

            public Enumerator(DictionaryRef<TK, TV> dictionaryRef)
            {
                DictionaryRef = dictionaryRef;
                if (dictionaryRef.Count == 0)
                    return;
                for (var i = 0; i < dictionaryRef._elements.Length; i++)
                {
                    CurrentIndex = i;
                    var firstEntryNode = dictionaryRef._elements[i];
                    if (firstEntryNode == null) continue;
                    NextNode = firstEntryNode;
                    return;
                }
            }

            public bool MoveNext()
            {
                CurrentNode = NextNode;
                if (CurrentNode == null) return false;
                NextNode = NextNode.Next;
                if (NextNode != null) return true;
                for (var i = CurrentIndex + 1; i < DictionaryRef._elements.Length; i++)
                {
                    CurrentIndex = i;
                    var firstEntryNode = DictionaryRef._elements[i];
                    if (firstEntryNode == null) continue;
                    NextNode = firstEntryNode;
                    break;
                }

                return true;
            }

            public void Reset() => throw new NotImplementedException();

            public IDictionaryRef<TK, TV>.EntryNode<TK, TV> Current => CurrentNode;

            #pragma warning disable 8632
            object? IEnumerator.Current => Current;
            #pragma warning restore 8632

            public void Dispose() { }
        }
    }
}
