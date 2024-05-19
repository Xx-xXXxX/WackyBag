using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures.Collections
{
	public interface ITypedList<TBase> :IReadOnlyList<TBase>, IEnumerable<TBase> {
		public int Add(TBase v);
		public TBase Get(int id);
		public T Get<T>(IdOf<T> id)
			where T : TBase;
	}

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
