using System;
using Mdlib.PE;

namespace UniversalDotNetTools {
	/// <summary>
	/// Fixer level
	/// </summary>
	public enum FixerLevel {
		/// <summary>
		/// No error
		/// </summary>
		None,

		/// <summary>
		/// A possible error
		/// </summary>
		Warning,

		/// <summary>
		/// An error
		/// </summary>
		Error
	}

	/// <summary>
	/// Fixer message
	/// </summary>
	public sealed class FixerMessage {
		private readonly FixerLevel _level;
		private readonly string _text;

		/// <summary>
		/// Level
		/// </summary>
		public FixerLevel Level => _level;

		/// <summary>
		/// Text
		/// </summary>
		public string Text => _text;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="level"></param>
		/// <param name="text"></param>
		public FixerMessage(FixerLevel level, string text) {
			switch (level) {
			case FixerLevel.None:
			case FixerLevel.Warning:
			case FixerLevel.Error:
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(level));
			}
			if (text is null)
				throw new ArgumentNullException(nameof(text));

			_level = level;
			_text = text;
		}
	}

	/// <summary>
	/// A fixer interface
	/// </summary>
	public interface IFixer {
		/// <summary>
		/// Fixer name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Check errors
		/// </summary>
		/// <param name="peImage"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		bool Check(IPEImage peImage, out FixerMessage message);

		/// <summary>
		/// Fix errors
		/// </summary>
		/// <param name="peImage"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		bool Fix(IPEImage peImage, out FixerMessage message);
	}
}
