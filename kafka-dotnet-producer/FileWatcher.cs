using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft;

public class FileWatcher {

    FileSystemWatcher _fileSystemWatcher;
    public string _path { get; set; }
    
    public FileWatcher(string path) {
        _path = path;

        _fileSystemWatcher = new FileSystemWatcher(_path);

        _fileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime;
        _fileSystemWatcher.Created += new FileSystemEventHandler(onCreated);
        _fileSystemWatcher.EnableRaisingEvents = true;
        _fileSystemWatcher.IncludeSubdirectories = false;

        Console.WriteLine(String.Format("Watching on: {0}", _path));
    }

    private void onCreated(object sender, FileSystemEventArgs e) {
        if (!e.Name.EndsWith(".xml")) {
            return;
        }

        Console.WriteLine(String.Format("File {0}: {1}", e.Name, e.ChangeType.ToString()));

        string jsonObject = String.Empty;
        
        XDocument doc = XDocument.Load(e.FullPath);

        Dictionary<string, string> dictionary = new Dictionary<string, string>(); 
        foreach (XElement elem in doc.Descendants().Where(a => !a.HasElements))
        {
            //avoid duplicates
            while (!dictionary.ContainsKey(elem.Name.LocalName))
            {
                dictionary.Add(elem.Name.LocalName, elem.Value);
            }
        }
                    
        try
        {
            string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary);
            Console.WriteLine($"Parsed {e.Name}");
            KafkaProducer.Produce(jsonStr);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}