using UnityEngine;
using UnityEngine.Events;

public class CollisonEvent : MonoBehaviour
{
    public UnityEvent onTriggerEnter;
    public string targetTag;
    public bool destroyAfter = false;
    public bool canBeFiredAgain = false;
    private bool m_eventFired = false;

    private void OnTiggerEnter(Collider _collider)
    {
        if (_collider.tag != targetTag)
            return;
        if (canBeFiredAgain == false && m_eventFired == true)
            return;
        onTriggerEnter.Invoke();

        m_eventFired = true;

        if (destroyAfter)
            Destroy(this);
    }
}
