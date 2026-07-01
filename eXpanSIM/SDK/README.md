| :warning: | **This repository is provided for reference only, and might be out of date. Always use the newest SDK included with the simulator to build your plugins.** | :warning: |
| --- | --- | --- |

# eXpanSIM SDK

[eXpanSIM][1] is a universal vehicle simulator supporting VR. The training mode lets you learn (eco-)driving cars and trucks.
Racing fans can test their skills in driving vehicles with realistic physics. It is also a modern research testbed for engineers,
designers and AI specialists working on autonomous vehicles. You can find it on [Steam][1]. We also offer enterprise licensing: [contact
us](mailto:contact@ravingbots.com) for details.

[1]: https://store.steampowered.com/app/1015370/eXpanSIM/

**IMPORTANT**: this is an early preview release of the plugins feature and the SDK. Everything is subject to change.

After installing eXpanSIM on Steam you can find the built SDK in the `<Steam directory>\steamapps\common\eXpanSIM\SDK` directory.
It's recommended to always use the SDK included with the simulator build. The GitHub repository is provided only for reference and might be out of date.

## Prerequisites

Building plugins with the current version of the SDK requires:

- Visual Studio 2019 16.7 or newer. Other compilers haven't yet been tested and aren't recommended at this time.
- C++17 support. C support will be released in the future.
- Windows SDK 1809 (10.0.17763.0).

## Getting started

1. Set `XSIM_SDK_PATH` environment variable to this folder.
2. Install the Visual Studio project template (see below).
3. After building your plugin, copy it to one of these directories (create it if it doesn't exist):
   1. `<installation path>\Simulator\Plugins`: this is recommended path to use for automated deployment
      - for Steam version: `<Steam library path>\steamapps\common\eXpanSIM\Simulator\Plugins`
        (stored under `installpath` value in `HKEY_LOCAL_MACHINE\WOW6432Node\SOFTWARE\Raving Bots\eXpanSIM\Steam`)
      - for enterprise version (default): `C:\Program Files\Raving Bots\eXpanSIM`
        (stored under `InstallPath` value in `HKEY_LOCAL_MACHINE\SOFTWARE\Raving Bots\eXpanSIM\Enterprise`)
   2. `Documents\eXpanSIM\Plugins` (if not redirected then `C:\Users\<username>\Documents` is the default)
4. Run eXpanSIM. Your plugin should now be loaded -- this will be noted in `Simulator.log`.

### Plugin framework

The SDK comes with a framework that makes implementing plugins easier, and takes care of some of the details for you. In the current version,
you have to implement the `xsim::GetPlugin` method, and inside call `xsim::MakePlugin<YourPluginClass>`. The plugin class specified must inherit
from `xsim::PluginV1<>` and in the template arguments specify which interfaces it wants to implement.

```cpp
#include <xsim/xsim.hpp>
#include <xsim/generated/IPluginNotifySpawnV1.hpp>

namespace my_plugin
{
	struct MyPlugin final : xsim::PluginV1<xsim::IPluginNotifySpawnV1>
	{
		MyPlugin()
		{
			// must be default-constructible
			// constructed when the plugin is loaded
		}

		~MyPlugin() noexcept
		{
			// destructed when the plugin is being unloaded gracefully
		}

		// ... override methods ...
	};
}

std::unique_ptr<xsim::IPluginWrapper> xsim::GetPlugin()
{
	return xsim::MakePlugin<my_plugin::MyPlugin>();
}
```

### Interfaces

The plugin system is now based on your plugin class inheriting from special interface classes and overriding methods in them.

Every plugin implements `IPluginV1` implicitly. Your plugin class will be created using the default constructor when the plugin is loaded,
and destructed when the plugin is gracefully unloaded. eXpanSIM doesn't guarantee that the plugins will be unloaded when it terminates, however.

Other than that you can implement any number of functionality-related interfaces. The unstable interfaces expose the latest features, but will change
in time and plugins using them will eventually stop working. The stable interfaces are versioned and never change (but might be removed in the future).
For the current list of interfaces see [INTERFACES.md](INTERFACES.md).

Your plugin will continue to work even if interfaces you implement have changed or are no longer supported. See ABI stability below.

### Dispositions

Some interfaces require you to specify when its methods are to be called. This is controlled by a JSON file deployed together with the plugin
(in previous version this was `API_v1.json` in the main configuration directory).

The file should be have the same base name as the plugin, and a `.json` extension, e.g. if your plugin is `Telemetry.dll` then the configuration is
`Telemetry.json`. The defaults look like this:

```json
{
	"DebugMode": false,
	"VehicleControllerV1": "None",
	"VehicleV1": {
		"MotorEngineV1": "None",
		"TransmissionV1": "None",
		"WheelHubV1": "None",
		"CatTrackHubV1": "None",
		"TelemetryV1": "None",
		"DashboardV1": "None"
	}
}
```

Setting `DebugMode` to `true` enables some additional diagnostic logging. For other fields the valid values are:

- `None` (the default if the field is not present) - the plugin method will not be called at all
- `Inherit` - the plugin method will be called after the default implementation is done
- `Replace` - the plugin method will be called instead of the default implementation

Note that this only applies to some of the interfaces, e.g. `IPluginNotifySpawnV1` doesn't require dispositions. See [INTERFACES.md](INTERFACES.md) for more
details.

For example a plugin that wants to receive telemetry would implement `IPluginTelemetryV1` and set `TelemetryV1` in the configuration to `Inherit`:

```cpp
{
	"VehicleV1": {
		"TelemetryV1": "Inherit"
	}
}
```

### ABI/API stability

The current ABI version is v1. Plugins built for ABI v0 will no longer work.

Your plugin will continue to work as long as ABI v1 and `IPluginV1` interface are supported, even if you implement unstable interfaces.
Interfaces are identified by a unique name and a checksum, so your methods will simply not be called on interfaces that have changed or have been removed.

You should no longer need to rebuild the plugin when new simulator builds are released. Implement only stable interfaces for the best guarantee that your
plugin will continue to work as intended with no changes.

API might change occasionally as the framework is improved, however. Expect your code to require changes to recompile with newer versions of the SDK.

### Utility functions

The SDK offers a few utilities to make developing the plugins easier:

- In `<xsim/logs.hpp>`
  - `xsim::Log(xsim::LogLevel level, std::wstring_view message, Args&&...)` - logging with formatting support (via [fmt](https://fmt.dev))
  - `xsim::Log(std::wstring_view message, Args&&...)` - for API compatibility with earlier SDK, calls `Log` with `LogLevel::Information`
- In `<xsim/math.hpp>`
  - `xsim::Vector2F`, `xsim::Vector3F`, `xsim::Vector4F` - vector classes with overloaded operators (including `xsim::Dot` and `xsim::Cross`)
  - `xsim::Quaternion` - quaternion class (no operator support yet)
- In `<xsim/utils.hpp>`
  - `xsim::PluginError` - exception class that supports `wstring` message
  - `XSIM_FAIL(message)` - unconditionally throws `xsim::PluginError` tagged with file/line/function
  - `XSIM_FAILF(message, ...)` - same as above, plus formatting support
  - `XSIM_FAIL_WINDOWS(message)` - unconditionally throws `xsim::PluginError` with a description based on `GetLastError()`, tagged with file/line/function
  - `XSIM_FAILF_WINDOWS(message, ...)` - same as above, plus formatting support
  - `XSIM_FAIL_WINDOWS_CODE(code, message)` - same as above, but with given error code instead of `GetLastError()`
  - `XSIM_FAILF_WINDOWS_CODE(code, message)` - same as above, plus formatting support
  - `XSIM_FAIL_HRESULT(hr, message)` - unconditionally throws `xsim::PluginError` with a description based on given `HRESULT`, tagged with file/line/function
  - `XSIM_FAILF_HRESULT(hr, message, ...)` - same as above, plus formatting support
  - `XSIM_CHECK_HRESULT(hr, message)` - throws `xsim::PluginError` if given `HRESULT` is `FAILED()`, tagged with file/line/function
  - `XSIM_CHECKF_HRESULT(hr, message, ...)` - same as above, plus formatting support
  - `std::wstring xsim::ToUTF16(std::string_view source)` - converts given string from UTF-8 to UTF-16
  - `std::string xsim::ToUTF8(std::wstring_view source)` - converts given string from UTF-16 to UTF-8
  - `const fs::path& xsim::GetDocumentsPath()` - returns the path to `Documents\eXpanSIM`
  - `const fs::path& xsim::GetSimulatorPath()` - returns the path to eXpanSIM executable directory
  - `int32_t xsim::GetSimulatorBuildID()` - returns the current build number (increases by 1 with every build)
  - `std::wstring_view xsim::GetSimulatorVersion()` - returns the current descriptive simulator version (in undefined format, use `GetSimulatorBuildID` for version checks)
  - `auto xsim::Protect(Fn&&)` - calls the given invokable and catches all unhandled exceptions

### Troubleshooting and logging

Any plugin-related errors can be found in the simulator log file: `%LOCALAPPDATA%\eXpanSIM\Logs\Simulator.log`. Use `xsim::Log` to append your own
messages there.

### Thread safety

Threading model will be clarified in the future. For now assume plugin-exported functions might be called at any time by any thread.

### Exceptions

Exceptions can be used within the plugins, but they must not cross the DLL boundary: the interface methods are all `noexcept`. Any unhandled exception
will terminate the simulator. You can use `xsim::Protect` to log the unhandled exceptions and continue instead:

```cpp
void SomeMethod() noexcept override
{
	xsim::Protect([&]
	{
		// code that might throw
	});
}
```

### Pointers

All pointers you receive are valid only for the duration of your exported function. Make a copy of any data you want to hold on for longer.

## Visual Studio project template

To use this template copy `VisualStudio\XSIM.VisualStudio.PluginTemplate.zip` to `Documents\Visual Studio 2019\Templates\ProjectTemplates` or
`Documents\Visual Studio 2017\Templates\ProjectTemplates`. Make sure Visual Studio is closed, or restart it after copying the file.

Afterwards you should be able to find "eXpanSIM plugin" in the Visual C++ category.

## Examples

Early examples can be found in the `Examples` folder. Built DLLs are included with the simulator SDK.

- `Telemetry` - a sample plugin that exports vehicle telemetry to a CSV file. Requires `Telemetry.json` to work properly.
