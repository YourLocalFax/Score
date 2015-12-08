using System;
using System.Collections.Generic;
using System.Linq;

namespace Ty
{
    public abstract class TyRef
    {
        public static bool operator ==(TyRef a, TyRef b) => a as object == null ? b as object == null : a.Equals(b);
        public static bool operator !=(TyRef a, TyRef b) => a as object == null ? b as object != null : !a.Equals(b);

        public bool IsVoid => (this as BuiltinTyRef) is TyVoid;

        private TyRef rawBacking = null;
        public TyRef Raw => rawBacking == null ? rawBacking = RawInternal : rawBacking;

        internal virtual TyRef RawInternal => this;

        public abstract override int GetHashCode();
        public abstract override bool Equals(object obj);
    }

    public sealed class InferTyRef : TyRef
    {
        public static readonly InferTyRef InferTy = new InferTyRef();

        private InferTyRef() { }

        public override int GetHashCode() => 37;
        public override bool Equals(object obj) => this as object == obj;
    }

    public sealed class FnTyRef : TyRef
    {
        public readonly List<TyRef> parameterTys;
        public readonly TyRef returnTy;

        internal override TyRef RawInternal => new FnTyRef(parameterTys.Select(ty => ty.RawInternal).ToList(), returnTy.RawInternal);

        public FnTyRef(List<TyRef> parameterTys, TyRef returnTy)
        {
            this.parameterTys = parameterTys;
            this.returnTy = returnTy;
        }

        public override int GetHashCode() => 31 * parameterTys.GetHashCode() * returnTy.GetHashCode();

        public override bool Equals(object obj)
        {
            var fnTy = obj as FnTyRef;
            if (fnTy != null)
            {
                var len = parameterTys.Count;
                if (len != fnTy.parameterTys.Count)
                    return false;
                for (int i = 0; i < len; i++)
                    if (parameterTys[i] != fnTy.parameterTys[i])
                        return false;
                return returnTy == fnTy.returnTy;
            }
            return false;
        }
    }

    public sealed class PointerTyRef : TyRef
    {
        public readonly TyRef ty;
        public readonly bool isMut;

        internal override TyRef RawInternal => new PointerTyRef(ty.RawInternal, isMut);

        public PointerTyRef(TyRef ty, bool isMut)
        {
            this.ty = ty;
            this.isMut = isMut;
        }

        public override string ToString() =>
            string.Format("^{0}{1}", isMut ? "mut " : "", ty.ToString());

        public override int GetHashCode() => 37 * ty.GetHashCode() * isMut.GetHashCode();
        public override bool Equals(object obj)
        {
            var pTy = obj as PointerTyRef;
            if (pTy == null)
                return false;
            return ty == pTy.ty && isMut == pTy.isMut;
        }
    }

    // TODO(kai): this is a temporary path type, so shh
    public sealed class PathTyRef : TyRef
    {
        public readonly string name;

        public TyRef Ty { get; private set; }
        public bool Resolved => Ty != null;

        internal override TyRef RawInternal => Ty.RawInternal;

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

        public override int GetHashCode() => 31 * name.GetHashCode();
        public override bool Equals(object obj)
        {
            var pathTy = obj as PathTyRef;
            if (pathTy == null)
                return false;
            return RawInternal == pathTy.RawInternal;
        }
    }
}
