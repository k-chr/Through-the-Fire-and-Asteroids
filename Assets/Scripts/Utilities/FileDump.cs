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
}
