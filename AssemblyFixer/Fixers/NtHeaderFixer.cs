using System;
using System.Collections.Generic;
using static Mdlib.NativeMethods;

namespace UniversalDotNetTools.Fixers {
	internal sealed class NtHeaderFixer : IFixer {
		public string Name => nameof(NtHeaderFixer);

		public bool Check(FixerContext context, out FixerMessage message) {
			if (context is null)
				throw new ArgumentNullException(nameof(context));

			return FixImpl(context, false, out message);
		}

		public bool Fix(FixerContext context, out FixerMessage message) {
			if (context is null)
				throw new ArgumentNullException(nameof(context));

			return FixImpl(context, true, out message);
		}

		private static unsafe bool FixImpl(FixerContext context, bool fix, out FixerMessage message) {
			FixerLevel level;
			List<string> texts;
			IMAGE_NT_HEADERS32* p;

			level = FixerLevel.None;
			texts = new List<string>();
			p = context.PEImage.NtHeader.RawValue32;
			Utils.FixErrorInternal("IMAGE_NT_HEADERS.Signature", &p->Signature, 0x4550, fix, ref level, texts);
			if (level == FixerLevel.None) {
				message = FixerMessage.None;
				return false;
			}
			else {
				message = new FixerMessage(level, string.Join(Environment.NewLine, texts));
				return true;
			}
		}
	}
}
