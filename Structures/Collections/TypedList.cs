using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures.Collections
{
	public interface IReadOnlyTypedList<TBase> : IReadOnlyList<TBase>, IEnumerable<TBase>
	{
		public T Get<T>(IdOf<T> id)
			where T : TBase;
	}

	public interface ITypedList<TBase> : IReadOnlyTypedList<TBase>
	{
		public int Add(TBase v);
	}
	//[Obsolete("Use Util.Get")]
	public class TypedList<TBase>: ITypedList<TBase>
	{
		protected readonly List<TBase> values = [];

		public int Count=>values.Count;

		public TypedList() { }
		public int Add(TBase v) {
			int id = values.Count;
			values.Add(v);
			return id;
		}
		public TBase Get(int id) => values[id];
		public T Get<T>(IdOf<T> id)
			where T : TBase
			=> (T)values[(int)id]!;
		public TBase this[int id]=>values[id];
		public IEnumerator<TBase> GetEnumerator()
		{
			return ((IEnumerable<TBase>)values).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)values).GetEnumerator();
		}
	}

	
}
