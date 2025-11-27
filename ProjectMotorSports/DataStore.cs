using System;
using System.Collections.Generic;

namespace ProjectMotorSports
{
    public class DataStore
    {
        public void WriteRaceInfo(UDPRaceInfo raceInfo)
        {
            lock (m_lock)
            {
                m_raceInfo = raceInfo;
                if (m_raceInfo.m_numParticipants == 0)
                {
                    m_raceStates.Clear();
                    m_statusUpdates.Clear();
                    m_telemetry.Clear();
                }

                UpdateRaceInfoString();
            }
        }

        public void WriteRaceState(UDPParticipantRaceState raceState)
        {
            lock (m_lock)
            {
                bool stateFound = false;
                for (int i = 0; i < m_raceStates.Count; i++)
                {
                    if (m_raceStates[i].m_vehicleId == raceState.m_vehicleId)
                    {
                        m_raceStates[i] = raceState;
                        stateFound = true;
                        break;
                    }
                }

                if (!stateFound)
                {
                    m_raceStates.Add(raceState);
                }

                UpdateStatus();
            }
        }

        public void WriteTelemetry(UDPVehicleTelemetry telemetry)
        {
            lock (m_lock)
            {
                for (int i = 0; i < m_telemetry.Count; i++)
                {
                    if (m_telemetry[i].m_vehicleId == telemetry.m_vehicleId)
                    {
                        m_telemetry[i] = telemetry;
                        return;
                    }
                }

                m_telemetry.Add(telemetry);
            }
        }

        public UDPVehicleTelemetry GetTelemetryForVehicle(Int32 vehicleId)
        {
            lock (m_lock)
            {
                for (int i = 0; i < m_telemetry.Count; i++)
                {
                    UDPVehicleTelemetry t = m_telemetry[i];
                    if (t.m_vehicleId == vehicleId)
                    {
                        return t;
                    }
                }

                return null;
            }
        }

        public void Clear()
        {
            lock (m_lock)
            {
                m_raceInfo = null;
                m_raceInfoString = string.Empty;

                m_raceStates?.Clear();
                m_prevRaceStates?.Clear();
                m_statusUpdates?.Clear();
                m_telemetry?.Clear();
            }

            WriteTimestamp();
        }

        public void UpdateRaceInfoString()
        {
            lock (m_lock)
            {
                if (m_raceInfo != null)
                {
                    m_raceInfoString = m_raceInfo.ToString();
                }
                else
                {
                    m_raceInfoString = string.Empty;
                }
            }
        }


        public List<UDPParticipantRaceState> GetLeaderboard()
        {
            lock (m_lock)
            {
                List<UDPParticipantRaceState> leaderboard = new List<UDPParticipantRaceState>();
                CopyRaceStates(m_raceStates, out leaderboard);
                leaderboard.Sort();
                return leaderboard;
            }
        }

        public void UpdateStatus()
        {
            if (m_prevRaceStates == null)
            {
                CopyRaceStates(m_raceStates, out m_prevRaceStates);
            }
            else
            {
                List<string> newUpdates = GetLapAndSectorUpdates();
                if (newUpdates != null)
                {
                    m_statusUpdates.AddRange(newUpdates);
                }

                CopyRaceStates(m_raceStates, out m_prevRaceStates);
            }
        }

        public void CopyRaceStates(List<UDPParticipantRaceState> from, out List<UDPParticipantRaceState> to)
        {
            to = new List<UDPParticipantRaceState>(from.Count);

            for (int i = 0; i < from.Count; i++)
            {
                to.Add(new UDPParticipantRaceState(from[i]));
            }
        }

        private List<string> GetLapAndSectorUpdates()
        {
            if (m_raceInfo == null || m_raceInfo.m_state == UDPRaceSessionState.Inactive)
            {
                return null;
            }

            if (m_raceStates.Count != m_prevRaceStates.Count)
            {
                return null;
            }

            List<string> updates = new List<string>();
            for (int i = 0; i < m_raceStates.Count; i++)
            {
                UDPParticipantRaceState curr = m_raceStates[i];
                UDPParticipantRaceState prev = m_prevRaceStates[i];
            }

            return updates;
        }

        public void WriteTimestamp()
        {
            lock (m_lock)
            {
                m_writeTimestamp = DateTime.Now;
            }
        }

        public int TimeSinceLastWrite()
        {
            lock (m_lock)
            {
                return (DateTime.Now - m_writeTimestamp).Seconds;
            }
        }

        private readonly object m_lock = new object();

        private UDPRaceInfo m_raceInfo = null;
        private readonly List<UDPParticipantRaceState> m_raceStates = new List<UDPParticipantRaceState>();
        private readonly List<UDPVehicleTelemetry> m_telemetry = new List<UDPVehicleTelemetry>();

        private List<UDPParticipantRaceState> m_prevRaceStates = null;
        private readonly List<UDPParticipantRaceState> m_leaderboardEntries = new List<UDPParticipantRaceState>();

        private DateTime m_writeTimestamp = DateTime.Now;
        public string m_raceInfoString = string.Empty;
        public List<string> m_statusUpdates = new List<string>();
    }
}
