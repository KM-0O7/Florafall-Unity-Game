public interface IGrowableEnemy
{
    bool IsGrown { get; }
    bool CanDie { get; }

    bool Dead { get; }
    int spiritCost { get; }

    void Grow();

    void Die();

}