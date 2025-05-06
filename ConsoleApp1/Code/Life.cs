public class Life
{
    public enum SpeciesType { PLANT, BACTERIA }
    public SpeciesType type;
    public double energy;
    public Life(SpeciesType type)
    {
        this.type = type;
        energy = 1;
    }
}