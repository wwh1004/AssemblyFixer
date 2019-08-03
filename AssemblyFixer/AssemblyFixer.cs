using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversalDotNetTools {
	/// <summary>
	/// Assembly fixer
	/// </summary>
	public static class AssemblyFixer {
		private static IFixer[] _fixers;

		/// <summary>
		/// Internal fixers
		/// </summary>
		public static IFixer[] Fixers {
			get {
				if (_fixers is null) {
					Type fixerType;

					fixerType = typeof(IFixer);
					_fixers = fixerType.Module.GetTypes().Where(t => t.GetInterfaces().Any(i => i.IsAssignableFrom(fixerType))).Select(t => (IFixer)Activator.CreateInstance(t)).ToArray();
				}
				return _fixers;
			}
		}

		/// <summary>
		/// Check errors
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static IDictionary<IFixer, FixerMessage> Check(FixerContext context) {
			if (context is null)
				throw new ArgumentNullException(nameof(context));

			Dictionary<IFixer, FixerMessage> messages;

			messages = new Dictionary<IFixer, FixerMessage>();
			foreach (IFixer fixer in Fixers) {
				FixerMessage message;

				if (fixer.Check(context, out message))
					messages.Add(fixer, message);
			}
			return messages;
		}

		/// <summary>
		/// Fix errors
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static IDictionary<IFixer, FixerMessage> Fix(FixerContext context) {
			if (context is null)
				throw new ArgumentNullException(nameof(context));

			Dictionary<IFixer, FixerMessage> messages;

			messages = new Dictionary<IFixer, FixerMessage>();
			foreach (IFixer fixer in Fixers) {
				FixerMessage message;

				if (fixer.Fix(context, out message))
					messages.Add(fixer, message);
			}
			return messages;
		}
	}
}
