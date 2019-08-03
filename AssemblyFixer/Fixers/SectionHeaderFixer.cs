using System;
using System.Collections.Generic;
using System.Linq;
using static Mdlib.NativeMethods;

namespace UniversalDotNetTools.Fixers {
	internal sealed class SectionHeaderFixer : IFixer {
		public string Name => nameof(SectionHeaderFixer);

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
			IMAGE_SECTION_HEADER* p;
			uint sectionCount;

			level = FixerLevel.None;
			texts = new List<string>();
			p = context.PEImage.SectionHeaders.First().RawValue;
			sectionCount = context.PEImage.FileHeader.SectionCount;
			for (uint i = 0; i < sectionCount; i++) {
				uint characteristics;

				characteristics = (p + i)->Characteristics;
				characteristics &= 0x00000020 | 0x00000040 | 0x02000000 | 0x04000000 | 0x08000000 | 0x20000000 | 0x40000000 | 0x80000000;
				Utils.FixWarningInternal($"IMAGE_SECTION_HEADER[{i}].Characteristics", &(p + i)->Characteristics, characteristics, fix, ref level, texts);
			}
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
