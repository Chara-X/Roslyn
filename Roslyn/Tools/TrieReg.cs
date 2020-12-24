using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roslyn.Tools
{
    public class TrieReg<T>
    {
        private readonly State _root = new State();
        private string _key;
        private int _offset;

        public void Add(string key, T value)
        {
            if (key == string.Empty) return;
            _key = key;
            _offset = 0;
            var unit = AddPath(_root, PathType.Acyclic);
            while (_offset < key.Length)
                unit = AddPath(unit, PathType.Acyclic);
            unit.Value = value;
        }

        private State AddPath(State start, PathType type)
        {
            switch (_key[_offset])
            {
                case '[':
                    return AddCharSet(start, type);
                case '*':
                    return AddQuantify(start, PathType.Cyclic);
                case '(':
                    return AddStringSet(start, type);
                case '\\':
                    _offset++;
                    return AddChar(start, type);
                default:
                    return AddChar(start, type);
            }
        }

        private State AddChar(State start, PathType type)
        {
            var set = new HashSet<char>() {_key[_offset++]};
            var next = start.Paths.FirstOrDefault(i => i.Equals(new Path(set, type)))?.State;
            if (next != null) return next;
            next = new State();
            start.AddPath(new Path(set, type, next));
            return next;
        }

        private State AddQuantify(State start, PathType type)
        {
            _offset++;
            foreach (var parent in AddPath(start, type).Parents)
                parent.Paths.Last().State = start;
            return start;
        }

        private State AddCharSet(State start, PathType type)
        {
            var builder = new StringBuilder();
            for (; _key[++_offset] != ']';)
                builder.Append(_key[_offset]);
            _offset++;
            var set = new HashSet<char>();
            foreach (var s in builder.ToString().Split('|'))
                for (var i = s[0]; i <= s[2]; i++)
                    if (!set.Contains(i))
                        set.Add(i);
            var next = start.Paths.FirstOrDefault(i => i.Equals(new Path(set, type)))?.State;
            if (next != null) return next;
            next = new State();
            start.AddPath(new Path(set, type, next));
            return next;
        }

        private State AddStringSet(State start, PathType type)
        {
            var next = new State();
            for (; _key[_offset++] != ')';)
            {
                var parents = AddString(start,type).Parents;
                foreach (var parent in parents)
                {
                    parent.Paths.Last().State = next;
                    next.Parents.Add(parent);
                }
            }
            return next;
        }

        private State AddString(State start, PathType type)
        {
            var @char = _key[_offset];
            if (@char != '|' && @char != ')')
                return AddString(AddPath(start, type), type);
            return start;
        }

        public (int offset, T value) this[string key] => _root.Switch(key, 0);

        public class State
        {
            public T Value { get; set; }
            public List<Path> Paths { get; set; }
            public List<State> Parents { get; set; }

            public State()
            {
                Paths = new List<Path>();
                Parents = new List<State>();
            }

            public void AddPath(Path path)
            {
                Paths.Add(path);
                path.State.Parents.Add(this);
            }

            public (int offset, T value) Switch(string key, int offset)
            {
                var max = offset;
                var value = Value;
                foreach (var path in Paths.Where(path => offset < key.Length && path.CharSet.Contains(key[offset])))
                {
                    var (off, val) = path.State.Switch(key, offset + 1);
                    if (off <= max || (val == null || val.Equals(default))) continue;
                    max = off;
                    value = val;
                }

                return (max, value);
            }
        }

        public class Path
        {
            public HashSet<char> CharSet { get; }
            public PathType Type { get; }
            public State State { get; set; }

            public Path(HashSet<char> charSet, PathType type)
            {
                CharSet = charSet;
                Type = type;
            }

            public Path(HashSet<char> charSet, PathType type, State state)
            {
                CharSet = charSet;
                Type = type;
                State = state;
            }

            public override bool Equals(object other) => Equals((Path) other);

            protected bool Equals(Path other) => other.CharSet.Count == CharSet.Count &&
                                                 other.CharSet.All(c => CharSet.Contains(c)) && other.Type == Type;

            public override int GetHashCode() => CharSet.Count + (int) Type;
        }

        public enum PathType
        {
            Cyclic,
            Acyclic
        }
    }
}
