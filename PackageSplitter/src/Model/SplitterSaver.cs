using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PackageSplitter.Model
{
    class SplitterSaver
    {
        private const string SAVED_FOLDER = "ParsedPackages";
        private readonly string SAVED_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SAVED_FOLDER);

        private string _path;
        private string _sha;

        public SplitterSaver(string name, string sha)
        {
            if (!Directory.Exists(SAVED_PATH))
                Directory.CreateDirectory(SAVED_PATH);

            _sha = sha;
            _path = Path.Combine(SAVED_PATH, name + ".splitter");

        }

        public void Save(Splitter splitter)
        {
            JObject jsonObject = JObject.FromObject(splitter);
            jsonObject.Add("SHA", _sha);
            var str = JsonConvert.SerializeObject(jsonObject);
            File.WriteAllText(_path, str);
        }

        public bool Load(out Splitter obj)
        {
            bool answer = false;
            obj = null;

            if (File.Exists(_path))
            {
                string fileText = File.ReadAllText(_path);
                obj = JsonConvert.DeserializeObject<Splitter>(fileText);
                answer = JObject.Parse(fileText).Value<string>("SHA") == _sha;
            }

            return answer;
        }
    }
}
