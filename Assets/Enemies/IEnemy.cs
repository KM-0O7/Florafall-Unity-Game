using UnityEngine;

public interface IEnemy
{
    bool Dead {  get; }
    bool GroundEnemy { get; }
    bool FlyingEnemy { get: }
}
