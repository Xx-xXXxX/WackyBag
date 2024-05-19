using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures
{
	public readonly unsafe struct Ref<T>
	{
		private readonly void* ptr;
		public readonly ref T Value => ref Unsafe.AsRef<T>(ptr);
		public Ref(ref T value){
			ptr = Unsafe.AsPointer(ref value);
		}

		public Ref(void* ptr)
		{
			this.ptr = ptr;
		}
	}
	//public ref struct RefR<T>
	//{
	//	ref T value;
	//	public ref T Value => ref value;
	//	public void SetValue(ref T value) { 
	//		this.value = value;
	//	}
	//	public RefR(ref T value) {
	//		this.value = value;
	//	}
	//}
}