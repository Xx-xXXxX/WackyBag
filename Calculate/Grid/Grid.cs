using FixMath;


using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Calculate.Grid
{
    /// <summary>
    /// 以格子的中心计算距离，按距离枚举格子
    /// </summary>
    public static class GridCEnum
    {
        /// <summary>
        /// 已计算的格子列表
        /// </summary>
        public static readonly List<(Point, Fix64)> CirclePointList = [];
		/// <summary>
		/// 已计算的距离半径,在该范围内的所有格子都在列表中
		/// </summary>
		public static Fix64 RealCalculatedSize { get; private set; }
        /// <summary>
        /// 已计算的格子半径
        /// </summary>
        public static int CalculatedSize
        {
            get => calculatedSize; private set
            {
                calculatedSize = value;
                RealCalculatedSize = CalculatedSize + (Fix64)0.5;
            }
        }
		/// <summary>
		/// 已计算但未确定顺序的格子列表
		/// </summary>
		public static readonly SortedDictionary<Fix64, List<(Point, Fix64)>> ValuesConsidering = new() {
            { 0,new(){(new(0,0),0) } }
        };
        private static int calculatedSize = 0;
		/// <summary>
		/// 将下一列格子填入ValuesConsidering,然后将可以确定顺序的格子填入CirclePointList
		/// </summary>
		public static void CalculateNext()
        {
            CalculatedSize++;
            for (int i = 0; i <= CalculatedSize; ++i)
            {
                Fix64 sx = (Fix64)i - (Fix64)0.5;
                Fix64 sy = (Fix64)CalculatedSize - (Fix64)0.5;
                (Point, Fix64) v = (new(i, CalculatedSize),
                    i == 0 ? sy :
                    Fix64.Sqrt(sx * sx + sy * sy));
                if (!ValuesConsidering.TryGetValue(v.Item2, out var values))
                {
                    ValuesConsidering.Add(v.Item2, values = []);
                }
                values.Add(v);
            }
            while (true)
            {
                var v = ValuesConsidering.First();
                if (v.Key >= RealCalculatedSize) break;
                ValuesConsidering.Remove(v.Key);//   .Remove(v.Key);
                CirclePointList.AddRange(v.Value);
            }
        }

		public static void GetCirclePoints(Func<(Point, Fix64),bool> f, int start = 0)
		{
			for (int I = start; ; I++)
			{
				if (I >= CirclePointList.Count) CalculateNext();
				var i = CirclePointList[I];
				//var p = i.Item1;
                if (!f(i)) return;
			}
		}

		public static void GetCirclePointsOct(Func<Fix64,Action<Point>?> f, int start = 0)
		{
			for (int I = start; ; I++)
			{
				if (I >= CirclePointList.Count) CalculateNext();
				var i = CirclePointList[I];
                //var p = i.Item1;
                var f2 = f(i.Item2);
                if (f2 == null) return;
                foreach (var item in ToOct(i.Item1))
                {
                    f2(item);
                }
			}
		}
		public static IEnumerable<Point> ToOct(Point p) {
            yield return p;
            bool y0 = p.Y != 0;
            bool x0= p.X != 0;
            bool xy = p.X != p.Y;
			if (y0) yield return (new(p.X, -p.Y));
            if (x0){ yield return(new(-p.X, -p.Y));
                if (y0) { yield return (new(-p.X, p.Y));
                    if (xy) {
						yield return (new(p.Y, p.X));
						yield return (new(p.Y, -p.X));
						yield return (new(-p.Y, -p.X));
						yield return (new(-p.Y, p.X));
					}
                }
            }
		}
	}
    
    public class Grid<T> : IEnumerable<KeyValuePair<Point, T>>
    {
        public Fix64 SizePerCell;
        public readonly int CountHalf;
        public Grid(Fix64 gridSize, int countHalf)
        {
            SizePerCell = gridSize;
            //values = new T[gridCount, gridCount];
            CountHalf = countHalf;
            values = new T[2 * countHalf, 2 * countHalf];
        }
        //public readonly T[,] values;
        //public readonly Side2List<Side2List<T>> values;
        private readonly T[,] values;
        public T GetValue(Vector2Fix pos)
        {
            Fix64 x = pos.x / SizePerCell;
            Fix64 y = pos.y / SizePerCell;
            int yi = (int)Fix64.Floor(y) + CountHalf;
            int xi = (int)Fix64.Floor(x) + CountHalf;
            return values[xi, yi];
        }
        public void SetValue(Vector2Fix pos, T value)
        {
            Fix64 x = pos.x / SizePerCell;
            Fix64 y = pos.y / SizePerCell;
            int yi = (int)Fix64.Floor(y) + CountHalf;
            int xi = (int)Fix64.Floor(x) + CountHalf;
            values[xi, yi] = value;
        }
        public T GetValue(int x, int y) => values[x + CountHalf, y + CountHalf];
        public void SetValue(int x, int y, T value) => values[x + CountHalf, y + CountHalf] = value;
        public T GetValue(Point point) => GetValue(point.X, point.Y);
        public void SetValue(Point point, T value) => SetValue(point.X, point.Y, value);
        public IEnumerable<Point> GetPointEnumerator()
        {
            for (int xi = -CountHalf; xi < CountHalf; xi++)
            {
                for (int yi = CountHalf; yi < CountHalf; yi++)
                {
                    yield return new(xi, yi);
                }
            }
        }
        public IEnumerator<KeyValuePair<Point, T>> GetEnumerator()
        {
            for (int xi = -CountHalf; xi < CountHalf; xi++)
            {
                for (int yi = CountHalf; yi < CountHalf; yi++)
                {
                    yield return new(new(xi, yi), values[xi, yi]);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T this[Vector2Fix pos]
        {
            get => GetValue(pos);
            set => SetValue(pos, value);
        }
        public T this[int x, int y]
        {
            get => GetValue(x, y);
            set => SetValue(x, y, value);
        }
        public T this[Point point]
        {
            get => GetValue(point);
            set => SetValue(point, value);
        }
    }
}
