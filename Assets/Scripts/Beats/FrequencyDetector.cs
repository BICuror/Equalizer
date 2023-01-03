using UnityEngine;

public sealed class FrequencyDetector 
{       
    private int _minimumFrequency = 60; 
    private const int _frequencyDivisions = 50; 
    private int _historyLength = 43;  
     
    private float[,] _frequencyHistory = new float[_frequencyDivisions, DetectionSettings.MaxHistory];
    private float[,] _frequencyAverageHistory = new float[_frequencyDivisions, DetectionSettings.MaxHistory];
    private bool[] _detectionHistory = new bool[_frequencyDivisions];
    private float[] _averages = new float[_frequencyDivisions];
    private float[] _lastTimeBeatTypeActivated = new float[_frequencyDivisions];
    private float[] _spectrum;

    private int _totalOctavesLength;
    private int _octaves;
    private int _linearOctaveDivisions = 3;      

    private int _numberHistory;

    private float _sampleRate;

    private SpectrumDataManager _spectrumDataManager;

    public bool CheckIfKick()
    {
        int upper = 6 >= _totalOctavesLength ? _totalOctavesLength : 6;

        return CheckIfInRange(1, upper, 2);
    }
    
    public bool CheckIfSnare()
    {
        int lower = 8 >= _totalOctavesLength ? _totalOctavesLength : 8;
        int upper = _totalOctavesLength - 5;
        int threshold = ((upper - lower) / 3);

        return CheckIfInRange(lower, upper, threshold);
    }
    
    public bool CheckIfHat()
    {
        int lower = _totalOctavesLength - 6 < 0 ? 0 : _totalOctavesLength - 6;
        int upper = _totalOctavesLength - 1;

        return CheckIfInRange(lower, upper, 1); 
    }

    public FrequencyDetector(SpectrumDataManager spectrumDataManager)
    {
        _spectrumDataManager = spectrumDataManager;
        
        _spectrum = new float[_spectrumDataManager.GetSampleRange()];
        
        _sampleRate = AudioSettings.outputSampleRate; 

        for (int i = 0; i < _frequencyDivisions; i++) 
        { 
            _lastTimeBeatTypeActivated[i] = Time.time; 
        }
        
        SetUpFrequency();
    }
    
    private void SetUpFrequency()
    {
        _octaves = 0;

        float sampleRateBuffer = _sampleRate;

        while ((sampleRateBuffer /= 2) > _minimumFrequency) 
        { 
            _octaves++; 
        }

        _totalOctavesLength = _octaves * _linearOctaveDivisions;

        for (int x = 0; x < _totalOctavesLength; x++)
        {
            for (int y = 0; y < _historyLength; y++)
            {
                _frequencyHistory[x, y] = 0f;
                _frequencyAverageHistory[x, y] = 0f;
            }
        }
    }

    public void SetNumberHistory(int numberHistory) => _numberHistory = numberHistory;
    
    public void UpdateSpectrumSamples() => _spectrum = _spectrumDataManager.GetSamples();   

    public void SetBeatFrequency(int circularHistory)
    {
        CheckOctavesForFrequency();

        for (int currentOctave = 1; currentOctave < _totalOctavesLength; currentOctave++)
        {   
            float instant = _averages[currentOctave];

            float averageFrequency = GetAverageFrequency(currentOctave);

            float frequencyDifference = GetDifferenceToAverageFrequency(currentOctave, averageFrequency);

            float clearedFrequency = (-0.0025714f * frequencyDifference + 1.5142857f);
            float firstDifference = Mathf.Max(instant - clearedFrequency * averageFrequency, 0f);

            float frequencyHistoryAverage = GetHistoryAverageFrequency(currentOctave);

            float secondDifference = (float)Mathf.Max(firstDifference - frequencyHistoryAverage, 0);

            float averageThreshold = GetAverageThreshold(currentOctave);
            float frequencyMultiplier = GetFrequencyMultiplier(currentOctave);

            bool detectedBeat = instant > frequencyMultiplier * averageFrequency && instant > averageThreshold;

            if (IsFarEnoughFromLastBeat(currentOctave) && detectedBeat == true)
            {
                _detectionHistory[currentOctave] = true;
                _lastTimeBeatTypeActivated[currentOctave] = Time.time;
            }
            else 
            {
                _detectionHistory[currentOctave] = false;
            }
            
            _frequencyHistory[currentOctave, circularHistory] = instant;
            _frequencyAverageHistory[currentOctave, circularHistory] = firstDifference;
        }
    }

    private float GetFrequencyMultiplier(int currentOctave)
    {
        if (currentOctave < 7) return 3f;
        else if(currentOctave < 20) return 4f;
        else return 4f;
    }
    
    private float GetAverageThreshold(int currentOctave)
    {
        if (currentOctave < 7) return 0.003f;
        else if (currentOctave < 20) return 0.001f;
        else return 0.001f;
    }
    
    private float GetAverageFrequency(int currentOctave)
    {
        float averageFrequency = 0f;

        for (int i = 0; i < _numberHistory; i++)
        {
            averageFrequency += _frequencyHistory[currentOctave, i];
        }

        averageFrequency /= _numberHistory;

        return averageFrequency;
    }
    
    private float GetDifferenceToAverageFrequency(int currentOctave ,float averageFrequency)
    {
        float frequencyDifference = 0f;

        for (int i = 0; i < _numberHistory; i++)
        {
            frequencyDifference += Mathf.Pow((_frequencyHistory[currentOctave, i] - averageFrequency), 2);
        }

        frequencyDifference /= _numberHistory;

        return frequencyDifference;
    }
    
    private float GetHistoryAverageFrequency(int currentOctave)
    {
        float frequencyHistoryAverage = 0f;
        int frequencyHistoryCount = 0;

        for (int i = 0; i < _numberHistory; i++)
        {
            if (_frequencyAverageHistory[currentOctave, i] > 0)
            {
                frequencyHistoryAverage += _frequencyAverageHistory[currentOctave, i];
                frequencyHistoryCount++;
            }
        }
        if (frequencyHistoryCount > 0) frequencyHistoryAverage /= frequencyHistoryCount;

        return frequencyHistoryAverage;
    }
    
    private bool IsFarEnoughFromLastBeat(int currentOctave) => Time.time - _lastTimeBeatTypeActivated[currentOctave] >= DetectionSettings.MinimalBeatSeparation;

    private void CheckOctavesForFrequency()
    {
        for (int x = 0; x < _octaves; x++)
        {
            float lowFrequency = 0f;
            if (x != 0) lowFrequency = (_sampleRate / 2) / Mathf.Pow(2, _octaves - x);

            float highFrequency = (_sampleRate / 2) / Mathf.Pow(2, _octaves - x - 1);
            
            float frequencyStep = (highFrequency - lowFrequency) / _linearOctaveDivisions;

            float currentFrequency = lowFrequency;

            for (int y = 0; y < _linearOctaveDivisions; y++)
            {
                int offset = y + x * _linearOctaveDivisions;
                float averageFrequency = GetAverage(currentFrequency, currentFrequency + frequencyStep);

                _averages[offset] = averageFrequency;
                currentFrequency += frequencyStep;
            }
        }
    }
    
    private float GetAverage(float lowFrequency, float highFrequency)
    {
        int lowBound = FrequencyToIndex(lowFrequency);
        int highBound = FrequencyToIndex(highFrequency);

        float average = 0f;

        for (int i = lowBound; i <= highBound; i++) 
        { 
            average += _spectrum[i]; 
        }
        
        average /= (highBound - lowBound + 1);
        
        return average;
    }

    private int FrequencyToIndex(float frequency)
    {
        float bandWidth = _sampleRate / _spectrumDataManager.GetSampleRange();

        if (frequency < bandWidth / 2) 
        {
            return 0;
        }
        else if (frequency > _sampleRate / 2 - bandWidth / 2) 
        {
            return (_spectrumDataManager.GetSampleRange() / 2) - 1;
        }
        else
        {
            float fraction = frequency / _sampleRate;

            int i = (int)(_spectrumDataManager.GetSampleRange() * fraction);

            return i;
        }
    }

    private bool CheckIfInRange(int low, int high, int threshold)
    {
        int number = 0;

        for (int i = low; i < high + 1; i++)
        {
            if (_detectionHistory[i] == true) number++;
        }

        return (number >= threshold);
    }
}
