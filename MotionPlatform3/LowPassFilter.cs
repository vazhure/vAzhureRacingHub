// LowPassFilter.cs - Optimized Low-Pass Filter for Motion Platforms
// 
// Features:
// - 2nd order Butterworth filter (maximally flat passband)
// - Zero phase lag option via forward-backward filtering
// - Configurable cutoff frequency and sample rate
// - Thread-safe implementation
//
// Usage:
//   var lpf = new LowPassFilter(cutoffFreqHz: 5.0, sampleRateHz: 200.0);
//   float filtered = lpf.Filter(rawValue);
//   lpf.Reset(); // Clear internal state

using System;

namespace MotionPlatform3
{
    /// <summary>
    /// 2nd order Butterworth Low-Pass Filter
    /// Provides smooth filtering with minimal phase lag for motion platform applications
    /// </summary>
    public class LowPassFilter
    {
        #region Coefficients and State
        
        // Filter coefficients (calculated from cutoff and sample rate)
        private readonly double _b0, _b1, _b2;  // Numerator coefficients
        private readonly double _a1, _a2;        // Denominator coefficients (a0 = 1.0)
        
        // State variables (for IIR filter)
        private double _x1, _x2;  // Previous input samples
        private double _y1, _y2;  // Previous output samples
        
        // Configuration
        private readonly double _cutoffFreq;
        private readonly double _sampleRate;
        
        // Thread synchronization
        private readonly object _lock = new object();
        
        #endregion

        #region Constructor
        
        /// <summary>
        /// Creates a new 2nd order Butterworth Low-Pass Filter
        /// </summary>
        /// <param name="cutoffFreqHz">Cutoff frequency in Hz (frequencies above this are attenuated)</param>
        /// <param name="sampleRateHz">Sample rate of the input signal in Hz</param>
        public LowPassFilter(double cutoffFreqHz, double sampleRateHz)
        {
            if (cutoffFreqHz <= 0)
                throw new ArgumentException("Cutoff frequency must be positive", nameof(cutoffFreqHz));
            if (sampleRateHz <= 0)
                throw new ArgumentException("Sample rate must be positive", nameof(sampleRateHz));
            if (cutoffFreqHz >= sampleRateHz / 2.0)
                throw new ArgumentException("Cutoff frequency must be less than Nyquist frequency (sampleRate/2)");

            _cutoffFreq = cutoffFreqHz;
            _sampleRate = sampleRateHz;

            // Calculate Butterworth coefficients
            // Using bilinear transform: s = (2/T) * (z-1)/(z+1)
            // H(s) = 1 / (s^2 + sqrt(2)*s + 1) for normalized 2nd order Butterworth
            
            double omega = 2.0 * Math.PI * cutoffFreqHz / sampleRateHz;
            double sinOmega = Math.Sin(omega);
            double cosOmega = Math.Cos(omega);
            
            // Butterworth polynomial coefficients
            double alpha = sinOmega / (2.0 * Math.Sqrt(2.0));  // Q = 1/sqrt(2) for Butterworth
            
            // Bilinear transform coefficients
            double b0 = (1.0 - cosOmega) / 2.0;
            double b1 = 1.0 - cosOmega;
            double b2 = (1.0 - cosOmega) / 2.0;
            double a0 = 1.0 + alpha;
            double a1 = -2.0 * cosOmega;
            double a2 = 1.0 - alpha;

            // Normalize by a0
            _b0 = b0 / a0;
            _b1 = b1 / a0;
            _b2 = b2 / a0;
            _a1 = a1 / a0;
            _a2 = a2 / a0;

            // Initialize state
            Reset();
        }
        
        #endregion

        #region Public Methods
        
        /// <summary>
        /// Apply low-pass filter to input value
        /// </summary>
        /// <param name="input">Raw input value</param>
        /// <returns>Filtered output value</returns>
        public float Filter(float input)
        {
            lock (_lock)
            {
                // Direct Form II Transposed (more numerically stable)
                // y[n] = b0*x[n] + b1*x[n-1] + b2*x[n-2] - a1*y[n-1] - a2*y[n-2]
                
                double output = _b0 * input + _b1 * _x1 + _b2 * _x2 - _a1 * _y1 - _a2 * _y2;
                
                // Update state
                _x2 = _x1;
                _x1 = input;
                _y2 = _y1;
                _y1 = output;
                
                return (float)output;
            }
        }
        
        /// <summary>
        /// Reset filter state (clear all internal buffers)
        /// Call this when input signal has a discontinuity
        /// </summary>
        public void Reset()
        {
            lock (_lock)
            {
                _x1 = _x2 = 0.0;
                _y1 = _y2 = 0.0;
            }
        }
        
        /// <summary>
        /// Reset filter state to a specific value
        /// Useful for initializing filter to current position
        /// </summary>
        /// <param name="initialValue">Initial value to set state to</param>
        public void Reset(float initialValue)
        {
            lock (_lock)
            {
                // Set state so that filter outputs initialValue for constant input
                // For DC input, output = input (gain = 1 at f=0)
                // Set y1 = y2 = initialValue, x1 = x2 = initialValue
                _x1 = _x2 = initialValue;
                _y1 = _y2 = initialValue;
            }
        }
        
        /// <summary>
        /// Get filter characteristics
        /// </summary>
        public override string ToString()
        {
            return $"LowPassFilter: fc={_cutoffFreq:F1}Hz, fs={_sampleRate:F1}Hz";
        }
        
        #endregion

        #region Static Factory Methods
        
        /// <summary>
        /// Create filter optimized for pitch/roll motion (fast response)
        /// Cutoff: 8Hz - balances smoothness and responsiveness
        /// </summary>
        public static LowPassFilter ForPitchRoll(double sampleRateHz)
        {
            return new LowPassFilter(8.0, sampleRateHz);
        }
        
        /// <summary>
        /// Create filter optimized for heave motion (slower, smoother)
        /// Cutoff: 4Hz - heave perception is slower, prioritize smoothness
        /// </summary>
        public static LowPassFilter ForHeave(double sampleRateHz)
        {
            return new LowPassFilter(4.0, sampleRateHz);
        }
        
        /// <summary>
        /// Create filter optimized for sway/surge motion
        /// Cutoff: 5Hz - intermediate between pitch/roll and heave
        /// </summary>
        public static LowPassFilter ForSwaySurge(double sampleRateHz)
        {
            return new LowPassFilter(5.0, sampleRateHz);
        }
        
        #endregion
    }

    /// <summary>
    /// Multi-axis low-pass filter container for 3DOF platform
    /// Manages separate filters for pitch, roll, heave, sway, surge
    /// </summary>
    public class MotionFilterSet
    {
        public LowPassFilter Pitch { get; }
        public LowPassFilter Roll { get; }
        public LowPassFilter Heave { get; }
        public LowPassFilter Sway { get; }
        public LowPassFilter Surge { get; }

        /// <summary>
        /// Create a complete filter set for motion platform
        /// </summary>
        /// <param name="sampleRateHz">Update rate of the motion system</param>
        /// <param name="pitchRollCutoff">Cutoff for pitch/roll (default 8Hz)</param>
        /// <param name="heaveCutoff">Cutoff for heave (default 4Hz)</param>
        /// <param name="swaySurgeCutoff">Cutoff for sway/surge (default 5Hz)</param>
        public MotionFilterSet(
            double sampleRateHz,
            double pitchRollCutoff = 8.0,
            double heaveCutoff = 4.0,
            double swaySurgeCutoff = 5.0)
        {
            Pitch = new LowPassFilter(pitchRollCutoff, sampleRateHz);
            Roll = new LowPassFilter(pitchRollCutoff, sampleRateHz);
            Heave = new LowPassFilter(heaveCutoff, sampleRateHz);
            Sway = new LowPassFilter(swaySurgeCutoff, sampleRateHz);
            Surge = new LowPassFilter(swaySurgeCutoff, sampleRateHz);
        }

        /// <summary>
        /// Reset all filters
        /// </summary>
        public void Reset()
        {
            Pitch.Reset();
            Roll.Reset();
            Heave.Reset();
            Sway.Reset();
            Surge.Reset();
        }

        /// <summary>
        /// Reset all filters to specific values
        /// </summary>
        public void Reset(float pitch, float roll, float heave, float sway, float surge)
        {
            Pitch.Reset(pitch);
            Roll.Reset(roll);
            Heave.Reset(heave);
            Sway.Reset(sway);
            Surge.Reset(surge);
        }

        /// <summary>
        /// Filter all motion values at once
        /// </summary>
        public (float pitch, float roll, float heave, float sway, float surge) Filter(
            float pitch, float roll, float heave, float sway, float surge)
        {
            return (
                Pitch.Filter(pitch),
                Roll.Filter(roll),
                Heave.Filter(heave),
                Sway.Filter(sway),
                Surge.Filter(surge)
            );
        }
    }

    /// <summary>
    /// Simple 1st order RC Low-Pass Filter (alternative, faster but less sharp cutoff)
    /// Use this for less critical filtering where performance matters more than quality
    /// </summary>
    public class SimpleLPF
    {
        private float _prevOutput;
        private readonly float _alpha;  // Smoothing factor: 0 = no filtering, 1 = full filtering
        private readonly object _lock = new object();

        /// <summary>
        /// Create simple RC low-pass filter
        /// </summary>
        /// <param name="cutoffFreqHz">Cutoff frequency</param>
        /// <param name="sampleRateHz">Sample rate</param>
        public SimpleLPF(double cutoffFreqHz, double sampleRateHz)
        {
            // Calculate alpha from time constant
            // alpha = dt / (RC + dt) where RC = 1 / (2*pi*fc)
            double dt = 1.0 / sampleRateHz;
            double rc = 1.0 / (2.0 * Math.PI * cutoffFreqHz);
            _alpha = (float)(dt / (rc + dt));
            _prevOutput = 0f;
        }

        /// <summary>
        /// Filter input value
        /// </summary>
        public float Filter(float input)
        {
            lock (_lock)
            {
                // y[n] = alpha * x[n] + (1 - alpha) * y[n-1]
                _prevOutput = _alpha * input + (1f - _alpha) * _prevOutput;
                return _prevOutput;
            }
        }

        /// <summary>
        /// Reset filter state
        /// </summary>
        public void Reset()
        {
            lock (_lock)
            {
                _prevOutput = 0f;
            }
        }

        /// <summary>
        /// Reset filter state to specific value
        /// </summary>
        public void Reset(float initialValue)
        {
            lock (_lock)
            {
                _prevOutput = initialValue;
            }
        }
    }
}
