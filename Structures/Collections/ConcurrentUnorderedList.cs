using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures.Collections
{
	[Obsolete("NotImplimented",true)]
	public class ConcurrentUnorderedList<T>:IEnumerable<Ref<T>>
	{
		public ConcurrentUnorderedList(int ALength = ALengthDef,int BLengthLog= BLengthLogDef) {
			this.BLengthLog = BLengthLog;
			this.BLength= 1 << BLengthLog;
			this.BLengthSelect = BLength - 1;
			lists = new T[ALength][];
			for (int i = 0; i < ALength; i++)
			{
				lists[i] = new T[BLength];
			}
			listCount = ALength;
			CreateBefore = ALength;
		}
		protected const int ALengthDef = 4;

		protected const int BLengthLogDef = 8;
		protected readonly int BLengthLog;
		protected readonly int BLength;
		protected readonly int BLengthSelect;

		protected volatile T[][] lists;
		protected int ListCapacity=>lists.Length;

		protected volatile int listCount;
		protected volatile int totalCount=0;
		public int Count => totalCount;

		protected int ISA(int i) => i >> BLengthLog;
		protected int ISB(int i)=>i& BLengthSelect;
		public ref T this[int index]=>ref lists[ISA(index)][ISB(index)];

		protected volatile bool mlock=false;

		protected readonly int CreateBefore;

		public WithId<T> Add(in T item) { 
			int id=Interlocked.Increment(ref totalCount)-1;
			int isa=ISA(id);
			int isb=ISB(id);
			int Create = isa + CreateBefore;
			if (Interlocked.CompareExchange(ref listCount, Create, Create + 1)== Create && isb==0) {
				SpinWait.SpinUntil(() => Create < lists.Length);
				lists[Create] =new T[BLength];
				if (listCount == ListCapacity - 1) {
					int NewCapacity = ListCapacity * 2;
					T[][] NewLists = new T[NewCapacity][];
					Array.Copy(lists, NewLists, lists.Length);
					lock (lists)
					{
						lists = NewLists;
					}
				}
			}
			SpinWait.SpinUntil(() => isa < lists.Length);
			SpinWait.SpinUntil(()=>lists[isa]!=null);
			T[] list = lists[isa];
			WithId<T> res = new(id, ref list[isb])
			{
				Value = item
			};
			return res;
		}

#pragma warning disable IDE0060 // 删除未使用的参数
		public void Remove(int index)
#pragma warning restore IDE0060 // 删除未使用的参数
		{
			throw new NotImplementedException();
#pragma warning disable CS0162 // 检测到无法访问的代码
			int idfrom = Interlocked.Decrement(ref totalCount);
			T value = this[idfrom];
#pragma warning restore CS0162 // 检测到无法访问的代码
		}

		public IEnumerator<Ref<T>> GetEnumerator()
		{
			int c = Count;
			for (int i = 0; i < lists.Length; i++)
			{
				var l= lists[i];
				for (int j = 0; j < BLength; j++) { 
					yield return l.GetRef(j);
					c--;
					if(c<=0) yield break;
				}
			}
			
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
