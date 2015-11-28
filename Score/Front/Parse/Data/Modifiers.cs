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

        // TODO(kai): implement these later

        public List<TokenKw> GetDuplicates()
        {
            // This will get all modifiers that have been given more than once.
            // e.g. pub pub fn main || -> ()
            return new List<TokenKw>();
        }

        public List<TokenKw> GetConflicting()
        {
            // This will find conflicting modifiers.
            // e.g. pub priv fn main || -> ()
            return new List<TokenKw>();
        }

        public Span GetSpan(Token.Type modifierType)
        {
            TokenKw token = null;
            foreach (var modifier in modifiers)
                if (modifier.type == modifierType)
                {
                    token = modifier;
                    break;
                }
            if (token != null)
                return token.span;
            // TODO(kai): maybe different exception or different method?
            throw new ArgumentException("Could not find the requested modifier.");
        }

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
