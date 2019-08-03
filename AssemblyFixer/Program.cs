using System;
using System.Collections.Generic;
using System.Diagnostics;
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
				IDictionary<IFixer, FixerMessage> messages;

				Console.WriteLine("Checking errors...");
				messages = AssemblyFixer.Check(peImage);
				Console.WriteLine("Checked errors:");
				foreach (KeyValuePair<IFixer, FixerMessage> fixerToMessage in messages)
					Console.WriteLine($"  -{fixerToMessage.Key.Name}: {fixerToMessage.Value.Text} ({fixerToMessage.Value.Level})");
				Console.WriteLine();
				Console.WriteLine("Fixing errors...");
				messages = AssemblyFixer.Fix(peImage);
				Console.WriteLine("Fixed errors:");
				foreach (KeyValuePair<IFixer, FixerMessage> fixerToMessage in messages)
					Console.WriteLine($"  -{fixerToMessage.Key.Name}: {fixerToMessage.Value.Text} ({fixerToMessage.Value.Level})");
				Console.WriteLine();
			}
			if (IsN00bUser() || Debugger.IsAttached) {
				Console.WriteLine("Press any key to exit...");
				try {
					Console.ReadKey(true);
				}
				catch {
				}
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

		private static bool IsN00bUser() {
			if (HasEnv("VisualStudioDir"))
				return false;
			if (HasEnv("SHELL"))
				return false;
			return HasEnv("windir") && !HasEnv("PROMPT");
		}

		private static bool HasEnv(string name) {
			foreach (object key in Environment.GetEnvironmentVariables().Keys) {
				string env;

				env = key as string;
				if (env == null)
					continue;
				if (string.Equals(env, name, StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}
	}
}
