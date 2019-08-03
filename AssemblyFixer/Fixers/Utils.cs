using System.Collections.Generic;

namespace UniversalDotNetTools.Fixers {
	internal static unsafe class Utils {
		public static void FixErrorInternal(string name, ushort* p, ushort correctValue, bool fix, ref FixerLevel level, List<string> texts) {
			if (*p != correctValue) {
				level.Ensure(FixerLevel.Error);
				texts.Add($"{name} should be 0x{correctValue.ToString("X4")} (now it is 0x{(*p).ToString("X4")})");
				if (fix)
					*p = correctValue;
			}
		}

		public static void FixErrorInternal(string name, uint* p, uint correctValue, bool fix, ref FixerLevel level, List<string> texts) {
			if (*p != correctValue) {
				level.Ensure(FixerLevel.Error);
				texts.Add($"{name} should be 0x{correctValue.ToString("X8")} (now it is 0x{(*p).ToString("X8")})");
				if (fix)
					*p = correctValue;
			}
		}

		public static void FixErrorInternal(string name, ulong* p, ulong correctValue, bool fix, ref FixerLevel level, List<string> texts) {
			if (*p != correctValue) {
				level.Ensure(FixerLevel.Error);
				texts.Add($"{name} should be 0x{correctValue.ToString("X16")} (now it is 0x{(*p).ToString("X16")})");
				if (fix)
					*p = correctValue;
			}
		}

		public static void FixWarningInternal(string name, ushort* p, ushort correctValue, bool fix, ref FixerLevel level, List<string> texts) {
			if (*p != correctValue) {
				level.Ensure(FixerLevel.Warning);
				texts.Add($"{name} may be 0x{correctValue.ToString("X4")} (now it is 0x{(*p).ToString("X4")})");
				if (fix)
					*p = correctValue;
			}
		}

		public static void FixWarningInternal(string name, uint* p, uint correctValue, bool fix, ref FixerLevel level, List<string> texts) {
			if (*p != correctValue) {
				level.Ensure(FixerLevel.Warning);
				texts.Add($"{name} may be 0x{correctValue.ToString("X8")} (now it is 0x{(*p).ToString("X8")})");
				if (fix)
					*p = correctValue;
			}
		}

		public static void FixWarningInternal(string name, ulong* p, ulong correctValue, bool fix, ref FixerLevel level, List<string> texts) {
			if (*p != correctValue) {
				level.Ensure(FixerLevel.Warning);
				texts.Add($"{name} may be 0x{correctValue.ToString("X16")} (now it is 0x{(*p).ToString("X16")})");
				if (fix)
					*p = correctValue;
			}
		}
	}
}
