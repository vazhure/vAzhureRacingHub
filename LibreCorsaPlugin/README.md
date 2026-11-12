# LibreCorsaPlugin

Плагин телеметрии **[LibreCorsa](https://github.com/KhazP/librecorsa)** для
[vAzhureRacingHub](https://github.com/vazhure/vAzhureRacingHub).

LibreCorsa — open-source браузерный симулятор (500 Гц физика, F1 2026 / GT3 /
прототип / дорожная машина). Плагин получает телеметрию через локальный FFB
bridge (UDP JSON, 60 Гц) и нормализует в `TelemetryDataSet` vAzhureRacingHub.

---

## Архитектура

```
Браузер (librecorsa.org) ──WebSocket──► Bridge (npm run ffb, :18500)
                                            │
                                     UDP JSON 60 Гц
                                            │
                                            ▼
                                  LibreCorsaPlugin (слушает :20790)
                                            │
                                            ▼
                                  vAzhureRacingHub → дашборды / LED / motion
```

Bridge — это Node.js процесс из репозитория LibreCorsa (`scripts/ffb-bridge.ts`).
Он принимает WebSocket от браузера и пересылает телеметрию как UDP JSON на порт,
указанный игрой.

---

## Установка и настройка

### 1. Bridge (LibreCorsa)

```powershell
git clone https://github.com/KhazP/librecorsa
cd librecorsa
npm install
```

Если играете на **librecorsa.org** (хостинг), задайте origin перед запуском bridge:

**PowerShell:**
```powershell
$env:FFB_ALLOW_ORIGIN="https://librecorsa.org"
npm run ffb
```

**cmd:**
```cmd
set "FFB_ALLOW_ORIGIN=https://librecorsa.org" && npm run ffb
```

Если играете **локально** (`npm run dev`), origin не нужен — просто `npm run ffb`.

Bridge должен быть запущен **до** открытия игры в браузере.

### 2. Порт телеметрии в игре

В настройках LibreCorsa (в браузере) укажите порт телеметрии = **20790**
(или другой — тогда измените порт в настройках плагина).

### 3. Плагин vAzhureRacingHub

Соберите `LibreCorsaPlugin.dll` и положите в папку плагинов хаба.
Настройки (порт, URL) — правый клик по игре LibreCorsa → Настройки.

---

## Формат телеметрии

UDP JSON, 60 Гц, плоский объект. Полная структура — в
`LibreCorsaPacket.cs` (соответствует `TelemetryOutPacket` в исходниках игры:
`src/lib/input/telemetry-out.ts`, верификация в `scripts/verify-telemetry-out.ts`).

| Поле wire | Тип | Назначение |
| --- | --- | --- |
| `speedKmh` | double | Скорость, км/ч |
| `rpm`, `maxRpm` | double | Обороты двигателя / макс |
| `gear` | int | Передача |
| `throttle`, `brake`, `steer`, `clutch` | double | Педали/руль (0..1, steer -1..1) |
| `gLat`, `gLon`, `gVert` | double | G-силы (3 оси) |
| `yawRate`, `pitch`, `roll` | double | Ориентация (рад) |
| `fuelKg` | double | Топливо, кг |
| `tyreTempC[4]` | double[] | Темп. шин °C [FL,FR,RL,RR] |
| `tyrePsi[4]` | double[] | Давление шин PSI |
| `tyreWearPct[4]` | double[] | Износ шин % |
| `brakeTempC[4]` | double[] | Темп. тормозов °C |
| `wheelSlip[4]` | double[] | Slip ratio |
| `suspTravelMm[4]` | double[] | Ход подвески, мм |
| `surface[4]` | string[] | Поверхность (asphalt/kerb/runoff/grass) |
| `lap`, `lapTimeMs`, `lastLapMs`, `bestLapMs` | int/long | Круг и времена (мс) |
| `position` | int | Позиция в гонке |
| `trackPos` | double | 0..1 позиция на круге |
| `abs`, `tc` | bool | ABS / TC активны |
| `ffb` | double | FFB torque (-1..1) |
| `oversteerPct`, `understeerPct` | double | % овер/андерстира |
| `ersPct`, `boost`, `drs`, `aeroMode` | mixed | F1-специфичные (ERS/DRS/aero) |

---

## Маппинг полей → vAzhureRacingHub

### AMCarData
| Поле API | Источник | Примечание |
| --- | --- | --- |
| `Speed` | `speedKmh` | уже км/ч |
| `RPM` | `rpm` | |
| `MaxRPM` | `maxRpm` | |
| `Gear` | `gear` | |
| `Steering` | `steer × 450` | -1..1 → градусы (приблизительно) |
| `Throttle/Brake/Clutch` | `throttle/brake/clutch` | 0..1 |
| `FuelLevel` | `fuelKg / 0.74` | кг → литры |
| `Position` | `position` | |
| `Lap` | `lap` | |

### AMTireData[FL,FR,RL,RR]
| Поле | Источник | Конверсия |
| --- | --- | --- |
| `Pressure` | `tyrePsi` | PSI → кПа (×6.89476) |
| `Wear` | `tyreWearPct` | 0..100% → 0..1 |
| `Temperature[L,M,R]` | `tyreTempC` | °C (без L/M/R — одно значение) |
| `BrakeTemperature` | `brakeTempC` | °C |

### AMMotionData
| Поле | Источник | Примечание |
| --- | --- | --- |
| `Sway` | `gLat` | G (если платформа слабая — ×9.81 для м/с²) |
| `Surge` | `-gLon` | торможение → отрицательный surge |
| `Heave` | `gVert` | |
| `Pitch` | `pitch / π` | нормализованные рад |
| `Roll` | `roll / π` | |
| `LocalRot` | `[yawRate, 0, 0]` | рад/с |

### UserData["LibreCorsa.*"]
Сырые поля: `Car`, `Track`, `ErsPct`, `Boost`, `Drs`, `AeroMode`, `FuelKg`,
`TrackPos`, `Kerb`, `OffTrack`, `Ffb`, `OversteerPct`, `UndersteerPct`,
`WheelSlip`, `SuspTravelMm`, `Surface`.

---

## Сборка

```
msbuild LibreCorsaPlugin.csproj /p:VAzhureApiDllPath=C:\path\to\api\bin\Release /p:Configuration=Release
```

Нужен .NET Framework 4.7.2 и `System.Text.Json` NuGet (как в vAzhureRacingAPI).

---

## Решение проблем

| Симптом | Причина / решение |
| --- | --- |
| Нет данных в хабе | Bridge не запущен. Проверьте: `npm run ffb` в консоли, нет ли ошибок. Bridge должен быть запущен ДО открытия игры |
| Bridge не запускается | Не выполнен `npm install` или нет `tsx`. Клонируйте репо и установите зависимости |
| Игра на librecorsa.org не подключается к bridge | Не задан `FFB_ALLOW_ORIGIN`. См. PowerShell/cmd команды выше |
| Порт занят | Другой процесс слушает 20790. Измените порт в настройках плагина И в настройках игры |
| Данные есть, но G-силы слабые | LibreCorsa отдаёт G (×9.81 = м/с²). Умножьте `gLat/gLon/gVert` на 9.81 в `ApplyToTelemetry` |
