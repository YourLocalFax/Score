using System;
using System.Collections;
using System.Collections.Generic;

namespace Score.Front.Parse.Data
{
    using Lex;
    using Ty;

    internal sealed class TyArgList : IEnumerable<TyRef>
    {
        public TokenOp lt;
        // TODO(kai): maybe make this an array?
        private readonly List<TyRef> tyList = new List<TyRef>();
        public TokenOp gt;

        public int Count => tyList.Count;
        public TyRef this[int index] => tyList[index];

        public TyArgList(List<TyRef> tyList)
        {
            this.tyList = tyList;
        }

        public void Add(TyRef ty) => tyList.Add(ty);
        public void ForEach(Action<TyRef> action) => tyList.ForEach(action);

        public IEnumerator<TyRef> GetEnumerator() => tyList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => tyList.GetEnumerator();
    }
}
