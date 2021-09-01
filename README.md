Iowa Systems
============
This is a simulation to represent Iowa's prairies before most of it was destroyed.
The simulation is displayed in a 3D world in which plants and animals interact with eachother in order to survive.
The simulation is not very accurate yet. 
Go to "Simulating Iowa's Praries" below to see what problems the simulation has.

#### Opening the Simulation
To run the IowaSystemsWindows version run Systems.exe.

To run the IowaSystemsWebGL version on a browser go to https://tuvus.github.io/iowa-systems-demo/.

#### Quick Simulation
To set up a basic simulation involving foxes, rabbits and grass launch the application follow these steps.
1. Launch the application
2. Click the "SetDefaultSimulation" button
3. Click the green "RunSimulation" button to begin
4. Now that the simulation has loaded use the buttons in the top left corner to adjust the simulation speed. Click the button with the ">" symbol to play the simulaiton.
5. The simulation will now run at 1 times speed. See using the simulation below to look around.

Seting up the Simulation
------------------------
There are multiple buttons that allow customization of the simulation. 
Once you are done setting up the simulation click the "RunSimulation" button.

The list in the middle shows what species and how many organisms will be spawned in the simulation. 
The first number in parenthesis shows the initial population of the species.
The seccond number in parenthesis will apear in every species that has seeds and will show the starting seed populaion.

To edit a species click on it, this will open a variant of the AddSpeciesPanel. 
To delete an individual species click the red "X" button beside it.

To Clear all species click on the red "Clear" button.

To Exit the simulation click the red "Quit" Button.

#### Default Simulations
There are two default simulations. 
* To simulate only plants click the "SetPlantSimulation" button.
* To simulate foxes, bunnies and grass click the "SetDefaultSimulation" button.

#### Species Customisation Panel
To add a custom species click the "AddSpecies" button. To edit a species click on the species button to edit it.
This will open up a new panel.
* The species dropdown will let you select which species preset you want to use. This will automatically set the rest of the stats to match the preset so choose this first.
* The species name is displayed in an input field, change it to change the species' name.
* The "PopulationCount" slider will set the initial population for the species.
* If the species uses seeds then a "SeedCount" will be shown that sets the initial seeds spawned.
* The three color sliders change the species' color. The color will be show on the image below the sliders.
* When you are finished editing press the green "CreateSpecies" button to create the species.
* If you want to scrap the species press the red "Cancel" button to scrap the species.

#### Simulation Settings Panel
To change the size or calculations press the "SimulationSettings" button.
This will open up a new panel.
* The "SimulationSpeed" input field determines the time that each calculation represents.
A faster speed does less calculations reletive to the simulation time, this allows the simulation to cover more time without require more prossesing power.
A lower speed does more calculations reletive to the simulation time, this allows the simulation to be more accurate.
* The "EarthSize" slider determines the size of the Earth.
* The "GraphRefreshRate" slider determines how quickly the graph refreshes in simulation time.
* The "SunRotationEffect" toggle toggles between plant growth modes. See "Growth and SunRotationEffect" below for more details.

---
Earth Size:

The size of the Earth does not greatly affect the preformance of the simulaiton.

The recommended earth size is 2000.

---

#### User Settings Panel
To change the graphical user settings press the "User Settings button". 
This will open up a new panel with varius settings. 
The settings will be changeable in the simulation as well.
* The "Render World" button toggles whether or not the Simulation will be rendered.
* The "Render Shadows" button toggles whether or not the Earth will be illuminated by the Sun.
* The "Render Sun" button toggles whether or not the Sun will be displayed in the Simulation.
* The "Render Skybox" button toggles whether or not the skybox will be displayed.
* The "Quality Settings" dropdown changes what level of graphics will be used.
* The "Frames Per Seccond" input field controlls the disired update rate of the application.

Using the Simulation
--------------------
The top right corner displays each of the species colors and how many organisms are alive. 
Plant Species that have seeds show the number of seeds that they have in parenthesis beside the ogranism number.

#### Manipulating the Simulation Camera
These keys on your keyboard can be pressed to look around the 3D Earth.
* The W, A, S, D or the Arrow keys to controll the camera's movement around the Earth.
* The Q, E or pageup, pagedown keys controll the camera's rotation.
* Scrolling in or out will zoom the camera relative to the Earth.

To move the camera using only the mouse click the buttons in the bottom right corner.
* The Bottom-Left, Center and Bottom-Right arrow buttons controll the camera's movent.
* The Top-Left and TopRight arrow buttons controll the camera's Rotation.
* Clicking and dragging the circle on the slider above the arrow buttons will controll zooming the camera.

#### Controlling the Simulation Speed
To change the simulation speed you can either click the three buttons in the top left corner or use their hotkeys. 
If the simulation cannot handle the desired speed within it's refresh rate it will display the actuall simulation update rate.
If the simulation update rate is too low consider lowering your desired frames per seccond or increasing the simulation speed.
* The "II" button pauses the simulation. Alternatively press the "/" key.
* The "<" button slows the simulation by one step. Alternatively press the "," key.
* The ">" button speeds up the simulation by one step. Alternatively press the "." key.

#### Bottom-Right Button Group
Extra buttons are displayed on the bottom-right of the screen.
* The "DisplayGraph" button opens up a graph panel. Alternatively press the "tab" key.
* The "UserSettings" button opens up the user settings panel as explained above.
* The red "EndSimulation" button stops the simulation and goes back to the setup screen. Alternatively press the "Esc" key.

#### Population Graph Panel
The population graph panel displays the population count of each species over time. 
On the left hand side you can see the scale of the populations and on the bottom you can see the time.

As the graph expands to the right you will need to move the viewport over to see other parts of the graph.

The graph will automatically update the scale and all points on it if a population equals or is more than the max number.

The simulation update speed can be changed in the simulation settings panel as shown above.

Simulating Iowa's Prairie 
-------------------------
Because Iowa's Prairies were converted into farm fields shortly after Americans began colonising the Louisiana Territory there was not much information on the population of local wildlife.

This simulation attempts to simulate the population levels by a bottom-up proccess: filling in variables according to the specifics of each organism. 
Instead of a top-down proccess: aligning the organism's variables so that it matches a representative graph of it's population.

---
General problems with the accuracy of the simulation:

Grass will scale infinatly.
Foxes reproduce too quickly and outscale and overhunt bunnies and then die off.

---

#### Simulating Plants
Plants are simulated by accumalating growth from the sun according to how much sun it gathers. 
Once they reach a certain growth they can grow their awns, which then spread their seeds around.
Plants can be eaten by bunnies.

Note: Plants are not affected by bottom-up regulation yet. 
Meaning they don't compete for reasources and will scale infinitely.

---
Seeds:

Seeds are spawned by a seperate variable at start that can be edited. 
The humidity on the earth is randomly changed each calculation. 
Seeds will germinate after a certain time and above a certain humidity.

If a seed fails to germinate within a certain time, it will die off.

Seeds are dispersed relative to their parents using random values and a dispersal range variable.

---
Growth and SunRotationEffect:

With SunRotationEffect turned on plants on the close side of the earth will grow at a rate of 1, and plants halfway between the closest and farthest part on earth will get no growth.

Note: Plants at the poles will always be at a disadvantage if SunRotationEffect is on.

Without SunRotationEffect turned on all plants will grow at a rate of .5 equally.

Plants currently have only two stages: Growing the plant itself and growing the awns, the plant's reproductive organ.
Roots, leaves and vegatative propagation have not yet been implemented.

---
Death:

The only way for grass to die is for it to be eaten by an animal.
Drought and recource competition has not been added yet.

---

#### Simulating Animals
Animals are simulated by growing over time and are required to eat food in order to stay alive.
The animal's actions are calculated using multiple processors to speed up the calculation time.
Then after all calculations are completed the animals will update on the main thread using the data that the processors comupted.

Animals have a set age that they will die at.

When animals die they spawn a corpse that will diteriorate after some time.

Here is a list of behaviors that the animals can display in the order of their priority:
* Check if there is a predators nearby the animal, if so run away.
* Check if the animal can eat food. (The food must be right in front of the animal)
* Check if the animal is hungry, if so and there is a source food nearby go to the food source.
* Check if the animal has a mate, if so attempt at reproduction. (Distance to mate is not a factor yet)
* Check if there is an eligible mate nearby, if so set them as mate. (There is not yet any compitition for mating, first come first served basically)
* Check if the animal is hungry, if so explore. If not and the animal has a mate and is ready to reproduce move to the mate. Otherwise sit still.

Note: Wait time in animals are no longer implamented but may be readded in the future.

---

Hunting:

Animals that bite other animals do not instantly kill them. 
The movement speed of the animal being bitten is reduced after each bite.
This makes it so that after the first bite the animal is much easier to catch and will surely get bitten again.
How many bites it takes to kill an animal depends on the prey's health and the predator's bite size.

Animals will prioritise the corpses of their prey over living prey in order to reduce overhunting. They will also target closer food sources rather than farther away ones.

---
Reproduction:

Animals are given a sex at birth and can only mate with the opposite sex.
Animals must be above a certain age to be capable of reproducing.
After an animal is concieved it has to survive until it is ready to give birth.
How many offspring are created depend on the birth amount and the birth succsses percent for each offspring.

Note: Currently there is no parenting behavior. Newborns will act as adults, except that they won't be able reproduce right away.

---

Food and Fullness:

Animals have a maxFood value and cannot store food above that value.

Animals will not eat food infront of them if they are over 90% of their stored food capacity.

The fullFood variable determines whether or not the animal will activily search for food.
The fullFood stat varies for each individual organism, calculated from a random number between 60 and 80 percent of the max food that they can eat.

---

Awareness:

Animals search their area using their eyes and smell to see predators and prey.
Bunnies have two eyes on either side and have a lower sight range while foxes only have one eye but have a higher sight range.
Smell is centered around the animal and goes in every direction.
Bunnies have a better sense of smell then foxes.

Note: Hearing is not implamented yet although there are some inactive scripts in the build.
---