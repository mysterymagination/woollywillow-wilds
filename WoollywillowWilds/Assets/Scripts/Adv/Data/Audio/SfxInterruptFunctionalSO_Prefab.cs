using UnityEngine;

namespace WildsAdv
{
    /// <summary>
    /// Interrupts an sfx stream Component with a TypeWriterSfx_PrefabClips Component playing for the given duration.
    /// </summary>
    [CreateAssetMenu(fileName = "FunctionalInterrupt_Prefab.asset", menuName = "SoundAndEffects/FunctionalInterruptSO with Prefab")]
    public class SfxInterruptFunctionalSO_Prefab : SfxInterruptFunctionalSO<TypeWriterSfx_PrefabClips>
    {

    }
}
