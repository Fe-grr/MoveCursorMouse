using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;


namespace MoveCursor
{
    class SampleMouseMove
    {
        public const UInt32 MOUSEEVENT_LEFTDOWN = 0X0002;
        public const UInt32 MOUSEEVENT_LEFTUP = 0X0004;

        static Random random = new Random();
        static int mouseSpeed = 15;

        static void Main(string[] args)
        {
            DateTime dtAtual = DateTime.Now;
            DateTime dtFutura = dtAtual.AddMinutes(1);
            DateTime dtStop = dtAtual.AddMinutes(60.0);
            Console.WriteLine("Hora de inicio " + dtAtual.ToShortTimeString());
            Console.Write("O programa encerrará as " + dtAtual.AddMinutes(60.0).ToShortTimeString() + "\nvolte antes para evitar que o TD te pegue MUAHAHAHA.");

            while (!DateTime.Now.ToShortTimeString().Equals(dtStop.ToShortTimeString()))
            {
                if (dtAtual.ToShortTimeString() == dtFutura.ToShortTimeString())
                {
                    MoveMouse(2, 2, 2, 2);
                    dtAtual = DateTime.Now;
                    dtFutura = dtAtual.AddSeconds(30);
                }
                dtAtual = DateTime.Now;
            }


        }

        static void MoveMouse(int x, int y, int rx, int ry)
        {
            Point c = new Point();
            GetCursorPos(out c);

            x += random.Next(rx);
            y += random.Next(ry);

            double randomSpeed = Math.Max((random.Next(mouseSpeed) / 2.0 + mouseSpeed) / 10.0, 0.1);

            WindMouse(c.X, c.Y, x, y, 9.0, 3.0, 10.0 / randomSpeed,
                15.0 / randomSpeed, 10.0 * randomSpeed, 10.0 * randomSpeed);
        }

        static void WindMouse(double xs, double ys, double xe, double ye,
            double gravity, double wind, double minWait, double maxWait,
            double maxStep, double targetArea)
        {

            double dist, windX = 0, windY = 0, veloX = 0, veloY = 0, randomDist, veloMag, step;
            int oldX, oldY, newX = (int)Math.Round(xs), newY = (int)Math.Round(ys);

            double waitDiff = maxWait - minWait;
            double sqrt2 = Math.Sqrt(2.0);
            double sqrt3 = Math.Sqrt(3.0);
            double sqrt5 = Math.Sqrt(5.0);

            dist = Hypot(xe - xs, ye - ys);

            double distBack = dist;
            double distCompare = distBack;
            bool returnCursor = false;
            double backX = 15, backY = 15;
            while (dist > 1.0)
            {
                wind = Math.Min(wind, dist);

                if (dist >= targetArea)
                {
                    int w = random.Next((int)Math.Round(wind) * 2 + 1);
                    windX = windX / sqrt3 + (w - wind) / sqrt5;
                    windY = windY / sqrt3 + (w - wind) / sqrt5;
                }
                else
                {
                    windX = windX / sqrt2;
                    windY = windY / sqrt2;
                    if (maxStep < 3)
                        maxStep = random.Next(3) + 3.0;
                    else
                        maxStep = maxStep / sqrt5;
                }

                veloX += windX;
                veloY += windY;
                veloX = veloX + gravity * (xe - xs) / dist;
                veloY = veloY + gravity * (ye - ys) / dist;

                if (Hypot(veloX, veloY) > maxStep)
                {
                    randomDist = maxStep / 2.0 + random.Next((int)Math.Round(maxStep) / 2);
                    veloMag = Hypot(veloX, veloY);
                    veloX = (veloX / veloMag) * randomDist;
                    veloY = (veloY / veloMag) * randomDist;
                }

                oldX = (int)Math.Round(xs);
                oldY = (int)Math.Round(ys);
                xs += veloX;
                ys += veloY;
                dist = Hypot(xe - xs, ye - ys);
                newX = (int)Math.Round(xs);
                newY = (int)Math.Round(ys);

                if (oldX != newX || oldY != newY)
                    SetCursorPos(newX, newY);

                step = Hypot(xs - oldX, ys - oldY);
                int wait = (int)Math.Round(waitDiff * (step / maxStep) + minWait);
                Thread.Sleep(wait);

                if (dist < 1.0)
                {
                    returnCursor = true;
                }
            }

            int endX = (int)Math.Round(xe);
            int endY = (int)Math.Round(ye);
            if (endX != newX || endY != newY)
                SetCursorPos(endX, endY);
            if (returnCursor)
            {
                int resto = 0;
                int stopBack = 0;
                while (stopBack != 10)
                {
                    //distBack -= 5;
                    stopBack++;
                    resto++;
                    backX = backX * 2;
                    backY = backY * 1.86;

                    SetCursorPos((int)Math.Round(backX), (int)Math.Round(backY));
                    if (stopBack == 10)
                    {
                        mouse_event(MOUSEEVENT_LEFTDOWN, 0, 0, 0, 0);
                        mouse_event(MOUSEEVENT_LEFTUP, 0, 0, 0, 0);
                    }

                    //oldX = (int)Math.Round(xs);
                    //oldY = (int)Math.Round(ys);
                    //step = Hypot(xs - oldX, ys - oldY);
                    //int wait = (int)Math.Round(waitDiff * (step / maxStep) + minWait);
                    if (resto % 2 == 0)
                        Thread.Sleep(200);
                    else
                        Thread.Sleep(201);

                }
            }
        }

        static double Hypot(double dx, double dy)
        {
            return Math.Sqrt(dx * dx + dy * dy);
        }


        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, int dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point p);

    }

}