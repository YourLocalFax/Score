using System;
using System.Collections.Generic;

namespace Ty
{
    public abstract class TyRef
    {
        public bool IsVoid => (this as BuiltinTyRef) is TyVoid;
    }

    public sealed class InferTyRef : TyRef
    {
        public static readonly InferTyRef InferTy = new InferTyRef();

        private InferTyRef() { }
    }

    public sealed class FnTyRef : TyRef
    {
        public readonly List<TyRef> parameters;
        public readonly TyRef returnParameter;

        public FnTyRef(List<TyRef> parameters, TyRef returnParameter)
        {
            this.parameters = parameters;
            this.returnParameter = returnParameter;
        }
    }

    public sealed class PointerTyRef : TyRef
    {
        public readonly TyRef ty;
        public readonly bool isMut;

        public PointerTyRef(TyRef ty, bool isMut)
        {
            this.ty = ty;
            this.isMut = isMut;
        }

        public override string ToString() =>
            string.Format("^{0}{1}", isMut ? "mut " : "", ty.ToString());
    }

    // TODO(kai): this is a temporary path type, so shh
    public sealed class PathTyRef : TyRef
    {
        public readonly string name;

        public TyRef Ty { get; private set; }
        public bool Resolved => Ty != null;

        public PathTyRef(string name)
        {
            this.name = name;
        }

        public void Resolve(TyRef ty)
        {
            if (ty is PathTyRef)
            {
                var path = ty as PathTyRef;
                if (!path.Resolved)
                    throw new ArgumentException("Cannot resolve this type with an unresolved path type.");
                Resolve(path.Ty);
            }
            else Ty = ty;
        }

        public override string ToString() => name;
    }
}
