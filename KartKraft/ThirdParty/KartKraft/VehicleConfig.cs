// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace KartKraft
{
    using global::FlatBuffers;

    /// Data associated with a vehicle which doesn't change dynamically during a race. e.g. class, num gears, driver name etc
    public struct VehicleConfig : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
        public static VehicleConfig GetRootAsVehicleConfig(ByteBuffer _bb) { return GetRootAsVehicleConfig(_bb, new VehicleConfig()); }
        public static VehicleConfig GetRootAsVehicleConfig(ByteBuffer _bb, VehicleConfig obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public VehicleConfig __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public float RpmLimit { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetFloat(o + __p.bb_pos) : (float)0.0f; } }
        public float RpmMax { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetFloat(o + __p.bb_pos) : (float)0.0f; } }
        public sbyte GearMax { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetSbyte(o + __p.bb_pos) : (sbyte)0; } }

        public static Offset<KartKraft.VehicleConfig> CreateVehicleConfig(FlatBufferBuilder builder,
            float rpmLimit = 0.0f,
            float rpmMax = 0.0f,
            sbyte gearMax = 0)
        {
            builder.StartTable(3);
            VehicleConfig.AddRpmMax(builder, rpmMax);
            VehicleConfig.AddRpmLimit(builder, rpmLimit);
            VehicleConfig.AddGearMax(builder, gearMax);
            return VehicleConfig.EndVehicleConfig(builder);
        }

        public static void StartVehicleConfig(FlatBufferBuilder builder) { builder.StartTable(3); }
        public static void AddRpmLimit(FlatBufferBuilder builder, float rpmLimit) { builder.AddFloat(0, rpmLimit, 0.0f); }
        public static void AddRpmMax(FlatBufferBuilder builder, float rpmMax) { builder.AddFloat(1, rpmMax, 0.0f); }
        public static void AddGearMax(FlatBufferBuilder builder, sbyte gearMax) { builder.AddSbyte(2, gearMax, 0); }
        public static Offset<KartKraft.VehicleConfig> EndVehicleConfig(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<KartKraft.VehicleConfig>(o);
        }
    };
}
