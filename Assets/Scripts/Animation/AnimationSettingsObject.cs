using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIRA.Animations
{
    [CreateAssetMenu(menuName = "Animations/AnimationSettings", fileName = "AnimationSettings_0")]
    public class AnimationSettingsObject : ScriptableObject
    {
        [SerializeField] List<DotweenAppendSequence.AnimationsSimultanious> _animationsSequence;

        public List<DotweenAppendSequence.AnimationsSimultanious> AnimationsSequence => _animationsSequence;
    }
}

