// Author: Daniel Detwiller & Warren Kidman
// Date: 11/25/2019
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TankWars;

namespace View
{
    /// <summary>
    /// The tankwars form class.
    /// </summary>
    public partial class Form1 : Form
    {

        /// <summary>
        /// The games controller of MVC.
        /// </summary>
        private Controller controller;

        /// <summary>
        /// The games world.
        /// </summary>
        private World world;

        /// <summary>
        /// The drawing panel.
        /// </summary>
        private DrawingPanel dPanel;

        /// <summary>
        /// True if an error occured, false otherwise.
        /// </summary>
        private bool errorOccured;

        /// <summary>
        /// Initializes all the components of the form.
        /// </summary>
        /// <param name="ctl"></param>
        public Form1(Controller ctl)
        {
            InitializeComponent();
            controller = ctl;
            world = controller.GetWorld();
            controller.RegisterServerUpdateHandler(OnFrame);
            controller.RegisterErrorOccuredHandler(ErrorOccured);
            ClientSize = new Size(800, 800);
            errorOccured = false;

            FormClosed += OnExit;
        }

        /// <summary>
        /// Called on every frame.
        /// </summary>
        private void OnFrame()
        {
            if (!IsHandleCreated)
                return;

            GetMousePosition();

            MethodInvoker m = new MethodInvoker(() => { this.Invalidate(true); });
            try
            {
                this.Invoke(m);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Called when the controls menu button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void controlsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DisplayHelpDialog();
        }

        /// <summary>
        /// Called when the about menu button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DisplayAboutDialog();
        }

        /// <summary>
        /// Called when an error occurs.
        /// </summary>
        private void ErrorOccured()
        {
            errorOccured = true;
            DialogResult result = MessageBox.Show("Error while trying to connect. Please retry.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (result.Equals(DialogResult.OK))
            {
                connectButton.Enabled = true;
                serverTextBox.Enabled = true;
                nameTextBox.Enabled = true;
            }
        }

        /// <summary>
        /// Connect button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectButton_Click(object sender, EventArgs e)
        {
            // call helper method from controller that essentially does what is below.
            if (serverTextBox.Text == "")
            {
                MessageBox.Show("Please enter a server address.");
                return;
            }

            // connect to server
            controller.Connect(serverTextBox.Text, nameTextBox.Text);

            if (!errorOccured)
            {
                // Disable the controls and try to connect
                connectButton.Enabled = false;
                serverTextBox.Enabled = false;
                nameTextBox.Enabled = false;

                while (world.GetSize() == 0)
                    world = controller.GetWorld();

                dPanel = new DrawingPanel(world, controller);
                dPanel.Location = new Point(0, 0);
                dPanel.Size = new Size(800, 800);
                dPanel.MouseDown += new MouseEventHandler(Form1_MouseDown);
                dPanel.MouseUp += new MouseEventHandler(Form1_MouseUp);
                this.Controls.Add(dPanel);
            }
            errorOccured = false;
        }

        /// <summary>
        /// Helper method that displays the controls dialog.
        /// </summary>
        private void DisplayHelpDialog()
        {
            string message = "Controls:"
                + "\n    - W / UP ARROW:                        MOVE UP."
                + "\n    - A / LEFT ARROW:                      MOVE LEFT."
                + "\n    - S / DOWN ARROW:                  MOVE DOWN."
                + "\n    - D / RIGHT ARROW:                   MOVE RIGHT."
                + "\n    - LEFT MOUSE CLICK/HOLD:       FIRE DEFAULT PROJECTILE."
                + "\n    - RIGHT MOUSE CLICK:               FIRE BEAM PROJECTILE."
                + "\n    - MOUSE MOVE:                         AIM TANK TURRET.";

            string title = "Help Information";

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Helper method that displays the about dialog.
        /// </summary>
        private void DisplayAboutDialog()
        {
            string message = "About:"
               + "\n TankWars solution by Daniel Detwiller and Warren Kidman"
               + "\n 2019 CS 3500";

            string title = "About information";

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Helper method thats called when the form is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExit(object sender, FormClosedEventArgs e)
        {
            controller.Exit();
        }

        /// <summary>
        /// Called when a key gets pressed down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.W) || e.KeyCode.Equals(Keys.Up))
            {
                controller.ProcessKeysDown("up");
            }
            else if (e.KeyCode.Equals(Keys.A) || e.KeyCode.Equals(Keys.Left))
            {
                controller.ProcessKeysDown("left");
            }
            else if (e.KeyCode.Equals(Keys.S) || e.KeyCode.Equals(Keys.Down))
            {
                controller.ProcessKeysDown("down");
            }
            else if (e.KeyCode.Equals(Keys.D) || e.KeyCode.Equals(Keys.Right))
            {
                controller.ProcessKeysDown("right");
            }
        }

        /// <summary>
        /// Called when a key gets released.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.W) || e.KeyCode.Equals(Keys.Up))
            {
                controller.ProcessKeysUp("up");
            }
            else if (e.KeyCode.Equals(Keys.A) || e.KeyCode.Equals(Keys.Left))
            {
                controller.ProcessKeysUp("left");
            }
            else if (e.KeyCode.Equals(Keys.S) || e.KeyCode.Equals(Keys.Down))
            {
                controller.ProcessKeysUp("down");
            }
            else if (e.KeyCode.Equals(Keys.D) || e.KeyCode.Equals(Keys.Right))
            {
                controller.ProcessKeysUp("right");
            }
        }

        /// <summary>
        /// Called when a mouse button is pressed down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Left))
                controller.ProcessMouseDown("main");
            else if (e.Button.Equals(MouseButtons.Right))
                controller.ProcessMouseDown("alt");
        }

        /// <summary>
        /// Called when a mouse button is released.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            controller.ProcessMouseUp();
        }

        /// <summary>
        /// Gets the position of the mouse.
        /// </summary>
        private void GetMousePosition()
        {
            MethodInvoker m = new MethodInvoker(() => { Point mouseRelativeToClient = dPanel.PointToClient(Cursor.Position);
                controller.ProcessMouseLocation(mouseRelativeToClient.X, mouseRelativeToClient.Y);
            });
            try
            {
                this.Invoke(m);
            }
            catch (Exception)
            {

            }
        }

    }
}
