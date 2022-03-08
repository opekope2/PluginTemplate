#!/usr/bin/env -S dotnet script
string[] args = Args.ToArray();
if (args.Length < 2)
{
    Console.WriteLine("Usage:\tdeploy.csx <DeployType> <AssemblyFile.dll>");
    return 0;
}

Console.WriteLine();
//Console.WriteLine("Parameters: " + string.Join(" ", Args));

string deployType = args[0], assemblyFile = args[1];

switch (deployType)
{
    case "Client":
        DeployClient();
        return 0;
    case "Dedicated":
        DeployDedicated();
        return 0;
    case "Torch":
        DeployTorch();
        return 0;
    case "TorchHarmony":
        DeployTorchHarmony();
        return 0;
    default:
        Console.WriteLine($"Unknown DeployType '{deployType}'");
        return 1;
}

void DeployClient()
{
    Console.WriteLine("Deploying CLIENT plugin binary:");

    Deploy(Path.Combine("..", "..", "..", "..", "Bin64", "Plugins", "Local"), assemblyFile, "0Harmony.dll");
}

void DeployDedicated()
{
    Console.WriteLine("Deploying DEDICATED SERVER plugin binary:");

    Deploy(Path.Combine("..", "..", "..", "..", "Torch", "DedicatedServer64", "Plugins"), assemblyFile, "0Harmony.dll");
}

void DeployTorch()
{
    Console.WriteLine("Deploying TORCH SERVER plugin binary:");

    Deploy(Path.Combine("..", "..", "..", "..", "Torch", "Plugins"), assemblyFile, "manifest.xml");
}

void DeployTorchHarmony()
{
    Console.WriteLine("Deploying TORCH SERVER plugin binary:");

    Deploy(Path.Combine("..", "..", "..", "..", "Torch", "Plugins"), assemblyFile, "0Harmony.dll", "manifest.xml");
}

void Deploy(string targetDir, params string[] files)
{
    if (!Directory.Exists(targetDir))
    {
        Directory.CreateDirectory(targetDir);
    }

    foreach (string file in files)
    {
        string fileName = Path.GetFileName(file);
        Console.WriteLine($"Copying {file} into {Path.GetFullPath(targetDir)}");
        File.Copy(file, Path.Combine(targetDir, fileName), overwrite: true);
    }
}