using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPushable
{
    void AddInstantForce(Vector2 force, float time);
}
