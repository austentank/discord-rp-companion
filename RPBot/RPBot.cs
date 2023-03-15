/**
 * Simple Discord RP Companion Bot.
 * Created by Austen Tankersley.
 * V 0.2 March 14th, 2023.
 */

using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;



public class RPBot
{
    
    private DiscordSocketClient _client;
    public static Task Main(string[] args) => new RPBot().MainAsync();

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();

        _client.Log += Log;

        var token = System.IO.File.ReadAllText(@"C:\Apps\Azemalolth\token.txt"); //read bot token from file.

        await _client.LoginAsync(TokenType.Bot, token); //login bot using token
        await _client.StartAsync(); //start connection
        _client.Ready += Client_Ready;
        _client.SlashCommandExecuted += SlashCommandHandler;

        // Prevent MainAsync from completing until program is closed
        await Task.Delay(-1);
    }
    
    public async Task Client_Ready()
    {
        //declare basic roll options.
        var numberOption = CreateIntOption("number", "the number of dice to roll", true, 1, 300);
        var sidesOption = CreateIntOption("sides", "the number of sides of the dice", true, 1, 1000);
        var plusOption = CreateIntOption("plus", "the number to add to the roll", true, -1000, 1000);
        SlashCommandOptionBuilder[] rollOptions = {numberOption, sidesOption, plusOption};

        //declare roll command
        var rollDiceCommand = new SlashCommandBuilder().WithName("roll").WithDescription("Roll dice!")
            .AddOptions(rollOptions).AddOption("display-rolls",
                ApplicationCommandOptionType.Boolean, "whether or not to list all of the rolls made.",
                isRequired: false);
        
        //declare analyze command
        var analyzeRollCommand = new SlashCommandBuilder().WithName("analyze")
            .WithDescription("Get min, max, and average for a roll.").AddOptions(rollOptions);
        
        try
        {
            //create slash commands
            await _client.CreateGlobalApplicationCommandAsync(rollDiceCommand.Build());
            await _client.CreateGlobalApplicationCommandAsync(analyzeRollCommand.Build());
        }
        catch(HttpException exception)
        {
            //serialize exception to json
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            
            //write error to console
            Console.WriteLine(json);
        }
    } 
    
    private async Task SlashCommandHandler(SocketSlashCommand command)
    {

        switch(command.Data.Name)
        {
            case "roll":
                await HandleRollCommand(command);
                break;
            case "analyze":
                await HandleAnalyzeCommand(command);
                break;
        }
    }

    private async Task HandleRollCommand(SocketSlashCommand command)
    {
        
        var numberOfDice = (int)(long)command.Data.Options.First().Value;
        var numberOfSides = (int)(long)command.Data.Options.ElementAt(1).Value;
        var numberToAdd = (int)(long)command.Data.Options.ElementAt(2).Value;
        var listRollsBool = true;

        if (command.Data.Options.Count == 4)
        {
            listRollsBool = (bool)command.Data.Options.ElementAt(3).Value;
        }
        
        Random random = new Random();
        var totalRoll = 0;
        var currRoll = 0;
        List<int> rollList = new List<int>();
        var outputString = "";
        
        //generate rolls
        for (int i = 0; i < numberOfDice; i++)
        {
            currRoll = random.Next(1, numberOfSides + 1);
            totalRoll = totalRoll + currRoll;
            rollList.Add(currRoll);
        }

        totalRoll = totalRoll + numberToAdd;
        
        outputString += $"Rolled {numberOfDice}d{numberOfSides}";
        
        //check if number to add is negative for formatting purposes
        outputString += GetPlusOrMinusString(numberToAdd) + "\n";

        outputString += $"Result: **{totalRoll}**\n";

        if (listRollsBool)
        {
            outputString += $"Rolls: `{GetRollList(rollList)}`";
        }
        await command.RespondAsync(outputString);
    }

    private async Task HandleAnalyzeCommand(SocketSlashCommand command)
    {
        var numberOfDice = (long)command.Data.Options.First().Value;
        var numberOfSides = (long)command.Data.Options.ElementAt(1).Value;
        var numberToAdd = (long)command.Data.Options.ElementAt(2).Value;

        var outputString = $"Analyzed {numberOfDice}d{numberOfSides}";
        outputString += GetPlusOrMinusString((int)numberToAdd) + "...\n";
        
        outputString += $"Minimum roll: `{(numberOfDice + numberToAdd)}`\n";
        outputString += $"Maximum roll: `{((numberOfDice * numberOfSides) + numberToAdd)}`\n";
        outputString += $"Average roll: `{(numberOfDice * (numberOfSides + 1.0)/2.0) +numberToAdd}`";

        await command.RespondAsync(outputString);
    }
    
/// <summary>
/// Gets a string listing individual rolls.
/// </summary>
/// <param name="rollList"> The list of rolls that need to be displayed.</param>
/// <returns>A string listing each individual roll in the given list separated by " + ".</returns>
    private string GetRollList(List<int> rollList)
    {
        var rollListString = "";

        for (int i = 0; i < rollList.Count; i++)
        {
            rollListString += $"{rollList.ElementAt(i)}";
            if (i != rollList.Count - 1)
            {
                rollListString += " + ";
            }
        }
        return rollListString;
    }

/// <summary>
/// Gets a string with an addition or subtraction operation dependent on whether the given number is negative or positive.
/// </summary>
/// <param name="numberToAdd">The number in the addition or subtraction operation.</param>
/// <returns>A string with the correct operation.</returns>
public static string GetPlusOrMinusString(int numberToAdd)
{
    var outString = "";
    if (numberToAdd < 0)
    {
        outString += $" - {numberToAdd * -1}";
    }
    else
    {
        outString += $" + {numberToAdd}";
    }

    return outString;
}
    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    /// <summary>
    /// Creates an option of input type int with basic properties.
    /// </summary>
    /// <param name="name">option name.</param>
    /// <param name="description"> option description.</param>
    /// <param name="required">Whether option is required for command.</param>
    /// <param name="minVal">Minimum value of the option.</param>
    /// <param name="maxVal">Maximum value of the option.</param>
    /// <returns></returns>
    private static SlashCommandOptionBuilder CreateIntOption(string name, string description, bool required, int minVal, int maxVal)
    {
        var option = new SlashCommandOptionBuilder();
        
        //set option properties
        option.Name = name;
        option.Description = description;
        option.Type = ApplicationCommandOptionType.Integer;
        option.IsRequired = required;
        option.MinValue = minVal;
        option.MaxValue = maxVal;
        return option;
    }

}