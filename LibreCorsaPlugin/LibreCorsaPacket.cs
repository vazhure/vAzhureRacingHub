// LibreCorsaPacket.cs — класс JSON-пакета телеметрии LibreCorsa.
//
// Формат: UDP JSON, 60 Гц, от bridge (npm run ffb).
// Структура полностью открыта в исходниках LibreCorsa:
//   src/lib/input/telemetry-out.ts → buildTelemetryPacket()
//   scripts/verify-telemetry-out.ts — верификация с полным примером пакета.
//
// Wire format — плоский JSON объект. Поля с короткими именами (gLat, tyrePsi, и т.д.).
// Все числа — finite (NaN/Infinity заменяются на 0 в buildTelemetryPacket).

namespace LibreCorsaPlugin
{
    /// <summary>
    /// Пакет телеметрии LibreCorsa (TelemetryOutPacket в исходниках игры).
    /// Получается десериализацией UDP JSON от bridge.
    /// </summary>
    public class LibreCorsaTelemetryPacket
    {
        public int V { get; set; }                  // версия формата (1)
        public string Car { get; set; } = "";       // модель машины
        public string Track { get; set; } = "";     // имя трассы
        public long T { get; set; }                 // timestamp (ms)

        // --- Двигатель / коробка ---
        public double SpeedKmh { get; set; }        // км/ч
        public double Rpm { get; set; }             // об/мин
        public double MaxRpm { get; set; }          // макс. об/мин
        public int Gear { get; set; }               // передача
        public double Throttle { get; set; }        // 0..1
        public double Brake { get; set; }           // 0..1
        public double Steer { get; set; }           // -1..1 (нормализованный)
        public double Clutch { get; set; }          // 0..1

        // --- G-силы / ориентация ---
        public double GLat { get; set; }            // боковое G
        public double GLon { get; set; }            // продольное G
        public double GVert { get; set; }           // вертикальное G
        public double YawRate { get; set; }         // рад/с
        public double Pitch { get; set; }           // рад
        public double Roll { get; set; }            // рад

        // --- Топливо / энергия (F1-специфично) ---
        public double FuelKg { get; set; }          // топливо, кг
        public double ErsPct { get; set; }          // ERS battery % (F1)
        public bool Boost { get; set; }             // boost активен
        public bool Drs { get; set; }               // DRS активен (F1)
        public string AeroMode { get; set; } = "";  // режим аэро

        // --- Шины [FL, FR, RL, RR] ---
        public double[] TyreTempC { get; set; } = new double[4];    // °C
        public double[] TyrePsi { get; set; } = new double[4];      // PSI
        public double[] TyreWearPct { get; set; } = new double[4];  // 0..100
        public double[] BrakeTempC { get; set; } = new double[4];   // °C
        public double[] WheelSlip { get; set; } = new double[4];    // slip ratio
        public double[] SuspTravelMm { get; set; } = new double[4]; // mm
        public string[] Surface { get; set; } = new string[4];      // asphalt/kerb/runoff/grass

        // --- Сессия / круг ---
        public int Lap { get; set; }                // номер круга
        public long LapTimeMs { get; set; }         // тек. время круга, ms
        public long LastLapMs { get; set; }         // прошлое время круга, ms
        public long BestLapMs { get; set; }         // лучшее время (0 = нет)
        public int Position { get; set; }           // позиция в гонке
        public double TrackPos { get; set; }        // 0..1 позиция на круге

        // --- Состояние / AIDS ---
        public bool Kerb { get; set; }              // на поребрике
        public bool OffTrack { get; set; }          // вне трассы
        public bool Abs { get; set; }               // ABS активен
        public bool Tc { get; set; }                // TC активен
        public double Ffb { get; set; }             // FFB torque, -1..1
        public double OversteerPct { get; set; }    // % оверстира
        public double UndersteerPct { get; set; }   // % андерстира
    }
}

