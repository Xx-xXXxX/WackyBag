using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag
{
	public class Fn<Res,Param>
	{
		public delegate Res F(Param p);
		public class P<Param2> : Fn<Func<Param, Res>, Param2> { }
	}

	public class Atn<Param> { 
		public delegate void F(Param p);
		public class P<Param2> : Fn<Action<Param>, Param2> { }
	}

	public class Fn<Res> {
		public delegate Res F();
		public class P<Param> : Fn<Res, Param> { }
	}
}
