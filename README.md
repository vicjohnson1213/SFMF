# SFMF

> SFMF is a framework facilitating the creation, distribution, and installation of mods for [Superflight](https://superflightgame.com/).

## Getting Started

### Prerequisites

You'll need to make sure you've got these few things set up in order to compile and run SFMF.

- Superflight (installed through Steam)
- [.NET 4.6.1 developer pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net461) (I haven't tested which other versions are compatible)
- Visual Studio

### Installing

Since SFMF references a couple of the game's .dll files, you'll need to update those references to point to your copy of Superflight. These .dll files can be found in Superflight's installation directory.

Update these references in _SFMF > SFMF_:

```
Assembly-CSharp -> ...\SuperFlight\superflight_Data\Managed\Assembly-CSharp.dll
UnityEngine.CoreModule -> ...\SuperFlight\superflight_Data\Managed\UnityEngine.CoreModule.dll
```

Once those references have been updated, you should be able run the project with no errors.

## Project Structure

- _SFMF_ - The assembly that is injected into Superflight with the core mod that searches for and loads all other mods.
- _SFMFLauncher_ - The front end application for managing SFMF and installed mods.
- _SFMFManager_ - The library responsible for managing downloading and installing mods as well as installing the framework.

## Developing a Mod

Check out [this guide](CreatingMods.md) if you'd like to create your own mod and contribute to SFMF.

## Acknowledgments

- Inspired and seeded by [@Phlarfl's](https://github.com/Phlarfl) implementation of SFMF.
