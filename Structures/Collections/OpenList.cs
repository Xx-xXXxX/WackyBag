using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures.Collections
{
	public class OpenList<T>:IRefIndexable<T>,IReadOnlyIndexable<T>
	{
		/// <summary>
		/// 是否需要在减少时删除数据
		/// </summary>
		protected static bool NeedsClear { get; } = RuntimeHelpers.IsReferenceOrContainsReferences<T>();
		protected const int DefCapacity = 4;
		public T[] Array { get; protected set; }
		public int Capacity => Array.Length;
		public ref T this[int i] => ref Array[i];
		T IReadOnlyIndexable<T>.this[int index]=> Array[index];
		/// <summary>
		/// 移动从index到末尾的数据，自动增加Count
		/// </summary>
		/// <param name="index"></param>
		/// <param name="offset"></param>
		/// <returns>移动的长度</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int MoveSlide(in int index,in int length, in int offset)
		{
			//Debug.Assert(index < Count);
			//Debug.Assert(index + offset > 0);
			CheckCapacity(index+length+offset);
            System.Array.Copy(Array, index, Array, index + offset, length);
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
		public void SetCapacity(in int i)
		{
			//Debug.Assert(i >= 0);
			T[] newArray = new T[i];
            System.Array.Copy(Array, newArray, Capacity);
			Array = newArray;
		}
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public void AddCapacity(in int i)
		//{
		//    T[] newArray = new T[Capacity + i];
		//    Array.Copy(array, newArray, Count);
		//    array = newArray;
		//}

		public OpenList(int Capacity = 0)
		{
			Array = new T[Capacity];
		}

		public void Clear()
		{
			if (NeedsClear) System.Array.Clear(Array, 0, Capacity);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void TrimExcess(int Count)
		{
			T[] New = new T[Count];
            System.Array.Copy(Array, New, Count);
			Array = New;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(T item)
		{
			return ((ICollection<T>)Array).Contains(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(T[] array, int arrayIndex)
		{
			this.Array.CopyTo(array, arrayIndex);
		}

		public Ref<T> GetRef(int index) => new Ref<T>(ref Array[index]);

		public void TrySet(int index, T value)
		{
			CheckIndex(index);
			Array[index] = value;
		}

	}
}
