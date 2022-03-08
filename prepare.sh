#!/bin/bash

# Location of the Bin64 folder of the Space Engineers game (SpaceEngineers.exe)
ln -s "/path/to/SteamLibrary/steamapps/common/SpaceEngineers/Bin64" Bin64

# Location of the local Torch instance (Torch.Server.exe and DedicatedServer64 folder)
# Torch is not supported on Linux. If your torch plugin does not use WPF, you can build it on Linux
# You only need the Torch and DedicatedServer64 assemblies as references, you don't need to run them
# You may extract Torch inside the project directory instead of symlinking
ln -s "/path/to/Torch" Torch
ln -s "/path/to/SteamLibrary/steamapps/common/SpaceEngineersDedicatedServer/DedicatedServer64" Torch/DedicatedServer64

# Enable running C# scripts
dotnet tool restore
