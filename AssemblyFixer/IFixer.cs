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
		/// <param name="level"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		bool Check(IPEImage peImage, out FixerLevel level, out string message);

		/// <summary>
		/// Fix errors
		/// </summary>
		/// <param name="peImage"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		bool Fix(IPEImage peImage, out string message);
	}
}
