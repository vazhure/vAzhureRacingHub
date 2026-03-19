using System;

namespace MotionPlatform3
{
    public class WashoutFilter
    {
        // Filter parameters
        private readonly double wn; // Natural frequency, rad/s
        private readonly double zeta; // Damping ratio
        private readonly double T; // Sample time, seconds
        private double a1, a2, b0, b1, b2; // Filter coefficients

        // Internal state variables
        private double previousOutput1;
        private double previousOutput2;

        public WashoutFilter(double wn, double zeta, double T)
        {
            this.wn = wn;
            this.zeta = zeta;
            this.T = T;
            ComputeCoefficients();
            // Initialize state to zero
            previousOutput1 = 0.0;
            previousOutput2 = 0.0;
        }

        private void ComputeCoefficients()
        {
            double omega = 2.0 * Math.PI * wn;
            double omegaT = omega * T;
            double tanHalfOmegaT = Math.Tan(omegaT / 2.0);

            // Bilinear transform for second-order high-pass filter
            b0 = (1.0 + tanHalfOmegaT) / (1.0 + tanHalfOmegaT + (2.0 * zeta * omegaT) * tanHalfOmegaT);
            b1 = -2.0 * (1.0 - tanHalfOmegaT) / (1.0 + tanHalfOmegaT + (2.0 * zeta * omegaT) * tanHalfOmegaT);
            b2 = (1.0 - tanHalfOmegaT) / (1.0 + tanHalfOmegaT + (2.0 * zeta * omegaT) * tanHalfOmegaT);
            a1 = -2.0 * Math.Cos(omegaT);
            a2 = 1.0;
        }

        // Apply the filter to the input signal
        public double ApplyFilter(double input)
        {
            double output = b0 * input + b1 * previousOutput1 + b2 * previousOutput2
                           - a1 * previousOutput1 - a2 * previousOutput2;

            // Update the internal state for the next iteration
            previousOutput2 = previousOutput1;
            previousOutput1 = output;

            return output;
        }
    }
}