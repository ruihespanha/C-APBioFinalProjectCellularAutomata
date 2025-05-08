using System.CodeDom.Compiler;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Serialization;
using SimplexNoise;
public class EcoSysField
{
    private static Random rand = new Random();
    private Life[,] field;
    private double[,] fieldLight = new double[1000, 1000];
    public EcoSysField(int width, int height)
    {
        field = new Life[width, height];
        for (int x = 0; x < field.GetLength(0); x++)
            for (int y = 0; y < field.GetLength(1); y++)
                if (rand.Next(1, 4) == 1)
                    field[x, y] = new Life(Life.SpeciesType.PLANT);
    }

    public Life[,] GetField()
    {
        return field;
    }

    public void Step()
    {


    }

    public void StepLight(int time)
    {
        for (int x = 0; x < fieldLight.GetLength(0); x++)
        {
            for (int y = 0; y < fieldLight.GetLength(1); y++)
            {
                float tempLightVal = 128f - Math.Abs(Noise.CalcPixel3D(x, y, time, 0.005f) - 128);
                fieldLight[x, y] = tempLightVal;
                //Console.WriteLine(tempLightVal + " , " + (tempLightVal >= 128f));
                if (tempLightVal >= 100f)
                {
                    SimMain.lightbmp.SetPixel(x, y, Color.FromArgb(1, (int)(256f * tempLightVal / 128f), (int)(256f * tempLightVal / 128f), (int)(256f * tempLightVal / 128f)));
                }
                else
                {
                    SimMain.lightbmp.SetPixel(x, y, Color.FromArgb(1, (int)(256f * tempLightVal / 128f), (int)(256f * tempLightVal / 128f), (int)(256f * tempLightVal / 128f)));
                }

            }
        }

    }

}