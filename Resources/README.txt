README TEXT FILE FOR PS8/PS9: CS 3500
WRITTEN BY: Warren Kidman / Daniel Detwiller
SUBMISSION DATE: December 5, 2019

PS8 - TankWars

PROJECT SUMMARIES:

- GameController:
 	- Contains logic for parsing the data received by the server and updating the model accordingly. 
 	- Contains the logic for key presses and mouse position. Anything sent to/from the server is 
 	  handled in GameController.

- Model:
	- Represents the state of our ‘world’ such as the world itself, the tanks, powerups, walls, 
	  projectiles, and beams. Any models, such as tanks, projectiles, etc. in world can be used by 
	  both the client and the server. 

- View
	- View, the GUI creates what the player sees. Form1 allows the user to interact with the GUI 
	  and implements buttons, labels, key press events, etc. 
	- The DrawingPanel is what ‘paints’ the world for the user to see and updates 60 frames per second.

- In short:
	- The View paints what/how the controller interacts with the model. 

DESIGN:

 -  NOV 13:
	- Set up the framework for the project in visual studios. Created all the projects, classes, 
	  added references etc.
	- Started adding code to drawing panel, creating and initializing constants.

- NOV 19:
	- Started writing code that allowed for establishing a connection to a server.
	- Added methods in the world class that allow for the creation and addition of new objects to the 
	  world such as tanks, walls, projectiles, etc. This allows for controller to add and update 
	  these objects to the model once they are received from the server.
	- Working on receiving and parsing the data from the server. Once data is received and parsed, it can be updated in the model. 
	- Working on key presses. 

- NOV 20:
	- Added ControlCommands class that holds the state of all the control features such as moving, firing etc.
	- Working on Deserializing world objects.
	- Fixed some errors that occured when parsing data.
	- Created Dictionaries that hold world objects like tanks so that our client can hold onto information about objects sent 
	  from the server in controller. Lets us know what, and how many of each object are in the world.
	- Trying to get DrawingPanel to pop up when client is connected to server

- NOV 21:
	- Got drawing panel to pop up when connected to server. 
	- OnPaint now draws things, for now we are just drawing a rectangle.
	- Now OnPaint draws the walls, but all in one spot. 
	- We are now drawing the walls but they are in the wrong spots, figured out a way to fill in the walls between point 1 and 
	  point 2 sent from the server
	- Got orientation correct, the tanks and walls are painted where they should be.
	- Got movement controls working, whenever key down events occur we tell the controller to send the server control commands 
	  that represent what key the user pressed.
	- Added names under tanks

- NOV 22: 
	- Got turret to rotate based on mouse position. Using vector2d class provided to us and the mouse position
	- Projectile direction and mouse clicks fixed so that projectiles are facing the same direction as the turret when mouse 
	  down occurs.
	- Added health bar under tank, done in DrawingPanel
	- Once tank is dead, or its HP is equal to 0, the tank ‘disappears’ or in other words the DrawingPanel stops drawing the 
	  tank until its health gets reset. We also stop drawing the player name and health bar until the health gets reset.
	- Added a help menu in Form1 that opens a dialog box that shows the user the control information.
	- Started the process of animating the explosion that occurs when the tank is ‘killed’.
	- Error messages pop up when an error occurs when trying to connect
	- Beam now shows up when the user collects a power up and uses right click to fire the beam; however, the beam does not 
	  disappear once fired.
	- Animation runs more than one cycle, needs to be fixed.

NOV 24:
	- Beam now disappears and the explosion animation only runs for one cycle. We had to count the amount of frames from when 
	  the beam was fired and then stop drawing the beam once a certain amount of frames had passed.
	- Fixed an error where an error message would not pop up when the client tried to connect to localhost when the localhost 
	  server isn't up and running. Changed the way we went about handling errors and used an event handler to tell the form 
	  when to show error messages. 
	- Finalized project, commented code that needed to be commented and fixed incorrect comments. Removed unnecessary code, etc.

NOTE TO GRADER:
	- You can use arrow keys to move. (For lefties)
	- Holding down left click continually fires projectiles.
	- Thank you for grading :-)


----------------------------------------------------------------------------------------------------------------------------------

 PS9 - TankWars [Server]

 PROJECT SUMMARIES:

- Server:
 	- The server is a standalone program that can run on a separate machine from any client. 
	- The server keeps the world up-to-date on every frame. 
	- It is up to the server to determine how often frames "tick".
	- Reads XML setting file to determine rules / settings for game
	- Starts an event-loop that listens for TCP socket connections from clients
	- Handles disconnects / malformed client data
	- Starts an event loop that handles http requests from web browsers (port 80)
	
- Web Server:
	- Supports a couple of basic http requests described below and return web pages with the requested information.
	- GET/games; Returns information about every game ever played, organized one game per table. 
	- Above each table is the game ID and the duration in seconds. 
	- The table includes one row per player in the game, and three columns: player name, score, and accuracy.
	- GET/games?player name; Return a single table containing the records for the games a given player has played in 
	- This table should contain one row for each game the player was in, and three columns: the game ID, 
	  the player's score in that game, and the player's accuracy in that game.

- View Webserver;
	- Provided by examples repository CS 3500

- In short:
	- The server allows for connections from multiple clients and sends the webserver information about games played.

 DESIGN:

 DEC 2:
	- Set up server project
	- Added getters and setters for model classes such as tanks, projectiles, powerups, etc.
	- Created a method that reads in settings.xml file and applies those settings to the world
	- Started establishing connection and sending clients data.

  DEC 3:
	- Update method added to update clients once per frame
	- Got update method working
	- Got movement controlls to work
	- Wall collisions method added
	- Issue where clients would control eachothers tanks was resolved
	- Powerups now spawn and get deleted when a tank 'collects' them

  DEC 4:
	- Had an issue where removeData wasnt working, fixed it but removing a Networking.getdata();
	- Collisions for projectiles working. If they hit a wall or tank, get set to dead and disappear.
	- Beams firing correctly
	- Started implementing database controller
	- Tanks die and respawn in random location when killed
	- Have an issue with the example client, once a proj is fied the client freezes but still
	  accepts / executes commands on other clients.

  DEC 5:
	- Fixed an issue with null vectors
	- Still implementing database
	- Fixed an issue with getdata when deserializing
	- Created a method that allows tanks to wrap-around once they have hit the edge of a map
	- Fixed issue where example client, the JSON property for turret direction was set as
	  'tdir' instead of 'dir', changing this one character fixed what took hours to debug.
	- Fixed issue with parsing in database
	- Database and webserver working.


  NOTE TO GRADER:
	- No extra features added
	- Accuracy doesnt account for beams going through two+ tanks, accuracy is for hits or misses.
	- If the beam hits, it counts as a hit but it doesnt matter how many it hits. 
	- Thank you for grading.




		




		
