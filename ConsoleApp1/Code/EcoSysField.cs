using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;
using System.Xml.Schema;
using SimplexNoise;
public class EcoSysField
{
    private static Random rand = new Random();
    public double speciationTolerance = 4;
    public static double ambiantRadiation = 1;
    private static double[,] convArr = {{0f, 0.02f, 0.03f, 0.02f, 0f},
                                        {0.02f, 0.1f, 0.15f,  0.1f, 0.02f},
                                        {0.03f, 0.15f, 0.5f,  0.15f, 0.03f},
                                        {0.02f, 0.1f, 0.15f,  0.1f, 0.02f},
                                        {0f, 0.02f, 0.03f, 0.02f, 0f}};
    private static double[,] replicationArr =  {{0f, 0.02f, 0.03f, 0.02f, 0f},
                                                {0.02, 0.2f, 0.35f,  0.2f, 0.02f},
                                                {0.03f, 0.35f, 0.0f,  0.35f, 0.03f},
                                                {0.02f, 0.2f, 0.35f,  0.2f, 0.02f},
                                                {0f, 0.02f, 0.03f, 0.02f, 0f}};
    private Life[,] fieldLife = new Life[SimMain.mapWidth, SimMain.mapHeight];
    private double[,] fieldLight = new double[SimMain.mapWidth, SimMain.mapHeight];
    public EcoSysField(int width, int height)
    {
        fieldLife = new Life[width, height];
        for (int x = 0; x < fieldLife.GetLength(0); x++)
            for (int y = 0; y < fieldLife.GetLength(1); y++)
                if (rand.Next(1, 400) == 1)
                    fieldLife[x, y] = new Life(Life.SpeciesType.PLANT);
    }


    public Life[,] GetField()
    {
        return fieldLife;
    }


    public void StepTime(int time)
    {
        StepLifeUpdated(time);
        //if (time % 10 == 1)
        StepLight(time);
    }


    public void StepLifeUpdated(int time)
    {
        Life[,] tempLife = new Life[(SimMain.mapWidth), (SimMain.mapHeight)];
        Life templateOrganisum = new Life(Life.SpeciesType.PLANT);


        double totalCount = 0;
        double partialCount = 0;
        int[,] binLife = new int[(SimMain.mapWidth), (SimMain.mapHeight)];
        int[,] speedLife = new int[(SimMain.mapWidth), (SimMain.mapHeight)];
        double[,] rangeLife = new double[(SimMain.mapWidth), (SimMain.mapHeight)];
        double[,] repFactLife = new double[(SimMain.mapWidth), (SimMain.mapHeight)];
        double[,] photoSythLife = new double[(SimMain.mapWidth), (SimMain.mapHeight)];
        double[,] energy = new double[(SimMain.mapWidth), (SimMain.mapHeight)];


        for (int x = 0; x < (SimMain.mapWidth); x++)
            for (int y = 0; y < (SimMain.mapHeight); y++)
            {
                if (fieldLife[x, y] != null)
                {
                    binLife[x, y] = 1;
                    speedLife[x, y] = fieldLife[x, y].speed;
                    rangeLife[x, y] = fieldLife[x, y].range;
                    repFactLife[x, y] = fieldLife[x, y].replicationFactor;
                    energy[x, y] = fieldLife[x, y].useReplicationEnergy();
                    fieldLife[x, y].photosynthisize(fieldLight[x, y]);
                    photoSythLife[x, y] = fieldLife[x, y].photosyntheticRate;
                }
                else
                {
                    binLife[x, y] = 0;
                    speedLife[x, y] = 0;
                    rangeLife[x, y] = 0;
                    repFactLife[x, y] = 0;
                    energy[x, y] = 0;
                    photoSythLife[x, y] = 0;
                }
            }
        double avgSpeed = 0;
        double avgRepFact = 0;
        double avgRange = 0;
        double avgPhytoshynth = 0;
        double avgEnergy = 0;
        double orgcount = 0;

        int hit1 = 0;
        int hit2 = 0;
        int hit3 = 0;
        for (int x = 0; x < (SimMain.mapWidth); x++)
        {
            for (int y = 0; y < (SimMain.mapHeight); y++)
            {
                try
                {
                    double count = 0;
                    double replicationTotal = 0;
                    double activationEnergy = 0;
                    double tempSpeed = 0;
                    double tempRange = 0;
                    double tempReplicationFactor = 0;
                    double tempPhotosynth = 0;
                    double tempEnergy = 0;


                    for (int i = -2; i < 3; i++)
                    {
                        for (int j = -2; j < 3; j++)
                        {
                            if ((x + i >= 0) && (y + j >= 0) && (x + i < SimMain.mapWidth) && (y + j < SimMain.mapHeight))
                            {
                                if (fieldLife[x + i, y + j] != null)
                                {
                                    //count += convArr[i + 2, j + 2];
                                    count += convArr[i + 2, j + 2] * repFactLife[x + i, y + j] * energy[x + i, y + j];
                                    //replicationTotal += repFactLife[x + i, y + j];
                                    activationEnergy += energy[x + i, y + j] * replicationArr[i + 2, j + 2];
                                    // tempEnergy += energy[x + i, y + j];
                                    tempSpeed += convArr[i + 2, j + 2] * speedLife[x + i, y + j] * repFactLife[x + i, y + j] * energy[x + i, y + j];
                                    tempRange += convArr[i + 2, j + 2] * rangeLife[x + i, y + j] * repFactLife[x + i, y + j] * energy[x + i, y + j];
                                    tempReplicationFactor += convArr[i + 2, j + 2] * repFactLife[x + i, y + j] * repFactLife[x + i, y + j] * energy[x + i, y + j];
                                    tempPhotosynth += convArr[i + 2, j + 2] * photoSythLife[x + i, y + j] * repFactLife[x + i, y + j] * energy[x + i, y + j];
                                }


                            }
                        }
                    }
                    if (count != 0)
                    {
                        totalCount++;
                        tempSpeed = (tempSpeed / count);
                        tempRange = (tempRange / count);
                        tempReplicationFactor = (tempReplicationFactor / count);
                        tempPhotosynth = (tempPhotosynth / count);






                        count = 0;
                        // replicationTotal = 0;
                        // tempEnergy = 0;
                        double newSpeed = 0;
                        double newRange = 0;
                        double newReplicationFactor = 0;
                        double replicationEnergy = 0;
                        double newPhotosynth = 0;
                        try
                        {
                            for (int i = -2; i < 3; i++)
                            {
                                for (int j = -2; j < 3; j++)
                                {
                                    if ((x + i >= 0) && (y + j >= 0) && (x + i < SimMain.mapWidth) && (y + j < SimMain.mapHeight))
                                    {
                                        if (Math.Abs(speedLife[x + i, y + j] - tempSpeed) + Math.Abs(rangeLife[x + i, y + j] - tempRange) + Math.Abs(repFactLife[x + i, y + j] - tempReplicationFactor) <= speciationTolerance)
                                        {
                                            // count += convArr[i + 2, j + 2];
                                            count += convArr[i + 2, j + 2] * repFactLife[x + i, y + j] * energy[x + i, y + j];
                                            tempEnergy += energy[x + i, y + j];
                                            replicationTotal += repFactLife[x + i, y + j];
                                            newPhotosynth += convArr[i + 2, j + 2] * photoSythLife[x + i, y + j] * repFactLife[x + i, y + j] * energy[x + i, y + j];
                                            newSpeed += convArr[i + 2, j + 2] * speedLife[x + i, y + j] * repFactLife[x + i, y + j] * energy[x + i, y + j];
                                            newRange += convArr[i + 2, j + 2] * rangeLife[x + i, y + j] * repFactLife[x + i, y + j] * energy[x + i, y + j];
                                            newReplicationFactor += convArr[i + 2, j + 2] * repFactLife[x + i, y + j] * repFactLife[x + i, y + j] * energy[x + i, y + j];
                                            replicationEnergy += replicationArr[i + 2, j + 2] * energy[x + i, y + j];
                                        }
                                    }
                                }
                            }
                            newSpeed = newSpeed / count;
                            newRange = newRange / count;
                            newReplicationFactor = newReplicationFactor / count;
                            replicationEnergy = replicationEnergy / count;
                            newPhotosynth = newPhotosynth / count;
                            double adjasent = 0;
                            for (int i = -2; i < 3; i++)
                                for (int j = -2; j < 3; j++)
                                {


                                    if (((x + i >= 0) && (y + j >= 0) && (x + i < SimMain.mapWidth) && (y + j < SimMain.mapHeight)) && Math.Abs(speedLife[x + i, y + j] - newSpeed) + Math.Abs(rangeLife[x + i, y + j] - newRange) + Math.Abs(repFactLife[x + i, y + j] - newReplicationFactor) <= speciationTolerance)
                                    {
                                        adjasent += replicationArr[2 + i, 2 + j];
                                    }
                                }


                            if (SimMain.instance.time % (int)(newSpeed + 0.5f) == 0)
                            {


                                if (fieldLife[x, y] != null && fieldLife[x, y].energy <= 0)
                                {
                                    tempLife[x, y] = null;
                                }
                                else if (adjasent * replicationEnergy * newReplicationFactor > energy[x, y] + 0.1 * rand.NextDouble())
                                {
                                    hit1++;
                                    tempLife[x, y] = new Life(Life.SpeciesType.PLANT, (int)(newSpeed + 0.5f) + (int)(rand.NextDouble() * (1 + 0.1 * ambiantRadiation)), newRange * (4 + Math.Pow(rand.NextDouble(), 2) * 2 * ambiantRadiation) / 5, newReplicationFactor * (4 + Math.Pow(rand.NextDouble(), 2) * 2 * ambiantRadiation) / 5, energy[x, y] * (4 + Math.Pow(rand.NextDouble(), 2) * 2 * ambiantRadiation) / 5, newPhotosynth * (4 + Math.Pow(rand.NextDouble(), 2) * 2 * ambiantRadiation) / 5);
                                }
                                else if (adjasent * 2 > repFactLife[x, y])
                                {
                                    //Console.WriteLine(adjasent + " * " + replicationEnergy + " * " + newReplicationFactor + " > " + energy[x, y] + " + 3");
                                    hit2++;
                                    tempLife[x, y] = null;
                                }
                                else
                                {
                                    hit3++;
                                    tempLife[x, y] = fieldLife[x, y];
                                }
                            }
                            else
                            {
                                tempLife[x, y] = fieldLife[x, y];
                            }


                            if (tempLife[x, y] != null)
                            {
                                orgcount++;
                                avgSpeed += tempLife[x, y].speed;
                                avgRepFact += tempLife[x, y].replicationFactor;
                                avgRange += tempLife[x, y].range;
                                avgPhytoshynth += tempLife[x, y].photosyntheticRate;
                                avgEnergy += tempLife[x, y].energy;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        //replicationEnergy += energy[];
                        //relication energy > cell energy;

                    }
                    else
                    {
                        //Console.WriteLine("divide by zero" + count + ", " + replicationTotal + ", " + tempEnergy);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Woops:" + e);
                }
            }


        }
        if (totalCount != 0)
        {
            Console.WriteLine("totalCount:" + totalCount + ", partialCount:" + partialCount);
        }
        fieldLife = tempLife;


        avgSpeed = avgSpeed / orgcount;
        avgRepFact = avgRepFact / orgcount;
        avgRange = avgRange / orgcount;
        avgPhytoshynth = avgPhytoshynth / orgcount;
        avgEnergy = avgEnergy / orgcount;
        Console.WriteLine("speed:" + avgSpeed + ", RepFact:" + avgRepFact + ", range:" + avgRange + ", photoSynthRate:" + avgPhytoshynth + ", energy:" + avgEnergy);
        Console.WriteLine("hit1:" + hit1 + ", hit2:" + hit2 + ", hit3:" + hit3);
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
                double activationEnergy = 0;
                int tempSpeed = 0;
                double tempRange = 0;
                double tempReplicationFactor = 0;
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if ((x + i >= 0) && (y + j >= 0) && (x + i < SimMain.mapWidth) && (y + j < SimMain.mapHeight))
                        {
                            if (fieldLife[x + i, y + j] != null)
                            {
                                count++;
                                activationEnergy += fieldLife[x + i, y + j].getReplicationEnergy();
                                tempSpeed += fieldLife[x + i, y + j].speed;
                                tempRange += fieldLife[x + i, y + j].range;
                                tempReplicationFactor += fieldLife[x + i, y + j].replicationFactor;


                            }


                        }
                    }
                }


                if ((count == 3) || (fieldLight[x, y] > 100f && fieldLife[x, y] != null) || (count == 4 && (fieldLife[x, y] != null || fieldLight[x, y] > 100f)))
                {
                    tempLife[x, y] = new Life(Life.SpeciesType.PLANT);//, (int)(tempSpeed / (double)count + 0.5f), tempRange / count, tempReplicationFactor / count, activationEnergy);
                }


            }
        }
        fieldLife = tempLife;
    }


    public void StepLight(int time)
    {
        try
        {//Console.WriteLine((int)(fieldLife[0, 0].energy * fieldLife[0, 0].speed * 25));
         //Console.WriteLine(255 + ", " + (int)(fieldLife[0, 0].energy * fieldLife[0, 0].speed * 25) + ", " + (int)(fieldLife[0, 0].energy * fieldLife[0, 0].range * 62) + ", " + (int)(fieldLife[0, 0].energy * fieldLife[0, 0].replicationFactor * 255));


            Brush cellColor;
            for (int x = 0; x < (SimMain.mapWidth); x++)
            {
                for (int y = 0; y < (SimMain.mapHeight); y++)
                {
                    if (fieldLife[x, y] != null)
                    {
                        cellColor = Brushes.White;
                    }
                    //cellColor = new SolidBrush(Color.FromArgb(255, (int)(fieldLife[x, y].energy * fieldLife[x, y].speed * 25), (int)(fieldLife[x, y].energy * fieldLife[x, y].range * 62), (int)(fieldLife[x, y].energy * fieldLife[x, y].replicationFactor * 255)));
                    else
                        cellColor = Brushes.Black;


                    float tempLightVal = 128f - Math.Abs(Noise.CalcPixel3D(x * (1000 / SimMain.mapWidth), y * (1000 / SimMain.mapHeight), (time * 3) / 10, 0.005f) - 128);
                    fieldLight[x, y] = tempLightVal;
                    if (tempLightVal >= 100f)
                    {
                        if (fieldLife[x, y] != null)
                        {
                            try
                            {
                                int R = (int)(fieldLife[x, y].energy * fieldLife[x, y].speed * (25 / 20f));
                                if (R > 255)
                                    R = 255;
                                int G = (int)(((fieldLife[x, y].energy * fieldLife[x, y].range * (62 / 20f)) + 64f) / (5f / 4f));
                                if (G > 255)
                                    G = 255;
                                int B = (int)((fieldLife[x, y].energy * fieldLife[x, y].replicationFactor * 25) / 10);
                                if (B > 255)
                                    B = 255;

                                //Console.WriteLine("color good:" + (int)(fieldLife[x, y].energy * fieldLife[x, y].speed * (25 / 5f)) + ", " + (((int)(fieldLife[x, y].energy * fieldLife[x, y].range * (62 / 5f)) + 127) / 2) + ", " + (int)(fieldLife[x, y].energy * fieldLife[x, y].replicationFactor * 25));
                                cellColor = new SolidBrush(
                                    Color.FromArgb(255,
                                    R,
                                    G,
                                    B
                                ));
                                Graphics.FromImage(SimMain.lightbmp).FillRectangle(cellColor, x * (1000 / SimMain.mapWidth), y * (1000 / SimMain.mapHeight), (1000 / SimMain.mapWidth), (1000 / SimMain.mapHeight));
                            }
                            catch
                            {
                                Console.WriteLine("color error:" + 255 + ", " + (int)(fieldLife[x, y].energy * fieldLife[x, y].speed * (25 / 20f)) + ", " + (((int)(fieldLife[x, y].energy * fieldLife[x, y].range * (62 / 20f)) + 127) / 2) + ", " + (int)((fieldLife[x, y].energy * fieldLife[x, y].replicationFactor * 25) / 10));
                                Console.WriteLine("color errorV:" + 255 + ", " + fieldLife[x, y].energy + ", " + fieldLife[x, y].speed + ", " + fieldLife[x, y].range * (62 / 20f) + ", " + fieldLife[x, y].replicationFactor * 25 / 10 + ", " + fieldLife[x, y].photosyntheticRate);
                                Graphics.FromImage(SimMain.lightbmp).FillRectangle(Brushes.Red, x * (1000 / SimMain.mapWidth), y * (1000 / SimMain.mapHeight), (1000 / SimMain.mapWidth), (1000 / SimMain.mapHeight));
                            }


                        }
                        else
                        {

                            //Graphics.FromImage(SimMain.lightbmp).FillRectangle(Brushes.LightBlue, x * (1000 / SimMain.mapWidth), y * (1000 / SimMain.mapHeight), (1000 / SimMain.mapWidth), (1000 / SimMain.mapHeight));
                        }


                    }
                    else
                    {
                        if (fieldLife[x, y] != null)
                        {
                            // Graphics.FromImage(SimMain.lightbmp).FillRectangle(Brushes.Blue, x * (1000 / SimMain.mapWidth), y * (1000 / SimMain.mapHeight), (1000 / SimMain.mapWidth), (1000 / SimMain.mapHeight));
                            try
                            {
                                int R = (int)(fieldLife[x, y].energy * fieldLife[x, y].speed * (25 / 20f));
                                if (R > 255)
                                    R = 255;
                                int G = (int)(fieldLife[x, y].energy * fieldLife[x, y].range * (62 / 20f));
                                if (G > 255)
                                    G = 255;
                                int B = (int)((fieldLife[x, y].energy * fieldLife[x, y].replicationFactor * 25) / 10);
                                if (B > 255)
                                    B = 255;

                                //Console.WriteLine("color good:" + (int)(fieldLife[x, y].energy * fieldLife[x, y].speed * (25 / 5f)) + ", " + (((int)(fieldLife[x, y].energy * fieldLife[x, y].range * (62 / 5f)) + 127) / 2) + ", " + (int)(fieldLife[x, y].energy * fieldLife[x, y].replicationFactor * 25));
                                cellColor = new SolidBrush(
                                    Color.FromArgb(255,
                                    R,
                                    G,
                                    B
                                ));
                                Graphics.FromImage(SimMain.lightbmp).FillRectangle(cellColor, x * (1000 / SimMain.mapWidth), y * (1000 / SimMain.mapHeight), (1000 / SimMain.mapWidth), (1000 / SimMain.mapHeight));


                                cellColor = new SolidBrush(Color.FromArgb(255, R, G, B));
                                Graphics.FromImage(SimMain.lightbmp).FillRectangle(cellColor, x * (1000 / SimMain.mapWidth), y * (1000 / SimMain.mapHeight), (1000 / SimMain.mapWidth), (1000 / SimMain.mapHeight));
                            }
                            catch
                            {
                                Console.WriteLine("color error:" + 255 + ", " + (int)(fieldLife[x, y].energy * fieldLife[x, y].speed * (25 / 20f)) + ", " + (int)(fieldLife[x, y].energy * fieldLife[x, y].range * (62 / 20f)) + ", " + (int)((fieldLife[x, y].energy * fieldLife[x, y].replicationFactor * 25) / 10));
                                Console.WriteLine("color errorV:" + 255 + ", " + fieldLife[x, y].energy + ", " + fieldLife[x, y].speed + ", " + fieldLife[x, y].range * (62 / 20f) + ", " + fieldLife[x, y].replicationFactor * 25 / 10 + ", " + fieldLife[x, y].photosyntheticRate);
                                Graphics.FromImage(SimMain.lightbmp).FillRectangle(Brushes.Red, x * (1000 / SimMain.mapWidth), y * (1000 / SimMain.mapHeight), (1000 / SimMain.mapWidth), (1000 / SimMain.mapHeight));
                            }
                        }
                        else
                        {
                            //Graphics.FromImage(SimMain.lightbmp).FillRectangle(Brushes.White, x * (1000 / SimMain.mapWidth), y * (1000 / SimMain.mapHeight), (1000 / SimMain.mapWidth), (1000 / SimMain.mapHeight));
                        }
                        //SimMain.lightbmp.SetPixel(x, y, Color.Black);
                    }




                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }


    }


}
