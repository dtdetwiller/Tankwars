// Author: Daniel Detwiller & Warren Kidman
// Date: 11/25/2019
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{
    /// <summary>
    /// Class that holds the state of each control feature.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ControlCommands
    {
        /// <summary>
        /// String that holds the possible moving values.
        /// </summary>
        [JsonProperty(PropertyName = "moving")]
        private string moving;

        /// <summary>
        /// String that holds the possible firing values.
        /// </summary>
        [JsonProperty(PropertyName = "fire")]
        private string firing;

        /// <summary>
        /// Vector2D that holds the turret direction.
        /// </summary>
        [JsonProperty(PropertyName = "tdir")]
        private Vector2D turretDirection;
        
        /// <summary>
        /// Constructor that initializes the default states each control feature.
        /// </summary>
        public ControlCommands()
        {
            moving = "none";
            firing = "none";
            turretDirection = new Vector2D();
        }

        /// <summary>
        /// Sets the moving value to a specified possible value: "none", "up", "left", "down", "right".
        /// </summary>
        public void SetMoving(string value)
        {
            moving = value;
        }

        /// <summary>
        /// Sets the firing value to a specified possible value: "none", "main", (for a normal projectile) and "alt" (for a beam attack).
        /// </summary>
        public void SetFiring(string value)
        {
            firing = value;
        }

        /// <summary>
        /// Sets the direction of the turret by taking in the mouse position x and y values.
        /// </summary>
        public void SetTurretDirection(double x, double y)
        {
            turretDirection = new Vector2D(x, y);
            // Normalize the vector.
            turretDirection.Normalize();
        }

        /// <summary>
        /// Gets the moving command sent by the client.
        /// </summary>
        /// <returns></returns>
        public string GetMoving()
        {
            return moving;
        }

        /// <summary>
        /// Gets the firing command sent by the client.
        /// </summary>
        /// <returns></returns>
        public string GetFiring()
        {
            return firing;
        }

        /// <summary>
        /// Gets the direction of the turret sent by the client.
        /// </summary>
        /// <returns></returns>
        public Vector2D GetTurretDirection()
        {
            return turretDirection;
        }

    }
}
