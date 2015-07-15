using System;
using System.IO;
using System.Collections.Generic;

namespace UnbanApp
{
	public class Utils
	{
		public static List<string> badWords ()
		{
			bool bp = false;
			List<string> bad = new List<string> ();
			using (var reader = File.OpenText ("vkban.config")) {
				string line;

				while ((line = reader.ReadLine ()) != null) {
					if (line.Equals ("[/badwords]"))
						break;
					if (bp) {
						bad.Add (line);
					}
					if (line.Equals ("[badwords]")) {
						bp = true;
					}

				}
			}
			return bad;
		}

		

		public static Dictionary<string, string> getDict ()
		{
			Dictionary<string, string> config = new Dictionary<string, string> ();
			using (var reader = File.OpenText ("vkban.config")) {
				string line;
				while ((line = reader.ReadLine ()) != null) {
					if (line.Contains ("=")) {
						string[] lines = line.Split ('=');
						config.Add (lines [0], lines [1]);
					}
				}
			}
			return config;
		}

	}
}

