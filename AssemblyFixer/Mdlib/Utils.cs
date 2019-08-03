using System;

namespace Mdlib {
	internal static unsafe class Utils {
		public static string PointerToString(IntPtr value) {
			return "0x" + ((ulong)value > uint.MaxValue ? value.ToString("X16") : value.ToString("X8"));
		}
	}
}
