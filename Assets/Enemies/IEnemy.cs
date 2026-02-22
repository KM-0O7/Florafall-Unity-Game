public interface IEnemy
{
    bool Dead {  get; }
    bool GroundEnemy { get; }
    bool FlyingEnemy { get; }
    bool IsLerping { get; }

    void SetLerp(bool value);
}
