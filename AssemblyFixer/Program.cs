using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Mdlib.PE;

namespace UniversalDotNetTools {
	internal sealed class Program {
		private static void Main(string[] args) {
			if (args is null || args.Length != 1)
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
				FixerContext context;
				IDictionary<IFixer, FixerMessage> messages;

				context = new FixerContext(peImage);
				Console.WriteLine("If it is dll, enter Y, otherwise enter N.");
				do {
					string userInput;

					userInput = Console.ReadLine().Trim().ToUpperInvariant();
					if (userInput == "Y") {
						context.IsDll = true;
						break;
					}
					else if (userInput == "N") {
						context.IsDll = false;
						break;
					}
					else
						Console.WriteLine("Invalid input");
				} while (true);
				Console.WriteLine();
				Console.WriteLine("Checking...");
				messages = AssemblyFixer.Check(context);
				Console.WriteLine("Checked errors:");
				Console.WriteLine();
				foreach (KeyValuePair<IFixer, FixerMessage> fixerToMessage in messages) {
					Console.WriteLine(fixerToMessage.Key.Name + ":");
					Console.WriteLine($"Level: {fixerToMessage.Value.Level}");
					Console.WriteLine("Message:");
					Console.WriteLine(fixerToMessage.Value.Text);
					Console.WriteLine();
				}
				Console.WriteLine();
				Console.WriteLine("Fixing...");
				messages = AssemblyFixer.Fix(context);
				Console.WriteLine("Fixed errors:");
				Console.WriteLine();
				foreach (KeyValuePair<IFixer, FixerMessage> fixerToMessage in messages) {
					Console.WriteLine(fixerToMessage.Key.Name + ":");
					Console.WriteLine($"Level: {fixerToMessage.Value.Level}");
					Console.WriteLine("Message:");
					Console.WriteLine(fixerToMessage.Value.Text);
					Console.WriteLine();
				}
				Console.WriteLine();
				Console.WriteLine("If the assembly still does NOT run, may be you should rebuild it!!!");
				Console.WriteLine();
				if (messages.Count != 0) {
					byte[] peImageData;
					string newAssemblyPath;

					peImageData = new byte[peImage.Length];
					Marshal.Copy(peImage.RawData, peImageData, 0, peImageData.Length);
					newAssemblyPath = PathInsertPostfix(assemblyPath, ".fix");
					Console.WriteLine("Saving: " + newAssemblyPath);
					File.WriteAllBytes(newAssemblyPath, peImageData);
					Console.WriteLine("Finished");
					Console.WriteLine();
				}
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

		private static string PathInsertPostfix(string path, string postfix) {
			return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + postfix + Path.GetExtension(path));
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
				if (env is null)
					continue;
				if (string.Equals(env, name, StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}
	}
}
