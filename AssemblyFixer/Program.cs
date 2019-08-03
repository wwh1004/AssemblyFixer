using System;
using System.IO;
using System.Reflection;
using Mdlib.PE;

namespace UniversalDotNetTools {
	internal sealed class Program {
		private static void Main(string[] args) {
			if (args == null || args.Length != 1)
				return;

			string assemblyPath;

			try {
				Console.Title = GetTitle();
			}
			catch {
			}
			assemblyPath = Path.GetFullPath(args[0]);
			if (!File.Exists(assemblyPath)) {
				Console.WriteLine("File doesn't exist.");
				return;
			}
			using (IPEImage peImage = PEImageFactory.Create(assemblyPath)) {
				var a = AssemblyFixer.Fixers;
			}
		}

		private static string GetTitle() {
			string productName;
			string version;
			string copyright;
			int firstBlankIndex;
			string copyrightOwnerName;
			string copyrightYear;

			productName = GetAssemblyAttribute<AssemblyProductAttribute>().Product;
			version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			copyright = GetAssemblyAttribute<AssemblyCopyrightAttribute>().Copyright.Substring(12);
			firstBlankIndex = copyright.IndexOf(' ');
			copyrightOwnerName = copyright.Substring(firstBlankIndex + 1);
			copyrightYear = copyright.Substring(0, firstBlankIndex);
			return $"{productName} v{version} by {copyrightOwnerName} {copyrightYear}";
		}

		private static T GetAssemblyAttribute<T>() {
			return (T)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false)[0];
		}
	}
}
