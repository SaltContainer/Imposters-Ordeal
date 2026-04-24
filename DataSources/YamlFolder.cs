using SharpYaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImpostersOrdeal
{
    public abstract class YamlFolder : DataSource
    {
        protected YamlSerializerOptions serializerOptions;

        protected Dictionary<string, YamlMonoContainer> yamls;

        protected bool AreYamlsLoaded => yamls != null;

        protected Dictionary<string, YamlMonoContainer> Yamls
        {
            get
            {
                if (!AreYamlsLoaded)
                    LoadAllYamlsFromFolder();

                return yamls;
            }
        }

        public YamlFolder(string path, string rootPath) : base(path, rootPath)
        {
            serializerOptions = new YamlSerializerOptions()
            {
                MappingOrder = YamlMappingOrderPolicy.Declaration,
                DefaultIgnoreCondition = YamlIgnoreCondition.WhenWritingNull,
                Converters = [new UnityFileConverter(), new FloatConverter(), new StringConverter()],
            };
        }

        public List<KeyValuePair<string, YamlMonoContainer>> GetYamlsWhere(Func<string, YamlMonoContainer, bool> predicate)
        {
            return Yamls.Where(kvp => predicate.Invoke(kvp.Key, kvp.Value)).ToList();
        }

        public List<KeyValuePair<string, YamlMonoContainer>> GetAllYamls()
        {
            return GetYamlsWhere((p, f) => true);
        }

        protected virtual YamlMonoBehaviour DeserializeYaml(string yaml)
        {
            return YamlSerializer.Deserialize<YamlMonoBehaviour>(yaml, serializerOptions);
        }

        protected YamlMonoContainer LoadYamlFromFile(string fullPath)
        {
            try
            {
                var yamlLines = File.ReadAllLines(fullPath);
                string yaml = string.Join("\n", yamlLines.Skip(4));
                return new YamlMonoContainer() { MonoBehaviour = DeserializeYaml(yaml) };
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected void LoadAllYamlsFromFolder()
        {
            yamls = new Dictionary<string, YamlMonoContainer>();
            foreach (var file in Directory.EnumerateFiles(System.IO.Path.Combine(rootPath, path), "*.asset"))
            {
                yamls.Add(System.IO.Path.GetFileName(file), LoadYamlFromFile(file));
            }
        }

        public override void Free()
        {
            // TODO
        }

        public override void Save(string outputPath)
        {
            if (dirty)
            {
                // TODO
            }
        }
    }
}
