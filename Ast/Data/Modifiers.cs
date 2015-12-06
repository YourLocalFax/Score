using System;
using System.Collections;
using System.Collections.Generic;

using Lex;
using Source;

namespace Ast.Data
{
    // TODO(kai): possibly make special cases or something for extern/export.
    // TODO(kai): Maybe even allow string parameters to all modifiers?
    // That'd simplify the problem a lot.

    public sealed class Modifiers : IEnumerable<Token>
    {
        public readonly List<Token> modifiers = new List<Token>();

        public Modifiers()
        {
        }

        public void Add(Token modifier) => modifiers.Add(modifier);
        public void ForEach(Action<Token> action) => modifiers.ForEach(action);

        // TODO(kai): implement these later

        public List<Token> GetDuplicates()
        {
            // This will get all modifiers that have been given more than once.
            // e.g. pub pub fn main || -> ()
            return new List<Token>();
        }

        public List<Token> GetConflicting()
        {
            // This will find conflicting modifiers.
            // e.g. pub priv fn main || -> ()
            return new List<Token>();
        }

        public Span GetSpan(TokenType modifierType)
        {
            Token token = null;
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

        public bool Has(TokenType modifierType)
        {
            foreach (var mod in modifiers)
                if (mod.type == modifierType)
                    return true;
            return false;
        }

        public IEnumerator<Token> GetEnumerator() => modifiers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => modifiers.GetEnumerator();
    }
}
