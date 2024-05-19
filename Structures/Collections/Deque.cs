using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures.Collections
{
	public class Deque<T>
	{
		/// <summary>
		/// 是否需要在减少时删除数据
		/// </summary>
		protected static bool NeedsClear { get; } = RuntimeHelpers.IsReferenceOrContainsReferences<T>();
		private const int DefCapacity = 4;
		protected T[] list;
		public int Capacity => list.Length;
		protected int begin;
		protected int end;
		protected int count;
		public int Count => count;
		protected void SetCapacity(int NewCapacity) {
			T[] NewList = new T[NewCapacity];
			if (begin < end)
			{
				Array.Copy(list, begin, NewList, 0, count);
			}
			else if (begin >= end) {
				if (count != 0)
				{
					int l = Capacity - begin;
					Array.Copy(list, begin, NewList, 0, l);
					Array.Copy(list, 0, NewList, l, count - l);
				}
			}
			begin = 0;
			end = count;
			list = NewList;
		}
		public void CheckCapacity(int count) {
			if (count > Capacity)
			{
				int Capacity2 = Capacity == 0 ? DefCapacity : 2 * Capacity;
				SetCapacity(Capacity2 > count ? Capacity2 : count);
			}
		}
		private void MoveBack(ref int i) {
			i += 1;
			if (i >= Capacity) i = 0;
		}
		private void MoveFront(ref int i) {
			i -= 1;
			if (i < 0) i = Capacity - 1;
		}
		public void PushFront(in T item) {
			CheckCapacity(count + 1);
			MoveFront(ref begin);
			count += 1;
			list[begin] = item;
		}
		public T PopFront() {
			T item = list[begin];
			if (NeedsClear) list[begin] = default!;
			MoveBack(ref begin);
			count -= 1;
			return item;
		}
		public T GetFront() => list[begin];
		public void PushBack(in T item)
		{
			CheckCapacity(count + 1);
			count += 1;
			list[end] = item;
			MoveBack(ref end);
		}
		public T PopBack()
		{
			MoveBack(ref end);
			T item = list[end];
			if (NeedsClear) list[end] = default!;
			count -= 1;
			return item;
		}
		public T GetBack() => list[end];

		public Deque(int Capacity = 0)
		{
			this.list = new T[Capacity];
		}
		public void Clear() {
			if (NeedsClear) {
				if (begin < end)
				{
					Array.Clear(list, begin, count);
				}
				else if (begin >= end)
				{
					if (count != 0)
					{
						int l = Capacity - begin;
						Array.Clear(list, begin, l);
						Array.Clear(list, 0, count - l);
					}
				}
			}
			begin = end =count= 0;
		}
	}	
}
