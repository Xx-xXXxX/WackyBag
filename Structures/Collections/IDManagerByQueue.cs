using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures.Collections
{
	/// <summary>
	/// IDManager, 记下未使用的Id并提供
	/// </summary>
	public class IDManagerByQueue
	{
		protected Queue<int> ids = new();
		private const int DefCapacity = 4;
		protected int Capacity = 0;
		public IDManagerByQueue(int Capacity = 0)
		{
			SetCapacity(Capacity);
		}
		/// <summary>
		/// 获取下一个Id的值
		/// </summary>
		/// <returns></returns>
		public int GetNext()
		{
			CheckGrow();
			bool t = ids.TryPeek(out var result);
			Debug.Assert(t);
			return result;
		}
		/// <summary>
		/// 申请一个新Id
		/// </summary>
		/// <returns></returns>
		public int Add()
		{
			CheckGrow();
			bool t = ids.TryDequeue(out var result);
			Debug.Assert(t);
			return result;
		}
		/// <summary>
		/// 释放一个新Id
		/// </summary>
		/// <param name="id"></param>
		public void Remove(int id)
		{
			ids.Enqueue(id);
		}

		protected void CheckGrow()
		{
			if (ids.Count==0) SetCapacity(Capacity == 0 ? DefCapacity : 2 * Capacity);
		}

		protected void CheckCapacity(int count)
		{
			if (count > Capacity)
			{
				int Capacity2 = Capacity == 0 ? DefCapacity : 2 * Capacity;
				SetCapacity(Capacity2 > count ? Capacity2 : count);
			}
		}
		protected void SetCapacity(int NewCapacity)
		{
			for (int i = Capacity; i < NewCapacity; i++)
			{
				ids.Enqueue(i);
			}
			Capacity = NewCapacity;
		}
		/// <summary>
		/// 清空至初始状态
		/// </summary>
		public void Clear() {
			ids.Clear();
			Capacity = 0;
		}
	}

	/// <summary>
	/// 线程安全的IDManager, 记下未使用的Id并提供
	/// </summary>
	public class IDManagerByQueueConcurrent
	{
		protected ConcurrentBag<int> ids=[];
		private const int DefCapacity = 4;
		protected int Capacity=0;
		public IDManagerByQueueConcurrent(int Capacity=0) { 
			SetCapacity(Capacity);
		}

		/// <summary>
		/// 获取下一个Id的值
		/// </summary>
		/// <returns></returns>
		public int GetNext() 
		{ 
			CheckGrow();
			bool t = ids.TryPeek(out var result);
			Debug.Assert(t);
			return result;
		}
		/// <summary>
		/// 申请一个新Id
		/// </summary>
		/// <returns></returns>
		public int Add() {
			CheckGrow();
			bool t = ids.TryTake(out var result);
			Debug.Assert(t);
			return result;
		}

		/// <summary>
		/// 释放一个新Id
		/// </summary>
		/// <param name="id"></param>
		public void Remove(int id) { 
			ids.Add(id);
		}

		protected void CheckGrow() { 
			if(ids.IsEmpty) SetCapacity(Capacity == 0 ? DefCapacity : 2 * Capacity);
		}

		protected void CheckCapacity(int count) {
			if (count > Capacity)
			{
				int Capacity2 = Capacity == 0 ? DefCapacity : 2 * Capacity;
				SetCapacity(Capacity2 > count ? Capacity2 : count);
			}
		}
		protected void SetCapacity(int NewCapacity) {
			for (int i = Capacity; i < NewCapacity; i++) {
				ids.Add(i);
			}
			Capacity = NewCapacity;
		}

		/// <summary>
		/// 清空至初始状态
		/// </summary>
		public void Clear() { ids = []; }
	}

	/// <summary>
	/// 有IDManager管理的列表
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class IDManagedListByQueue<T> //: IEnumerable<T>
	{
		/// <summary>
		/// 值列表
		/// </summary>
		public MyList<UNullable<T>> Values { get; protected set; } = [];
		public IDManagerByQueue Manager { get; protected set; } = new();
		/// <summary>
		/// 添加在最小可用ID
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int Add(T value)
		{
			int index = Manager.Add();
			//while (Values.Count < index) Values.Add(default);
			Values.CheckIndex(index);
			Values[index] = value;
			//Manager.Add(index);
			return index;
		}
		//public int Add(int index, T value)
		//{
		//	while (Values.Count <= index) Values.Add(default);
		//	Values[index] = value;
		//	Manager.Add(index);
		//	return index;
		//}
		public void Remove(int index)
		{
			Values[index] = default;
			Manager.Remove(index);
			//if (Manager.LastOrDef() < Values.Count)
			//{
			//	index = Manager.LastOrDef();
			//	Values.RemoveRange(index, Values.Count - index);
			//}
		}
		public void Clear() { Values.Clear(); Manager.Clear(); }

		public ref UNullable<T> this[int i] => ref Values[i];

	}

}
