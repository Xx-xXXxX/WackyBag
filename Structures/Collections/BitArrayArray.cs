using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

using WackyBag.Utils;

namespace WackyBag.Structures.Collections
{
	public class BitArrayArray
	{


		public int ArrayLength { get; }
		public int BitCount { get; }
		public int BitArrLength { get; }

		protected const int IntSize = 4;
		protected const int IntSizeLog = 2;
		protected const int ByteSize = 8;
		protected const int ByteSizeLog = 3;

		protected int[,] array;

		public BitArrayArray(int ArrayLength, int BitCount)
		{
			this.ArrayLength = ArrayLength;
			this.BitCount = BitCount;
			var (bl, e) = BitOperate.Separate(BitCount, IntSizeLog + ByteSizeLog);
			BitArrLength = bl;
			if (e > 0) BitArrLength += 1;
			array = new int[ArrayLength, BitArrLength];
		}

		public bool Get(int arrayIndex, int BitIndex)
		{
			var (a, b) = BitOperate.Separate(BitIndex, IntSizeLog + ByteSizeLog);
			int v = array[arrayIndex, a];
			return BitOperate.GetBits(v, b, 1) == 1;
		}

		public void Set(int arrayIndex, int BitIndex, bool value)
		{
			var (a, b) = BitOperate.Separate(BitIndex, IntSizeLog + ByteSizeLog);
			ref int v = ref array[arrayIndex, a];
			v = BitOperate.SetBits(v, value ? 1 : 0, b, 1);
		}

		public bool this[int arrayIndex, int BitIndex]
		{
			get => Get(arrayIndex, BitIndex);
			set => Set(BitIndex, BitIndex, value);
		}

		public void SetArray(int arrayIndex, int[] array)
		{
			//Array.Copy(array, 0, this.array, arrayIndex * BitArrLength, BitArrLength);
			if (array.Length < BitArrLength)
			{
				throw new InvalidOperationException($"array.Length {array.Length} < BitArrLength {BitArrLength}");
			}
			for (int i = 0; i < BitArrLength; i++)
			{
				this.array[arrayIndex, i] = array[i];
			}
		}
		private readonly List<int> existsList = [];
		public IEnumerable<int> Match(int[] array)
		{
			existsList.Clear();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != 0)
					existsList.Add(i);
			}
			int[] existArray = existsList.ToArray();

			for (int arrindex = 0; arrindex < ArrayLength; arrindex++)
			{
				bool pass = true;
				for (int existindex = 0; existindex < existArray.Length; existindex++)
				{
					int intindex = existArray[existindex];
					if ((this.array[arrindex, intindex] & array[intindex]) != array[intindex]) { pass = false; break; }
				}
				if (pass) { yield return arrindex; }
			}
		}
	}
}
