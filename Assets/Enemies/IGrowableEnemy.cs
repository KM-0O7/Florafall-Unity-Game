public interface IGrowableEnemy
{
    bool IsGrown { get; }
    bool CanDie { get; }

    void Grow();

    void Die();
}