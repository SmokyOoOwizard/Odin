using Odin.Contexts;

namespace Odin.Systems;

public class SystemsExecutor
{
    private readonly ISystemInstaller[] _installers;

    public SystemsExecutor(ISystemInstaller[] installers)
    {
        _installers = installers.OrderBy(c => c.Priority).ToArray();
    }

    public async Task Execute()
    {
        foreach (var installer in _installers)
        {
            if (installer.System is not IForeachSystem system)
                continue;

            var context = installer.Context;
            var entities = context.GetEntities();

            EntityContexts.Clear();
            // push async local context for changes entity contexts 

            foreach (var entity in entities)
            {
                try
                {
                    await system.Do(entity);
                }
                catch (Exception e)
                {
                    // tmp
                    Console.WriteLine(e);
                }
            }
            
            // save changes
            EntityContexts.Save();
        }
    }
}