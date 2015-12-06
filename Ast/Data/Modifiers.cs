using System;
using System.Collections;
using System.Collections.Generic;

using Lex;
using Source;

namespace Ast.Data
{
    public enum ModifierType
    {
        //PUB,
        //PRIV,
        //INTERN,
        EXTERN,
        //EXPORT
    }

    public sealed class Modifier
    {
        public readonly Token token;

        public readonly ModifierType type;
        public readonly Spanned<string> optArg;

        public Span Span =>
            optArg != null ? new Span(token.span.fileName, token.span.start, optArg.span.end) : token.span;

        public Modifier(Token token, Spanned<string> optArg)
        {
            this.token = token;
            type = (ModifierType)Enum.Parse(typeof(ModifierType), token.Image.ToUpper());
            this.optArg = optArg;
        }
    }

    public sealed class Modifiers : IEnumerable<Modifier>
    {
        public readonly List<Modifier> modifiers = new List<Modifier>();

        public Modifiers()
        {
        }

        public void Add(Token token, Spanned<string> optArg) => modifiers.Add(new Modifier(token, optArg));
        public void ForEach(Action<Modifier> action) => modifiers.ForEach(action);

        // TODO(kai): implement these later

        public List<Modifier> GetDuplicates()
        {
            // This will get all modifiers that have been given more than once.
            // e.g. pub pub fn main || -> ()
            return new List<Modifier>();
        }

        public List<Modifier> GetConflicting()
        {
            // This will find conflicting modifiers.
            // e.g. pub priv fn main || -> ()
            return new List<Modifier>();
        }

        public Span GetSpan(ModifierType modifierType)
        {
            Modifier result = null;
            foreach (var modifier in modifiers)
                if (modifier.type == modifierType)
                {
                    result = modifier;
                    break;
                }
            if (result != null)
                return result.Span;
            // TODO(kai): maybe different exception or different method?
            throw new ArgumentException("Could not find the requested modifier.");
        }

        public bool Has(ModifierType modifierType)
        {
            foreach (var mod in modifiers)
                if (mod.type == modifierType)
                    return true;
            return false;
        }

        public IEnumerator<Modifier> GetEnumerator() => modifiers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => modifiers.GetEnumerator();
    }
}
