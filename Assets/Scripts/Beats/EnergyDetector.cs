using UnityEngine;

public sealed class EnergyDetector
{
    private float[] _energyHistory = new float[DetectionSettings.MaxHistory];
    private float[] _energyAverageHistory = new float[DetectionSettings.MaxHistory];   
    private float[] _frames;
    private int _numberHistory;

    private SpectrumDataManager _spectrumDataManager;

    public EnergyDetector(SpectrumDataManager spectrumDataManager)
    {
        _spectrumDataManager = spectrumDataManager;

        _frames = new float[_spectrumDataManager.GetSampleRange()];
    }

    public void UpgradeFrameSamples() => _frames = _spectrumDataManager.GetOutputData();
    public void SetNumberHistory(int numberHistory) => _numberHistory = numberHistory;

    public bool CheckIfBeatEnergy(int circularHistory)
    {
        float instant = GetInstantEnergyLevel();

        float averageEnergy = GetAverageEnergy();

        float energyDifference = GetEnergyDifferenceToAverage(averageEnergy);
    
        float clearedDifference = (-0.0025714f * energyDifference) + 1.5142857f;

        float firstDifference = (float)Mathf.Max(instant - clearedDifference * averageEnergy, 0f);
        
        float energyHistoryAverage = GetEnergyHistoryAverage();
        
        float secondDifference = (float)Mathf.Max(firstDifference - energyHistoryAverage, 0f);

        bool detectedEnergyBeat = secondDifference > 0f && instant > 2f;

        _energyHistory[circularHistory] = instant;
        _energyAverageHistory[circularHistory] = firstDifference;
        
        return detectedEnergyBeat;
    }
    
    private float GetInstantEnergyLevel()
    {
        float level = 0f;

        for (int i = 0; i < _spectrumDataManager.GetSampleRange(); i++)
        {
            level += (_frames[i] * _frames[i]);
        }

        level /= _spectrumDataManager.GetSampleRange();

        level = Mathf.Sqrt(level) * 100f;

        return level;
    }
    
    private float GetAverageEnergy()
    {
        float averageEnergy = 0f;

        for (int i = 0; i < _numberHistory; i++)
        {
            averageEnergy += _energyHistory[i];
        }
        
        averageEnergy /= _numberHistory;

        return averageEnergy;
    }

    private float GetEnergyDifferenceToAverage(float averageEnergy)
    {
        float energyDifference = 0f;

        for (int i = 0; i < _numberHistory; i++)
        {
            energyDifference += Mathf.Pow((_energyHistory[i] - averageEnergy), 2);
        }
            
        energyDifference /= _numberHistory;

        return energyDifference;
    }

    private float GetEnergyHistoryAverage()
    {
        float energyHistoryAverage = 0f;
        int energyHistoryCount = 0;

        for (int i = 0; i < _numberHistory; i++)
        {
            if (_energyAverageHistory[i] > 0)
            {
                energyHistoryAverage += _energyAverageHistory[i];
                energyHistoryCount++;
            }
        }

        if (energyHistoryCount > 0) energyHistoryAverage /= energyHistoryCount;

        return energyHistoryAverage;
    }
}
