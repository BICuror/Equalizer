using UnityEngine;
using UnityEngine.Events;

public static class DetectionSettings
{
    public readonly static int MaxHistory = 500;  
    public readonly static float MinimalBeatSeparation = 0.05f; 
}

public sealed class BeatDetection : MonoBehaviour
{     
    private int _historyLength = 43;                                       
    private int _numberHistory;
    private int _circularHistory;
    private int _currenHistoryLength;

    [Header("Visualisers")]
    [SerializeField] private UnityEvent _hatBeatHit;
    [SerializeField] private UnityEvent _snareBeatHit;
    [SerializeField] private UnityEvent _kickBeatHit;
    [SerializeField] private UnityEvent _energyBeatHit;

    [Header("Links")]
    [SerializeField] private SpectrumDataManager _spectrumDataManager;
    private FrequencyDetector _frequencyDetector;
    private EnergyDetector _energyDetector;

    private void Awake()
    {    
        _currenHistoryLength = _historyLength;
        _numberHistory = 0;
        _circularHistory = 0;

        _frequencyDetector = new FrequencyDetector(_spectrumDataManager);
        _energyDetector = new EnergyDetector(_spectrumDataManager);
    }

	private void Update()
    {
        CheckForBeat(); 

        _circularHistory++;
        _circularHistory %= _currenHistoryLength;
        
        _numberHistory = (_numberHistory < _historyLength) ? _numberHistory + 2 : _numberHistory;
    }

    private void CheckForBeat()
    {
        _energyDetector.UpgradeFrameSamples();
        _frequencyDetector.UpdateSpectrumSamples();

        _energyDetector.SetNumberHistory(_numberHistory);

        _frequencyDetector.SetBeatFrequency(_circularHistory);
        _frequencyDetector.SetNumberHistory(_numberHistory);

        if (_energyDetector.CheckIfBeatEnergy(_circularHistory)) _energyBeatHit.Invoke(); 
        if (_frequencyDetector.CheckIfKick()) _kickBeatHit.Invoke();                   
        if (_frequencyDetector.CheckIfSnare()) _snareBeatHit.Invoke();             
        if (_frequencyDetector.CheckIfHat()) _hatBeatHit.Invoke();                     
    }
}
