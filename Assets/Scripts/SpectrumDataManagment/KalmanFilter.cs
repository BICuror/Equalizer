public sealed class KalmanFilter
{
    private readonly float _a = 0.8f;
    private readonly float _h = 1f;
    private readonly float _filtrationStrength = 0.5f; // Q
    private readonly float _maxPossibleNoiseValue = 0.3f; 
    private float _p = 1f;
    private float _x;

    public KalmanFilter(float a, float h, float filtrationStrength, float maxPossibleNoiseValue)
    {
        _a = a;

        _h = h;
        
        _filtrationStrength = filtrationStrength;

        _maxPossibleNoiseValue = maxPossibleNoiseValue;
    }
    
    public float Filter(float input)
    {
        _x = _a * _x;
        _p = _a * _p * _a + _filtrationStrength;

        float k = _p * _h / (_h * _p * _h + _maxPossibleNoiseValue);
        _x = _x + k * (input - _x * _x);
        _p = (1 - k * _h) * _p;

        return _x;
    }
}
