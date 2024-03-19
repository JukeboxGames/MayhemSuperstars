using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="ScriptableObjects/Player/PlayerEvents")]
public class SO_PlayerEvents : ScriptableObject
{
    public UnityEvent<int> event_PlayerHealthChanged;
    public UnityEvent event_PlayerDead;
}
