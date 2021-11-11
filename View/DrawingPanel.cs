// Author: Daniel Detwiller & Warren Kidman
// Date: 11/25/2019
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TankWars;

namespace View
{
    /// <summary>
    /// Drawing panel class.
    /// </summary>
    public class DrawingPanel : Panel
    {
        /// <summary>
        /// The world.
        /// </summary>
        private World world;

        /// <summary>
        /// The controller.
        /// </summary>
        private Controller controller;

        /// <summary>
        /// The view size.
        /// </summary>
        private int viewSize;

        /// <summary>
        /// List of all the tank images.
        /// </summary>
        private List<Image> tankPNGs;

        /// <summary>
        /// List of all the turret images.
        /// </summary>
        private List<Image> turretPNGs;

        /// <summary>
        /// The wall image.
        /// </summary>
        private Image wallPNG;

        /// <summary>
        /// List of all the projectile images.
        /// </summary>
        private List<Image> projPNGs;

        /// <summary>
        /// The background image.
        /// </summary>
        private Image backgroundPNG;

        /// <summary>
        /// The powerup image.
        /// </summary>
        private Image powerupPNG;

        /// <summary>
        /// List of the explosion images (each frame).
        /// </summary>
        private List<Image> explosionPNGs;

        /// <summary>
        /// Dictionary that holds amount of frames since tank exploded. (key is the tank id)
        /// </summary>
        private Dictionary<int, int> tankCounters;

        /// <summary>
        /// int that holds the size of the world.
        /// </summary>
        private int worldSize;

        /// <summary>
        /// Holds a beam that may need to be removed.
        /// </summary>
        private List<int> beamToRemove;

        /// <summary>
        /// DrawingPanel constructor that initialized everything.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="ctl"></param>
        public DrawingPanel(World w, Controller ctl)
        {
            this.DoubleBuffered = true;
            world = w;
            viewSize = 800;
            worldSize = world.GetSize();
            controller = ctl;
            tankCounters = new Dictionary<int, int>();
            beamToRemove = new List<int>();

            // Add all the tank images.
            tankPNGs = new List<Image>();
            tankPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\BlueTank.png"));
            tankPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\YellowTank.png"));
            tankPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\DarkTank.png"));
            tankPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\GreenTank.png"));
            tankPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\LightGreenTank.png"));
            tankPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\OrangeTank.png"));
            tankPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\RedTank.png"));
            tankPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\PurpleTank.png"));

            // Add all the turret images.
            turretPNGs = new List<Image>();
            turretPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\BlueTurret.png"));
            turretPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\YellowTurret.png"));
            turretPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\DarkTurret.png"));
            turretPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\GreenTurret.png"));
            turretPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\LightGreenTurret.png"));
            turretPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\OrangeTurret.png"));
            turretPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\RedTurret.png"));
            turretPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\PurpleTurret.png"));

            // Add all the projectle images.
            projPNGs = new List<Image>();
            projPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\shot-blue.png"));
            projPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\shot-yellow.png"));
            projPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\shot-grey.png"));
            projPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\shot-green.png"));
            projPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\shot_grey.png"));
            projPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\shot-brown.png"));
            projPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\shot-red.png"));
            projPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\shot-violet.png"));
            
            wallPNG = Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\WallSprite1.png");
            backgroundPNG = Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\Background1.png");
            powerupPNG = Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\powerup1.png");

            // Add all the explosion images.
            explosionPNGs = new List<Image>();
            explosionPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\explosion-1.gif"));
            explosionPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\explosion-2.gif"));
            explosionPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\explosion-3.gif"));
            explosionPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\explosion-4.gif"));
            explosionPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\explosion-5.gif"));
            explosionPNGs.Add(Image.FromFile("..\\..\\..\\Resources\\Libraries\\Images\\explosion-6.gif"));
        }

        /// <summary>
        /// Helper method for DrawObjectWithTransform
        /// </summary>
        /// <param name="size">The world (and image) size</param>
        /// <param name="w">The worldspace coordinate</param>
        /// <returns></returns>
        private static int WorldSpaceToImageSpace(int size, double w)
        {
            return (int)w + size / 2;
        }


        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e 
        public delegate void ObjectDrawer(object o, PaintEventArgs e);

        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldSize">The size of one edge of the world (assuming the world is square)</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);
            e.Graphics.TranslateTransform(x, y);
            // Thre this in because the server was sending NaN and crashing the client.
            if (double.IsNaN(angle))
                e.Graphics.RotateTransform(0);
            else
                e.Graphics.RotateTransform((float)angle);

            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }

        /// <summary>
        /// Draws the tanks.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void TankDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            if (t.GetHealth() >= 1)
            {
                e.Graphics.DrawImage(tankPNGs[t.GetID() % 8], -30, -30, 60, 60);
                // Adds this tank to tankCounters.
                if (!tankCounters.ContainsKey(t.GetID()))
                    tankCounters.Add(t.GetID(), 0);
            }
        }

        /// <summary>
        /// Draws the explosions.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void ExplosionDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            int count = tankCounters[t.GetID()];

            int x = -30;
            int y = -30;

            // If the animation has played for 90 frames, stop it.
            if (count >= 90)
            {
                tankCounters.Remove(t.GetID());
                return;
            }
            // A new frame of the animation happens every 15 frames.
            else if (count < 15)
                e.Graphics.DrawImage(explosionPNGs[0], x, y, 60, 60);
            else if (count >= 15 && count < 30)
            {
                e.Graphics.DrawImage(explosionPNGs[1], x, y, 60, 60);
            }
            else if (count >= 30 && count < 45)
            {
                e.Graphics.DrawImage(explosionPNGs[2], x, y, 60, 60);
            }
            else if (count >= 45 && count < 60)
            {
                e.Graphics.DrawImage(explosionPNGs[3], x, y, 60, 60);
            }
            else if (count >= 60 && count < 75)
            {
                e.Graphics.DrawImage(explosionPNGs[4], x, y, 60, 60);
            }
            else if (count >= 75 && count < 90)
            {
                e.Graphics.DrawImage(explosionPNGs[5], x, y, 60, 60);
            }

        }

        /// <summary>
        /// Draws the tanks turrets.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void TurretDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            // Only draw the turret if the tank is alive.
            if (t.GetHealth() >= 1)
                e.Graphics.DrawImage(turretPNGs[t.GetID() % 8], -25, -25, 50, 50);

        }

        /// <summary>
        /// Draws the walls.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void WallDrawer(object o, PaintEventArgs e)
        {
            e.Graphics.DrawImage(wallPNG, 175, 175, 50, 50);
        }

        /// <summary>
        /// Draws the projectile.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            Projectile p = o as Projectile;
            e.Graphics.DrawImage(projPNGs[p.GetFiringTankID() % 8], -15, -15, 30, 30);
        }

        /// <summary>
        /// Draws the powerups.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void PowerupDrawer(object o, PaintEventArgs e)
        {
            e.Graphics.DrawImage(powerupPNG, -15, -15, 30, 30);
        }

        /// <summary>
        /// Draws the beams.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void BeamDrawer(object o, PaintEventArgs e)
        {

            Beam b = o as Beam;

            // The beams are rainbow
            Pen redPen = new Pen(Color.Red);
            Pen orangePen = new Pen(Color.Orange);
            Pen yellowPen = new Pen(Color.Yellow);
            Pen greenPen = new Pen(Color.LightGreen);
            Pen bluePen = new Pen(Color.LightBlue);
            Pen purplePen = new Pen(Color.MediumPurple);

            int x = 0;
            int y = 0;
            int x2 = 0;
            int y2 = -2000;

            // Draws each line off by 2 to get the rainbow effect.
            e.Graphics.DrawLine(redPen, -5, 0, x2 - 5, y2);
            e.Graphics.DrawLine(orangePen, -3, 0, x2 - 3, y2);
            e.Graphics.DrawLine(yellowPen, -1, 0, x2 - 1, y2);
            e.Graphics.DrawLine(greenPen, 1, 0, x2 + 1, y2);
            e.Graphics.DrawLine(bluePen, 3, 0, x2 + 3, y2);
            e.Graphics.DrawLine(purplePen, 5, 0, x2 + 5, y2);
           
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Dictionary<int, Tank> tanks = world.GetTanks();

            // Start drawing once the server sends tanks.
            if (tanks.Count > 0)
            {
                // Get the id of the player playing on this client.
                Tank player = tanks[controller.GetPlayerID()];

                // Get the x and y position of the player.
                double playerX = player.GetLocation().GetX();
                double playerY = player.GetLocation().GetY();

                // calculate view/world size ratio
                double ratio = (double)viewSize / (double)worldSize;
                int halfSizeScaled = (int)(worldSize / 2.0 * ratio);

                double inverseTranslateX = -WorldSpaceToImageSpace(worldSize, playerX) + halfSizeScaled;
                double inverseTranslateY = -WorldSpaceToImageSpace(worldSize, playerY) + halfSizeScaled;

                e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);

                Dictionary<int, Projectile> projectiles = world.GetProjectiles();
                Dictionary<int, Wall> walls = world.GetWalls();
                Dictionary<int, Beam> beams = world.GetBeams();
                Dictionary<int, Powerup> powerups = world.GetPowerups();

                // Draw the background.
                e.Graphics.DrawImage(backgroundPNG, 0, 0, worldSize, worldSize);
                
                
                lock (walls)
                {
                    // Go through each wall and draw it.
                    foreach (Wall w in walls.Values)
                    {
                        DrawObjectWithTransform(e, w, viewSize, w.GetPoint1().GetX(), w.GetPoint1().GetY(), 0, WallDrawer);
                        FillInWalls(w, e);
                    }
                }

                lock (tanks)
                {
                    // Go through each tank and draw it as well as its turret, name, healt, and score.
                    foreach (Tank t in tanks.Values)
                    {
                        DrawObjectWithTransform(e, t, worldSize, t.GetLocation().GetX(), t.GetLocation().GetY(), t.GetOrientation().ToAngle(), TankDrawer);
                        DrawObjectWithTransform(e, t, worldSize, t.GetLocation().GetX(), t.GetLocation().GetY(), t.GetTurretDirection().ToAngle(), TurretDrawer);

                        DrawPlayerNameAndScore(e, t);
                        DrawTankHealthBar(e, t);

                        // If the tank died, draw an explosion.
                        if (t.GetHealth() == 0)
                        {

                            if (tankCounters.ContainsKey(t.GetID()))
                            {
                                // Increase the number of frames this tank has been dead for.
                                tankCounters[t.GetID()] += 1;

                                int x = (int)t.GetLocation().GetX();
                                int y = (int)t.GetLocation().GetY();

                                DrawObjectWithTransform(e, t, worldSize, x, y, t.GetOrientation().ToAngle(), ExplosionDrawer);
                            }

                        }
                    }
                }

                lock (projectiles)
                {
                    if (projectiles.Count > 0)
                    {
                        // Goes through each projectile and draws it.
                        foreach (Projectile p in projectiles.Values)
                        {
                            // Only draw it if the tank that's firing exists.
                            if (tanks.ContainsKey(p.GetFiringTankID()))
                            {
                                DrawObjectWithTransform(e, p, worldSize, p.GetLocation().GetX(), p.GetLocation().GetY(), tanks[p.GetFiringTankID()].GetTurretDirection().ToAngle(), ProjectileDrawer);
                            }
                        }
                    }
                }

                lock (powerups)
                {
                    // Go through each powerup and draw it.
                    foreach (Powerup p in powerups.Values)
                    {
                        DrawObjectWithTransform(e, p, worldSize, p.GetLocation().GetX(), p.GetLocation().GetY(), 0, PowerupDrawer);
                    }
                }

                lock (beams)
                {
                    if (beams.Values.Count > 0)
                    {
                        // Go through each beam and draw it.
                        foreach (Beam b in beams.Values)
                        {
                            // Increment the amount of frames the beam has been drawn for.
                            b.SetBeamFrameCount();

                            // Only let the beam be drawn for 30 frames.
                            if (b.GetBeamFrameCount() < 30)
                            {
                                DrawObjectWithTransform(e, b, worldSize, b.GetOrigin().GetX(), b.GetOrigin().GetY(), b.GetDirection().ToAngle(), BeamDrawer);
                                break;
                            }
                            else
                                // Hold this beam so it can be removed.
                                beamToRemove.Add(b.GetID());
                        }
                        // Removes the beam after it has been shot.
                        foreach (int id in beamToRemove)
                            beams.Remove(id);

                    }
                }

            }

            base.OnPaint(e);
        }

        /// <summary>
        /// Draws the players health above their tank.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="t"></param>
        private void DrawTankHealthBar(PaintEventArgs e, Tank t)
        {
            // Get the position of the tank.
            int x = WorldSpaceToImageSpace(worldSize, t.GetLocation().GetX());
            int y = WorldSpaceToImageSpace(worldSize, t.GetLocation().GetY());

            SolidBrush greenBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green);
            SolidBrush yellowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Yellow);
            SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);

            Rectangle healthbar;

            // If the tank has full health.
            if (t.GetHealth() == 3)
            {
                healthbar = new Rectangle(x - 21, y - 40, 42, 5);
                e.Graphics.FillRectangle(greenBrush, healthbar);
            }
            // If the tank has been shot once.
            else if (t.GetHealth() == 2)
            {
                healthbar = new Rectangle(x - 21, y - 40, 28, 5);
                e.Graphics.FillRectangle(yellowBrush, healthbar);
            }
            // If the tank has been shot twice.
            else if (t.GetHealth() == 1)
            {
                healthbar = new Rectangle(x - 21, y - 40, 14, 5);
                e.Graphics.FillRectangle(redBrush, healthbar);
            }

        }

        /// <summary>
        /// Draws the player name below the tank.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="t"></param>
        private void DrawPlayerNameAndScore(PaintEventArgs e, Tank t)
        {
            // Get the position of the tank.
            int x = WorldSpaceToImageSpace(worldSize, t.GetLocation().GetX());
            int y = WorldSpaceToImageSpace(worldSize, t.GetLocation().GetY());

            // Text formatting and style.
            SolidBrush whiteBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            Font f = new Font("Arial", 11, FontStyle.Bold);
            Point p = new Point(x, y);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            // If the tank is not dead, draw the name and score.
            if (t.GetHealth() >= 1)
                e.Graphics.DrawString(t.GetName() + ": " + t.GetScore(), f, whiteBrush, x, y + 38, sf);
        }

        /// <summary>
        /// Helper method that fills in all the walls.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="e"></param>
        private void FillInWalls(Wall w, PaintEventArgs e)
        {
            double difference;
            double increment = 0;
            double divide;

            // If p1 x position equals p2 x position, then we are filling vertically.
            if (w.GetPoint1().GetX() == w.GetPoint2().GetX())
            {
                // Get the difference between the two points y values.
                difference = w.GetPoint1().GetY() - w.GetPoint2().GetY();
                // Get the number of walls needed to draw.
                divide = Math.Abs(difference / 50);
                // While the wall position is less than the destination position, draw.
                while (Math.Abs(increment) < Math.Abs(difference))
                {
                    DrawObjectWithTransform(e, w, viewSize, w.GetPoint2().GetX(), w.GetPoint2().GetY() + increment, 0, WallDrawer);
                    // Change the y value by 50 each time.
                    increment += difference / divide;
                }
            }
            // Else we are filling horizontally.
            else
            {
                // Get the difference between the two points x values.
                difference = w.GetPoint1().GetX() - w.GetPoint2().GetX();
                // Get the number of walls needed to draw.
                divide = Math.Abs(difference / 50);
                // While the wall position is less than the destination position, draw.
                while (Math.Abs(increment) < Math.Abs(difference))
                {
                    DrawObjectWithTransform(e, w, viewSize, w.GetPoint2().GetX() + increment, w.GetPoint2().GetY(), 0, WallDrawer);
                    // Change the x value by 50 each time.
                    increment += difference / divide;
                }
            }
        }
    }
}
