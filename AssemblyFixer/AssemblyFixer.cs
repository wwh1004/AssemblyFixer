using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalDotNetTools {
	/// <summary>
	/// Assembly fixer
	/// </summary>
	public static class AssemblyFixer {
		private static readonly IFixer[] _fixers;

		/// <summary>
		/// Internal fixers
		/// </summary>
		public static IFixer[] Fixers => _fixers;

		static AssemblyFixer() {
			Type fixerType;

			fixerType = typeof(IFixer);
			_fixers = fixerType.Module.GetTypes().Where(t => t.GetInterfaces().Any(i => i.IsAssignableFrom(fixerType))).Select(t => (IFixer)Activator.CreateInstance(t)).ToArray();
		}

	}
}
