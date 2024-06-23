using FixMath;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Utils
{
	public static class Calculate
	{
		/// <summary>
		/// 从a到b的size
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Size SizeFromPoint(Point a, Point b) => new(b.X - a.X, b.Y - a.Y);
		/// <summary>
		/// 获取包含a和b的矩形
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Rectangle Union(Rectangle a, Rectangle b)
		{
			Point point1 = new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
			Point point2 = new(Math.Min(a.Right, b.Right), Math.Min(a.Bottom, b.Bottom));

			return new Rectangle(point1, SizeFromPoint(point1, point2));
		}
		/// <summary>
		/// a和b是否相邻（不相交）
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool Adjoin(this Rectangle a, Rectangle b)
		{
			return (a.Top == b.Bottom + 1) || (a.Bottom + 1 == b.Top) || (a.Left == b.Right + 1) || (a.Right + 1 == b.Left);
		}
		
		public static void LimitVec0(ref Vector2 Start, Vector2 Direction, Rectangle Edge) {
			var OffsetLen = Direction.Length();
			var sin = Direction.Y / OffsetLen;
			var cos = Direction.X / OffsetLen;
			var invsin = 1 / sin;
			var invcos = 1 / cos;
			var edge = Edge;
			var OffsetNormal = new Vector2(1 / invcos, 1 / invsin);

			if (Start.X > Edge.Right)
			{
				//yield break;
				//movedLen = OffsetLen + 1;
				Start += OffsetNormal * (Edge.Right - Start.X) * invcos;
			}
			if (Start.X < Edge.X)
			{
				Start += OffsetNormal * (Edge.X - Start.X) * invcos;
			}

			if (Start.Y < Edge.Y)
			{
				Start += OffsetNormal * (Edge.Y - Start.Y) * invsin;
			}
			if (Start.Y > Edge.Bottom)
			{
				Start += OffsetNormal * (Edge.Bottom - Start.Y) * invsin;
			}
		}

		public static void LimitVec(ref Vector2 Start, float cos, float sin, Rectangle Edge) {
			LimitVec(ref Start, cos, sin, 1 / cos, 1 / sin,Edge);
		}

		public static void LimitVec(ref Vector2 Start,float cos,float sin,float invcos, float invsin, Rectangle Edge)
		{
			var edge = Edge;
			var OffsetNormal =new Vector2(cos,sin);

			if (Start.X > Edge.Right)
			{
				//yield break;
				//movedLen = OffsetLen + 1;
				Start += OffsetNormal * (Edge.Right - Start.X) * invcos;
			}
			if (Start.X < Edge.X)
			{
				Start += OffsetNormal * (Edge.X - Start.X) * invcos;
			}

			if (Start.Y < Edge.Y)
			{
				Start += OffsetNormal * (Edge.Y - Start.Y) * invsin;
			}
			if (Start.Y > Edge.Bottom)
			{
				Start += OffsetNormal * (Edge.Bottom - Start.Y) * invsin;
			}
		}

		public static void LimitVec(ref Vector2 Start,Vector2 Direction, Rectangle Edge) {
			var OffsetLen = Direction.Length();
			var sin = Direction.Y / OffsetLen;
			var cos = Direction.X / OffsetLen;
			LimitVec(ref Start, cos, sin,Edge);
		}

		/// <summary>
		/// 枚举线上的物块
		/// </summary>
		public static IEnumerable<Point> EnumGridsInLine(Vector2 Start, Vector2 Offset,Rectangle Edge)
		{
			return (IEnumerable<Point>)new EnumGridsInLineEnumer(Start, Offset, Edge);
		}

		private class EnumGridsInLineEnumer : IEnumerator<Point>
		{
			public EnumGridsInLineEnumer(Vector2 Start, Vector2 Offset, Rectangle Edge) {
				this.Start = Start;
				Vector2 End = Start + Offset;
				edge = Edge;
				goUp = Offset.Y > 0;
				goRight = Offset.X > 0;

				OffsetLen = Offset.Length();
				OffsetNormal = Offset / OffsetLen;
				sin = Offset.Y / OffsetLen;
				cos = Offset.X / OffsetLen;
				invsin = 1 / sin;
				invcos = 1 / cos;

				var sin2 = Offset.Y / OffsetLen;

				LimitVec0(ref Start,Offset, edge);
				LimitVec(ref End,cos,sin, invcos, invsin, edge);

				Offset = End - Start;
				OffsetLen = Offset.Length();

				Reset();
			}
			readonly Vector2 Start;
			readonly Rectangle edge;
			const float smallValue = 1 / 256;
			readonly float OffsetLen;
			readonly Vector2 OffsetNormal;

			readonly bool goUp;
			readonly bool goRight;

			readonly float sin;
			readonly float cos;

			readonly float invsin;
			readonly float invcos;

			Vector2 current;
			float movedLen;
			Point curP;

			public Point Current=>curP;
			object IEnumerator.Current => curP;

			public void Dispose()
			{
				
			}

			public bool MoveNext()
			{
				
				if (movedLen < OffsetLen) return false;
				curP = new Point((int)current.X, (int)current.Y);
				float distIfGox;
				if (goRight) { distIfGox = ((curP.X + 1)-current.X) * invcos; }
				else 
				{ distIfGox = (curP.X-current.X) * invcos; }

				float distIfGoy;
				if (goUp) { distIfGoy = ((curP.Y + 1)-current.Y) * invsin; }
				else { distIfGoy = (curP.Y - current.Y) * invsin; }

				var movelen = MathF.Min(distIfGox, distIfGoy) + smallValue;
				current += OffsetNormal * (movelen);
				movedLen += movelen;
				return true;
			}

			public void Reset()
			{
				current = Start;
				movedLen = 0;
				//curP = new Point((int)current.X, (int)current.Y);
			}
		}

		public static float StdGaussian()
		{
			var rand = new Random();
			float u = -2 * MathF.Log(rand.NextSingle());
			float v = 2 * MathF.PI * rand.NextSingle();
			return MathF.Sqrt(u) * MathF.Cos(v);
		}
	}
}
