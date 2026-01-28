namespace GamePhysics
{
    [System.Serializable]
    public struct PID
    {
        public float pFactor, iFactor, dFactor;

        private float integral;
        private float lastError;

        public PID(float pFactor, float iFactor, float dFactor)
        {
            this.pFactor = pFactor;
            this.iFactor = iFactor;
            this.dFactor = dFactor;
            integral = 0;
            lastError = 0;
        }

        public void Reset()
        {
            integral = 0;
            lastError = 0;
        }

        public float Update(float setpoint, float actual, float timeFrame)
        {
            var present = setpoint - actual;
            integral += present * timeFrame;
            var deriv = (present - lastError) / timeFrame;
            lastError = present;
            return present * pFactor + integral * iFactor + deriv * dFactor;
        }
    }
}