using System;
using Mdlib.PE;

namespace UniversalDotNetTools {
	/// <summary>
	/// Fixer context
	/// </summary>
	public sealed class FixerContext {
		private readonly IPEImage _peImage;
		private bool _isDll;

		/// <summary />
		public IPEImage PEImage => _peImage;

		/// <summary />
		public bool IsDll {
			get => _isDll;
			set => _isDll = value;
		}

		public FixerContext(IPEImage peImage) {
			if (peImage is null)
				throw new ArgumentNullException(nameof(peImage));

			_peImage = peImage;
		}
	}
}
