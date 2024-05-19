using WackyBag;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures
{
	/// <summary>
	/// 并查集，给出一些节点的相关性，得到按节点关联的分组
	/// </summary>
	public class DisjointSet
	{
		/// <summary>
		/// 节点的父节点
		/// </summary>
		public readonly List<int> ToParent = [];
		/// <summary>
		/// 节点所在组的大小
		/// </summary>
		public readonly List<int> Counts = [];
		/// <summary>
		/// 节点总数
		/// </summary>
		public int Count => ToParent.Count;
		public void Clear()
		{
			ToParent.Clear();
			Counts.Clear();
		}
		/// <summary>
		/// 加一个节点
		/// </summary>
		/// <returns>节点的id</returns>
		public int Add()
		{
			int i = Count;
			ToParent.Add(i);
			Counts.Add(1);
			return i;
		}
		/// <summary>
		/// 加count个节点
		/// </summary>
		/// <param name="count"></param>
		/// <returns>节点的范围</returns>
		public (int, int) Add(int count)
		{
			int l = Count;
			int r = Count + count;
			for (int i = l; i < r; ++i)
			{
				ToParent.Add(i);
				Counts.Add(1);
			}
			return (l, r);
		}
		/// <summary>
		/// 获取a的父节点，并调整路径上的节点的ToParent
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public int Parent(int a)
		{
			int RealParent = a;
			while (ToParent[RealParent] != RealParent)
			{
				RealParent = ToParent[RealParent];
			}
			int next = a;
			int current = a;
			while (ToParent[next] != RealParent)
			{
				next = ToParent[next];
				ToParent[current] = RealParent;
				current = next;
			}
			return RealParent;
		}
		/// <summary>
		/// 设置a与b相关
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public void AddLink(int a, int b)
		{
			a = Parent(a);
			b = Parent(b);
			if (a == b) return;

			if (Counts[b] > Counts[a]) Util.Swap(ref a, ref b);
			ToParent[b] = a;
			Counts[a] += Counts[b];
		}
		/// <summary>
		/// 获取组的总数
		/// </summary>
		/// <returns></returns>
		public int GetSetsCount()
		{
			int res = 0;
			for (int i = 0; i < Count; ++i)
			{
				if (i == ToParent[i]) res += 1;
			}
			return res;
		}
		/// <summary>
		/// 获取结果，给出每个组的每个节点
		/// </summary>
		/// <returns></returns>
		public List<List<int>> GetResult()
		{
			List<List<int>> Contains = [];
			for (int i = 0; i < Count; ++i)
			{
				Contains[i].Add(new());
			}
			for (int i = 0; i < Count; ++i)
			{
				Contains[Parent(i)].Add(i);
			}
			List<List<int>> Result = [.. Contains];
			return Result;
		}
	}
}
