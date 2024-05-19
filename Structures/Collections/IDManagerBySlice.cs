using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures.Collections
{
	public interface IReadOnlyIDManager
	{
		bool Check(int id);
		IEnumerator<int> GetEnumerator();
		int LastOrDef(int def = 0);
		int NextID(int id);
		int NextOrDef(int leftEdge = 0);
		List<int> ToArray();
	}

	/// <summary>
	/// 用于管理检查id
	/// O(log n) 添加，移除，检查
	/// O(n) 遍历
	/// </summary>
	public class IDManagerBySlice : IEnumerable<int>, IReadOnlyIDManager
	{
		private readonly SetEx<int> IDSection = [];
		public IDManagerBySlice()
		{
		}
		/// <summary>
		/// 改变该id的使用状态
		/// O(log n)
		/// </summary>
		public void ChangeUsing(int id)
		{
			if (!IDSection.Remove(id)) IDSection.Add(id);
			if (!IDSection.Remove(id + 1)) IDSection.Add(id + 1);
		}
		/// <summary>
		/// 添加id使用
		/// 实为ChangeUsing
		/// 不检查
		/// O(log n)
		/// </summary>
		public void Add(int id) => ChangeUsing(id);
		public int Add()
		{
			int id = NextOrDef();
			Add(id);
			return id;
		}
		/// <summary>
		/// 移除id使用
		/// 实为ChangeUsing
		/// 不检查
		/// O(log n)
		/// </summary>
		public void Remove(int id) => ChangeUsing(id);
		///// <summary>
		///// 获取下一个可用的ID
		///// O(1)
		///// </summary>
		//public int NextID()
		//{
		//	return IDSection.First.Key;
		//}
		/// <summary>
		/// 获取大于等于leftEdge的最小可用的ID，如果没有，返回def
		/// leftEdge应该为左边界
		/// </summary>
		public int NextOrDef(int leftEdge = 0)
		{
			//return IDSection.First?.Next!.Key??def;
			var v = IDSection.First;
			if (v == null) return leftEdge;
			bool begin = true;
			var i = v.Key - 1;
			while (i < leftEdge)
			{
				v = v.next;
				begin = !begin;
				if (v == null) return leftEdge;
				i = begin ? v.Key - 1 : v.Key;
			}
			return begin?leftEdge:i;
		}

		/// <summary>
		/// 获取比所有已用ID都大的第一个可用的ID，如果没有，返回def
		/// O(1)
		/// </summary>
		public int LastOrDef(int def = 0)
		{
			return IDSection.Last?.Key ?? def;
		}
		/// <summary>
		/// 枚举所有正在使用的id
		/// O(n)
		/// </summary>
		public IEnumerator<int> GetEnumerator()
		{
			int now = 0;
			bool Having = false;
			foreach (var i in IDSection)
			{
				if (Having)
				{
					while (now < i)
					{
						yield return now;
						now += 1;
					}
					Having = false;
				}
				else
				{
					now = i; Having = true;
				}
			}
		}
		/// <summary>
		/// 检查该ID是否在使用
		/// O(log n)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool Check(int id)
		{
			if (IDSection.First == null) return false;
			if (id < IDSection.First.Key) return false;
			var i = IDSection.FindNotGreater(id);
			if (i == null) return false;
			return IDSection.GetIndex(i.Key) % 2 == 0;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		/// <summary>
		/// 重置
		/// </summary>
		public void Clear()
		{
			IDSection.Clear();
		}

		/// <summary>
		/// 返回不小于该id的下一个未使用ID
		/// O(log n)
		/// </summary>
		public int NextID(int id)
		{
			if (IDSection.Last == null) return id;
			if (id >= IDSection.Last.Key) return id;
			var i = IDSection.FindGreater(id);
			if (i == null) return id;
			if (IDSection.GetIndex(i.Key) % 2 == 1) return i.Key;
			else return id;
		}
		/// <summary>
		/// 所有使用的ID
		/// O(n)
		/// </summary>
		public List<int> ToArray()
		{
			List<int> vs = [.. this];
			return vs;
		}
	}
	/// <summary>
	/// 有IDManagerBySlice管理的列表
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class IDManagedListBySlice<T> :IEnumerable<T>
	{
		/// <summary>
		/// 值列表
		/// </summary>
		public MyList<UNullable<T>> Values { get; protected set; } = [];
		public IDManagerBySlice Manager { get; protected set; } = [];
		/// <summary>
		/// 添加在最小可用ID
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int Add(T value) {
			int index = Manager.NextOrDef();
			while (Values.Count <= index) Values.Add(default);
			Values[index] = value;
			Manager.Add(index);
			return index;
		}
		public int Add(int index,T value)
		{
			while (Values.Count <= index) Values.Add(default);
			Values[index] = value;
			Manager.Add(index);
			return index;
		}
		public void Remove(int index) { 
			Values[index]= default;
			Manager.Remove(index);
			if (Manager.LastOrDef()<Values.Count)
			{
				index = Manager.LastOrDef();
				Values.RemoveRange(index, Values.Count - index);
			}
		}
		public void Clear() { Values.Clear();Manager.Clear(); }

		public ref UNullable<T> this[int i]=>ref Values[i];

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var i in Manager)
			{
				//Debug.Assert(Values[i].Value != null);
				yield return Values[i].Value!;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

}
