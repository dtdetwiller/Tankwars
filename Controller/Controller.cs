// Author: Daniel Detwiller & Warren Kidman
// Date: 11/25/2019
using System;
using Newtonsoft.Json;
using NetworkUtil;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

namespace TankWars
{
    /// <summary>
    /// The controller of TankWars (MVC)
    /// </summary>
    public class Controller
    {

        /// <summary>
        /// SocketState representing the server.
        /// </summary>
        private SocketState theServer;

        /// <summary>
        /// Stirng that holds the players name.
        /// </summary>
        private string playerName;

        /// <summary>
        /// int that holds the players ID.
        /// </summary>
        private int playerID;

        /// <summary>
        /// The worlds.
        /// </summary>
        private World world;

        /// <summary>
        /// The control commands.
        /// </summary>
        private ControlCommands commands;

        /// <summary>
        /// The server update handler delegate.
        /// </summary>
        public delegate void ServerUpdateHandler();

        /// <summary>
        /// The error occured handler delagate.
        /// </summary>
        public delegate void ErrorOccuredHandler();

        /// <summary>
        /// The server update event handler.
        /// </summary>
        private event ServerUpdateHandler UpdateArrived;

        /// <summary>
        /// The error occured event handler.
        /// </summary>
        private event ErrorOccuredHandler ErrorOccured;

        /// <summary>
        /// List to keep track of the keys being pressed.
        /// </summary>
        private List<string> keys;

        /// <summary>
        /// True if a beam attack was sent out, false otherwise.
        /// </summary>
        private bool beamAttack;

        /// <summary>
        /// Default constructor for Controller.
        /// </summary>
        public Controller()
        {
            // Initialize the world.
            world = new World();
            // Intialize control commands.
            commands = new ControlCommands();
            keys = new List<string>();
            beamAttack = false;
        }

        /// <summary>
        /// When an update arrives, the method OnFrame is called in Form1.
        /// </summary>
        /// <param name="suh"></param>
        public void RegisterServerUpdateHandler(ServerUpdateHandler suh)
        {
            UpdateArrived += suh;
        }

        /// <summary>
        /// When an error occurs, the ErrorOccured metod is called in Form1.
        /// </summary>
        /// <param name="eoh"></param>
        public void RegisterErrorOccuredHandler(ErrorOccuredHandler eoh)
        {
            ErrorOccured += eoh;
        }

        /// <summary>
        /// Method that gets the IP and player name, then trys to connect to the server.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="name"></param>
        public void Connect(string ip, string name)
        {
            playerName = name + "\n";
            Networking.ConnectToServer(OnConnect, ip, 11000);
        }

        /// <summary>
        /// When the form closes, this method is called.
        /// </summary>
        public void Exit()
        {
            if (theServer != null)
                // Disconnects from the server.
                theServer.TheSocket.Shutdown(SocketShutdown.Both);
        }

        /// <summary>
        /// Returns the world.
        /// </summary>
        /// <returns></returns>
        public World GetWorld()
        {
            return world;
        }

        /// <summary>
        /// Returns the players ID.
        /// </summary>
        /// <returns></returns>
        public int GetPlayerID()
        {
            return playerID;
        }

        /// <summary>
        /// Callback used when trying to connect to the server.
        /// </summary>
        /// <param name="ss"></param>
        private void OnConnect(SocketState ss)
        {
            // If an error occured, let the view know so it can display the error.
            if (ss.ErrorOccured)
            {
                ErrorOccured?.Invoke();
                return;
            }

            // Save the SocketState so we can use it to send messages.
            theServer = ss;
            // Sends the player name to the server.
            Networking.Send(theServer.TheSocket, playerName);

            // Start an event loop to receive messages from the server.
            ss.OnNetworkAction = ReceiveStartup;
            Networking.GetData(ss);

        }

        /// <summary>
        /// Callback to receive the startup information.
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveStartup(SocketState state)
        {
            // If an error occured, let the view know so it can display the error.
            if (state.ErrorOccured)
            {
                ErrorOccured?.Invoke();
                return;
            }

            // Gets and stores data sent from the server.
            string startupData = state.GetData();
            // Splits up the data so it can be parsed.
            string[] parts = Regex.Split(startupData, @"(?<=[\n])");

            // Get the player ID from the server.
            playerID = int.Parse(parts[0]);
            // Get the size of the world from the server.
            world.SetSize(int.Parse(parts[1]));
           
            // Continue the event loop.
            state.OnNetworkAction = RecieveWorld;
            Networking.GetData(state);
        }

        /// <summary>
        /// Callback to receive the world information.
        /// </summary>
        /// <param name="state"></param>
        private void RecieveWorld(SocketState state)
        {
            ParseData(state);
            Networking.GetData(state);
        }

        /// <summary>
        /// Goes through all the data sent by the server and parses it accordingly.
        /// </summary>
        /// <param name="state"></param>
        private void ParseData(SocketState state)
        {
            // Gets and stores data sent from the server.
            string totalData = state.GetData();
            // Splits up the data so it can be parsed.
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            //Console.WriteLine(totalData);

            // Loop through the array with data.
            foreach (string p in parts)
            {
                // Ignore empty strings added by the regex splitter
                if (p.Length == 0)
                    continue;
                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens.
                if (p[p.Length - 1] != '\n')
                    break;

              

                // If p is wall data, adds the wall to the world.
                if (p.Contains("wall"))
                {
                    Wall deserializedWall = JsonConvert.DeserializeObject<Wall>(p);
                    world.AddWall(deserializedWall);
                }
                // If p is tank data, add the tank to the world.
                else if (p.Contains("tank"))
                {
                    Tank deserializedTank = JsonConvert.DeserializeObject<Tank>(p);
                    // If the tank disconnected, removes it from the world.
                    if (deserializedTank.Disconnected())
                        world.RemoveTank(deserializedTank.GetID());
                    else
                        world.AddTank(deserializedTank);
                }
                // If p is powerup data, add the powerup to the world.
                else if (p.Contains("power"))
                {
                    Powerup deserializedPowerup = JsonConvert.DeserializeObject<Powerup>(p);
                    // If the powerup was collected, remove it from the world.
                    if (deserializedPowerup.WasCollected())
                        world.RemovePowerup(deserializedPowerup.GetID());
                    else
                        world.AddPowerup(deserializedPowerup);
                }
                // If p is projectile data, add the projectile to the world.
                else if (p.Contains("proj"))
                {
                    Projectile deserializedProjectile = JsonConvert.DeserializeObject<Projectile>(p);
                    // If the projectile hit something or died out, remove it from the world.
                    if (deserializedProjectile.GetDied())
                        world.RemoveProjectile(deserializedProjectile.GetID());
                    else
                        world.AddProjectile(deserializedProjectile);
                }
                // If p is beam data, add the beam to the world.
                else if (p.Contains("beam"))
                {
                    Beam deserializedBeam = JsonConvert.DeserializeObject<Beam>(p);
                    world.AddBeam(deserializedBeam);
                }

                // Then remove it from the SocketState's growable buffer
                state.RemoveData(0, p.Length);
            }

            // Convert the control commands to Json string.
            string com = JsonConvert.SerializeObject(commands) + "\n";
            // Send the control commands to the server.
            Networking.Send(theServer.TheSocket, com);

            // Makes sure only one beam attack is sent to the server per powerup.
            if (beamAttack)
            {
                commands.SetFiring("none");
                beamAttack = false;
            }

            // Informs the view that an update arrived.
            UpdateArrived?.Invoke();
        }

        /// <summary>
        /// Processes the key control command moving values when the key is pressed down.
        /// </summary>
        /// <param name="movement"></param>
        public void ProcessKeysDown(string movement)
        {
            if (!keys.Contains(movement))
                keys.Add(movement);

            // Only allows for 2 keys to be pressed.
            if (keys.Count < 3)
            {
                // Sets the moving value of the control commands.
                switch (movement)
                {
                    case "up":
                        commands.SetMoving(movement);
                        break;
                    case "down":
                        commands.SetMoving(movement);
                        break;
                    case "left":
                        commands.SetMoving(movement);
                        break;
                    case "right":
                        commands.SetMoving(movement);
                        break;
                }
            }
        }

        /// <summary>
        /// Processes the key control command moving values when the key is released.
        /// </summary>
        /// <param name="s"></param>
        public void ProcessKeysUp(string movement)
        {
            keys.Remove(movement);
            if (keys.Count == 0)
                commands.SetMoving("none");
            else
                commands.SetMoving(keys[keys.Count - 1]);
        }

        /// <summary>
        /// Processes the control command firing values when a mouse button is pressed down.
        /// </summary>
        /// <param name="button"></param>
        public void ProcessMouseDown(string button)
        {
            switch (button)
            {
                case "main":
                    commands.SetFiring(button);
                    break;
                case "alt":
                    beamAttack = true;
                    commands.SetFiring(button);
                    break;
            }
        }

        /// <summary>
        /// Processes the control command firing vlaue when a mouse button is released.
        /// </summary>
        public void ProcessMouseUp()
        {
            commands.SetFiring("none");
        }

        /// <summary>
        /// Processes the mouse location and makes it relative to the turret.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ProcessMouseLocation(double x, double y)
        {
            commands.SetTurretDirection(x - 400, y - 400);
        }

    }
}

