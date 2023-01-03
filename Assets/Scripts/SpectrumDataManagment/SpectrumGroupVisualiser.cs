using UnityEngine;

public sealed class SpectrumGroupVisualiser : MonoBehaviour
{
    [SerializeField] private float _radius;

    [SerializeField] private float _scaleMultiplier;

    [SerializeField] private float _minimalScale = 0.001f;

    [SerializeField] private LineRenderer _lineRenderer;

    public void UpdateScale(float scale)
    {
        if (scale < _minimalScale) scale = 0f;

        Vector3[] positions = new Vector3[2];

        positions[0] = transform.localPosition.normalized * _radius;

        positions[1] = transform.localPosition.normalized * (_radius + _scaleMultiplier * scale);

        _lineRenderer.endWidth = ((_radius + _scaleMultiplier * scale) * 2 * Mathf.PI) / 8;

        _lineRenderer.SetPositions(positions);
    }
}
