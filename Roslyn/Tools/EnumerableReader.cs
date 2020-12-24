using System.Collections.Generic;
using System.Linq;

namespace Roslyn.Tools
{
    public interface IEnumerableReader<out T>
    {
        T Read();

        T Peek();

        T Peek(int offset);

        bool End();

        bool End(int offset);

        IEnumerable<T> AsEnumerable();
    }

    public class EnumerableReader<T> : IEnumerableReader<T>
    {
        private readonly List<T> _source;

        private int _offset;

        public EnumerableReader(IEnumerable<T> array)
        {
            _source = array.ToList();
            _source.Add(default);
        }

        public T Read() => _source[_offset++];

        public T Peek() => Peek(0);

        public T Peek(int offset) => _source[_offset + offset];

        public bool End() => End(0);

        public bool End(int offset) => _offset + offset >= _source.Count - 1;

        public IEnumerable<T> AsEnumerable() => _source.Skip(_offset);
    }
}
