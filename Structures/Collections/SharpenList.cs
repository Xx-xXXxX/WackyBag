

using System.Collections;
using System.Diagnostics;

namespace WackyBag.Structures.Collections
{
	/// <summary>
	/// 分别管理Id和值列表，保证值列表连续，但是不保证顺序
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SharpenList<T> : IEnumerable<WithId<T>>, IEnumerable<T>
	{
		protected UnorderedList<T> values;
		protected UIdToSIdList idList;

		public IReadOnlyList<T> Values => values;
		public IReadOnlyList<int> SIdToUId => idList.SIdToUId;
		public IReadOnlyIndexable<int> UIdToSId => idList.UIdToSId;

		public int Count => values.Count;


		//public IdOf<T> Add(in T v)
		//{
		//	var (UId,SId)=idList.Add();
		//	return new(UId);
		//}

		public void Add(int UId, in T v)
		{
			idList.Add(UId);
			values.Add(v);
			//return new(UId);
		}

		public void Add(IdOf<T> UId, in T v)
		{
			idList.Add(UId.Id);
			values.Add(v);
			//return new(UId);
		}

		public void Remove(int UId)
		{
			int SId = idList.UIdToSId[UId];
			values.Remove(SId);
			idList.RemoveByUId(UId);
		}

		public void Remove(IdOf<T> UId)
		{
			int SId = idList.UIdToSId[UId.Id];
			values.Remove(SId);
			idList.RemoveByUId(UId.Id);
		}

		public IEnumerator<WithId<T>> GetEnumerator()
		{
			//return values.GetEnumerator();
			for (int i = 0; i < values.Count; i++)
			{
				yield return new(SIdToUId[i],values.GetRef(i));
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)values).GetEnumerator();
		}


		public SharpenList()
		{
			values = [];
			idList = new();
		}
		public ref T this[int UId] => ref values[idList.UIdToSId[UId]];
		public ref T this[IdOf<T> UId] => ref values[idList.UIdToSId[UId.Id]];

		public void Clear() { 
			values.Clear();
			idList.Clear();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return Values.GetEnumerator();
		}
	}

}
