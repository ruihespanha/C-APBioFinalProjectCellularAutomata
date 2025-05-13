using System.CodeDom.Compiler;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Serialization;
using SimplexNoise;
public class EcoSysField
{
    private static Random rand = new Random();
    private Life[,] fieldLife = new Life[SimMain.mapWidth, SimMain.mapHeight];
    private double[,] fieldLight = new double[SimMain.mapWidth, SimMain.mapHeight];
    public EcoSysField(int width, int height)
    {
        fieldLife = new Life[width, height];
        for (int x = 0; x < fieldLife.GetLength(0); x++)
            for (int y = 0; y < fieldLife.GetLength(1); y++)
                if (rand.Next(1, 4) == 1)
                    fieldLife[x, y] = new Life(Life.SpeciesType.PLANT);
    }

    public Life[,] GetField()
    {
        return fieldLife;
    }

    public void StepTime(int time)
    {
        StepLife(time);
        StepLight(time);
    }

    public void StepLife(int time)
    {
        Life[,] tempLife = new Life[(SimMain.mapWidth), (SimMain.mapHeight)];
        Life templateOrganisum = new Life(Life.SpeciesType.PLANT);
        for (int x = 0; x < (SimMain.mapWidth); x++)
        {
            for (int y = 0; y < (SimMain.mapHeight); y++)
            {
                int count = 0;
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if ((x + i >= 0) && (y + j >= 0) && (x + i < SimMain.mapWidth) && (y + j < SimMain.mapHeight))
                        {
                            if (fieldLife[x + i, y + j] != null)
                            {
                                count++;
                            }

                        }
                    }
                }
                if ((count == 3) || (fieldLight[x, y] > 100f && fieldLife[x, y] != null) || (count == 4 && (fieldLife[x, y] != null || fieldLight[x, y] > 100f)))
                {
                    tempLife[x, y] = templateOrganisum;
                }
            }
        }
        fieldLife = tempLife;
    }

    public void StepLight(int time)
    {
        for (int x = 0; x < (SimMain.mapWidth); x++)
        {
            for (int y = 0; y < (SimMain.mapHeight); y++)
            {
                float tempLightVal = 128f - Math.Abs(Noise.CalcPixel3D(x * (1000 / SimMain.mapWidth), y * (1000 / SimMain.mapHeight), time * 3, 0.005f) - 128);
                fieldLight[x, y] = tempLightVal;
                if (tempLightVal >= 100f)
                {
                    if (fieldLife[x, y] != null)
                    {
                        Graphics.FromImage(SimMain.lightbmp).FillRectangle(Brushes.LightGreen, x * (1000 / SimMain.mapWidth), y * (1000 / SimMain.mapHeight), (1000 / SimMain.mapWidth), (1000 / SimMain.mapHeight));
                    }
                    else
                    {
                        Graphics.FromImage(SimMain.lightbmp).FillRectangle(Brushes.Yellow, x * (1000 / SimMain.mapWidth), y * (1000 / SimMain.mapHeight), (1000 / SimMain.mapWidth), (1000 / SimMain.mapHeight));
                    }

                }
                else
                {
                    if (fieldLife[x, y] != null)
                    {
                        Graphics.FromImage(SimMain.lightbmp).FillRectangle(Brushes.Blue, x * (1000 / SimMain.mapWidth), y * (1000 / SimMain.mapHeight), (1000 / SimMain.mapWidth), (1000 / SimMain.mapHeight));
                    }
                    else
                    {
                        Graphics.FromImage(SimMain.lightbmp).FillRectangle(Brushes.Black, x * (1000 / SimMain.mapWidth), y * (1000 / SimMain.mapHeight), (1000 / SimMain.mapWidth), (1000 / SimMain.mapHeight));
                    }
                    //SimMain.lightbmp.SetPixel(x, y, Color.Black);
                }


            }
        }

    }

}