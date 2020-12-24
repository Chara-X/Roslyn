using Roslyn.Tools;

namespace Roslyn.Expressions
{
    public abstract class Expression
    {
        public ValueTransmission Value { get; set; }
        public ReferTransmission Refer { get; set; }
    }

    public delegate ref object ReferTransmission(ExpressionContext context);

    public delegate object ValueTransmission(ExpressionContext context);

    public class ExpressionContext
    {
        public IDictionaryRef<string, object> Objects { get; set; }

        public ExpressionContext() => Objects = new DictionaryRef<string, object>();

        public ExpressionContext(IDictionaryRef<string, object> objects) => Objects = objects;

        public ref object this[string key] => ref Objects[key];

        public ref object Last() => ref Objects.Last();

        public void Add(string key, object value) => Objects.Add(key, value);
    }
}
