using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CafeFieldCharacter : MonoBehaviour
{
    [SerializeField] private RotationConstraint visualConstraint;
    public void Init(Transform lookAt = null)
    {
        //if(lookAt != null)
        //{
        //    ConstraintSource cameraSource = new();
        //    cameraSource.sourceTransform = lookAt;
        //    cameraSource.weight = 1.0f;
        //    visualConstraint.AddSource(cameraSource);
        //}
    }
}
