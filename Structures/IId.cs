using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures
{
	
	public interface IReadonlyId {
		int Id { get; }
	}
	[Obsolete("Id Normaly won't change")]
	public interface IId : IReadonlyId
	{
		new int Id { get; set; }
		int IReadonlyId.Id => Id;
	}

	//public interface IIdOf<out T>
	//{
	//	int Id { get; set; }
	//	void Deconstruct(out int Id);
	//}

	public record struct IdOf<T>(int Id) :IIdOf<T>, IReadonlyId, IComparable<IdOf<T>>, IEquatable<IdOf<T>>/*, IIdOf<T>*/
	{
		public static explicit operator int(in IdOf<T> id) => id.Id;
		public static explicit operator IdOf<T>(in int id) => new(id);
		public static readonly IdOf<T> Empty = new(-1);
		public readonly int CompareTo(IdOf<T> other) => Id.CompareTo(other.Id);
		//public static bool operator <(in IdOf<T> id, in int v) => id.Id < v;
		//public static bool operator >(in IdOf<T> id, in int v) => id.Id > v;
		//public static IdOf<T> operator ++(in IdOf<T> id) => new(id.Id + 1);
		//public static IdOf<T> operator --(in IdOf<T> id) => new(id.Id - 1);
	}
	//public record struct IdOf<T0, T1>(int Id) : IReadonlyId, IComparable<IdOf<T0, T1>>, IEquatable<IdOf<T0, T1>>/*, IIdOf<T>*/
	//{
	//	public static explicit operator int(in IdOf<T0, T1> id) => id.Id;
	//	public static explicit operator IdOf<T0, T1>(in int id) => new(id);
	//	public static readonly IdOf<T0, T1> Empty = new(-1);
	//	public readonly int CompareTo(IdOf<T0, T1> other) => Id.CompareTo(other.Id);
	//	public static implicit operator	IdOf<T0>(in IdOf<T0, T1> id) => (IdOf<T0>)id.Id;
	//	public static implicit operator IdOf<T1>(in IdOf<T0, T1> id) => (IdOf<T1>)id.Id;
	//}
	public record struct WithId<T>(IdOf<T> Id, Ref<T> RefValue) :IReadonlyId,IComparable<WithId<T>>{
		public readonly ref T Value=>ref RefValue.Value;
		public WithId(int Id, ref T RefValue) : this(new IdOf<T>(Id), new(ref RefValue)) { }
		public WithId(IdOf<T> Id, ref T RefValue) : this(Id, new(ref RefValue)) { }
		public WithId(int Id, Ref<T> RefValue) : this(new IdOf<T>(Id), RefValue) { }
		readonly int IReadonlyId.Id => Id.Id;
		public readonly int CompareTo(WithId<T> other) => Id.CompareTo(other.Id);
	}
	public ref struct WithIdR<T>
	{
		public IdOf<T> Id;
		public ref T Value;

		public WithIdR(IdOf<T> id,ref T value)
		{
			Id = id;
			Value = ref value;
		}
		public WithIdR(int id,ref T value)
		{
			Id =new( id);
			Value =ref value;
		}
	}
}
