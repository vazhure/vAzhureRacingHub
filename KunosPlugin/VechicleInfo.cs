using System.Collections.Generic;
using System.IO;
using vAzhureRacingAPI;

namespace KunosPlugin
{
    public class VechicleInfo
    {
        public string[] vechicleName = { };
        public string[] carModel = { };
        public int[] brakeBiasOffset = { };
        public int[] maxSteeringAngle = { };
        public int[] carModelID = { };
        public int[] maxRPM = { };

        bool IsValid
        {
            get {
                return vechicleName.Length == carModel.Length &&
                  vechicleName.Length == brakeBiasOffset.Length &&
                  vechicleName.Length == maxSteeringAngle.Length &&
                  vechicleName.Length == carModelID.Length &&
                  vechicleName.Length == maxRPM.Length;
            }
        }

        readonly Dictionary<string, int> sIndexByModel = new Dictionary<string, int>();
        public int GetBiasOffset(string carModel)
        {
            if (sIndexByModel.TryGetValue(carModel, out int idx))
                return brakeBiasOffset[idx];
            return 0;
        }

        public int GetSteeringAnlge(string carModel)
        {
            if (sIndexByModel.TryGetValue(carModel, out int idx))
                return maxSteeringAngle[idx];
            return 0;
        }

        public int GetMaxRPM(string carModel, int defValue = 0)
        {
            if (sIndexByModel.TryGetValue(carModel, out int idx))
                return maxRPM[idx];
            return defValue;
        }

        public string GetVehicleName(string carModel)
        {
            if (sIndexByModel.TryGetValue(carModel, out int idx))
                return vechicleName[idx];
            return carModel;
        }

        public static VechicleInfo Load(string filename)
        {
            try
            {
                string json = File.ReadAllText(filename);
                VechicleInfo vi = ObjectSerializeHelper.DeserializeJson<VechicleInfo>(json);

                for (int t = 0; t < vi.carModel.Length; t++)
                    vi.sIndexByModel.Add(vi.carModel[t], t);

                if (vi.IsValid)
                    return vi;
            }
            catch { }
            return new VechicleInfo();
        }
    }
}