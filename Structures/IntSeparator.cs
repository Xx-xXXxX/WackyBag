using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WackyBag.Utils;

namespace WackyBag.Structures
{
	/// <summary>
	/// 按位分离的工厂
	/// </summary>
	public class BitSeparator
	{
		/// <summary>
		/// 按位分离长度
		/// </summary>
		public readonly int[] SeparateDistance;
		/// <summary>
		/// 按位分离位置
		/// </summary>
		public readonly int[] SeparateIndex;
		/// <summary>
		/// 初始化，自动生成SeparateIndex
		/// </summary>
		public BitSeparator(params int[] SeparateDistance)
		{
			this.SeparateDistance = SeparateDistance;
			int n = 0;
			SeparateIndex = new int[SeparateDistance.Length];
			for (int i = 0; i < SeparateDistance.Length; ++i)
			{
				SeparateIndex[i] = n;
				n += SeparateDistance[i];
			}
		}
		/// <summary>
		/// 获取分离的值
		/// </summary>
		public int Get(int SeparatedNumber, int index)
		{
			return BitOperate.GetBits(SeparatedNumber, SeparateIndex[index], SeparateDistance[index]);
		}
		/// <summary>
		/// 设置分离的值
		/// </summary>
		public int Set(ref int SeparatedNumber, int index, int value)
		{
			return SeparatedNumber = BitOperate.SetBits(SeparatedNumber, value, SeparateIndex[index], SeparateDistance[index]);
		}

		public int Build(params int[] values) {
			int res = 0;
			for (int i = 0; i < values.Length; i++)
			{
				Set(ref res, i, values[i]);
			}
			return res;
		}
	}

	/// <summary>
	/// uint按值分离的工厂
	/// value = a0*index0 + a1*index1 + a2*index2 ...
	/// </summary>
	public class UIntSeparator
	{
		/// <summary>
		/// 分离长度
		/// </summary>
		public readonly uint[] SeparateDistance;
		/// <summary>
		/// 分离位置
		/// </summary>
		public readonly uint[] SeparateIndex;
		/// <summary>
		/// 初始化，自动生成SeparateIndex
		/// </summary>
		public UIntSeparator(params uint[] SeparateDistance)
		{
			this.SeparateDistance = SeparateDistance;
			uint n = 1;
			SeparateIndex = new uint[SeparateDistance.Length];
			for (uint i = 0; i < SeparateDistance.Length; ++i)
			{
				SeparateIndex[i] = n;
				n *= SeparateDistance[i];
			}
		}
		/// <summary>
		/// 获取分离的值
		/// </summary>
		public uint Get(uint SeparatedNumber, int index)
		{
			return (SeparatedNumber / SeparateIndex[index]) % SeparateDistance[index];
		}
		/// <summary>
		/// 设置分离的值
		/// </summary>
		public uint Set(ref uint SeparatedNumber, int index, uint value)
		{
			Check(index, value);
			SeparatedNumber -= ((SeparatedNumber / SeparateIndex[index]) % SeparateDistance[index]) * SeparateIndex[index];
			SeparatedNumber += value * SeparateIndex[index];
			return SeparatedNumber;
		}
		/// <summary>
		/// 设置分离的值
		/// </summary>
		public uint Set(uint SeparatedNumber, int index, uint value)
		{
			Check(index, value);
			SeparatedNumber -= ((SeparatedNumber / SeparateIndex[index]) % SeparateDistance[index]) * SeparateIndex[index];
			SeparatedNumber += value * SeparateIndex[index];
			return SeparatedNumber;
		}

		/// <summary>
		/// 检查值是否在范围内
		/// </summary>
		public void Check(int index, uint value)
		{
			if (value < 0 || value >= SeparateDistance[index]) throw new ArgumentOutOfRangeException("value", $"value:{value} 不在index:{index} 范围 [0,{SeparateDistance[index]})内");
		}

		/// <summary>
		/// 根据values生成uint
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public uint Build(params uint[] values) {
			uint res = 0;
			for (int i = 0; i < values.Length; i++) {
				Set(ref res, i, values[i]);
			}
			return res;
		}
	}


	/// <summary>
	/// uint按值分离的工厂
	/// value = a0*index0 + a1*index1 + a2*index2 ...
	/// </summary>
	public class IntSeparator
	{
		public readonly int[] SeparateOffset;
		/// <summary>
		/// 分离长度
		/// </summary>
		public readonly uint[] SeparateDistance;
		/// <summary>
		/// 分离位置
		/// </summary>
		public readonly uint[] SeparateIndex;
		/// <summary>
		/// 初始化，自动生成SeparateIndex
		/// (Offset,Distance)
		/// </summary>
		public IntSeparator(params (int offset,uint distance)[] Values)
		{


			//this.SeparateDistance = SeparateDistance;
			uint n = 1;
			SeparateIndex = new uint[Values.Length];
			SeparateDistance = new uint[Values.Length];
			SeparateOffset = new int[Values.Length];
			for (uint i = 0; i < SeparateDistance.Length; ++i)
			{
				SeparateOffset[i] = Values[i].offset;
				SeparateDistance[i] = Values[i].distance;
				SeparateIndex[i] = n;
				n *= SeparateDistance[i];
			}
		}
		/// <summary>
		/// 获取分离的值
		/// </summary>
		public int Get(int SeparatedNumber, int index)
		{
			return 
				(int)(((uint)SeparatedNumber/ SeparateIndex[index]) % SeparateDistance[index] 
				+SeparateOffset[index]);
		}
		/// <summary>
		/// 设置分离的值
		/// </summary>
		public int Set(ref int SeparatedNumber, int index, int value)
		{
			value -= SeparateOffset[index];
			uint uSep = (uint)SeparatedNumber;
			uint uval = (uint)value;
			Check(index, uval);

			uSep -= ((uSep / SeparateIndex[index]) % SeparateDistance[index]) * SeparateIndex[index];
			uSep += uval * SeparateIndex[index];
			SeparatedNumber = (int)uSep;
			return SeparatedNumber;
		}
		/// <summary>
		/// 设置分离的值
		/// </summary>
		public int Set(int SeparatedNumber, int index, int value)
		{
			value -= SeparateOffset[index];
			uint uSep = (uint)SeparatedNumber;
			uint uval = (uint)value;
			Check(index, uval);

			uSep -= ((uSep / SeparateIndex[index]) % SeparateDistance[index]) * SeparateIndex[index];
			uSep += uval * SeparateIndex[index];
			SeparatedNumber = (int)uSep;
			return SeparatedNumber;
		}

		/// <summary>
		/// 检查值是否在范围内
		/// </summary>
		protected void Check(int index, uint value)
		{
			if (value < 0 || value >= SeparateDistance[index]) throw new ArgumentOutOfRangeException("value", $"value:{value} 不在index:{index} 范围 [0,{SeparateDistance[index]})内");
		}

		/// <summary>
		/// 根据values生成uint
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public int Build(params int[] values)
		{
			int res = 0;
			for (int i = 0; i < values.Length; i++)
			{
				Set(ref res, i, values[i]);
			}
			return res;
		}
	}
}
