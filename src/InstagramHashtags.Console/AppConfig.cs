namespace InstagramHashtags.Console;

using System;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Config
{
    public string ApiKey { get; set; } = string.Empty;
    public string ApiHost { get; set; } = string.Empty;
    public override string ToString()
    {
        return $"ApiKey: {ApiKey}{Environment.NewLine}ApiHost: {ApiHost}";
    }
}

public class AppConfig(string configPath)
{
    public string ConfigPath { get; set; } = configPath;

    public Config ReadConfig()
    {
        IDeserializer? deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        string yaml = File.ReadAllText(ConfigPath);
        Config config = deserializer.Deserialize<Config>(yaml);
        if (string.IsNullOrEmpty(config.ApiKey) || string.IsNullOrEmpty(config.ApiHost))
        {
            Console.WriteLine("Config file is missing required fields.");
            Environment.Exit(1);
        }
        return config;
    }
}