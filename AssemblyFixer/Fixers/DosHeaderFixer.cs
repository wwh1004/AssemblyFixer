using System;
using System.Collections.Generic;
using static Mdlib.NativeMethods;

namespace UniversalDotNetTools.Fixers {
	internal sealed class DosHeaderFixer : IFixer {
		public string Name => nameof(DosHeaderFixer);

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
			IMAGE_DOS_HEADER* p;

			level = FixerLevel.None;
			texts = new List<string>();
			p = context.PEImage.DosHeader.RawValue;
			Utils.FixErrorInternal("IMAGE_DOS_HEADER.e_magic", &p->e_magic, 0x5A4D, fix, ref level, texts);
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
