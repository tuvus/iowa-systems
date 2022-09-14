Iowa Systems
============
This is a simulation to represent Iowa's prairies before most of it was destroyed.
The simulation is displayed in a 3D world in which plants and animals interact with each other in order to survive.
The simulation is not very accurate yet. 
Go to "Simulating Iowa's Prairies" below to see what problems the simulation has.

#### Opening the Simulation
To run the IowaSystemsWindows version run Systems.exe.

To run the IowaSystemsWebGL version on a browser go to https://tuvus.github.io/iowa-systems-demo/.

#### Quick Simulation
To set up a basic simulation involving foxes, rabbits and grass launch the application follow these steps.
1. Launch the application
2. Click the "SetDefaultSimulation" button
3. Click the green "RunSimulation" button to begin
4. Now that the simulation has loaded, use the buttons in the top left corner to adjust the simulation speed. Click the button with the ">" symbol to play the simulation.
5. The simulation will now run at 1 times speed. See using the simulation below to look around.

Setting up the Simulation
------------------------
There are multiple buttons that allow customization of the simulation. 
Once you are done setting up the simulation click the "RunSimulation" button.

The list in the middle shows what species and how many organisms will be spawned in the simulation. 
The first number in parenthesis shows the initial population of the species.
The second number in parenthesis will appear in every species that has seeds and will show the starting seed population.

To edit a species click on it, this will open a variant of the AddSpeciesPanel. 
To delete an individual species click the red "X" button beside it.

To Clear all species click on the red "Clear" button.

To Exit the simulation click the red "Quit" Button.

#### Default Simulations
There are two default simulations. 
* To simulate only plants click the "SetPlantSimulation" button.
* To simulate foxes, rabbits and grass click the "SetDefaultSimulation" button.

#### Species Customisation Panel
To add a custom species click the "AddSpecies" button. To edit a species click on the species button to edit it.
This will open up a new panel.
* The species dropdown will let you select which species preset you want to use. This will automatically set the rest of the stats to match the preset so choose this first.
* The species name is displayed in an input field, change it to change the species' name.
* The "PopulationCount" slider will set the initial population for the species.
* If the species uses seeds then a "SeedCount" will be shown that sets the initial seeds spawned.
* The three color sliders change the species' color. The color will be shown on the image below the sliders.
* When you are finished editing press the green "CreateSpecies" button to create the species.
* If you want to scrap the species press the red "Cancel" button to scrap the species.

#### Simulation Settings Panel
To change the size or calculations press the "SimulationSettings" button.
This will open up a new panel.
* The "SimulationSpeed" input field determines how many hours are represented in each iteration of the simulation.
A higher value does less calculations relative to each year, this allows the simulation to cover more time without requireing more processing power.
A lower value does more calculations relative to each year, this allows the simulation to be more accurate.
* The "EarthSize" slider determines the size of the Earth.
* The "GraphRefreshRate" slider determines how quickly the graph refreshes in simulation time.
Displayed in years, months, weeks, days and hours.
* The "SunRotationEffect" toggle between plant growth modes. See "Growth and SunRotationEffect" below for more details.

---
Earth Size:

The size of the Earth does not greatly affect the performance of the simulation.

The recommended earth size is 2000 or greater.
A bigger earth size requires higher food populations for animals because they encounter food less often.

---

#### User Settings Panel
To change the graphical user settings press the "User Settings button". 
This will open up a new panel with various settings. 
The settings will be changeable in the simulation as well.
* The "Render World" button toggles whether or not the Simulation will be rendered.
* The "Render Shadows" button toggles whether or not the Earth will be illuminated by the Sun.
* The "Render Sun" button toggles whether or not the Sun will be displayed in the Simulation.
* The "Render Skybox" button toggles whether or not the skybox will be displayed.
* The "Quality Settings" dropdown changes what level of graphics will be used.
* The "Frames Per Second" input field controls the desired update rate of the application.

Using the Simulation
--------------------
The top right corner displays each of the species colors and how many organisms are alive. 
Plant Species that have seeds show the number of seeds that they have in parenthesis beside the organism number.

#### Manipulating the Simulation Camera
These keys on your keyboard can be pressed to look around the 3D Earth.
* The W, A, S, D or the Arrow keys to control the camera's movement around the Earth.
* The Q, E or pageup, pagedown keys control the camera's rotation.
* Scrolling in or out will zoom the camera relative to the Earth.

To move the camera using only the mouse click the buttons in the bottom right corner.
* The Bottom-Left, Center and Bottom-Right arrow buttons control the camera's movement.
* The Top-Left and TopRight arrow buttons control the camera's Rotation.
* Clicking and dragging the circle on the slider above the arrow buttons will control zooming the camera.

#### Controlling the Simulation Speed
To change the number of hours the simulated per second you can either click the three buttons in the top left corner or use their hotkeys. 
The top left text box shows how many hours are currently being simulated per seccond.
The target text box shows how many target hours, days, months, weeks, years the simulation is trying to reach in a second.
* The "II" button pauses the simulation. Alternatively press the "/" key.
* The "<" button slows the simulation by one step. Alternatively press the "," key.
* The ">" button speeds up the simulation by one step. Alternatively press the "." key.

Note: If the top left box value is lower than the target the simulation is not capable of calculating quickly enough.

#### Bottom-Right Button Group
Extra buttons are displayed on the bottom-right of the screen.
* The "DisplayGraph" button opens up a graph panel. Alternatively press the "tab" key.
* The "UserSettings" button opens up the user settings panel as explained above.
* The red "EndSimulation" button stops the simulation and goes back to the setup screen. Alternatively press the "Esc" key.

#### Population Graph Panel
The population graph panel displays the population count of each species over time. 
On the left hand side you can see the scale of the populations and on the bottom you can see the time.
As the graph expands to the right you will need to move the viewport over to see other parts of the graph.

* The Left scrollbar controlls the scope of the graph. 
Moving it up will show less points and moving it down will show more points.
* The Bottom scrollbar controlls the position of the points.
It is only interactable if the points are going off the graph

The graph will automatically update the scale and all points on it if a population equals or is more than the max number.

The simulation update rate can be changed in the simulation settings panel as shown above.

Simulating Iowa's Prairie 
-------------------------
Because Iowa's Prairies were converted into farm fields shortly after Americans began colonizing the Louisiana Territory there was not much information on the population of local wildlife.

This simulation attempts to simulate the population levels by a bottom-up process: filling in variables according to the specifics of each organism. 
Instead of a top-down process: aligning the organism's variables so that it matches a representative graph of its population.

---
General problems with the accuracy of the simulation:
Grass will scale infinitely.
Foxes move too quickly and over hunt rabbits and then die off.
Foxes and rabbits always move at a running speed instead of a normal walking speed.
Plants die off completely when fully eaten when in reality they would have a chance to grow back with thier stored resources.

---

#### Time scale
Each update of the simulation equals one hour.
Decreasing the simulation speed will in the simulation settings panel (in theory) result in a more accurate simulation but uses more processing power per unit of time.

#### Zones
The earth is split up somewhat evenly into zones, by default this is 20. 
Plants will only check in thier zone and neighboring zones for resource competition. (not implemented yet)
In the future each zone will have its own temperature, water and sunlight.

Animals only check for prey in their zone and neighboring zones.
When an animal moves it only checks feasible neighboring zones to see if it has changed zones.

#### Simulating Plants
Plants are simulated by accumulating growth from the sun according to how much sun it gathers. 
Once they reach a certain growth they can grow their awns, which then spread their seeds around.
Plants can be eaten by rabbits but thier seeds can't.
When a plants stem is fully eaten it dies off.

Note: Plants are not affected by bottom-up regulation. 
Meaning they don't really compete for resources and will scale infinitely.

---
Seeds:

The initial starting seed population is governed by a separate variable at start that can be edited.
Seeds will form a seed bank protecting the species from overgrasing from the rabbits
The humidity on the earth is randomly changed each calculation. 
Seeds will germinate after a certain time and above a certain humidity.

If a seed fails to germinate within a certain time, it will die off.

Seeds are dispersed relative to their parents using random values and a dispersal range variable.

---
Growth and SunRotationEffect:

With SunRotationEffect turned on, plants on the close side of the earth will grow at a rate of 1, and plants halfway between the closest and farthest part on earth will get no growth.

Note: Plants at the poles will always be at a disadvantage if SunRotationEffect is on.

Without SunRotationEffect turned on all plants will grow at a rate of .5 equally.

Plants currently have 7 stages: 
 * Dead, the plant object is not currently in use
 * Seed, the plant is in it's seed stage and must check for germination
 * Germinating, the plant is growing roots
 * Sprout, the plants stem has breached the surface
 * Seedling, a tiny blade and stem but with most of the growth going to the roots
 * Youngling, growing stem, blade and roots
 * Adult, fully mature and is growing awns or spreading seeds

---
Death:

The only way for grass to die is for it to be eaten by an animal.
Drought and resource competition has not been added yet.

---

#### Simulating Animals
Animals are simulated by growing over time and are required to eat food in order to stay alive.
The animal's actions are calculated using multiple processors to speed up the calculation time.
Then after all calculations are completed the animals will update on the main thread using the data that the processors computed.

Animals have a set age that they will die at.

When animals die they spawn a corpse that will deteriorate after some time.
Corpses can be eaten by a predetor species.

An animal is considered full if they have 90% food in their somach and hungry if they are below 70%.

Here is a list of behaviors that the animals can display in the order of their priority:
* Check if there are predators nearby the animal, if so run away.
* Check if the animal can eat food. (The food must be right in front of the animal)
* Check if the animal is hungry, if so and there is a source food nearby go to the food source.
* Check if the animal is not hungry and if there is an eligible mate nearby, if so attempt at reproduction with them.
* Check if the animal is hungry, if so explore. If not and the animal has a mate and is ready to reproduce, move to the mate. Otherwise sit still.

Note: Wait time in animals is no longer implemented except for eating but may be readded in the future.

An animal that tries to eat an organism or find a mate but was beaten by another animal in the same frame the animal will explore or sit still instead.

---

Hunting:

Animals that bite other animals do not instantly kill them. 
The movement speed of the animal being bitten is reduced after each bite.
This makes it so that after the first bite the animal is much easier to catch and will surely get bitten again.
How many bites it takes to kill an animal depends on the prey's health and the predator's bite size.

Animals will prioritize the corpses of their prey over living prey in order to reduce overhunting. 
They will also target closer food sources rather than farther away ones.

---
Reproduction:

Animals are given a sex at birth and can only mate with the opposite sex.
Animals must be above a certain age to be capable of reproducing.
After an animal is conceived  it has to survive until it is ready to give birth.
How many offspring are created depend on the birth amount and the birth success percent for each offspring.

Note: Currently there is no parenting behavior. 
Newborns will act as adults, except that they won't be able to reproduce right away.

---

Food and Fullness:

Animals have a maxFood value and cannot store food above that value.

Animals will not eat food in front of them if they are over 90% of their stored food capacity.

The fullFood variable determines whether or not the animal will actively search for food.

Once an animal starts starving it will lose health and slow down until it dies.

The amount of food in a corpse varies on how much food the animal had while it was living and it's body weight.

---

Awareness:

Animals search their area using their eyes and smell to see predators and prey.
Bunnies have two eyes on either side and have a lower sight range while foxes only have one eye but have a higher sight range.
Smell is centered around the animal and goes in every direction.
Bunnies have a better sense of smell then foxes.

---

## Authors
<hr>

[tuvus](https://github.com/tuvus/) - 
    **Oskar Niesen** <<oskar-github@niesens.com>> (he/him)

## License
<hr>
The Iowa Systems project is licensed under [GNU GPL v3](https://github.com/tuvus/iowa-systems/blob/master/LICENSE.md).
