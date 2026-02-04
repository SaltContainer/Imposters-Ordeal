using System;
using System.IO;
using System.Linq;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    /// <summary>
    /// Base class for YAML-based data sources.
    /// </summary>
    public class YAMLDataSource : DataSource
    {
        private static Serializer serializer;
        private YAMLMonoBehaviour loadedData;
        private string unityPrefix;

        /// <summary>
        /// Gets the YAML serializer with custom serializers registered.
        /// </summary>
        protected static Serializer Serializer
        {
            get
            {
                if (serializer == null)
                {
                    var settings = new SerializerSettings
                    {
                        SortKeyForMapping = false,
                        IgnoreNulls = true,
                        EmitAlias = false,
                        IndentLess = true,
                    };
                    settings.RegisterSerializer(typeof(YAMLUnityFile), new YAMLUnityFileSerializer());
                    settings.RegisterSerializer(typeof(float), new YAMLFloatSerializer());
                    settings.RegisterSerializer(typeof(string), new YAMLStringSerializer());
                    serializer = new Serializer(settings);
                }
                return serializer;
            }
        }

        /// <summary>
        /// The loaded MonoBehaviour data.
        /// </summary>
        public YAMLMonoBehaviour Data
        {
            get
            {
                if (loadedData == null)
                    LoadData();
                return loadedData;
            }
            set
            {
                loadedData = value;
                SetModified();
            }
        }

        public YAMLDataSource(string path, string rootPath) : base(path, rootPath)
        {
        }

        /// <summary>
        /// Loads and parses the YAML file.
        /// </summary>
        protected virtual void LoadData()
        {
            string fullPath = System.IO.Path.Combine(rootPath, path);
            if (!File.Exists(fullPath))
            {
                loadedData = null;
                return;
            }

            var yamlLines = File.ReadAllLines(fullPath);

            // Unity YAML files have a 4-line header (3 lines + blank)
            // Preserve it for writing back
            if (yamlLines.Length >= 4)
            {
                unityPrefix = string.Join("\n", yamlLines.Take(3)) + "\n";
                string yaml = string.Join("\n", yamlLines.Skip(4));

                try
                {
                    var container = Serializer.Deserialize<YAMLMonoContainer>(yaml);
                    loadedData = container?.MonoBehaviour;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to parse YAML: {ex.Message}");
                    loadedData = null;
                }
            }
            else
            {
                // Try to parse as raw YAML
                string yaml = string.Join("\n", yamlLines);
                try
                {
                    var container = Serializer.Deserialize<YAMLMonoContainer>(yaml);
                    loadedData = container?.MonoBehaviour;
                }
                catch
                {
                    loadedData = null;
                }
            }
        }

        public override void Free()
        {
            loadedData = null;
        }

        public override void Save(string outputPath)
        {
            if (!dirty || loadedData == null)
                return;

            string fullPath = System.IO.Path.Combine(outputPath, path);

            // Ensure directory exists
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath));

            // Serialize back to YAML
            var container = new YAMLMonoContainer { MonoBehaviour = loadedData };
            string yaml = Serializer.Serialize(container);

            // Write with Unity header if we have one
            using var writer = new StreamWriter(fullPath);
            if (!string.IsNullOrEmpty(unityPrefix))
            {
                writer.Write(unityPrefix);
                writer.WriteLine();
            }
            writer.Write(yaml);
        }
    }

    /// <summary>
    /// Generic YAML data source for specific MonoBehaviour types.
    /// </summary>
    /// <typeparam name="T">The specific MonoBehaviour type.</typeparam>
    public class YAMLDataSource<T> : YAMLDataSource where T : YAMLMonoBehaviour
    {
        public new T Data
        {
            get => base.Data as T;
            set => base.Data = value;
        }

        public YAMLDataSource(string path, string rootPath) : base(path, rootPath)
        {
        }
    }
}
