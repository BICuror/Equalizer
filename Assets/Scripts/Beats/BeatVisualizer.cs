using UnityEngine;

public sealed class BeatVisualizer : MonoBehaviour
{
    [SerializeField] private float _decreaseSpeed = 0.05f;

    [SerializeField] private float _decreaseMultyplyer = 1.2f;

    [SerializeField] private float _maxObjectScale = 20f;

    private float _currentSpeed;
    private float _value;

    public void AddValue(float value) => _value = Mathf.Clamp(_value + value, 0f, 1f); 
    
    private void FixedUpdate()
    {
        if (_value > 0)
        {
            _currentSpeed *= _decreaseMultyplyer;

            _value -= _currentSpeed;

            UpdateScale();
        }
    
    }
    private void UpdateScale()
    {
        _currentSpeed = _decreaseSpeed;

        transform.localScale = new Vector3(Mathf.Lerp(0, _maxObjectScale, _value), Mathf.Lerp(1, _maxObjectScale, _value), transform.localScale.z);
    }
}
