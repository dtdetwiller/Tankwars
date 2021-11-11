// Author: Daniel Detwiller & Warren Kidman
// Date: 11/25/2019
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// Represents a beam object in tankwars.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {
        /// <summary>
        /// int that holds the ID of the beam.
        /// </summary>
        [JsonProperty(PropertyName = "beam")]
        private int id;

        /// <summary>
        /// Vector2D that holds the origin of the beam.
        /// </summary>
        [JsonProperty(PropertyName = "org")]
        private Vector2D origin;

        /// <summary>
        /// Vector2D that holds the direction of the beam.
        /// </summary>
        [JsonProperty(PropertyName = "dir")]
        private Vector2D direction;

        /// <summary>
        /// int that holds the ID of the tank that fired it.
        /// </summary>
        [JsonProperty(PropertyName = "owner")]
        private int firingTankID;

        /// <summary>
        /// int to keep track of the frames so th beam can disappear.
        /// </summary>
        private int beamFrameCounter = 0;

        private static int nextID = 0;

        /// <summary>
        /// Constructor for beam.
        /// </summary>
        /// <param name="origin">Where the beam is coming from</param>
        /// <param name="direction">Turret direction</param>
        /// <param name="firingTankID">Tank that fired</param>
        /// <param name="world">Used to detect if beam hit a tank</param>
        public Beam(Vector2D origin, Vector2D direction, int firingTankID, World world)
        {
            id = nextID++;
            this.origin = origin;
            this.direction = direction;
            this.firingTankID = firingTankID;

            // when beam is fired, check if it hit a tank(s)
            world.BeamHitTank(origin, direction, 30, firingTankID);
        }

        /// <summary>
        /// Returns the ID of the beam.
        /// </summary>
        /// <returns></returns>
        public int GetID() { return id; }

        /// <summary>
        /// Return the origin of the beam.
        /// </summary>
        /// <returns></returns>
        public Vector2D GetOrigin() { return origin; }

        /// <summary>
        /// Returns the direction of the beam.
        /// </summary>
        /// <returns></returns>
        public Vector2D GetDirection() { direction.Normalize(); return direction; }

        /// <summary>
        /// Returns the ID of the tank firing the beam.
        /// </summary>
        /// <returns></returns>
        public int GetFiringTankID() { return firingTankID; }

        /// <summary>
        /// Returns the number of frames the beam has been showing for.
        /// </summary>
        /// <returns></returns>
        public int GetBeamFrameCount() { return beamFrameCounter; }

        /// <summary>
        /// Increments the beamFrameCounter by one each it is called.
        /// </summary>
        public void SetBeamFrameCount() { beamFrameCounter++; }

    }
}
