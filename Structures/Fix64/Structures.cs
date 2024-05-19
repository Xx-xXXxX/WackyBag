using FixMath;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;



namespace FixMath
{

	public record struct Vector2Fix(Fix64 x,Fix64 y)
	{
		public Fix64 x=x;
		public Fix64 y=y;
		public static Vector2Fix operator +(Vector2Fix a, Vector2Fix b)
			=> new(a.x + b.x, a.y + b.y);
		public static Vector2Fix operator -(Vector2Fix a, Vector2Fix b)
			=> new(a.x - b.x, a.y - b.y);
		public static Vector2Fix operator -(Vector2Fix b)
			=> new(- b.x, - b.y);
		public static Vector2Fix operator *(Vector2Fix a, Vector2Fix b)
			=> new(a.x * b.x, a.y * b.y);
		public static Vector2Fix operator *(Vector2Fix a, Fix64 b)
			=> new(a.x * b, a.y * b);
		public static Vector2Fix operator *(Fix64 b, Vector2Fix a)
			=> new(a.x * b, a.y * b);
		public static Vector2Fix operator /(Vector2Fix a, Vector2Fix b)
			=> new(a.x / b.x, a.y / b.y);
		public static Vector2Fix operator /(Vector2Fix a, Fix64 b)
			=> new(a.x / b, a.y / b);
		public void Set(Fix64 x, Fix64 y)
		{
			this.x = x;
			this.y = y;
		}
		public readonly void Deconstruct(out Fix64 x, out Fix64 y) {
			x = this.x;
			y = this.y;
		}
		public readonly Fix64 LengthSQ => x * x + y * y;
		public readonly Fix64 Length => Fix64.Sqrt(x * x + y * y);
		public readonly Fix64 Angle => Fix64.Atan2(y, x);
		public Vector2Fix Normalize() {
			Fix64 l = Length;
			x /= l;
			y /= l;
			return this;
		}
		public static explicit operator Vector2Fix(System.Numerics.Vector2 vector)=>new((Fix64)vector.X, (Fix64)vector.Y);
		public static explicit operator Vector2Fix(System.Drawing.Point vector) => new(vector.X, vector.Y);
		public static explicit operator Vector2Fix(System.Drawing.Size vector) => new(vector.Width, vector.Height);
		public static explicit operator System.Numerics.Vector2(Vector2Fix vector) => new((float)vector.x, (float)vector.y);
		public static explicit operator System.Drawing.Point(Vector2Fix vector) => new((int)vector.x, (int)vector.y);
		public static explicit operator System.Drawing.Size(Vector2Fix vector) => new((int)vector.x, (int)vector.y);
		public override readonly string ToString() => $"({x},{y})";
	}

	[Serializable]
	public record struct RectFix (Fix64 x, Fix64 y, Fix64 width, Fix64 height)
	{
		public Fix64 x=x, y = y, width=width, height=height;
		public Vector2Fix Position { 
			readonly get => new(x, y);
			set =>(x,y)=value;
		}
		public Vector2Fix Size { 
			readonly get => new(width, height);
			set => (width, height) = value;
		}
		public Vector2Fix Center {
			readonly get =>new(x+width/ 2, y+height/ 2);
			set {
				x = value.x - width / 2;
				y=value.y-height / 2;
			}
		}
		public Vector2Fix Min
		{
			readonly get => new(x, y);
			set => (x, y) = value;
		}
		public Vector2Fix Max {
			readonly get => new(x+width, y+height);
			set {
				width = value.x - x;
				height = value.y - y;
			}
		}
		public Fix64 XMin { 
			readonly get => x;
			set => x = value;
		}
		public Fix64 YMin
		{
			readonly get => y;
			set => y = value;
		}
		public Fix64 XMax {
			readonly get => x + width;
			set {
				width = value - x;
			}
		}
		public Fix64 YMax {
			readonly get =>y + height;
			set { 
				height= value - y;
			}
		}
		public RectFix(Vector2Fix position, Vector2Fix size) : this(position.x,position.y,size.x,size.y)
		{
		}
		public static implicit operator RectFix(System.Drawing.Rectangle rect) => new(rect.X, rect.Y, rect.Width, rect.Height);
		public static implicit operator System.Drawing.Rectangle(RectFix rect) => new((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
		public override readonly string ToString() => $"({x},{y},{width},{height})";
	}
}
