using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Threading;
using System.Drawing.Text;
using System.Diagnostics;
using SimplexNoise;

class SimMain : Form
{
    private bool WaitPeriod = false;
    private Graphics graphics;
    private Random rand = new Random();
    private int time = 0;
    public static SimMain instance;
    private static System.Timers.Timer periodicTimer;
    private EcoSysField field;
    public static int mapHeight = 50;
    public static int mapWidth = 50;
    public static Bitmap bmp = new Bitmap(1000, 1000);
    public static Bitmap lightbmp = new Bitmap(1000, 1000);

    static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
    public SimMain()
    {
        //InitializeComponent();
        instance = this;
        this.Init();
    }
    public async void Init()
    {


        this.Width = 1017;
        this.Height = 1040;
        this.Text = "Sim";
        this.Show();
        graphics = this.CreateGraphics();
        instance.graphics.FillRectangle(Brushes.Black, 0, 0, mapWidth, mapHeight);
        SetPeriodic(10);

        field = new EcoSysField(mapWidth, mapHeight);
    }

    private static void SetPeriodic(int speed)
    {
        // Create a timer with a two second interval.
        periodicTimer = new System.Timers.Timer(speed);

        // Hook up the Elapsed event for the timer. 
        periodicTimer.Elapsed += OnTimedEvent;
        periodicTimer.AutoReset = true;
        periodicTimer.Enabled = true;
        // Creating and initializing a thread


        // Setting the thread to run in the background

    }
    private static void Periodic()
    {
        // if (!instance.WaitPeriod)
        // {
        //     instance.WaitPeriod = true;
        //     instance.time++;
        //     instance.Display(instance.field);
        //     instance.field.StepTime(instance.time);
        //     instance.WaitPeriod = false;
        // }

    }
    private static void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        VisualizeLight();
        //Periodic();
        //Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}", e.SignalTime);
    }
    public void Display(EcoSysField field)
    {
        graphics.DrawImage(bmp, 0, 0);
    }
    public static void VisualizeLight()
    {
        if (!instance.WaitPeriod)
        {
            Console.WriteLine(instance.time);
            instance.WaitPeriod = true;
            instance.time++;

            instance.Display(instance.field);
            // instance.field.StepLight(instance.time);
            instance.field.StepTime(instance.time);
            instance.Display(instance.field);
            instance.graphics.DrawImage(lightbmp, 0, 0);

            instance.WaitPeriod = false;
        }
    }

}