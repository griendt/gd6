using System.Reflection;
using engine.Models;

namespace engine.Engine.Commands;

public static class CommandValidator
{
    public static void Validate(IEnumerable<Command> commands, World world)
    {
        var commandList = commands.ToList();
        foreach (var commandsByType in commandList.GroupBy(command => command.GetType())) {
            ValidateForType(commandsByType.Key, commandsByType.ToList(), world);
        }

        var constructs = commandList.Where(command => command is CreateConstructCommand)
            .Cast<CreateConstructCommand>()
            .ToList();
        
        ValidateForType(typeof(CreateConstructCommand), constructs, world);
    }

    private static void ValidateForType(Type T, IEnumerable<Command> commands, World world)
    {
        var validators = T
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(method => method.GetCustomAttributes(typeof(Validator), false).Length > 0);

        foreach (var validator in validators) {
            // Magic to dynamically generate generic lists of inner type equal to the command type
            var genericType = typeof(List<>).MakeGenericType(T);
            var constructorInfo = genericType.GetConstructor([])!;
            var collection = constructorInfo.Invoke(null);
            var adder = genericType.GetMethod("Add")!;
            foreach (var command in commands.Where(command => !command.Force)) {
                adder.Invoke(collection, [command]);
            }
                
            // Run the validator
            validator.Invoke(null, [collection, world]);
        }
    }


}