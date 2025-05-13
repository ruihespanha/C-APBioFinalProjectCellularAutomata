public class Life
{
    public enum SpeciesType { PLANT, BACTERIA }
    public SpeciesType type;
    public int speed;
    public double range;
    public double replicationFactor;
    public enum energySource { FULLCHEM, MOSTCHEM, EITHER, MOSTPHOTO, FULLPHOTO }
    public double energy;
    public Life(SpeciesType type)
    {
        this.type = type;
        energy = 1;
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
    public bool canMateWith()
    {
        return true;
    }
}