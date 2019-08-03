using System;
using System.Collections.Generic;
using Mdlib.PE;
using static Mdlib.NativeMethods;

namespace UniversalDotNetTools.Fixers {
	internal sealed class FileHeaderFixer : IFixer {
		public string Name => nameof(FileHeaderFixer);

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
			IMAGE_FILE_HEADER* p;
			ushort machine;
			ushort characteristics;

			level = FixerLevel.None;
			texts = new List<string>();
			p = context.PEImage.FileHeader.RawValue;
			machine = (ushort)(context.PEImage.Is64Bit ? MachineType.AMD64 : MachineType.I386);
			Utils.FixErrorInternal("IMAGE_FILE_HEADER.Machine", &p->Machine, machine, fix, ref level, texts);
			Utils.FixErrorInternal("IMAGE_FILE_HEADER.PointerToSymbolTable", &p->PointerToSymbolTable, 0, fix, ref level, texts);
			Utils.FixErrorInternal("IMAGE_FILE_HEADER.NumberOfSymbols", &p->NumberOfSymbols, 0, fix, ref level, texts);
			characteristics = (ushort)(context.IsDll ? 0x2022 : 0x0022);
			/*
			 * IMAGE_FILE_EXECUTABLE_IMAGE    = 0x0002
			 * IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x0020
			 * IMAGE_FILE_DLL                 = 0x2000
			 */
			Utils.FixErrorInternal("IMAGE_FILE_HEADER.Characteristics", &p->Characteristics, characteristics, fix, ref level, texts);
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
