using FixMath;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Utils
{
	public static class CalculateFix
	{
		public static Vector2Fix ToRotatedVector(this Rotation rotation)
		{
			return rotation switch
			{
				Rotation.Left => new(-1, 0),
				Rotation.Down => new(0, -1),
				Rotation.Right => new(1, 0),
				Rotation.Up => new(0, 1),
				_ => throw new Exception($"Unknow Rotation{rotation}"),
			};
		}
		public static Fix64 ToRadium(this Rotation rotation) => Fix64.PiOver2 * (byte)rotation;
		public static Fix64 GetRotation(this Vector2Fix vec) => Fix64.Atan2(vec.y, vec.x);
		public static Vector2Fix Rotate(this Vector2Fix vec, Fix64 rot)
		{
			Fix64 cos = Fix64.Cos(rot);
			Fix64 sin = Fix64.Sin(rot);
			Fix64 x = vec.x;
			Fix64 y = vec.y;
			return new(
				x * cos - y * sin,
				x * sin + y * cos
				);
		}

		/// <summary>
		/// 逆时针旋转
		/// </summary>
		/// <param name="vec"></param>
		/// <param name="rotation"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static Vector2Fix Rotate90(this Vector2Fix vec, int rotation)
		{
			Fix64 x = vec.x;
			Fix64 y = vec.y;
			rotation %= 4;
			if (rotation < 0) rotation += 4;
			return rotation switch
			{
				0 => vec,
				1 => new(-y, x),
				2 => -vec,
				3 => new(y, -x),
				_ => throw new ArgumentOutOfRangeException(nameof(rotation)),
			};
		}

		public static Vector2Fix ToRotatedVector(this Fix64 rot) => new(Fix64.Cos(rot), Fix64.Sin(rot));
		//public static float AngleWrap(this float angle) { 
		//	while(angle<-MathF.PI) { angle+=MathF.PI*2; }
		//	while (angle > MathF.PI) { angle -= MathF.PI * 2; }
		//	return angle;
		//}
		public static Fix64 AngleWrap(this Fix64 angle)
		{
			while (angle < -Fix64.Pi) { angle += Fix64.PiTimes2; }
			while (angle > Fix64.Pi) { angle -= Fix64.PiTimes2; }
			return angle;
		}
		//public static float AngleBetween(Vector2 a, Vector2 b) => AngleWrap(a.ToRotation()-b.ToRotation());
		//public static float AngleBetween(Vector2 a, float b) => AngleWrap(a.ToRotation() - b);
		//public static float AngleBetween(float a, float b) => AngleWrap(a - b);
		public static Fix64 AngleBetween(Vector2Fix a, Vector2Fix b) => AngleWrap(a.Angle - b.Angle);
		public static Fix64 AngleBetween(Vector2Fix a, Fix64 b) => AngleWrap(a.Angle - b);
		public static Fix64 AngleBetween(Fix64 a, Fix64 b) => AngleWrap(a - b);
		//public static Fix64 ToRadian(this Fix64 degree) => degree * Mathf.Deg2Rad; //MathF.PI * degree / 180;
		//public static Fix64 ToDegree(this Fix64 radian) => radian * Mathf.Rad2Deg;//radian / MathF.PI * 180;
		//public static float MaxSpeed(this Rigidbody2D rb, float thrust)
		//	=> thrust / rb.mass / rb.drag;
		//public static float MaxTurn(this Rigidbody2D rb, float turn)
		//	=> turn / rb.inertia / rb.angularDrag;
		public static Vector2Fix GetFarestPoint(this RectFix rect, Vector2Fix point)
		{
			Vector2Fix center = rect.Center;
			Vector2Fix res = new();
			if (point.x < center.x) res.x = rect.XMax;
			else res.x = rect.XMin;
			if (point.y < center.y) res.y = rect.YMax;
			else res.y = rect.YMin;
			return res;
		}
		public static Vector2Fix GetClosestPoint(this RectFix rect, Vector2Fix point)
		{
			if (point.x < rect.XMin) point.x = rect.XMin;
			if (point.x > rect.XMax) point.x = rect.XMax;
			if (point.y < rect.YMin) point.y = rect.YMin;
			if (point.y > rect.YMax) point.y = rect.YMax;
			return point;
		}
		public static Fix64 GetDistance(this RectFix rect, Vector2Fix point)
		{
			Vector2Fix p = rect.GetClosestPoint(point);
			return (point - p).Length;
		}
		public static Fix64 GetDistanceSQ(this RectFix rect, Vector2Fix point)
		{
			Vector2Fix p = rect.GetClosestPoint(point);
			return (point - p).LengthSQ;
		}
		//public static bool HasValue<T>(this Nullable<T>? value) {
		//	if ( value is null) return false;
		//	else return true;
		//}
		public static Vector2Fix Sacle(this Vector2Fix a, Vector2Fix b)
			=> new(a.x * b.x, a.y * b.y);
		public static Fix64 Dot(this Vector2Fix a, Vector2Fix b)
			=> a.x * b.x + a.y * b.y;
		public static Vector2Fix ToVector2Fix(this Point point) => new(point.X, point.Y);
		public static Vector2Fix ToVector2Fix(this Size size) => new(size.Width, size.Height);
		/// <summary>
		/// 计算二维向量叉乘的长
		/// </summary>
		public static Fix64 CrossProduct(Vector2Fix v1, Vector2Fix v2) => v1.x * v2.y - v1.y * v2.x;
		/// <summary>
		/// 根据相对位置，相对速度，固定发射速度进行预判
		/// </summary>
		/// <param name="OffsetPos">相对位置</param>
		/// <param name="OffsetVel">相对速度</param>
		/// <param name="Speed">固定发射速度</param>
		/// <returns>预判的角度结果</returns>
		public static Fix64? Predict(Vector2Fix OffsetPos, Vector2Fix OffsetVel, Fix64 Speed)
		{
			Fix64 D = CrossProduct(OffsetPos, OffsetVel) / (Speed * OffsetPos.Length);
			if (D > 1 || D < -1) return null;
			else return Fix64.Asin(D) + OffsetPos.GetRotation();
		}

		public static Func<Vector2Fix, Vector2Fix> GetRotateFn(Fix64 rot)
		{
			Fix64 cos = Fix64.Cos(rot);
			Fix64 sin = Fix64.Sin(rot);
			return vec =>
			{
				Fix64 x = vec.x;
				Fix64 y = vec.y;
				return new(
				   x * cos - y * sin,
				   x * sin + y * cos
				   );
			};
		}
	}
}
