using Microsoft.Extensions.Configuration;

namespace Obelix.Api.Aspire.AppHost.Extension;
public static class ResourceBuilderExtensions
{
    public static IResourceBuilder<T> WithEnvironment<T>(
        this IResourceBuilder<T> builder, 
        string rootKey) where T : IResourceWithEnvironment
    {
        // Fetch the targeted configuration section using builder's application configuration
        var targetedConfiguration = builder.ApplicationBuilder.Configuration.GetSection(rootKey);
            
        // Prepare a dictionary to hold the configuration values
        var configValues = new Dictionary<string, string>();
        LoadConfigurationSectionsAsKeyValuePairs(targetedConfiguration.GetChildren(), configValues, rootKey);

        // For each configuration setting, call WithEnvironment to set it
        foreach (var configValue in configValues)
        {
            builder = builder.WithEnvironment(context =>
            {
                context.EnvironmentVariables[configValue.Key] = configValue.Value;
            });
        }

        return builder;
    }

    private static void LoadConfigurationSectionsAsKeyValuePairs(IEnumerable<IConfigurationSection> sections, IDictionary<string, string> keyValuePairs, string prefix)
    {
        foreach (var section in sections)
        {
            var key = string.IsNullOrEmpty(prefix) ? section.Key : $"{prefix}__{section.Key}";

            // If the section has a value, it's a leaf node, add it to the dictionary
            if (section.Value != null)
                keyValuePairs[key] = section.Value;
            else
                // If the section has children, recursively process them
                LoadConfigurationSectionsAsKeyValuePairs(section.GetChildren(), keyValuePairs, key);
        }
    }
}