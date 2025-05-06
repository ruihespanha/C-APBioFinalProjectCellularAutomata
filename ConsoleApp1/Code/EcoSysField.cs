using System.Diagnostics.Eventing.Reader;

public class EcoSysField
{
    private static Random rand = new Random();
    private Life[,] field;
    private double[,] fieldLight;
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
        SimMain.bmp = new Bitmap(1000, 1000);
        Life[,] tempField = new Life[field.GetLength(0), field.GetLength(1)];
        for (int x = 0; x < field.GetLength(0); x++)
            for (int y = 0; y < field.GetLength(1); y++)
            {

                int temp = 0;
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                    {
                        if (
                            (x - 1 + i) >= 0
                            && ((y - 1 + j) >= 0)
                            && ((x - 1 + i) < field.GetLength(0))
                            && ((y - 1 + j) < field.GetLength(1))
                            )
                        {
                            if (field[x - 1 + i, y - 1 + j] != null)
                                if (field[x - 1 + i, y - 1 + j].type == Life.SpeciesType.PLANT)
                                    temp++;

                        }
                    }
                if (temp == 3)
                {
                    tempField[x, y] = new Life(Life.SpeciesType.PLANT);
                    if (field[x, y] == null)
                    {
                        SimMain.bmp.SetPixel(x, y, Color.Green);
                    }
                    else if (field[x, y].type != Life.SpeciesType.PLANT)
                    {
                        SimMain.bmp.SetPixel(x, y, Color.Green);
                    }
                }
                else if (temp == 4 && field[x, y] != null)
                {
                    if (field[x, y].type == Life.SpeciesType.PLANT)
                    {
                        tempField[x, y] = new Life(Life.SpeciesType.PLANT);
                    }
                }
                else if (field[x, y] != null)
                {
                    if (field[x, y].type == Life.SpeciesType.PLANT)
                    {
                        SimMain.bmp.SetPixel(x, y, Color.Black);
                    }
                }
            }
        field = tempField;

    }

}