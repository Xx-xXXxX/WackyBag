using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures
{
	public interface IReadOnlyIndexable<T>
	{
		public T this[int index] { get; }
	}
	public interface IIndexable<T>: IReadOnlyIndexable<T>
	{
		public new T this[int index] { get; set; }
	}
	public interface IRefIndexable<T> { 
		public ref T this[int index] { get; }
	}
}
