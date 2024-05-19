using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures.Collections
{
	
	public class MyList<T> : IList<T>,IEnumerable<Ref<T>>,IList
	{
		/// <summary>
		/// 是否需要在减少时删除数据
		/// </summary>
		protected static bool NeedsClear { get; } = RuntimeHelpers.IsReferenceOrContainsReferences<T>();
		protected const int DefCapacity = 4;
		protected T[] array;
		public int Capacity => array.Length;
		protected int __count;
		public int Count => __count;
		public bool IsReadOnly => false;

		bool IList.IsFixedSize { get=>false; }
		bool IList.IsReadOnly { get=>false; }
		bool ICollection.IsSynchronized { get=>false; }
		object ICollection.SyncRoot { get=> array; }

		object? IList.this[int index] { get=>array[index]; set=>array[index]= (T)value!; }

		public ref T this[int i] => ref array[i];
		T IList<T>.this[int index]
		{
			get => array[index];
			set => array[index] = value;
		}
		/// <summary>
		/// 移动从index到末尾的数据，自动增加Count
		/// </summary>
		/// <param name="index"></param>
		/// <param name="offset"></param>
		/// <returns>被移动的元素的数量</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected int MoveSlide(in int index, in int offset)
		{
			//Debug.Assert(index < Count);
			//Debug.Assert(index + offset > 0);
			int ncount=__count + offset;
			CheckCapacity(ncount);
			int length= __count -index;
			Array.Copy(array, index, array, index + offset, length);
			__count = ncount;
			return length;
		}


		/// <summary>
		/// 确保数组可以包含<paramref name="index"/>
		/// </summary>
		/// <param name="count"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CheckIndex(in int index)
		{
			int ncount = index + 1;
			CheckCapacity(ncount);
			__count=ncount;
		}

		/// <summary>
		/// 确保数组容量不小于<paramref name="count"/>
		/// </summary>
		/// <param name="count"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CheckCapacity(in int count)
		{
			if (count > Capacity)
			{
				int Capacity2 = Capacity == 0 ? DefCapacity : 2 * Capacity;
				SetCapacity(Capacity2 > count ? Capacity2 : count);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetCapacity(in int i)
		{
			//Debug.Assert(i >= 0);
			T[] newArray = new T[i];
			Array.Copy(array, newArray, Count);
			array = newArray;
		}
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public void AddCapacity(in int i)
		//{
		//    T[] newArray = new T[Capacity + i];
		//    Array.Copy(array, newArray, Count);
		//    array = newArray;
		//}

		public MyList(int Capacity = 0)
		{
			array = new T[Capacity];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Add(in T item)
		{
			int ncount = __count + 1;
			CheckCapacity(ncount);
			array[__count] = item;
			__count = ncount;
			return ncount-1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddRange(in T[] values)
		{
			int length = values.Length;
			int ncount = __count + length;
			CheckCapacity(ncount);
			Array.Copy(values, 0, array, __count, length);
			__count = ncount;
		}

		public void AddRange(in IEnumerable<T> values, in int length)
		{
			CheckCapacity(__count+length);
			foreach (var item in values)
			{
				array[__count++] = item;//Add(item);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddRange(in IEnumerable<T> values)
		{
			AddRange(values.ToArray());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(in int index, in T value)
		{
			Debug.Assert(index >= 0 && index < Count);
			MoveSlide(index, 1);
			array[index] = value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void InsertRange(in int index, in T[] collection)
		{
			Debug.Assert(index >= 0 && index < Count);
			int length = collection.Length;
			MoveSlide(index, length);
			Array.Copy(collection, 0, array, index, length);
		}
		public void InsertRange(int index, in IEnumerable<T> collection, in int length)
		{
			Debug.Assert(index >= 0 && index < Count);
			MoveSlide(index, length);
			foreach (var item in collection)
			{
				array[index++] = item;
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void InsertRange(in int index, in IEnumerable<T> collection)
		{
			Debug.Assert(index >= 0 && index < Count);
			InsertRange(index, collection.ToArray());
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(in int index) => RemoveRange(index, 1);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveRange(in int index, in int count)
		{
			Debug.Assert(index >= 0 && index + count < Count);
			int length=MoveSlide(index + count, -count);
			if (NeedsClear) Array.Clear(array, index+length, count);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveLast() {
			Debug.Assert(__count > 0);
			__count--;
			if (NeedsClear) array[__count] = default!;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveLast(int count) {
			Debug.Assert(__count-count >= 0);
			__count -= count;
			if (NeedsClear) Array.Clear(array,__count,count);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			if (NeedsClear) Array.Clear(array, 0, Count);
			__count = 0;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void TrimExcess()
		{
			T[] New = new T[Count];
			Array.Copy(array, New, Count);
			array = New;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item)
		{
			return ((IList<T>)array).IndexOf(item);
		}

		void IList<T>.Insert(int index, T item)
		{
			Insert(index, item);
		}

		void IList<T>.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		void ICollection<T>.Add(T item)
		{
			Add(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(T item)
		{
			return ((ICollection<T>)array).Contains(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(T[] array, int arrayIndex)
		{
			((ICollection<T>)this.array).CopyTo(array, arrayIndex);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(T item)
		{
			int index = IndexOf(item);
			if (index == -1) return false;
			RemoveAt(index);
			return true;
		}

		public IEnumerator<Ref<T>> GetEnumerator()
		{
			return new Enumrator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumrator(this);
		}
		public T[] ToArray()
		{
			T[] nArr = new T[Count];
			Array.Copy(array, nArr, Count);
			return nArr;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return (IEnumerator<T>)GetEnumerator();
		}

		public Ref<T> GetRef(int index) => new Ref<T>(ref array[index]);

		int IList.Add(object? value)
		{
			if (value != null)
				return Add((T)value);
			else return Add(default!);
		}

		bool IList.Contains(object? value)
		{
			return Contains((T)value!);
		}

		int IList.IndexOf(object? value)
		{
			return IndexOf((T)value!);
		}

		void IList.Insert(int index, object? value)
		{
			Insert(index, (T)value!);
		}

		void IList.Remove(object? value)
		{
			Remove((T)value!);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			Array.Copy(this.array,index,array,0,Count);
		}

		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		public class Enumrator : IEnumerator<Ref<T>>,IEnumerator<T> {
			readonly MyList<T> list;
			int index=-1;

			public Enumrator(MyList<T> list)
			{
				this.list = list;
			}

			public Ref<T> Current { get; private set; }
			object IEnumerator.Current => Current;

			T IEnumerator<T>.Current => Current.Value;

			bool IEnumerator.MoveNext()
			{
				index++;
				if (index < list.__count) {
					Current = new(ref list[index]);
					return true;
				}
				else return false;
			}

			void IEnumerator.Reset()
			{
				index = 0;
			}

			void IDisposable.Dispose()
			{
				GC.SuppressFinalize(this);
			}
		}
	}

}
