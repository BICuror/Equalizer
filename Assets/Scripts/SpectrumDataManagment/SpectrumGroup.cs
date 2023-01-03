using UnityEngine;

public sealed class SpectrumGroup : MonoBehaviour
{
    [SerializeField] private SpectrumGroupVisualiser _visualiser;

    [SerializeField] private float _defaultDecreaseSpeed = 0.05f;

    [SerializeField] private float _deacreaseSpeedMultiplier = 1.2f;

    private float _previousSpectrumValue;

    private float _deacreaseSpeed;

    public void SetSpectrumValue(float value)
    {
        if (value > _previousSpectrumValue)
        {
            _previousSpectrumValue = value;

            _deacreaseSpeed = _defaultDecreaseSpeed;
        }
        else if (value < _previousSpectrumValue)
        {
            _previousSpectrumValue -= _deacreaseSpeed;

            _deacreaseSpeed *= _deacreaseSpeedMultiplier;
        }

        _visualiser.UpdateScale(_previousSpectrumValue);
    }
}
