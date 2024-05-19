using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures
{
	//[Obsolete("Don't use",error:false)]
	public static class StoreTypeID<TTab>
	{
		//private static FieldInfo fieldInfo = typeof(StoreID<>)
		private static class StoreID<T>
		{
			public static int id=-1;
		}
		//private static FieldInfo FieldInfo= typeof(StoreID<object>).GetField("id", BindingFlags.Static | BindingFlags.Public)!;
		
		private static FieldInfo GetFieldInfo(Type type) => GetStoreIDType(type).GetField("id", BindingFlags.Static | BindingFlags.Public)!;
		private static Type GetStoreIDType(Type t) => typeof(StoreID<>).MakeGenericType(typeof(TTab), t);
		
		public static int GetID<T>() => StoreID<T>.id;
		public static int GetID(Type t) => (int)GetFieldInfo(t).GetValue(null)!;
		public static void SetID<T>(int id) => StoreID<T>.id = id;
		public static void SetID(Type t, int id) => GetFieldInfo(t).SetValue(null, id);
	}
}
