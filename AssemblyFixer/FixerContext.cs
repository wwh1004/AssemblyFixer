using System;
using Mdlib.PE;

namespace UniversalDotNetTools {
	/// <summary>
	/// Fixer context
	/// </summary>
	public sealed class FixerContext : IDisposable {
		private readonly IPEImage _peImage;
		private bool _isDll;
		private bool _isDisposed;

		/// <summary />
		internal IPEImage PEImage => _peImage;

		/// <summary />
		public IntPtr RawData => _peImage.RawData;

		/// <summary />
		public uint Length => _peImage.Length;

		/// <summary />
		public bool IsDll {
			get => _isDll;
			set => _isDll = value;
		}

		internal FixerContext(IPEImage peImage) {
			if (peImage is null)
				throw new ArgumentNullException(nameof(peImage));

			_peImage = peImage;
		}

		/// <summary />
		public void Dispose() {
			if (_isDisposed)
				return;
			_peImage.Dispose();
			_isDisposed = true;
		}
	}
}
