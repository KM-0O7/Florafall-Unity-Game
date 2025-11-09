public interface IGrowableEnemy
{
    bool IsGrown { get; }
    bool CanDie { get; }

    bool Dead { get; }

    void Grow();

    void Die();

}