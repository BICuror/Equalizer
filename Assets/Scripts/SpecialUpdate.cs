using UnityEngine;
using UnityEngine.Events;

public sealed class SpecialUpdate : MonoBehaviour
{
    [SerializeField] private UnityEvent _updateEvent;

    private void Start() => Invoke("UpdateSpecialLoop", 0.0083f);

    //updates 120 times per second

    private void UpdateSpecialLoop()
    {
        _updateEvent.Invoke();

        Invoke("UpdateSpecialLoop", 0.0083f);
    }
}
