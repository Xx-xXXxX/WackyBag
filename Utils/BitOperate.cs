using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Utils
{
	/// <summary>
	/// 位操作
	/// </summary>
	public static class BitOperate
	{
		/// <summary>
		/// 获取长为s的1值
		/// MakeBit1s(4)==0xf (1111)
		/// </summary>
		/// <param name="s">1的长度</param>
		public static int MakeBit1s(int s)
		{
			return (int)((1 << (s)) - 1);
		}
		/// <summary>
		/// 从d将[l,l+s)分离
		/// <code>example:GetBits(101101110,2,3)
		///_101101110
		///_    [ ]
		///_      011(return)</code>
		/// </summary>
		/// <param name="d"></param>
		/// <param name="l"></param>
		/// <param name="s"></param>
		/// <returns></returns>
		public static int GetBits(int d, int l, int s)
		{
			return ((d >> l) & MakeBit1s(s));
		}
		/// <summary>
		/// <code>将d中[l,l+s）的位设为0
		/// example:ClearBits(101101110,2,3)
		///_101101110
		///_    [ ]
		///_101100010(return)</code>
		/// </summary>
		/// <param name="d"></param>
		/// <param name="l"></param>
		/// <param name="s"></param>
		/// <returns></returns>
		public static int ClearBits(int d, int l, int s)
		{
			return d & ~(MakeBit1s(s) << l);
		}
		/// <summary>
		/// <code>d中[l,l+s）外的位设为0
		/// example:ClearBits(101101110,2,3)
		///_101101110
		///_    [ ]
		///_000001100(return)</code>
		/// </summary>
		/// <param name="d"></param>
		/// <param name="l"></param>
		/// <param name="s"></param>
		/// <returns></returns>
		public static int ClearOutsideBits(int d, int l, int s)
		{
			return d & (MakeBit1s(s) << l);
		}
		/// <summary>
		/// <para>将d中[l,l+s)中的值设为v</para>
		/// <code>example:SetBits(101101110,1110,2,3)
		///_101001110
		///_    [ ]
		///_   1110
		///_101011010(return)</code>
		/// </summary>
		/// <param name="d"></param>
		/// <param name="v"></param>
		/// <param name="l"></param>
		/// <param name="s"></param>
		/// <returns></returns>
		public static int SetBits(int d, int v, int l, int s)
		{
			d = ClearBits(d, l, s);
			v = ClearOutsideBits(v, 0, s);
			d |= v << l;
			return d;
		}

		/// <summary>
		/// <para>将d中[l,l+s)中的值设为v</para>
		/// <code>example:SetBits(101101110,1110,2,3)
		///_101001110
		///_    [ ]
		///_   1110
		///_101011010(return)</code>
		/// </summary>
		/// <param name="d"></param>
		/// <param name="v"></param>
		/// <param name="l"></param>
		/// <param name="s"></param>
		/// <returns></returns>
		public static int SetBits(ref int d, int v, int l, int s)
		{
			return d = SetBits(d, v, l, s);
		}

		public static (int, int) Separate(int d, int l)
		{
			return (d >> l, d & MakeBit1s(l));
		}
		/// <summary>
		/// 将data转为二进制字符串
		/// 右为低位
		/// <code>example:ToBitString(IToBytes(0xf))
		/// 0xf7(int)
		/// 00000000000000000000000011110111(string)</code>
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string ToBitString(byte[] data)
		{

			StringBuilder sb = new StringBuilder();
			//string R = "";
			for (int i = 0; i < data.Length; ++i)
			{
				for (int j = 0; j < 8; ++j)
				{
					//R = ((((data[i] >> j) & 1) == 1) ? '1' : '0')+R;
					sb.Insert(0, ((((data[i] >> j) & 1) == 1) ? '1' : '0'));
				}
			}
			return sb.ToString();
		}

		private const int BytePerInt = sizeof(int);
		private const int BitPerByte = 3;

		public static bool GetBitInArray(int[] arr, int index)
		{
			var (a, b) = BitOperate.Separate(index, sizeof(int) + BitPerByte);
			return BitOperate.GetBits(arr[a], b, 1) == 1;
		}
		public static void SetBitInArray(int[] arr, int index, bool value)
		{
			var (a, b) = BitOperate.Separate(index, sizeof(int) + BitPerByte);
			arr[a] = BitOperate.SetBits(arr[a], value ? 1 : 0, b, 1);
		}
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
		public static string ToBitString(int data)
		{
			return ToBitString(BitConverter.GetBytes(data));
		}
		/*
		public static int ToInt(float d)
		{
			return BitConverter.SingleToInt32Bits(d);//BitConverter.ToInt32(BitConverter.GetBytes(d), 0);
		}
		public static int ToInt(uint d)
		{
			return (int)d;
		}
		public static uint ToUInt(int d)
		{
			return (uint)d;
		}
		public static uint ToUInt(float d)
		{
			return BitConverter.SingleToUInt32Bits(d);
		}
		public static float ToFloat(int d)
		{
			return BitConverter.Int32BitsToSingle(d);//.ToSingle(BitConverter.GetBytes(d), 0);
		}
		public static float ToFloat(uint d)
		{
			return BitConverter.UInt32BitsToSingle(d);//BitConverter.ToSingle(BitConverter.GetBytes(d), 0);
		}
		public static ulong ToULong(long d)
		{
			return (ulong)d;
		}
		public static ulong ToULong(double d)
		{
			return (ulong)BitConverter.DoubleToInt64Bits(d);
		}
		public static ulong ToULong(int d1, int d2)
		{
			return (ulong)//BitConverter.ToUInt64(vs, 0);
		}
		public static ulong ToULong(float d1, float d2)
		{
			byte[] vs = new byte[8];
			Array.Copy(BitConverter.GetBytes(d1), 0, vs, 0, 4);
			Array.Copy(BitConverter.GetBytes(d2), 0, vs, 4, 4);
			return BitConverter.ToUInt64(vs, 0);
		}
		public static ulong ToULong(uint d1, uint d2)
		{
			byte[] vs = new byte[8];
			Array.Copy(BitConverter.GetBytes(d1), 0, vs, 0, 4);
			Array.Copy(BitConverter.GetBytes(d2), 0, vs, 4, 4);
			return BitConverter.ToUInt64(vs, 0);
		}
		public static long ToLong(ulong d)
		{
			return (long)d;
		}
		public static long ToLong(double d)
		{
			return BitConverter.DoubleToInt64Bits(d);
		}
		public static long ToLong(int d1, int d2)
		{
			byte[] vs = new byte[8];
			Array.Copy(BitConverter.GetBytes(d1), 0, vs, 0, 4);
			Array.Copy(BitConverter.GetBytes(d2), 0, vs, 4, 4);
			return BitConverter.ToInt64(vs, 0);
		}
		public static long ToLong(float d1, float d2)
		{
			byte[] vs = new byte[8];
			Array.Copy(BitConverter.GetBytes(d1), 0, vs, 0, 4);
			Array.Copy(BitConverter.GetBytes(d2), 0, vs, 4, 4);
			return BitConverter.ToInt64(vs, 0);
		}
		public static long ToLong(uint d1, uint d2)
		{
			byte[] vs = new byte[8];
			Array.Copy(BitConverter.GetBytes(d1), 0, vs, 0, 4);
			Array.Copy(BitConverter.GetBytes(d2), 0, vs, 4, 4);
			return BitConverter.ToInt64(vs, 0);
		}
		public static double ToDouble(long d)
		{
			return BitConverter.Int64BitsToDouble(d);
		}
		public static double ToDouble(ulong d)
		{
			return BitConverter.Int64BitsToDouble((long)d);
		}
		public static double ToDouble(int d1, int d2)
		{
			byte[] vs = new byte[8];
			Array.Copy(BitConverter.GetBytes(d1), 0, vs, 0, 4);
			Array.Copy(BitConverter.GetBytes(d2), 0, vs, 4, 4);
			return BitConverter.ToDouble(vs, 0);
		}
		public static double ToDouble(float d1, float d2)
		{
			byte[] vs = new byte[8];
			Array.Copy(BitConverter.GetBytes(d1), 0, vs, 0, 4);
			Array.Copy(BitConverter.GetBytes(d2), 0, vs, 4, 4);
			return BitConverter.ToDouble(vs, 0);
		}
		public static double ToDouble(uint d1, uint d2)
		{
			byte[] vs = new byte[8];
			Array.Copy(BitConverter.GetBytes(d1), 0, vs, 0, 4);
			Array.Copy(BitConverter.GetBytes(d2), 0, vs, 4, 4);
			return BitConverter.ToDouble(vs, 0);
		}
		public static (int, int) ToInt2(long d)
		{
			byte[] vs = BitConverter.GetBytes(d);
			return (BitConverter.ToInt32(vs, 0), BitConverter.ToInt32(vs, 4));
		}
		public static (int, int) ToInt2(ulong d)
		{
			byte[] vs = BitConverter.GetBytes(d);
			return (BitConverter.ToInt32(vs, 0), BitConverter.ToInt32(vs, 4));
		}
		public static (int, int) ToInt2(double d)
		{
			byte[] vs = BitConverter.GetBytes(d);
			return (BitConverter.ToInt32(vs, 0), BitConverter.ToInt32(vs, 4));
		}
		public static (uint, uint) ToUInt2(long d)
		{
			byte[] vs = BitConverter.GetBytes(d);
			return (BitConverter.ToUInt32(vs, 0), BitConverter.ToUInt32(vs, 4));
		}
		public static (uint, uint) ToUInt2(ulong d)
		{
			byte[] vs = BitConverter.GetBytes(d);
			return (BitConverter.ToUInt32(vs, 0), BitConverter.ToUInt32(vs, 4));
		}
		public static (uint, uint) ToUInt2(double d)
		{
			byte[] vs = BitConverter.GetBytes(d);
			return (BitConverter.ToUInt32(vs, 0), BitConverter.ToUInt32(vs, 4));
		}
		public static (float, float) ToFloat2(long d)
		{
			byte[] vs = BitConverter.GetBytes(d);
			return (BitConverter.ToSingle(vs, 0), BitConverter.ToSingle(vs, 4));
		}
		public static (float, float) ToFloat2(ulong d)
		{
			byte[] vs = BitConverter.GetBytes(d);
			return (BitConverter.ToSingle(vs, 0), BitConverter.ToSingle(vs, 4));
		}
		public static (float, float) ToFloat2(double d)
		{
			byte[] vs = BitConverter.GetBytes(d);
			return (BitConverter.ToSingle(vs, 0), BitConverter.ToSingle(vs, 4));
		}*/
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
	}
}
