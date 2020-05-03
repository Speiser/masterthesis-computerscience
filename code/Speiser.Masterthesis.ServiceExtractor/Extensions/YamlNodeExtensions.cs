using YamlDotNet.RepresentationModel;

namespace Speiser.Masterthesis.ServiceExtractor.Extensions
{
    internal static class YamlNodeExtensions
    {
        public static YamlNode GetChild(this YamlMappingNode node, string key)
        {
            var scalar = new YamlScalarNode(key);
            return node.Children.TryGetValue(scalar, out var value) ? value : null;
        }
    }
}
