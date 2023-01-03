using UnityEngine;

public sealed class SpectrumGroupsManager : MonoBehaviour
{
    [Header("SpectrumGroupsSettins")]

    [SerializeField] private int _amountOfGroups = 8;

    [SerializeField] private SpectrumGroup[] _spectrumGroups; 

    private float[] _frequencyGroups;

    [SerializeField] private float[] _frequencyGroupHighBorders;

    [Header("FilteringSettings")]

    [SerializeField] private bool _useFiltering;

    [Range(0f, 1f)] [SerializeField] private float _a = 0.8f;

    [Range(0f, 1f)] [SerializeField] private float _h = 1f;

    [Range(0f, 1f)] [SerializeField] private float _filtrationStrength = 0.5f;

    [Range(0f, 1f)] [SerializeField] private float _maxPossibleNoiseValue = 0.3f;

    private KalmanFilter[] _filters;

    [Header("Links")]

    [SerializeField] private SpectrumDataManager _spectrumDataManager;

    private void Start()
    {
        _frequencyGroups = new float[_amountOfGroups];

        CreateKalmanFilters();
    }

    public void CreateKalmanFilters()
    {
        _filters = new KalmanFilter[_amountOfGroups];

        for (int i = 0; i <_amountOfGroups; i++)
        {
            _filters[i] = new KalmanFilter(_a, _h, _filtrationStrength, _maxPossibleNoiseValue);
        }
    }

    private void LateUpdate()
    {
        CreateFrequencyGroups();

        for (int i = 0; i < _amountOfGroups; i++)
        {
            _spectrumGroups[i].SetSpectrumValue(_frequencyGroups[i]);
        }
    }

    private void CreateFrequencyGroups()
    {
        int count = 0;

        float[] samples = _spectrumDataManager.GetSamples();

        int sampleRange = _spectrumDataManager.GetSampleRange();

        float frequencySetp = AudioSettings.outputSampleRate / sampleRange; 

        float currentFrequency = 0f;

        int sampleIndex = 0;

        for (int x = 0; x < _amountOfGroups; x++)
        {
            float groupAverage = 0f;
            int sampleCount = 0;

            while (currentFrequency < _frequencyGroupHighBorders[x] && sampleIndex < samples.Length)
            {
                groupAverage += samples[sampleIndex++] * (sampleIndex + 1);

                currentFrequency += frequencySetp;

                sampleCount++;
            }

            groupAverage /= sampleCount;

            if (_useFiltering == true) _frequencyGroups[x] = _filters[x].Filter(groupAverage);
            else _frequencyGroups[x] = groupAverage;
        }
    }
}

