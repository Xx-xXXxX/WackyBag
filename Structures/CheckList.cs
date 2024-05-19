using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WackyBag.Structures.Collections;

namespace WackyBag.Structures
{
	public class CheckList
	{
		public static void MakeParallel(int count, Action<Action, int> ForEach, Action OnAllDone)
		{
			UIdToSIdList _list = new();
			Action onAllDone = OnAllDone;
			if (count == 0) { OnAllDone(); return; }
			Action DoneCheck(int id)
			{
				//if (id >= count) throw new ArgumentOutOfRangeException(nameof(id));
				return () =>
				{
					lock (_list)
						if (_list.TryRemoveByUId(id))
						{
							if (_list.Count == 0) onAllDone?.Invoke();
						}
						else throw new InvalidOperationException($"Check {id} twice");
				};
			}
			for (int i = 0; i < count; i++)
			{
				_list.Add(i);
				//ForEach(DoneCheck, i);
			}
			Parallel.For(0, count, (i) => ForEach(DoneCheck(i), i));
			
		}

		public static void Make(int count, Action<Action, int> ForEach, Action OnAllDone)
		{
			UIdToSIdList _list = new();
			Action onAllDone = OnAllDone;
			if (count == 0) { OnAllDone(); return; }
			Action DoneCheck(int id)
			{
				//if (id >= count) throw new ArgumentOutOfRangeException(nameof(id));
				return () =>
				{
					lock (_list)
						if (_list.TryRemoveByUId(id))
						{
							if (_list.Count == 0) onAllDone?.Invoke();
						}
						else throw new InvalidOperationException($"Check {id} twice");
				};
			}
			for (int i = 0; i < count; i++)
			{
				_list.Add(i);
				//ForEach(DoneCheck, i);
			}
			//Parallel.For(0, count, (i) => ForEach(DoneCheck(i), i));
			for (int i = 0; i < count; i++)
			{
				ForEach(DoneCheck(i), i);
			}
		}
		public CheckList(int count, Action<Action, int> ForEach, Action OnAllDone)
		{
			if (count == 0) { OnAllDone(); }
			else
			{
				CheckTotalCount = count;
				for (int i = 0; i < count; i++)
				{
					_list.Add(i);
				}

				this.OnAllDone += OnAllDone;

				Parallel.For(0, count, (i) => ForEach(()=>DoneCheck(i), i));
			}//if(CheckRemainCount==0) OnAllDone();
		}
		private readonly UIdToSIdList _list = new();
		public event Action? OnAllDone;
		public int CheckTotalCount { get; private set; }
		public int CheckRemainCount => _list.Count;
		public void DoneCheck(int id)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(id, CheckTotalCount);
			lock (_list)
			{
				if (_list.TryRemoveByUId(id))
				{
					if (CheckRemainCount == 0) OnAllDone?.Invoke();
					return;
				}
				else throw new InvalidOperationException($"Check {id} twice");
			}
		}
	}
}
