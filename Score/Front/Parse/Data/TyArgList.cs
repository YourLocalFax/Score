using System;
using System.Collections;
using System.Collections.Generic;

namespace Score.Front.Parse.Data
{
    using Lex;
    using Ty;

    internal sealed class TyArgList : IEnumerable<TyOrVoid>
    {
        public readonly TokenOp lt;
        // TODO(kai): maybe make this an array?
        private readonly List<TyOrVoid> tyList = new List<TyOrVoid>();
        public readonly TokenOp gt;

        public int Count => tyList.Count;
        public TyOrVoid this[int index] => tyList[index];

        public TyArgList(List<TyOrVoid> tyList)
        {
            this.tyList = tyList;
        }

        public void Add(TyOrVoid ty) => tyList.Add(ty);
        public void ForEach(Action<TyOrVoid> action) => tyList.ForEach(action);

        public IEnumerator<TyOrVoid> GetEnumerator() => tyList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => tyList.GetEnumerator();
    }
}
