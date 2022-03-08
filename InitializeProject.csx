#!/usr/bin/env -S dotnet script
// Replaces project GUIDs and renames the solution

using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

bool DRY_RUN = false;
string PT_PROJECT_NAME = "^([A-Z][a-z_0-9]+)+$";
Regex RX_PROJECT_NAME = new Regex(PT_PROJECT_NAME);

string[] PROJECT_NAMES = {
    "ClientPlugin",
    "TorchPlugin",
    "DedicatedPlugin",
    "Shared",
};


string GenerateGuid()
{
    return Guid.NewGuid().ToString();
}


void ReplaceTextInFile(Dictionary<string, string> replacements, string path)
{
    Encoding encoding = Encoding.UTF8;

    string text = File.ReadAllText(path, encoding);

    string original = text;

    foreach (var replace in replacements)
    {
        text = text.Replace(replace.Key, replace.Value);
    }

    if (DRY_RUN || text == original)
    {
        return;
    }

    File.WriteAllText(path, text, encoding);
}


string InputPluginName()
{
    string pluginName;
    while (true)
    {
        Console.Write("Name of the plugin (in CapitalizedWords format): ");
        pluginName = Console.ReadLine();
        if (string.IsNullOrEmpty(pluginName))
        {
            break;
        }

        if (RX_PROJECT_NAME.IsMatch(pluginName))
        {
            break;
        }

        Console.WriteLine("Invalid plugin name, it must match regexp: " + PT_PROJECT_NAME);
    }

    return pluginName;
}


if (!File.Exists("PluginTemplate.sln"))
{
    Console.WriteLine("Run this script only once from the working copy (solution) folder");
    return -1;
}

string pluginName = InputPluginName();
if (string.IsNullOrWhiteSpace(pluginName))
{
    return -2;
}

string torchGuid = GenerateGuid();
Dictionary<string, string> replacements = new Dictionary<string, string>
{
    {"PluginTemplate", pluginName},
    {"A061FC6C-713E-42CD-B413-151AC8A5074C", GenerateGuid().ToUpper()},
    {"FFB7FCA3-B168-43F4-8DBF-6247C0D331C8", GenerateGuid().ToUpper()},
    {"C5784FE0-CF0A-4870-9DEF-7BEA8B64C01A", GenerateGuid().ToUpper()},
    {"C889318F-9835-4814-B26E-979242CAEB0C", torchGuid.ToUpper()},
    {"c889318f-9835-4814-b26e-979242caeb0c", torchGuid},
};

IEnumerator<string> IterPaths()
{
    Console.WriteLine("Solution:");
    yield return "PluginTemplate.sln";

    if (File.Exists("PluginTemplate.sln.DotSettings.user"))
    {
        yield return "PluginTemplate.sln.DotSettings.user";
    }

    foreach (string projectName in PROJECT_NAMES)
    {
        Console.WriteLine();

        Console.WriteLine($"{projectName}:");

        foreach (string file in Directory.GetFiles(projectName, "*", SearchOption.AllDirectories))
        {
            if (new[] { @"/bin/", @"/obj/", @"\bin\", @"\obj\" }.Any(s => file.Contains(s)))
            {
                continue;
            }

            string ext = Path.GetExtension(file);

            if (new[] { ".xml", ".xaml", ".cs", ".sln", ".csproj", ".shproj" }.Contains(ext))
            {
                yield return file;
            }
        }
    }
}

class EnumeratorWrapper : IEnumerable<string>
{
    private IEnumerator<string> enumerator;

    public EnumeratorWrapper(IEnumerator<string> enumerator)
    {
        this.enumerator = enumerator;
    }

    public IEnumerator<string> GetEnumerator()
    {
        return enumerator;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return enumerator;
    }
}

List<string> renameFiles = new List<string>();
foreach (string path in new EnumeratorWrapper(IterPaths()))
{
    Console.WriteLine($"  {path}");

    ReplaceTextInFile(replacements, path);
    if (path.Contains("PluginTemplate"))
    {
        renameFiles.Add(path);
    }

}
if (!DRY_RUN)
{
    foreach (string path in renameFiles)
    {
        string dirPath = Path.GetDirectoryName(path);
        string dstName = Path.GetFileName(path).Replace("PluginTemplate", pluginName);
        string dstPath = Path.Join(dirPath, dstName);
        File.Move(path, dstPath);
    }
}

Console.WriteLine("Done.");
