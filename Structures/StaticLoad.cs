using WackyBag;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace WackyBag.Structures
{
	/// <summary>
	/// 在加载时就执行，有顺序
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
	public class StaticLoadAttribute : Attribute
	{
		public static bool Updated { get; private set; } = false;
		public readonly string ActionName;
		public readonly string? FnName;
		public readonly string[]? befores;
		public readonly string[]? afters;
		private static readonly SortedByOrderWithKey<string, List<Action>> actions = [];

		public static List<Type[]> Types { get; private set; } = [Assembly.GetExecutingAssembly().GetTypes()];

		/// <summary>
		/// 在加载时就执行，有顺序
		/// </summary>
		/// <param name="ActionName">操作的名称</param>
		/// <param name="fnName">函数的名称</param>
		/// <param name="befores"></param>
		/// <param name="afters"></param>
		public StaticLoadAttribute(string ActionName, string? fnName, string[]? befores, string[]? afters)
		{
			this.ActionName = ActionName;
			FnName = fnName;
			this.befores = befores;
			this.afters = afters;
		}
		//static StaticLoadAttribute()
		//{
		//	StaticLoad();
		//}
		public static void StaticLoad()
		{
			//UnityEngine.Debug.Log("StaticLoadAttrubute Tried");
			FindAndLoadFn();
			//foreach (var i in actions) {

			//	i?.Invoke();
			//}

			foreach (var i in actions)
			{
				foreach (var j in i) j.Invoke();
			}
			//UnityEngine.Debug.Log("StaticLoadAttrubute Ended");

			Updated = true;
		}
		/// <summary>
		/// 查找所有引用并执行
		/// </summary>
		private static void FindAndLoadFn()
		{
			//var types = Assembly.GetExecutingAssembly().GetTypes();//获取所有类
			foreach (var ts in Types)//枚举
			{
				foreach (var type in ts)
				{
					if (type.IsClass)
					{
						var attrubutes1 = type.GetCustomAttributes<StaticLoadAttribute>();//获取StaticLoadAttribute
						if (attrubutes1 != null)
						{
							if (!type.HaveAttribute<NoStaticLoadAttribute>())//如果没有NoStaticLoad
							{
								foreach (var i in attrubutes1)
								{
									Action action;
									if (i.FnName == null)
									{
										action = () => RuntimeHelpers.RunClassConstructor(type.TypeHandle);
									}
									else
									{
										var fn = type.GetMethod(i.FnName, BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
										action = () => fn!.Invoke(null, null);//获取方法
									}
									//actions.AddOrderedValue(new(action,i.ActionName,i.befores,i.afters));//加入
									List<Action> list = actions.Get(i.ActionName).Value.Value.Value!;
									if (list == null) actions.Add(i.ActionName, list = []);
									list.Add(action);
									actions.AddOrders(i.ActionName, i.befores, i.afters);

								}
							}
						}
					}
					var attrubutes2 = type.GetCustomAttributes<StaticLoadInheritAttribute>();//获取StaticLoadInheritAttribute
					if (attrubutes2 != null)
					{
						Type? Ptype = type.BaseType;
						bool SetAction = false;
						if (type.IsClass && !type.IsAbstract && !type.HaveAttribute<NoStaticLoadAttribute>())
							SetAction = true;
						foreach (var i in attrubutes2)
						{
							if (Ptype == null || !Ptype.HaveAttribute(i.GetType()))
								actions.AddOrders(i.ActionName, i.befores, i.afters);
							if (SetAction)//如果没有NoStaticLoad
							{
								var fn = type.GetMethod(i.FnName, BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy) ?? throw new ArgumentException(i.FnName+"isn't exist",nameof(i.FnName));
								void action() => fn.Invoke(null, [type]);//获取带type的方法
																						//actions.AddOrderedValue(new(action, i.ActionName, i.befores, i.afters));//加入
																						//actions.Get(i.ActionName).Value.Value.Add(action);
								List<Action> list = actions.Get(i.ActionName).Value.Value.Value!;
								if (list == null) actions.Add(i.ActionName, list = []);
								list.Add(action);
							}
						}
					}
				}
			}
		}
	}
	/// <summary>
	/// 标记 不 加载时就执行
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public class NoStaticLoadAttribute : Attribute
	{
	}
	/// <summary>
	/// 继承类 加载时就执行 函数，提供type
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
	public class StaticLoadInheritAttribute : Attribute
	{
		public string ActionName;
		public string FnName;
		public readonly string[]? befores;
		public readonly string[]? afters;
		/// <summary>
		/// 在加载时就执行，有顺序
		/// </summary>
		/// <param name="ActionName">操作的名称</param>
		/// <param name="fnName">函数的名称</param>
		/// <param name="befores"></param>
		/// <param name="afters"></param>
		public StaticLoadInheritAttribute(string ActionName, string fnName, string[]? befores, string[]? afters)
		{
			this.ActionName = ActionName;
			FnName = fnName;
			this.befores = befores;
			this.afters = afters;
		}
	}
}
