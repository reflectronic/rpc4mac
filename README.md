# Rpc4Mac

Discord Rich Presence on Visual Studio for Mac

<img width="312" alt="image" src="https://user-images.githubusercontent.com/27514983/79609575-fde7e580-80c4-11ea-94f6-72ed5d0cb320.png">

### Installation Instructions

```bash
msbuild -p:Configuration=Release -p:InstallAddin=true
```

### Configuration

You can configure this extension in the preferences menu, under the "Discord Rich Presence" section.

For custom Application IDs, Rpc4Mac uses `csharp`, `fsharp`, `unknown` and `vs` as image keys.