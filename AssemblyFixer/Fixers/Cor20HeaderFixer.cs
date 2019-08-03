using System;
using System.Collections.Generic;
using static Mdlib.DotNet.Metadata.NativeMethods;

namespace UniversalDotNetTools.Fixers {
	internal sealed class Cor20HeaderFixer : IFixer {
		public string Name => nameof(Cor20HeaderFixer);

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
			IMAGE_COR20_HEADER* p;
			uint flags;

			level = FixerLevel.None;
			texts = new List<string>();
			p = context.PEImage.Metadata.Cor20Header.RawValue;
			flags = p->Flags;
			flags |= 0x1;
			// ILONLY
			Utils.FixErrorInternal("IMAGE_COR20_HEADER.Flags", &p->Flags, flags, fix, ref level, texts);
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
