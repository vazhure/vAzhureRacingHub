# eXpanSIM Telemetry Bridge for vAzhureRacingHub

## Architecture

```
┌─────────────────┐     Shared Memory      ┌──────────────────┐
│   eXpanSIM      │ ─────────────────────> │  vAzhureRacingHub │
│  (C++ Plugin)   │   $eXpanSIM_Telemetry$ │   (C# Plugin)     │
└─────────────────┘                        └──────────────────┘
```

## Components

### 1. C++ Bridge Plugin (`Telemetry.dll`)
- Installed into eXpanSIM `Plugins` folder
- Receives telemetry via official `IPluginTelemetryV1` API
- Writes data to Windows shared memory `$eXpanSIM_Telemetry$`
- Built with Visual Studio 2022, C++17, x64

### 2. C# vAzhureRacingHub Plugin
- Reads shared memory and converts to `TelemetryDataSet`
- Provides motion data: Roll, Pitch, Yaw, Surge, Sway, Heave
- Dashboard data: Speed, RPM, Fuel

## Build Instructions

### C++ Bridge
1. Install eXpanSIM SDK from Steam (right-click eXpanSIM -> Properties -> DLC)
2. Set environment variable `EXPANSIM_SDK_PATH` to SDK location
3. Open `eXpanSIM_TelemetryBridge.sln` in Visual Studio 2022
4. Build Release x64
5. Output: `bin/x64/Release/Telemetry.dll`

### C# Plugin
1. Add reference to `vAzhureRacingAPI.dll`
2. Build project
3. Copy `eXpanSIM.dll` to vAzhureRacingHub plugins folder

## Installation

1. In vAzhureRacingHub, click Settings on eXpanSIM game
2. Click "Install eXpanSIM telemetry bridge"
3. Select eXpanSIM installation folder
4. Plugin copies `Telemetry.dll` and `Telemetry.json` to game's `Plugins` folder
5. Start eXpanSIM — telemetry will appear automatically

## File Structure

```
eXpanSIM/
├── Plugins/
│   ├── Telemetry.dll          # C++ bridge plugin
│   └── Telemetry.json         # Plugin config (TelemetryV1: Inherit)
```

```
vAzhureRacingHub/
├── Plugins/
│   └── eXpanSIM.dll           # C# plugin
```

## Telemetry Data Mapping

| eXpanSIM Field | vAzhureRacingHub | Notes |
|---|---|---|
| `m_PitchYawRoll` | `MotionData.Roll/Pitch/Yaw` | Normalized by π |
| `m_LinearAcceleration` | `MotionData.Surge/Sway/Heave` | Converted to g |
| `m_AngularVelocity` | `MotionData.LocalRot` | rad/s |
| `m_AngularAcceleration` | `MotionData.LocalRotAcceleration` | rad/s² |
| `m_Speed` | `CarData.Speed` | Converted to km/h |
| `m_Rpm` | `CarData.RPM` | Direct |
| `m_FuelTankReserveNorm` | `CarData.FuelLevel` | 0.0-1.0 |

## Troubleshooting

- **"Shared memory not found"** — Make sure eXpanSIM is running with C++ plugin installed
- **No telemetry** — Check that `Telemetry.json` has `"TelemetryV1": "Inherit"`
- **Access denied** — Run vAzhureRacingHub as administrator for plugin installation
