namespace bi_dict_api.Utils.DefinitionProvider
{
    using bi_dict_api.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class DefinitionProviderGroup : IDefinitionProvider
    {
        private readonly List<IDefinitionProvider> providers;
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        public DefinitionProviderGroup(IEnumerable<IDefinitionProvider> providers)
        {
            this.providers = providers.ToList();
        }

        public async Task<Definition> Get(string word)
        {
            var getDefinitionTasks = providers.Select(provider => Task.Run(() => provider.Get(word), tokenSource.Token))
                                              .ToList();

            foreach (var task in getDefinitionTasks)
            {
                try
                {
                    var definition = await task;
                    tokenSource.Cancel();
                    tokenSource.Dispose();
                    return definition;
                }
                catch (DefinitionException e) //TODO: Actual Logging system
                {
                    Console.WriteLine(e);
                }
            };

            tokenSource.Dispose();
            throw new Exception($"Failed to get translation of {word}");
        }
    }
}
