using WackyBag;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;

namespace WackyBag.Structures
{
	public struct UNullable<T> : IEquatable<UNullable<T>>
	{
		[MemberNotNullWhen(true, nameof(Value))]
		public bool HasValue { get;private set; } = false;
		public T? Value = default;
		public UNullable(T Value) {
			HasValue = true;this.Value = Value;
		}

		public override readonly bool Equals(object? obj)
		{
			return (obj is null && !HasValue)||
				( obj is UNullable<T> nullable && Equals(nullable))||
				(obj is T value &&(HasValue&& value.Equals(Value)));
		}

		public readonly bool Equals(UNullable<T> other)
		{
			return HasValue == other.HasValue &&
				   EqualityComparer<T?>.Default.Equals(Value, other.Value);
		}

		public override readonly int GetHashCode()
		{
			return HasValue ? Value.GetHashCode() : 0;
		}

		public static bool operator ==(UNullable<T> left, UNullable<T> right)
		{
			return left.Equals(right);
		}
		public static bool operator ==(UNullable<T> left, UNullable<T>? right)
		{
			return left.Equals(right.ToU());
		}
		public static bool operator ==(UNullable<T>? left, UNullable<T> right)
		{
			return left.ToU().Equals(right);
		}
		public static bool operator ==(UNullable<T>? left, UNullable<T>? right)
		{
			return left.ToU().Equals(right.ToU());
		}

		public static bool operator !=(UNullable<T> left, UNullable<T> right)
		{
			return !(left == right);
		}
		
		public static bool operator !=(UNullable<T> left, UNullable<T>? right)
		{
			return! left.Equals(right.ToU());
		}
		public static bool operator !=(UNullable<T>? left, UNullable<T> right)
		{
			return! left.ToU().Equals(right);
		}
		public static bool operator !=(UNullable<T>? left, UNullable<T>? right)
		{
			return! left.ToU().Equals(right.ToU());
		}
		public static readonly UNullable<T> Null = new();
		public static implicit operator UNullable<T>(T v) => new(v);
		//public static implicit operator UNullable<T>(object? Null)
		//{
		//	if (Null == null) return UNullable<T>.Null;
		//	if (Null is T v) return new(v);
		//	return UNullable<T>.Null;
		//}
		public static explicit operator T(UNullable<T> v) =>v.Value!;
		public override readonly string ToString()
		{
			if (!HasValue) return "null";
			return Value.ToString()!;
		}
	}
}
