using FixMath;

using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

using WackyBag.Structures;
using WackyBag.Structures.Collections;

namespace WackyBag
{
	//[StaticLoad("FirstLoad", null, null, ["Load1"])]
	public static partial class Util
	{
		///// <summary>
		///// 游戏频率
		///// </summary>
		//public static readonly int frequency = 50;
		///// <summary>
		///// 游戏每帧时间
		///// </summary>
		//public static readonly Fix64 deltaTime = Fix64.One / frequency;
		//public const string FirstLoadStr = "FirstLoad";
		static Util()
		{
			//GetAsset<Texture2D>.GetValueFn = Resources.Load<Texture2D>;
		}
		/// <summary>
		/// 对tar和func返回值执行func
		/// </summary>
		/// <typeparam name="Ttar"></typeparam>
		/// <param name="tar"></param>
		/// <param name="func">执行并返回</param>
		public static void Enum<Ttar>(Ttar tar, Func<Ttar, IEnumerable<Ttar>> func) => Enum<Ttar>(new List<Ttar>() { tar }, func);
		/// <summary>
		/// 对tars和func返回值执行func
		/// </summary>
		/// <typeparam name="Ttar"></typeparam>
		/// <param name="tars"></param>
		/// <param name="func">执行并返回</param>
		public static void Enum<Ttar>(IEnumerable<Ttar> tars, Func<Ttar, IEnumerable<Ttar>> func)
		{
			Queue<Ttar> Tars = new();
			foreach (var i in tars)
			{
				Tars.Enqueue(i);
			}
			while (Tars.Count > 0)
			{
				var t = Tars.Dequeue();
				foreach (var i in func(t))
				{
					Tars.Enqueue(i);
				}
			}
		}
		public static void Swap<T>(ref T a, ref T b)
		{
			(b, a) = (a, b);
		}
		
		public static T Max<T>(params T[] values)
			where T : IComparable<T>
		{
			T t = values[0];
			for (int i = 1; i < values.Length; ++i)
			{
				if (t.CompareTo(values[i]) < 0) t = values[i];
			}
			return t;
		}
		public static T Min<T>(params T[] values)
			where T : IComparable<T>
		{
			T t = values[0];
			for (int i = 1; i < values.Length; ++i)
			{
				if (t.CompareTo(values[i]) > 0) t = values[i];
			}
			return t;
		}
		public static bool HaveAttribute<T>(this Type type, bool inhert = false)
			where T : Attribute
			=> type.GetCustomAttributes<T>(inhert).Any();
		public static bool HaveAttribute(this Type type, Type attribute, bool inhert = false)
			=> type.GetCustomAttributes(attribute, inhert).Length != 0;
		
		public static UNullable<T> ToU<T>(this T? v)
			where T : struct
		{
			if (!v.HasValue) return UNullable<T>.Null;
			else return new(v.Value);
		}
		public static UNullable<T> ToU<T>(this UNullable<T>? v)
		{
			if (!v.HasValue) return UNullable<T>.Null;
			return v.Value;
		}
		public static RectFix GetGridRect(Point point, Fix64 GridSize)
		{
			return new(point.X * GridSize, point.Y * GridSize, GridSize, GridSize);
		}
		public static void Invoke(this DrawFn DrawFn, Vector2Fix pos, Fix64 rot, Fix64 scale)
		{
			DrawFn((Vector2)pos, (float)rot, (float)scale);
		}
		public delegate void DrawFn(Vector2 pos, float rot, float scale);
		public delegate DrawFn? GetDrawFn(string? str);
		public static void InvokeAndSet<TTar, TRes>(IEnumerable<Func<TTar, TRes>> Fns, TTar tar, Action<TRes> AddFn)
		{
			foreach (var item in Fns)
			{
				AddFn(item(tar));
			}
		}
		




		

		//public static int FlipInt(this in int v) => -v - 1;
		public static Ref<T> ToRef<T>(this ref T v)where T:struct => new(ref v);
		public static Ref<T> GetRef<T>(this T[] list, in int index) => new(ref list[index]);

		public static ref T Get<T>(this T[] values, IdOf<T> Index) => ref values[(int)Index];

		public static T Get<T,TBase>(this IReadOnlyList<TBase> values, IdOf<T> Index) 
			where T:TBase
			=> (T)values[(int)Index]!;
		#region GetTypes
		public static IEnumerable<Type> GetTypes<T1>()
		{
			yield return typeof(T1);

		}
		public static IEnumerable<Type> GetTypes<T1, T2>()
		{
			yield return typeof(T1);
			yield return typeof(T2);

		}
		public static IEnumerable<Type> GetTypes<T1, T2, T3>()
		{
			yield return typeof(T1);
			yield return typeof(T2);
			yield return typeof(T3);

		}
		public static IEnumerable<Type> GetTypes<T1, T2, T3, T4>()
		{
			yield return typeof(T1);
			yield return typeof(T2);
			yield return typeof(T3);
			yield return typeof(T4);

		}
		public static IEnumerable<Type> GetTypes<T1, T2, T3, T4, T5>()
		{
			yield return typeof(T1);
			yield return typeof(T2);
			yield return typeof(T3);
			yield return typeof(T4);
			yield return typeof(T5);

		}
		public static IEnumerable<Type> GetTypes<T1, T2, T3, T4, T5, T6>()
		{
			yield return typeof(T1);
			yield return typeof(T2);
			yield return typeof(T3);
			yield return typeof(T4);
			yield return typeof(T5);
			yield return typeof(T6);

		}
		public static IEnumerable<Type> GetTypes<T1, T2, T3, T4, T5, T6, T7>()
		{
			yield return typeof(T1);
			yield return typeof(T2);
			yield return typeof(T3);
			yield return typeof(T4);
			yield return typeof(T5);
			yield return typeof(T6);
			yield return typeof(T7);

		}
		public static IEnumerable<Type> GetTypes<T1, T2, T3, T4, T5, T6, T7, T8>()
		{
			yield return typeof(T1);
			yield return typeof(T2);
			yield return typeof(T3);
			yield return typeof(T4);
			yield return typeof(T5);
			yield return typeof(T6);
			yield return typeof(T7);
			yield return typeof(T8);

		}
		public static IEnumerable<Type> GetTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
		{
			yield return typeof(T1);
			yield return typeof(T2);
			yield return typeof(T3);
			yield return typeof(T4);
			yield return typeof(T5);
			yield return typeof(T6);
			yield return typeof(T7);
			yield return typeof(T8);
			yield return typeof(T9);

		}
		public static IEnumerable<Type> GetTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
		{
			yield return typeof(T1);
			yield return typeof(T2);
			yield return typeof(T3);
			yield return typeof(T4);
			yield return typeof(T5);
			yield return typeof(T6);
			yield return typeof(T7);
			yield return typeof(T8);
			yield return typeof(T9);
			yield return typeof(T10);

		}
		public static IEnumerable<Type> GetTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>()
		{
			yield return typeof(T1);
			yield return typeof(T2);
			yield return typeof(T3);
			yield return typeof(T4);
			yield return typeof(T5);
			yield return typeof(T6);
			yield return typeof(T7);
			yield return typeof(T8);
			yield return typeof(T9);
			yield return typeof(T10);
			yield return typeof(T11);

		}
		public static IEnumerable<Type> GetTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>()
		{
			yield return typeof(T1);
			yield return typeof(T2);
			yield return typeof(T3);
			yield return typeof(T4);
			yield return typeof(T5);
			yield return typeof(T6);
			yield return typeof(T7);
			yield return typeof(T8);
			yield return typeof(T9);
			yield return typeof(T10);
			yield return typeof(T11);
			yield return typeof(T12);

		}
		public static IEnumerable<Type> GetTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>()
		{
			yield return typeof(T1);
			yield return typeof(T2);
			yield return typeof(T3);
			yield return typeof(T4);
			yield return typeof(T5);
			yield return typeof(T6);
			yield return typeof(T7);
			yield return typeof(T8);
			yield return typeof(T9);
			yield return typeof(T10);
			yield return typeof(T11);
			yield return typeof(T12);
			yield return typeof(T13);

		}
		public static IEnumerable<Type> GetTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>()
		{
			yield return typeof(T1);
			yield return typeof(T2);
			yield return typeof(T3);
			yield return typeof(T4);
			yield return typeof(T5);
			yield return typeof(T6);
			yield return typeof(T7);
			yield return typeof(T8);
			yield return typeof(T9);
			yield return typeof(T10);
			yield return typeof(T11);
			yield return typeof(T12);
			yield return typeof(T13);
			yield return typeof(T14);

		}
		public static IEnumerable<Type> GetTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>()
		{
			yield return typeof(T1);
			yield return typeof(T2);
			yield return typeof(T3);
			yield return typeof(T4);
			yield return typeof(T5);
			yield return typeof(T6);
			yield return typeof(T7);
			yield return typeof(T8);
			yield return typeof(T9);
			yield return typeof(T10);
			yield return typeof(T11);
			yield return typeof(T12);
			yield return typeof(T13);
			yield return typeof(T14);
			yield return typeof(T15);

		}
		public static IEnumerable<Type> GetTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>()
		{
			yield return typeof(T1);
			yield return typeof(T2);
			yield return typeof(T3);
			yield return typeof(T4);
			yield return typeof(T5);
			yield return typeof(T6);
			yield return typeof(T7);
			yield return typeof(T8);
			yield return typeof(T9);
			yield return typeof(T10);
			yield return typeof(T11);
			yield return typeof(T12);
			yield return typeof(T13);
			yield return typeof(T14);
			yield return typeof(T15);
			yield return typeof(T16);

		}
		#endregion

		public static void CombineEnum(int n, int r, Action<List<int>> Fn) {
			if (r == 0) return;
			if (r == 1)
			{
				for (int i = 0; i < n; i++)
				{
					Fn([i]);
				}
			}
			else if (n == r) {
				List<int> l=[];
				for (int i = 0; i < n; i++) {
					l.Add(i);
				}
				Fn(l);
			}
			else
			for (int i=r-1;i<n;i++) {
				CombineEnum(i, r - 1,
					(list)=> {
						list.Add(i);
						Fn(list);
					}
				);
			}
		}

		public static T Get<TBase,T>(IReadOnlyList<TBase> values, IdOf<T> id)
			where T : TBase
			=> (T)values[(int)id]!;

		/// <summary>
		/// enum (ak,ak+1), and (an,a0)
		/// 每个元素都会出现两次
		/// </summary>
		public static IEnumerable<(T, T)> EnumPairs<T>(this IEnumerable<T> ts)
		{
			List<T> l = ts.ToList();
			for (int i = 0; i < l.Count - 1; ++i) yield return (l[i], l[i + 1]);
			yield return (l[^1], l[0]);
		}
		public static int ToDirect(this int v) => v == 0 ? -1 : 1;
		public static int ToDirect(this uint v) => v == 0 ? -1 : 1;

		public static void AddTo<TKey, TValue>(this TValue value, IDictionary<TKey, TValue> dict, TKey key) {
			dict.Add(key, value);
		}
		public static void AddTo<TKey, TValue>(this KeyValuePair<TKey,TValue> kvp, IDictionary<TKey, TValue> dict)
		{
			dict.Add(kvp);
		}

		public static void AddTo<TKey, TValue>(this (TKey Key,TValue Value) kvp, IDictionary<TKey, TValue> dict)
		{
			dict.Add(kvp.Key,kvp.Value);
		}
	}
	public enum Rotation : byte
	{
		Right = 0,	Up = 1,			Left = 2,		Down = 3,
		d0 = Right, d90 = Up,		d180 = Left,	d270 = Down,
		r0=Right,	r1 = Up,		r2 = Left,		r3 = Down,
		dn0=Right,	dn90 = Down,	dn180 = Left,	dn270 = Up,
		rn0=Right,	rn1 = Down,		rn2 = Left,		rn3 = Up,
	}
	#region ActionR
	public delegate void ActionR<T1>(ref T1 arg1);
	public delegate void ActionR<T1, T2>(ref T1 arg1, ref T2 arg2);
	public delegate void ActionR<T1, T2, T3>(ref T1 arg1, ref T2 arg2, ref T3 arg3);
	public delegate void ActionR<T1, T2, T3, T4>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4);
	public delegate void ActionR<T1, T2, T3, T4, T5>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4, ref T5 arg5);
	public delegate void ActionR<T1, T2, T3, T4, T5, T6>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4, ref T5 arg5, ref T6 arg6);
	public delegate void ActionR<T1, T2, T3, T4, T5, T6, T7>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4, ref T5 arg5, ref T6 arg6, ref T7 arg7);
	public delegate void ActionR<T1, T2, T3, T4, T5, T6, T7, T8>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4, ref T5 arg5, ref T6 arg6, ref T7 arg7, ref T8 arg8);
	public delegate void ActionR<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4, ref T5 arg5, ref T6 arg6, ref T7 arg7, ref T8 arg8, ref T9 arg9);
	public delegate void ActionR<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4, ref T5 arg5, ref T6 arg6, ref T7 arg7, ref T8 arg8, ref T9 arg9, ref T10 arg10);
	public delegate void ActionR<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4, ref T5 arg5, ref T6 arg6, ref T7 arg7, ref T8 arg8, ref T9 arg9, ref T10 arg10, ref T11 arg11);
	public delegate void ActionR<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4, ref T5 arg5, ref T6 arg6, ref T7 arg7, ref T8 arg8, ref T9 arg9, ref T10 arg10, ref T11 arg11, ref T12 arg12);
	public delegate void ActionR<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4, ref T5 arg5, ref T6 arg6, ref T7 arg7, ref T8 arg8, ref T9 arg9, ref T10 arg10, ref T11 arg11, ref T12 arg12, ref T13 arg13);
	public delegate void ActionR<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4, ref T5 arg5, ref T6 arg6, ref T7 arg7, ref T8 arg8, ref T9 arg9, ref T10 arg10, ref T11 arg11, ref T12 arg12, ref T13 arg13, ref T14 arg14);
	public delegate void ActionR<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4, ref T5 arg5, ref T6 arg6, ref T7 arg7, ref T8 arg8, ref T9 arg9, ref T10 arg10, ref T11 arg11, ref T12 arg12, ref T13 arg13, ref T14 arg14, ref T15 arg15);
	public delegate void ActionR<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4, ref T5 arg5, ref T6 arg6, ref T7 arg7, ref T8 arg8, ref T9 arg9, ref T10 arg10, ref T11 arg11, ref T12 arg12, ref T13 arg13, ref T14 arg14, ref T15 arg15, ref T16 arg16);
	#endregion
	public delegate void ActionI<T>(in T v);
}
