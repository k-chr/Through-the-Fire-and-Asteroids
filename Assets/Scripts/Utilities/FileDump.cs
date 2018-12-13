using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class FileDump {

	public static FileStream CreateFile(string path) {
		return new FileStream(path, FileMode.OpenOrCreate);
	}

	public static void SerializeToFile(string contents, string path) {
		FileStream fs = CreateFile(path);
		if (fs == null) throw new FileNotFoundException();
		using (StreamWriter writer = new StreamWriter(fs)) {
			writer.WriteLine(contents);
		}
		fs.Close();
	}

	public static uint CountFileLines(string path) {
		if (string.IsNullOrEmpty(path) || !File.Exists(path)) throw new FileNotFoundException();
		uint counter = 0;
		FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
		using (StreamReader reader = new StreamReader(fs)) {
			while (!reader.EndOfStream) {
				++counter;
				reader.ReadLine();
			}
		}
		fs.Close();
		return counter;
	}

	public static string[] LoadPlayerNames() { 
		string[] names = null;
		using (StreamReader reader = new StreamReader(@"JsonFiles/PlayerNames.json")) {
			names= JsonConvert.DeserializeObject<string[]>(reader.ReadToEnd());
		}
		return names;
	}

	public static void DumpPlayerNames(string[] names) {
		FileStream fs = FileDump.CreateFile(@"JsonFiles/PlayerNames.json");
		using (StreamWriter writer = new StreamWriter(fs)) {
			writer.WriteLine(JsonConvert.SerializeObject(names, Formatting.Indented));
		}
		fs.Close();
	}
}
