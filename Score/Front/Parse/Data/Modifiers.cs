using System;
using System.Collections;
using System.Collections.Generic;

namespace Score.Front.Parse.Data
{
    using Lex;

    internal sealed class Modifiers : IEnumerable<TokenKw>
    {
        public readonly List<TokenKw> modifiers = new List<TokenKw>();

        public Modifiers()
        {
        }

        public void Add(TokenKw modifier) => modifiers.Add(modifier);
        public void ForEach(Action<TokenKw> action) => modifiers.ForEach(action);

        public bool Has(Token.Type modifierType)
        {
            foreach (var mod in modifiers)
                if (mod.type == modifierType)
                    return true;
            return false;
        }

        public IEnumerator<TokenKw> GetEnumerator() => modifiers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => modifiers.GetEnumerator();
    }
}
