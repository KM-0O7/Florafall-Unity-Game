public interface IGrowablePlant
{
    bool IsGrown { get; }
    bool CanDie { get; }
    int spiritCost { get; }

    void Grow();

    void Die();
}