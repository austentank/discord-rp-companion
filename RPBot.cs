/**
 * Simple Discord RP Companion Bot.
 * Created by Austen Tankersley.
 * V 0.1 March 9th, 2023.
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

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        _client.Ready += Client_Ready;
        _client.SlashCommandExecuted += SlashCommandHandler;

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
    
    public async Task Client_Ready()
    {
        var rollDiceCommand = new SlashCommandBuilder().WithName("roll").WithDescription("Roll dice!")
            .AddOption("number", ApplicationCommandOptionType.Integer, "the number of dice to roll", isRequired: true)
            .AddOption("sides", ApplicationCommandOptionType.Integer, "the number of sides of the dice",
                isRequired: true).AddOption("plus", ApplicationCommandOptionType.Integer,
                "the number to add to the roll.", isRequired: true).AddOption("display-rolls",
                ApplicationCommandOptionType.Boolean, "whether or not to list all of the rolls made.",
                isRequired: false);
        
        try
        {
            await _client.CreateGlobalApplicationCommandAsync(rollDiceCommand.Build());
        }
        catch(HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
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
        
        if (numberOfDice <= 0)
        {
            await command.RespondAsync("*Cannot roll 0 or less dice.*");
        }

        if (numberOfSides <= 0)
        {
            await command.RespondAsync("*Cannot roll dice with number of sides less than 1.*");
        }
        
        for (int i = 0; i < numberOfDice; i++)
        {
            currRoll = random.Next(1, numberOfSides + 1);
            totalRoll = totalRoll + currRoll;
            rollList.Add(currRoll);
        }

        totalRoll = totalRoll + numberToAdd;
        
        outputString += $"Rolled {numberOfDice}d{numberOfSides}";
        if (numberToAdd < 0)
        {
            outputString += $" {numberToAdd}\n";
        }
        else
        {
            outputString += $" + {numberToAdd}\n";
        }

        outputString += $"Result: **{totalRoll}**\n";

        if (listRollsBool)
        {
            outputString += $"Rolls: {getRollList(rollList)}";
        }
        await command.RespondAsync(outputString);
    }
    
/// <summary>
/// Gets a string listing individual rolls.
/// </summary>
/// <param name="rollList"> The list of rolls that need to be displayed.</param>
/// <returns>A string listing each individual roll in the given list separated by " + ".</returns>
    private string getRollList(List<int> rollList)
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
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
    

}