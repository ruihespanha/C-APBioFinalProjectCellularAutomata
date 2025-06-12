public class Life
{
    private static Random rand = new Random();

    public enum SpeciesType { PLANT, BACTERIA }
    public SpeciesType type;
    public int speed = 1;
    public double range = 1;
    public double replicationFactor = 1;
    public double photosyntheticRate = 1;
    public enum energySource { FULLCHEM, MOSTCHEM, EITHER, MOSTPHOTO, FULLPHOTO }
    public double energy;
    public Life(SpeciesType type)
    {
        this.type = type;
        this.energy = rand.NextDouble() * 5f;
        this.speed = rand.Next(1, 10);
        this.range = rand.NextDouble() * 4;
        this.replicationFactor = rand.NextDouble() * 4 + 1;
        this.photosyntheticRate = rand.NextDouble() * 4;
    }
    public Life(SpeciesType type, int speed, double range, double replicationFactor, double energy, double photosyntheticRate)
    {
        this.type = type;
        this.energy = energy;
        this.speed = speed;
        this.range = range;
        this.replicationFactor = replicationFactor;
        this.photosyntheticRate = photosyntheticRate;
    }

    public double getReplicationEnergy()
    {
        if (energy > 20)
            energy = 20;
        return 0.4f * replicationFactor * energy + 0.4f * (1 / Math.Pow(speed, 2)) + 0.3f * range;
    }
    public double useReplicationEnergy()
    {
        double tempReplicationEnergy = 0.4f * replicationFactor * energy + 0.4f * (1 / Math.Pow(speed, 2)) + 0.3f * range;
        this.energy -= tempReplicationEnergy;
        if (energy < 0)
        {
            energy = 0;
            return tempReplicationEnergy;
        }

        return tempReplicationEnergy;
    }
    public double GetEnergyConsumptionRate()
    {
        if (type == SpeciesType.PLANT)
        {
            return (1 / Math.Pow(speed, 1.5)) * Math.Pow(range, 0.25);
        }
        else
            return 1;

    }
    public bool canMateWith(Life mate)
    {
        bool canMate = (Math.Abs(mate.speed - this.speed) <= 1f) && (Math.Abs(mate.range - this.range) <= 0.3f) && (Math.Abs(mate.replicationFactor - this.replicationFactor) <= 0.5);
        return canMate;
    }
    public void photosynthisize(double light)
    {
        energy += (light + photosyntheticRate) / (Math.Pow(photosyntheticRate, 2));
        if (energy > 20)
        {
            energy = 20;
        }
    }

    public Color GetColor()
    {
        return Color.FromArgb(255, speed * 10, 255, 255);
    }
    // public Life mate()
    // {
    //     speedMutation = (rand.Next(-15, 15)/15);
    //     return new Life(this.type, this.speed + , double range, double replicationFactor, double energy)
    // }
}