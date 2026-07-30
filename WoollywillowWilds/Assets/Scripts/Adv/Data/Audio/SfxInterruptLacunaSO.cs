using UnityEngine;

[CreateAssetMenu(fileName = "LacunaInterrupt.asset", menuName = "SoundAndEffects/LacunaInterruptSO")]
public class SfxInterruptLacunaSO : ScriptableObject
{
    [Header("Audio Options")]
    [SerializeField] public float delay = 0.0F;
    [SerializeField] public float duration = 0.0F;
    [SerializeField] public float variance = 0.0F;
}