using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures.Collections
{
	public class LinkedForwardStar:IList2DIndex
	{
		private readonly MyList<int> next=[];
		private readonly MyList<int> previous = [];
		private readonly OpenList<int> root=new();
		public void AddRoot(int rootId) {
			root.TrySet(rootId, -1);
		}
		public void Add(int rootId,int index) {
			int n = root[rootId];
			int p = -1;
			previous.CheckIndex(index);
			previous[index] = p;
			if (n >= 0) {
				previous[n] = index;
			}
			root[rootId] = index;
			//Array.Resize
		}
		public void Remove(int rootId, int index)
		{
			int n = next[index];
			int p = previous[index];
			if (n >= 0) previous[n] = p;
			if (p >= 0) next[p] = n;
			else root[rootId] = n;
		}
		public IEnumerable<int> RemoveAll(int rootId) {
			int i = root[rootId];
			root[rootId] = default;
			while (i >= 0)
			{
				int j = next[i];
				yield return i;
				next[i] = default;
				i = j;
			}
		}
		public IEnumerable<int> GetEnum(int rootId) {
			int i = root[rootId];
			while (i >= 0)
			{
				yield return i;
				i = next[i];
			}
		}
	}

	public class LinkedForwardStarList<T>:IList2D<T> {
		private readonly LinkedForwardStar link = new();
		private readonly MyList<T> list = [];
		public void AddRoot(int rootId) => link.AddRoot(rootId);
		
		public void Add(int rootId,int index, in T value) {
			link.Add(rootId,index);
			list.CheckIndex(index);
			list[index] = value;
		}
		public void Remove(int rootId, int index) {
			list[index] = default!;
			link.Remove(rootId, index);
		}
		public IEnumerable<WithId<T>> GetEnum(int startId) {
			foreach (var item in link.GetEnum(startId)) {
				yield return new(item,list.GetRef(item));
			}
		}
		public IEnumerable<int> RemoveAll(int rootId) {
			foreach (var item in link.RemoveAll(rootId))
			{
				yield return item;
				list[item] = default!;
			}
		}
	}
}
