using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures.Collections
{
	public class Side2List<T>:IList<T>
	{
		public List<T> Positive = [];
		public List<T> Negative = [];
		public static int ToNeg(int i) => 1 - i;
		public T GetValue(int i) {
			if (i < 0) return Negative[ToNeg(i)];
			else return Positive[i];
		}
		public void Insert(int i, T value)
		{
			if (i < 0) Negative[ToNeg(i)] = value;
			else Positive[i] = value;
		}

		public int IndexOf(T item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			if(index<0) Negative.RemoveAt(ToNeg( index));
		}

		public void Add(T item)
		{
			Positive.Add(item);
		}
		public void AddNeg(T item) { 
			Negative.Add(item);
		}

		public void Clear()
		{
			Positive.Clear();
			Negative.Clear();
		}

		public bool Contains(T item)
		{
			return Negative.Contains(item)||Positive.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			for (int i = 0,n=Negative.Count-1; i < Negative.Count; i++,n--)
			{
				array[arrayIndex + i] = Negative[n];
			}
			arrayIndex += Negative.Count;
			for (int i = 0; i < Positive.Count; i++)
			{
				array[arrayIndex+i] = Positive[i];
			}
		}

		public bool Remove(T item)
		{
			if (!Negative.Remove(item))
				return Positive.Remove(item);
			else return true;
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = Negative.Count - 1; i >= 0; i--)
			{
				yield return Negative[i];
			}
			for (int i = 0; i < Positive.Count; i++)
			{
				yield return Positive[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public T this[int i] {
			get => GetValue(i);
			set => Insert(i, value);
		}
		public int Count => Positive.Count + Negative.Count;

		public bool IsReadOnly => false;

		public T AddTo(int i, Func<T>? value=null) {
			value ??= () => default!;
			if (i < 0) { 
				i=ToNeg(i);
				while (Negative.Count < i) Negative.Add(value());
				return Negative[i];
			}
			else{ 
				while(Positive.Count<i) Positive.Add(value());
				return Positive[i];
			}
		}
	}
}
