using WackyBag.Structures;

using WackyBag;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WackyBag.Structures.Collections;

//#nullable enable
namespace WackyBag.Structures
{


	/// <summary>
	/// 根据要求排序，支持继承
	/// int为索引
	/// </summary>
	public class SortedByOrder<TValue> : IEnumerable<TValue>
	{
		public SortedByOrderNode Get(int Index) => Nodes[Index];
		//public SortedByOrderNode this[(int,int) index]=>Find(index);
		/// <summary>
		/// 为false时需要重新计算结果
		/// </summary>
		protected bool updated = false;
		/// <summary>
		/// 继承所在层
		/// </summary>
		public readonly int level = 0;
		/// <summary>
		/// 父对象
		/// </summary>
		public readonly SortedByOrder<TValue>? Parent = null;
		/// <summary>
		/// 所有节点
		/// </summary>
		protected readonly List<SortedByOrderNode> Nodes = [];
		protected List<SortedByOrderNode> CurrentNodes => Nodes;
		/// <summary>
		/// 所有子对象
		/// </summary>
		protected readonly List<SortedByOrder<TValue>> Sons = [];
		/// <summary>
		/// 节点
		/// </summary>
		public class SortedByOrderNode
		{
			/// <summary>
			/// 获取其索引
			/// </summary>
			/// <returns></returns>
			public int GetIndex() => index;
			/// <summary>
			/// 值
			/// </summary>
			public TValue Value;
			/// <summary>
			/// 容器
			/// </summary>
			public SortedByOrder<TValue> Container;
			/// <summary>
			/// 位置
			/// </summary>
			public readonly int index;
			public readonly SortedByOrderNode? super;
			/// <summary>
			/// 排序时的状态
			/// </summary>
			public enum State
			{
				/// <summary>
				/// 未计算
				/// </summary>
				UnChecked,
				/// <summary>
				/// 正在计算
				/// </summary>
				Checking,
				/// <summary>
				/// 计算完毕
				/// </summary>
				Checked
			}
			/// <summary>
			/// 状态
			/// </summary>
			public State state;
			public SortedByOrderNode(SortedByOrder<TValue> container, TValue Value, int index)
			{
				Container = container;
				this.Value = Value;
				this.index = index;
			}
			public SortedByOrderNode(SortedByOrder<TValue> container, SortedByOrderNode extend) : this(container, extend.Value, extend.index)
			{
				this.super = extend;
			}
			/// <summary>
			/// 在该节点之前的节点
			/// </summary>
			public readonly List<int> befores = [];
			/// <summary>
			/// 在该节点之前的节点
			/// </summary>
			public IEnumerable<int> Befores() => befores;
			/// <summary>
			/// 将在这之前的节点和该节点加入列表
			/// </summary>
			/// <exception cref="Exception">循环引用</exception>
			public void AddInList(List<SortedByOrderNode> res)
			{
				switch (state)
				{
					case State.Checked: return;
					case State.Checking: throw new Exception("Looping Requires");
					case State.UnChecked:
						state = State.Checking;
						foreach (var i in Befores()) Container.Get(i).AddInList(res);
						res.Add(this);
						state = State.Checked;
						break;
				}
			}
		}
		public SortedByOrder()
		{
		}
		/// <summary>
		/// 设置自己和所有引用的update
		/// </summary>
		protected void SetUpdate()
		{
			Util.Enum(this, (SortedByOrder<TValue> tar) =>
			{
				tar.updated = false;
				return tar.Sons;
			});
		}
		/// <summary>
		/// 加入元素
		/// </summary>
		/// <param name="value"></param>
		/// <returns>元素索引</returns>
		public int Add(TValue value)
		{
			SortedByOrder<TValue>.SortedByOrderNode res = new(this, value, CurrentNodes.Count);
			CurrentNodes.Add(res);

			SetUpdate();
			//Utils.Enum(this.Sons, (SortedByOrder<TValue>  tar) => { 
			//	tar.Add(new SortedByOrderNode(tar,res));
			//	tar.updated = false;
			//	return tar.Sons;
			//});

			return res.GetIndex();
		}
		/// <summary>
		/// 添加顺序
		/// </summary>
		public void AddOrder(int before, int after)
		{
			Get(after).befores.Add(before);
			//Utils.Enum(this, (SortedByOrder<TValue> tar) =>
			//{
			//	//tar.FindCurrent(after).requires.Add(before);
			//	tar.updated = false;
			//	return tar.Sons;
			//});
			SetUpdate();
		}
		public IEnumerator<TValue> GetEnumerator()
		{
			return ((IEnumerable<TValue>)Res).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)Res).GetEnumerator();
		}
		protected void GetRes() {
			res.Clear();
			foreach (var node in Nodes)
			{
				node.state = SortedByOrderNode.State.UnChecked;
			}
			foreach (var node in Nodes)
			{
				node.AddInList(res);
			}
			updated = true;
		}
		protected List<SortedByOrderNode> res = [];
		/// <summary>
		/// 结果，自动更新
		/// </summary>
		public List<SortedByOrderNode> Res
		{
			get
			{
				if (!updated)
				{
					GetRes();
				}
				return res;
			}
		}
		/// <summary>
		/// 添加顺序
		/// </summary>
		public void AddOrders(int tar, IEnumerable<int>? befores, IEnumerable<int>? afters)
		{
			if (befores != null) foreach (var i in befores) AddOrder(i, tar);
			if (afters != null) foreach (var i in afters) AddOrder(tar, i);
		}
		/// <summary>
		/// 添加顺序
		/// </summary>
		public void AddOrderChain(params int[] values)
		{
			int l = values.Length - 1;
			for (int i = 0; i < l; ++i)
			{
				AddOrder(values[i], values[i + 1]);
			}
		}
		
	}
	public interface IOrder<TKey>
		where TKey:notnull
	{
		public TKey Key { get; }
		public List<TKey> Befores { get; }
		public List<TKey> Afters { get; }
	}
	public record Order<TKey>(TKey Key, List<TKey>? Befores = null, List<TKey>? Afters = null) : IOrder<TKey> where TKey : notnull {
		public List<TKey> Befores { get; init; } =Befores??[];
		public List<TKey> Afters { get; init; } =Afters ?? [];
	}

	/// <summary>
	/// 按key索引，可以在没有加入key的情况下使用key
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class SortedByOrderWithKey<TKey, TValue> : SortedByOrder<KeyValuePair<TKey,UNullable< TValue>>>, IEnumerable<TValue>
		//where TValue=class
		where TKey : notnull
	{
		protected readonly Dictionary<TKey, SortedByOrderNode> dictionary = [];
		public SortedByOrderNode GetByKey(TKey key) => dictionary[key];
		public int Add(TKey key, TValue value)
		{
			if (dictionary.TryGetValue(key, out var res))
			{
				res.Value = new(key, value);
			}
			else
			{
				res = Get(base.Add((KeyValuePair<TKey, UNullable<TValue>>)new(key, value)));
				dictionary.Add(key, res);
			}
			return res.GetIndex();
		}
		/// <summary>
		/// 获取key所在的节点，没有则自动创建
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public SortedByOrderNode Get(TKey key)
		{

			SortedByOrderNode? res = null;
			var n = this;
			while (n != null)
			{
				if (n.dictionary.TryGetValue(key, out res)) break;
				n = (SortedByOrderWithKey<TKey, TValue>?)n.Parent;
			}
			if (res == null)
			{
				res = Get(base.Add((KeyValuePair<TKey, UNullable<TValue>>)new(key, default)));
				dictionary.Add(key, res);
			}
			return res;
		}
		public void AddOrder(TKey before, TKey after)
		{
			var b = Get(before);
			var a = Get(after);
			a.befores.Add(b.GetIndex());
		}
		public void AddOrderChain(params TKey[] keys)
			=> AddOrderChain(keys.Select((i) => Get(i).GetIndex()).ToArray());

		public void AddOrders(Order<TKey> order) { 
			var (k,b,a)=order;
			AddOrders(k, b, a);
		}
		public void AddOrders(TKey tar, IEnumerable<TKey>? befores, IEnumerable<TKey>? afters)
			=> AddOrders(Get(tar).GetIndex(), befores?.Select((i) => Get(i).GetIndex()), afters?.Select((i) => Get(i).GetIndex()));
		public int Add(OrderedValue orderedValue)
		{
			var res = Add(orderedValue.Key, orderedValue.Value);
			AddOrders(orderedValue.Key, orderedValue.Befores, orderedValue.Afters);
			return res;
		}


		public int AddOrdered<T>(T value)
			where T : TValue, IOrder<TKey>
		{
			var res = Add(value.Key, value);
			AddOrders(value.Key, value.Befores, value.Afters);
			return res;
		}
		public new IEnumerator<TValue> GetEnumerator()
		{
			// => Res.Select((i) => i.Value).Where((i)=>i!=null).GetEnumerator();
			foreach (var i in Res)
			{
				var v = i.Value.Value;
				if (v != null) yield return v.Value!;
			}
		}


		/// <summary>
		/// 将值，键，顺序结合
		/// </summary>
		/// <param name="Key"><inheritdoc/></param>
		/// <param name="Befores"><inheritdoc/></param>
		/// <param name="Afters"><inheritdoc/></param>
		/// <param name="Value"></param>
		public record OrderedValue(TValue Value, TKey Key, List<TKey>? Befores = null, List<TKey>? Afters=null) : Order<TKey>(Key, Befores, Afters)
		{
			public OrderedValue(TValue value, IOrder<TKey> order) : this(value, order.Key, order.Befores, order.Afters)
			{
			}
		}
		public Order<TKey> GetOrder(SortedByOrderNode node) {
			return new(node.Value.Key,new( node.Befores().Select(i=>Get(i).Value.Key)),null);
		}
	}
	//[Obsolete("Use SortedByOrderWithKey<TKey,Func<TTar,TValue>>")]
	//public class SBOKOrder<TKey, TIndex, TValue>
	//where TKey : notnull
	//{
	//	protected SortedByOrderWithKey<TKey, (TIndex, int)> KeyToId = [];
	//	public void Add(IReadOnlyList<IOrder<TKey>> values, TIndex index)
	//	{
	//		if (values == null) return;
	//		for (int i = 0; i < values.Count; ++i)
	//		{
	//			var value = values[i];
	//			KeyToId.Add(value.Key, (index, i));
	//			KeyToId.AddOrders(value.Key, value.Befores, value.Afters);
	//		}
	//	}
	//	public List<TValue> GetValues(Func<TIndex, IList<TValue>> func)
	//	{
	//		List<TValue> res = [];
	//		foreach (var i in KeyToId)
	//		{
	//			res.Add(func(i.Item1)[i.Item2]);
	//		}
	//		return res;
	//	}
	//	public void GetValues(Func<TIndex, IList<SortedByOrderWithKey<TKey, TValue>.OrderedValue>> func, List<TValue> res)
	//	{
	//		foreach (var i in KeyToId)
	//		{
	//			var v = func(i.Item1);
	//			res.Add(v[i.Item2].Value);
	//		}
	//	}
	//	public IEnumerable<TValue> EnumValues(Func<TIndex, IList<TValue>> func)
	//	{
	//		foreach (var i in KeyToId)
	//		{
	//			yield return func(i.Item1)[i.Item2];
	//		}
	//	}
	//	public IEnumerable<SortedByOrderWithKey<TKey, TValue>.OrderedValue> EnumOrderedValue(Func<TIndex, IList<TValue>> func) {
	//		foreach (var i in KeyToId.Res)
	//		{
	//			var v = i.Value.Value.Value;
	//			yield return new( func(v.Item1)[v.Item2],KeyToId.GetOrder(i));
	//		}
	//	}
	//	public IEnumerable<(TIndex, int)> EnumIndex() { 
	//		return (IEnumerable<(TIndex, int)>)KeyToId.GetEnumerator();
	//	}
	//	public IEnumerable<SortedByOrderWithKey<TKey, (TIndex, int)>.OrderedValue> EnumOrderedIndex() {
	//		foreach (var i in KeyToId.Res) {
	//			var v = i.Value.Value;
	//			if (v == null) continue;
	//			yield return new(v.Value,KeyToId.GetOrder(i));
	//		}
	//	}
	//}

	/// <summary>
	/// 根据要求排序，支持继承
	/// (int,int)为索引
	/// </summary>
	public class SortedByOrderExtendable<TValue> : IEnumerable<TValue>
	{
		public SortedByOrderNode Get((int, int) Index) => Nodes[Index.Item1][Index.Item2];
		//public SortedByOrderNode this[(int,int) index]=>Find(index);
		/// <summary>
		/// 为false时需要重新计算结果
		/// </summary>
		protected bool updated = false;
		/// <summary>
		/// 继承所在层
		/// </summary>
		public readonly int level = 0;
		/// <summary>
		/// 父对象
		/// </summary>
		public readonly SortedByOrderExtendable<TValue>? Parent = null;
		/// <summary>
		/// 所有节点
		/// </summary>
		protected readonly List<List<SortedByOrderNode>> Nodes = [];
		protected List<SortedByOrderNode> CurrentNodes => Nodes[level];
		/// <summary>
		/// 所有子对象
		/// </summary>
		protected readonly List<SortedByOrderExtendable<TValue>> Sons = [];
		/// <summary>
		/// 节点
		/// </summary>
		public class SortedByOrderNode
		{
			/// <summary>
			/// 获取其索引
			/// </summary>
			/// <returns></returns>
			public (int, int) GetIndex() => (level, index);
			/// <summary>
			/// 值
			/// </summary>
			public TValue Value;
			/// <summary>
			/// 容器
			/// </summary>
			public SortedByOrderExtendable<TValue> Container;
			/// <summary>
			/// 位置
			/// </summary>
			public readonly int index;
			/// <summary>
			/// 层
			/// </summary>
			public readonly int level;
			public readonly SortedByOrderNode? super;
			/// <summary>
			/// 排序时的状态
			/// </summary>
			public enum State
			{
				/// <summary>
				/// 未计算
				/// </summary>
				UnChecked,
				/// <summary>
				/// 正在计算
				/// </summary>
				Checking,
				/// <summary>
				/// 计算完毕
				/// </summary>
				Checked
			}
			/// <summary>
			/// 状态
			/// </summary>
			public State state;
			public SortedByOrderNode(SortedByOrderExtendable<TValue> container, TValue Value, int index, int level)
			{
				Container = container;
				this.Value = Value;
				this.index = index;
				this.level = level;
			}
			public SortedByOrderNode(SortedByOrderExtendable<TValue> container, SortedByOrderNode extend) : this(container, extend.Value, extend.index, extend.level)
			{
				this.super = extend;
			}
			/// <summary>
			/// 在该节点之前的节点
			/// </summary>
			public readonly List<(int, int)> befores = [];
			/// <summary>
			/// 在该节点之前的节点
			/// </summary>
			public IEnumerable<(int, int)> Befores()
			{
				var t = this;
				while (t != null)
				{
					foreach (var i in t.befores) yield return i;
					t = t.super;
				}
				//return befores;
			}
			/// <summary>
			/// 将在这之前的节点和该节点加入列表
			/// </summary>
			/// <exception cref="Exception">循环引用</exception>
			public void AddInList(List<SortedByOrderNode> res)
			{
				switch (state)
				{
					case State.Checked: return;
					case State.Checking: throw new Exception("Looping Requires");
					case State.UnChecked:
						state = State.Checking;
						foreach (var i in Befores()) Container.Get(i).AddInList(res);
						res.Add(this);
						state = State.Checked;
						break;
				}
			}
		}
		public SortedByOrderExtendable(SortedByOrderExtendable<TValue>? Parent = null)
		{
			if (Parent != null)
			{
				Parent.Sons.Add(this);
				foreach (var i in Parent.Nodes)
				{
					List<SortedByOrderExtendable<TValue>.SortedByOrderNode> l = [];
					//Nodes.Add(l);
					foreach (var j in i)
					{
						l.Add(new(this, j));
					}
					Nodes.Add(i);
				}
				level = Parent.level + 1;
			}
			Nodes.Add([]);
		}
		/// <summary>
		/// 设置自己和所有引用的update
		/// </summary>
		protected void SetUpdate()
		{
			Util.Enum(this, (SortedByOrderExtendable<TValue> tar) =>
			{
				tar.updated = false;
				return tar.Sons;
			});
		}
		/// <summary>
		/// 加入元素
		/// </summary>
		/// <param name="value"></param>
		/// <returns>元素索引</returns>
		public (int, int) Add(TValue value)
		{
			SortedByOrderExtendable<TValue>.SortedByOrderNode res = new(this, value, CurrentNodes.Count, level);
			CurrentNodes.Add(res);

			SetUpdate();
			//Utils.Enum(this.Sons, (SortedByOrder<TValue>  tar) => { 
			//	tar.Add(new SortedByOrderNode(tar,res));
			//	tar.updated = false;
			//	return tar.Sons;
			//});

			return res.GetIndex();
		}
		/// <summary>
		/// 添加顺序
		/// </summary>
		public void AddOrder((int, int) before, (int, int) after)
		{
			Get(after).befores.Add(before);
			//Utils.Enum(this, (SortedByOrder<TValue> tar) =>
			//{
			//	//tar.FindCurrent(after).requires.Add(before);
			//	tar.updated = false;
			//	return tar.Sons;
			//});
			SetUpdate();
		}
		public IEnumerator<TValue> GetEnumerator()
		{
			return ((IEnumerable<TValue>)Res).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)Res).GetEnumerator();
		}
		protected void GetRes()
		{
			res.Clear();
			foreach (var nodes in Nodes)
			{
				foreach (var node in nodes)
				{
					node.state = SortedByOrderNode.State.UnChecked;
				}
			}
			foreach (var nodes in Nodes)
			{
				foreach (var node in nodes)
				{
					node.AddInList(res);
				}
			}
			updated = true;
		}
		protected List<SortedByOrderNode> res = [];
		/// <summary>
		/// 结果，自动更新
		/// </summary>
		public List<SortedByOrderNode> Res
		{
			get
			{
				if (!updated)
				{
					GetRes();
				}
				return res;
			}
		}
		/// <summary>
		/// 添加顺序
		/// </summary>
		public void AddOrders((int, int) tar, IEnumerable<(int, int)>? befores, IEnumerable<(int, int)>? afters)
		{
			if (befores != null) foreach (var i in befores) AddOrder(i, tar);
			if (afters != null) foreach (var i in afters) AddOrder(tar, i);
		}
		/// <summary>
		/// 添加顺序
		/// </summary>
		public void AddOrderChain(params (int, int)[] values)
		{
			int l = values.Length - 1;
			for (int i = 0; i < l; ++i)
			{
				AddOrder(values[i], values[i + 1]);
			}
		}

	}
}
