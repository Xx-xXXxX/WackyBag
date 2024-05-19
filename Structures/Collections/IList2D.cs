using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures.Collections
{
	public interface IList2DIndex {
		public void AddRoot(int root);
		public void Add(int root, int index);
		public IEnumerable<int> GetEnum(int root);
		public IEnumerable<int> RemoveAll(int root);
		public void Remove(int root, int index);
	}

	public interface IList2D<T>
	{
		public void AddRoot(int root);
		public void Add(int root, int index, in T value);
		public IEnumerable<WithId<T>> GetEnum(int root);
		public IEnumerable<int> RemoveAll(int root);
		public void Remove(int root, int index);
	}
}
