using Gitcheatsheet.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using Hjson;
using System.Diagnostics;
using YamlDotNet.RepresentationModel;

namespace Gitcheatsheet.TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var model = new CheatSheet();

            YamlTests(model);
            JsonTests(model);
            HJsonTests(model);
        }

        static void YamlTests(CheatSheet model)
        {
            // ------------ Convert model to YAML. ------------
            var serializer = new YamlDotNet.Serialization.Serializer();
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            serializer.Serialize(sw, model);
            string yaml = sb.ToString();
            File.WriteAllText("model.yaml", yaml);
            //dynamic exp = YamlUtils.ToExpando(yaml);

            // ------------ Convert YAML to Expando. ------------
            var sr = new StringReader(yaml);
            var deserializer = new YamlDotNet.Serialization.Deserializer();
            
            dynamic expando = deserializer.Deserialize<ExpandoObject>(sr);
            // expando.Leader
            // expando.Sections[0]["Title"]
            // expando.Sections[0]["SubSections"][0]["Title"]
            // expando.Sections[0]["SubSections"][0]["Cheats"][0]["Text"]

            // https://visualstudiogallery.msdn.microsoft.com/34423c06-f756-4721-8394-bc3d23b91ca7
            // https://github.com/xoofx/SharpYaml

            var stream = new YamlStream();
            var rdr = new StringReader(yaml);
            stream.Load(rdr);
            var x = (YamlMappingNode)stream.Documents[0].RootNode;
        }

        static void JsonTests(CheatSheet model)
        {
            // ------------ Convert model to JSON. ------------
            string json = JsonConvert.SerializeObject(model, Formatting.Indented);

            // ------------ Convert JSON to Expando. Works exactly as you would expect! ------------
            dynamic expando = JsonConvert.DeserializeObject<ExpandoObject>(json);

            // ------------ Convert JSON to CheatSheet. ------------
            CheatSheet cs = JsonConvert.DeserializeObject<CheatSheet>(json);

            File.WriteAllText("model.json", json);
        }

        static void HJsonTests(CheatSheet model)
        {
            // Use Newtonsoft to get some JSON. HJson does not appear to support
            // serializing models, it only works with JSON <-> HJSON conversions.
            string json = JsonConvert.SerializeObject(model, Formatting.Indented);

            // ------------ Convert JSON to HJSON. ------------
            var sr = new StringReader(json);
            JsonObject data = HjsonValue.Load(sr).Qo();
            string hjson = data.ToString(Stringify.Hjson);              // This is HJSON.
            string hplain = data.ToString(Stringify.Plain);             // This is proper JSON, one big string.
            string hformatted = data.ToString(Stringify.Formatted);     // This is proper JSON, indented.
            File.WriteAllText("model.hjson", hjson);

            // ------------ Convert HJSON to JSON. ------------
            var sr2 = new StringReader(hjson);
            JsonObject data2 = HjsonValue.Load(sr2).Qo();
            string hjson2 = data2.ToString(Stringify.Hjson);              // This is HJSON.
            string hplain2 = data2.ToString(Stringify.Plain);             // This is proper JSON, one big string.
            string hformatted2 = data2.ToString(Stringify.Formatted);     // This is proper JSON, indented.

            Debug.Assert(hjson == hjson2);
            Debug.Assert(hplain == hplain2);
            Debug.Assert(hformatted == hformatted2);
        }
    }
}
