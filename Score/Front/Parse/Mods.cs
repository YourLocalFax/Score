using System.Text;

namespace Score.Front.Parse
{
    using Lex;

    using static Lex.Token.Type;

    // TODO(kai): maybe get span information

    internal sealed class Mods
    {
        public bool Set(Token.Type type, out string error)
        {
            error = "Unrecognized modifier type '" + type + "'.";
            switch (type)
            {
                case PUB: return SetPub(out error);
                case PRIV: return SetPriv(out error);
                case EXTERN: return SetExtern(out error);
                case INTERN: return SetIntern(out error);
                default: return false;
            }
        }

        public bool NoneSet => !(Pub || Priv || Intern || Extern);

        public bool Pub { get; private set; }
        public bool SetPub(out string error)
        {
            error = null;
            if (Pub || Priv)
            {
                error = Pub ? "Modifier 'pub' cannot be used twice." : "Modifier 'pub' cannot be used; modifier 'priv' found.";
                return false;
            }
            return Pub = true;
        }

        public bool Priv { get; private set; }
        public bool SetPriv(out string error)
        {
            error = null;
            if (Pub || Priv)
            {
                error = Priv ? "Modifier 'priv' cannot be used twice." : "Modifier 'priv' cannot be used; modifier 'pub' found.";
                return false;
            }
            return Priv = true;
        }

        public bool Extern { get; private set; }
        public bool SetExtern(out string error)
        {
            error = null;
            if (Extern)
            {
                error = "Modifier 'extern' cannot be used twice.";
                return false;
            }
            return Extern = true;
        }

        public bool Intern { get; private set; }
        public bool SetIntern(out string error)
        {
            error = null;
            if (Intern)
            {
                error = "Modifier 'intern' cannot be used twice.";
                return false;
            }
            return Intern = true;
        }
    }
}
