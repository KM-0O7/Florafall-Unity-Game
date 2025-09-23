public interface IGrowablePlant
{
    bool IsGrown { get; }
    bool CanDie { get; }

    void Grow();

    void Die();
}