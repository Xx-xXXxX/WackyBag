using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WackyBag.Structures.Collections
{

	public class UnorderedList<T>:IEnumerable<Ref<T>>,IReadOnlyList<T>
	{
		public delegate void MoveFn(in int from, in int to);
		public UnorderedList(MoveFn? OnMove=null) {
			this.OnMove = OnMove;
		}
		public event MoveFn? OnMove;

		protected readonly MyList<T> list = [];

		public ref T this[int i]=>ref list[i];

		public int Count => list.Count;

		T IReadOnlyList<T>.this[int index]=>list[index];

		public int Add(in T item)
		{
			list.Add(item);
			return list.Count-1;
		}

		public void Clear()
		{
			list.Clear();
		}

		public void Remove(in int index)
		{
			OnMove?.Invoke(list.Count - 1, index);

			list[index]=list[^1];
			list.RemoveLast();
		}

		public IEnumerator<Ref<T>> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)list).GetEnumerator();
		}

		public Ref<T> GetRef(int index) { 
			return list.GetRef(index);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return ((IEnumerable<T>)list).GetEnumerator();
		}
	}
	
}
