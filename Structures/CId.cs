using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures
{
	public interface IIdOf<T> : IReadonlyId { }

	public record class CIdOf<T>(int Id) : IIdOf<T>
	{
		public static explicit operator int(in CIdOf<T> id) => id.Id;
		public static explicit operator CIdOf<T>(in int id) => new(id);
		public static readonly CIdOf<T> Empty = new(-1);
		public int CompareTo(IIdOf<T> other) => Id.CompareTo(other.Id);
	}
	//public record class CIdOf<T1,T2>(int Id) : IIdOf<T1>, IIdOf<T2>
	//{
	//	public static explicit operator int(in CIdOf<T1, T2> id) => id.Id;
	//	public static explicit operator CIdOf<T1, T2>(in int id) => new(id);
	//	public static readonly CIdOf<T1, T2> Empty = new(-1);
	//	//public int CompareTo(IIdOf<T1> other) => Id.CompareTo(other.Id);
	//}
}
