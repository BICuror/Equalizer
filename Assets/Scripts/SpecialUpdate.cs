using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public sealed class SpecialUpdate : MonoBehaviour
{
    [SerializeField] private UnityEvent _updateEvent;

    private void Start() => StartCoroutine(UpdateCorutine());

    //updates 120 times per second

    private IEnumerator UpdateCorutine()
    {
        yield return new WaitForSeconds(0.0083f);

        _updateEvent.Invoke();

        StartCoroutine(UpdateCorutine());
    }
}
