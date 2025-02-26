using System.Reflection;
using engine.Models;

namespace engine.Engine.Commands;

public static class CommandValidator
{
    public static void Validate(IEnumerable<Command> commands, World world)
    {
        foreach (var commandsByType in commands.GroupBy(command => command.GetType())) {
            var commandType = commandsByType.Key;
            
            var validators = commandType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(method => method.GetCustomAttributes(typeof(Validator), false).Length > 0);

            foreach (var validator in validators) {
                // Magic to dynamically generate generic lists of inner type equal to the command type
                var genericType = typeof(List<>).MakeGenericType(commandType);
                var constructorInfo = genericType.GetConstructor([])!;
                var collection = constructorInfo.Invoke(null);
                var adder = genericType.GetMethod("Add")!;
                foreach (var command in commandsByType.Where(command => !command.Force)) {
                    adder.Invoke(collection, [command]);
                }
                
                // Run the validator
                validator.Invoke(null, [collection, world]);
            }
        }
    }


}