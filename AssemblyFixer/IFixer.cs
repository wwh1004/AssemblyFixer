using System;

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
		/// An empty message
		/// </summary>
		public static readonly FixerMessage None = new FixerMessage(FixerLevel.None, string.Empty);

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
		/// <param name="context"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		bool Check(FixerContext context, out FixerMessage message);

		/// <summary>
		/// Fix errors
		/// </summary>
		/// <param name="context"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		bool Fix(FixerContext context, out FixerMessage message);
	}

	internal static class Extensions {
		public static void Ensure(this ref FixerLevel level, FixerLevel minLevel) {
			level = level < minLevel ? minLevel : level;
		}
	}
}
