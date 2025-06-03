using System;

namespace NoiseFilters
{
    public class NoiseFilter
    {
        private readonly float[] samples;
        private readonly int maxSampleCount;
        private float maxInputDelta = float.MaxValue; //maximum input delta between last and new sample.
        private int liveSampleCount;
        private int currSample = 0;
        private float currPrediction = 0.0f;
        private float currvalue = 0;

        //higher _maxSampleCount = more smoothing
        public NoiseFilter(int _maxSampleCount, float _maxInputDelta = float.MaxValue)
        {
            maxSampleCount = Math.Max(1, _maxSampleCount);
            samples = new float[maxSampleCount];
            maxInputDelta = _maxInputDelta;
        }

        public float MaxInputDelta
        {
            get => maxInputDelta;
            set => maxInputDelta = value;
        }

        public float Filter(float sample)
        {
            //early out
            if (maxSampleCount == 1)
                return sample;

            if (maxInputDelta != float.MaxValue && liveSampleCount > 0)
            {
                float sampleDiff = sample - samples[currSample];
                float absSampleDiff = Math.Abs(sampleDiff);
                if (absSampleDiff > maxInputDelta)
                {
                    float direction = sampleDiff / absSampleDiff;

                    sample = samples[currSample] + (direction * maxInputDelta);
                }
            }

            currSample = (currSample + 1) >= maxSampleCount ? 0 : currSample + 1;
            samples[currSample] = sample;

            liveSampleCount = (liveSampleCount + 1) >= maxSampleCount ? maxSampleCount : liveSampleCount + 1;

            //average all samples
            float total = 0.0f;
            for (int i = 0; i < liveSampleCount; ++i)
            {
                total += samples[i];
            }

            float prediction = 0.0f;
            int sampleCounter = 0;
            for (int i = currSample; sampleCounter < liveSampleCount; i--)
            {
                int nextSample = currSample - 1 < 0 ? liveSampleCount - 1 : currSample - 1;
                prediction += samples[currSample] - samples[nextSample];
                sampleCounter++;
            }

            currPrediction = prediction = ((currPrediction + (prediction / sampleCounter)) * 0.5f) * 0.5f;
            float smoothedValue = total / liveSampleCount;

            return currvalue = smoothedValue + prediction;
        }

        public static implicit operator float(NoiseFilter value)
        {
            return value.currvalue;
        }

        public void Reset()
        {
            currSample = 0;
            liveSampleCount = 0;
        }
    }

    public class KalmanFilter
    {
        private readonly float A, H, Q, R;
        private float P, x;

        public KalmanFilter(float A, float H, float Q, float R, float initial_P, float initial_x)
        {
            this.A = A;
            this.H = H;
            this.Q = Q;
            this.R = R;
            P = initial_P;
            x = initial_x;
        }

        public float Filter(float input)
        {
            // time update - prediction
            x = A * x;
            P = A * P * A + Q;

            // measurement update - correction
            float K = P * H / (H * P * H + R);
            x += K * (input - H * x);
            P = (1 - K * H) * P;

            return x;
        }

        public static implicit operator float(KalmanFilter value)
        {
            return value.x;
        }
    }

    public class NestedSmooth
    {
        readonly NoiseFilter filter;
        readonly NestedSmooth nest;
        public NestedSmooth(int nestCount, int _maxSampleCount, float _maxInputDelta = float.MaxValue)
        {
            if (nestCount != 0)
                nest = new NestedSmooth(nestCount - 1, _maxSampleCount, _maxInputDelta);

            filter = new NoiseFilter(_maxSampleCount, _maxInputDelta);
        }

        public float Filter(float value)
        {
            if (nest != null)
                return nest.Filter(filter.Filter(value));

            return filter.Filter(value);
        }
    }
}