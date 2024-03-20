using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface used for objects capable of being pushed
// such as players and enemies
public interface IPushable
{
    void AddInstantForce(Vector2 force, float time);
}
