# DISCORD RP COMPANION



A roleplay companion discord bot written in C#.

## GETTING STARTED:

First, install all Discord.Net packages via NuGet. Afterwards:

- Clone the repository
- Open the .sln in your IDE
- Open RPBot.cs
- Change the filepath for the variable named "token" to a file containing your bot's login token for secure authentication
- Compile and run RPBot.cs

Your bot should then come online and have the ability to use the slash commands.


## COMMANDS:

- /roll - rolls dice. User specifies number of dice, number of sides, number to add to the roll, and whether or not to display all roles (default true).
- /analyze - analyzes a roll, giving min, max, and average for the configuration of sides, number of dice, and addition.


## CHANGELOG:

### v.0.2: 3/14/2023
- added /analyze
- changed roll options to have minimum and maximum values.
- cleaned up some of the code and added private methods with introduction of /analyze to improve code readability and to prevent code duplication.
- improved formatting

### v.0.1: 3/9/2023

- added /roll

## DEPENDENCIES:

- Discord.Net v3.9.0
- Discord API v10
