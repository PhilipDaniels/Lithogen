using System.Diagnostics;

namespace Gitcheatsheet.Model
{
    [DebuggerDisplay("{Affected}, {Text}")]
    public class Cheat
    {
        // The order is intentional, so that the Yaml serializer writes them out in this order.
        public string Text { get; private set; }
        public string Affected { get; private set; }
        public string Url { get; private set; }
        public string Hint { get; private set; }

        public Cheat(string affected, string text, string hint, string url)
        {
            Affected = (affected ?? "").ToUpperInvariant();
            Text = text;
            Hint = hint;
            Url = url ?? "#";
        }
    }
}
