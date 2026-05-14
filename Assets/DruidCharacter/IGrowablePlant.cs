public interface IGrowablePlant
{
    bool IsGrown { get; }
    bool CanDie { get; }
    int spiritCost { get; }
    bool CanGrow { get; }

    bool WaterGrown { get; }
    void setWaterGrow(bool value);
    void Grow();

    void Die();
}