namespace WackyBag.Structures.Collections
{
	/// <summary>
	/// <para>存储两组 Id 的相互引用</para>
	/// <para>UId:不变的 Id </para>
	/// <para>SId:由 UnorderedList 管理的，在删除元素时变化的 Id</para>
	/// </summary>
	public class UIdToSIdList {
		public int Count => sIdToUId.Count;
		protected OpenList<int> uIdToSId;
		protected UnorderedList<int> sIdToUId;

		public IReadOnlyList<int> SIdToUId => sIdToUId;
		public IReadOnlyIndexable<int> UIdToSId => (IReadOnlyIndexable<int>)uIdToSId;

		//public (int,int) Add()
		//{
		//	int SId = sIdToUId.Count;// values.Add(value);
		//	int UId = uIdToSId.Add(SId);
		//	sIdToUId.Add(UId);
		//	return (UId,SId);
		//}
		public int Add(int UId)
		{
			int SId = sIdToUId.Count;//sIdToUId.Add(UId);
			sIdToUId.Add(  UId);
			//uIdToSId.Add(UId, SId);
			uIdToSId.TrySet( UId, SId);
			return SId;
		}
		public void RemoveBySId(int SId)
		{
			int UId = SIdToUId[SId];
			uIdToSId[UId]=-1;
			sIdToUId.Remove(SId);
		}
		public void RemoveByUId(int UId)
		{
			int SId = uIdToSId[UId];
			uIdToSId[UId] = -1;
			sIdToUId.Remove(SId);
		}
		public bool TryRemoveBySId(int SId)
		{
			if (sIdToUId.Count >= SId) return false;
			int UId = SIdToUId[SId];
			uIdToSId[UId] = -1;
			sIdToUId.Remove(SId);
			return true;
		}
		public bool TryRemoveByUId(int UId)
		{
			int SId = uIdToSId[UId];
			if (SId==-1) return false;
			uIdToSId[UId] = -1;
			sIdToUId.Remove(SId);
			return true;
		}
		public UIdToSIdList(UnorderedList<int>.MoveFn? moveFn=null)
		{
			sIdToUId = new(Move);
			sIdToUId.OnMove += moveFn;
			uIdToSId = new();
		}
		public void Clear() {
			uIdToSId.Clear();
			sIdToUId.Clear();
		}
		protected void Move(in int sIdFrom,in int sIdTo) {
			int uId= sIdToUId[sIdFrom];
			uIdToSId[uId] = sIdTo;
		}
	}

}
