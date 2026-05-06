using System;

namespace GT7Telemetry
{
    /// <summary>
    /// Salsa20 stream cipher для GT7
    /// </summary>
    public class Salsa20
    {
        private uint[] state = new uint[16];

        public void SetKey(byte[] key, byte[] nonce)
        {
            // Константы "expand 32-byte k"
            state[0] = 0x61707865;
            state[5] = 0x3320646e;
            state[10] = 0x79622d32;
            state[15] = 0x6b206574;

            // Ключ (32 байта)
            for (int i = 0; i < 8; i++)
            {
                state[1 + i] = BitConverter.ToUInt32(key, i * 4);
            }

            // Nonce (8 байт)
            state[6] = BitConverter.ToUInt32(nonce, 0);
            state[7] = BitConverter.ToUInt32(nonce, 4);

            // Counter = 0
            state[8] = 0;
            state[9] = 0;
        }

        public void Decrypt(byte[] data, int length)
        {
            byte[] block = new byte[64];
            uint[] workingState = new uint[16];

            for (int offset = 0; offset < length; offset += 64)
            {
                // Копируем состояние
                Array.Copy(state, workingState, 16);

                // 20 раундов (10 double rounds)
                for (int i = 0; i < 10; i++)
                {
                    QuarterRound(ref workingState[0], ref workingState[4], ref workingState[8], ref workingState[12]);
                    QuarterRound(ref workingState[1], ref workingState[5], ref workingState[9], ref workingState[13]);
                    QuarterRound(ref workingState[2], ref workingState[6], ref workingState[10], ref workingState[14]);
                    QuarterRound(ref workingState[3], ref workingState[7], ref workingState[11], ref workingState[15]);
                    QuarterRound(ref workingState[0], ref workingState[5], ref workingState[10], ref workingState[15]);
                    QuarterRound(ref workingState[1], ref workingState[6], ref workingState[11], ref workingState[12]);
                    QuarterRound(ref workingState[2], ref workingState[7], ref workingState[8], ref workingState[13]);
                    QuarterRound(ref workingState[3], ref workingState[4], ref workingState[9], ref workingState[14]);
                }

                // Складываем с начальным состоянием
                for (int i = 0; i < 16; i++)
                {
                    workingState[i] += state[i];
                }

                // Преобразуем в байты
                for (int i = 0; i < 16; i++)
                {
                    byte[] bytes = BitConverter.GetBytes(workingState[i]);
                    Buffer.BlockCopy(bytes, 0, block, i * 4, 4);
                }

                // XOR с данными
                int blockSize = Math.Min(64, length - offset);
                for (int i = 0; i < blockSize; i++)
                {
                    data[offset + i] ^= block[i];
                }

                // Увеличиваем counter
                state[8]++;
                if (state[8] == 0) state[9]++;
            }
        }

        private void QuarterRound(ref uint a, ref uint b, ref uint c, ref uint d)
        {
            a += b; d ^= a; d = RotateLeft(d, 16);
            c += d; b ^= c; b = RotateLeft(b, 12);
            a += b; d ^= a; d = RotateLeft(d, 8);
            c += d; b ^= c; b = RotateLeft(b, 7);
        }

        private uint RotateLeft(uint value, int bits)
        {
            return (value << bits) | (value >> (32 - bits));
        }
    }
}