using System;
using System.Collections.Generic;
using System.Linq;
using Mdlib.PE;
using static Mdlib.NativeMethods;

namespace UniversalDotNetTools.Fixers {
	internal sealed class OptionalHeaderFixer : IFixer {
		public string Name => nameof(OptionalHeaderFixer);

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
			DataDirectory* pDataDirectories;

			level = FixerLevel.None;
			texts = new List<string>();
			if (!context.PEImage.Is64Bit) {
				IMAGE_OPTIONAL_HEADER32* p;
				uint imageSize;
				SectionHeader lastSectionHeader;
				uint headersSize;

				p = context.PEImage.OptionalHeader.RawValue32;
				Utils.FixErrorInternal("IMAGE_OPTIONAL_HEADER.SizeOfUninitializedData", &p->SizeOfUninitializedData, 0, fix, ref level, texts);
				Utils.FixWarningInternal("(TODO!!!) IMAGE_OPTIONAL_HEADER.AddressOfEntryPoint", &p->AddressOfEntryPoint, 0, fix, ref level, texts);
				imageSize = GetImageSize(context.PEImage);
				Utils.FixErrorInternal("IMAGE_OPTIONAL_HEADER.SizeOfImage", &p->SizeOfImage, imageSize, fix, ref level, texts);
				lastSectionHeader = context.PEImage.SectionHeaders.Last();
				headersSize = AlignUp((uint)lastSectionHeader.FOA + lastSectionHeader.Length, context.PEImage.OptionalHeader.FileAlignment);
				Utils.FixErrorInternal("IMAGE_OPTIONAL_HEADER.SizeOfHeaders", &p->SizeOfHeaders, headersSize, fix, ref level, texts);
				Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.CheckSum", &p->CheckSum, 0, fix, ref level, texts);
				Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.DllCharacteristics", &p->DllCharacteristics, 0x8540, fix, ref level, texts);
				// DYNAMIC_BASE  | NX_COMPAT | NO_SEH | TERMINAL_SERVER_AWARE
				Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.SizeOfStackReserve", &p->SizeOfStackReserve, 0x00100000, fix, ref level, texts);
				Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.SizeOfStackCommit", &p->SizeOfStackCommit, 0x00001000, fix, ref level, texts);
				Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.SizeOfHeapReserve", &p->SizeOfHeapReserve, 0x00100000, fix, ref level, texts);
				Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.SizeOfHeapCommit", &p->SizeOfHeapCommit, 0x00001000, fix, ref level, texts);
				Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.NumberOfRvaAndSizes", &p->NumberOfRvaAndSizes, 0x10, fix, ref level, texts);
				pDataDirectories = (DataDirectory*)p->DataDirectory;
			}
			else {
				IMAGE_OPTIONAL_HEADER64* p;
				uint imageSize;
				SectionHeader lastSectionHeader;
				uint headersSize;

				p = context.PEImage.OptionalHeader.RawValue64;
				Utils.FixErrorInternal("IMAGE_OPTIONAL_HEADER.SizeOfUninitializedData", &p->SizeOfUninitializedData, 0, fix, ref level, texts);
				Utils.FixErrorInternal("IMAGE_OPTIONAL_HEADER.AddressOfEntryPoint", &p->AddressOfEntryPoint, 0, fix, ref level, texts);
				imageSize = GetImageSize(context.PEImage);
				Utils.FixErrorInternal("IMAGE_OPTIONAL_HEADER.SizeOfImage", &p->SizeOfImage, imageSize, fix, ref level, texts);
				lastSectionHeader = context.PEImage.SectionHeaders.Last();
				headersSize = AlignUp((uint)lastSectionHeader.FOA + lastSectionHeader.Length, context.PEImage.OptionalHeader.FileAlignment);
				Utils.FixErrorInternal("IMAGE_OPTIONAL_HEADER.SizeOfHeaders", &p->SizeOfHeaders, headersSize, fix, ref level, texts);
				Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.CheckSum", &p->CheckSum, 0, fix, ref level, texts);
				Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.DllCharacteristics", &p->DllCharacteristics, 0x8540, fix, ref level, texts);
				// DYNAMIC_BASE  | NX_COMPAT | NO_SEH | TERMINAL_SERVER_AWARE
				Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.SizeOfStackReserve", &p->SizeOfStackReserve, 0x0000000000400000, fix, ref level, texts);
				Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.SizeOfStackCommit", &p->SizeOfStackCommit, 0x0000000000004000, fix, ref level, texts);
				Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.SizeOfHeapReserve", &p->SizeOfHeapReserve, 0x0000000000100000, fix, ref level, texts);
				Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.SizeOfHeapCommit", &p->SizeOfHeapCommit, 0x0000000000002000, fix, ref level, texts);
				Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.NumberOfRvaAndSizes", &p->NumberOfRvaAndSizes, 0x10, fix, ref level, texts);
				pDataDirectories = (DataDirectory*)p->DataDirectory;
			}
			Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.DataDirectory[IMAGE_DIRECTORY_ENTRY_EXPORT]", (ulong*)pDataDirectories, 0, fix, ref level, texts);
			Utils.FixWarningInternal("(TODO!!!)IMAGE_OPTIONAL_HEADER.DataDirectory[IMAGE_DIRECTORY_ENTRY_IMPORT]", (ulong*)(pDataDirectories + 1), 0, fix, ref level, texts);
			Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.DataDirectory[IMAGE_DIRECTORY_ENTRY_EXCEPTION]", (ulong*)(pDataDirectories + 3), 0, fix, ref level, texts);
			Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.DataDirectory[IMAGE_DIRECTORY_ENTRY_SECURITY]", (ulong*)(pDataDirectories + 4), 0, fix, ref level, texts);
			Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.DataDirectory[IMAGE_DIRECTORY_ENTRY_DEBUG]", (ulong*)(pDataDirectories + 6), 0, fix, ref level, texts);
			Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.DataDirectory[IMAGE_DIRECTORY_ENTRY_ARCHITECTURE]", (ulong*)(pDataDirectories + 7), 0, fix, ref level, texts);
			Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.DataDirectory[IMAGE_DIRECTORY_ENTRY_GLOBALPTR]", (ulong*)(pDataDirectories + 8), 0, fix, ref level, texts);
			Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.DataDirectory[IMAGE_DIRECTORY_ENTRY_TLS]", (ulong*)(pDataDirectories + 9), 0, fix, ref level, texts);
			Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.DataDirectory[IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG]", (ulong*)(pDataDirectories + 10), 0, fix, ref level, texts);
			Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.DataDirectory[IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT]", (ulong*)(pDataDirectories + 11), 0, fix, ref level, texts);
			Utils.FixWarningInternal("(TODO!!!)IMAGE_OPTIONAL_HEADER.DataDirectory[IMAGE_DIRECTORY_ENTRY_IAT]", (ulong*)(pDataDirectories + 12), 0, fix, ref level, texts);
			Utils.FixWarningInternal("IMAGE_OPTIONAL_HEADER.DataDirectory[IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT]", (ulong*)(pDataDirectories + 13), 0, fix, ref level, texts);
			if (level == FixerLevel.None) {
				message = FixerMessage.None;
				return false;
			}
			else {
				message = new FixerMessage(level, string.Join(Environment.NewLine, texts));
				return true;
			}
		}

		private static uint GetImageSize(IPEImage peImage) {
			SectionHeader lastSectionHeader;
			uint alignment;
			uint imageSize;

			lastSectionHeader = peImage.SectionHeaders.Last();
			alignment = peImage.OptionalHeader.SectionAlignment;
			imageSize = (uint)lastSectionHeader.VirtualAddress + lastSectionHeader.VirtualSize;
			if (imageSize % alignment != 0)
				imageSize = AlignUp(imageSize, alignment);
			return imageSize;
		}

		private static uint AlignUp(uint value, uint alignment) {
			return value - (value % alignment) + alignment;
		}
	}
}
