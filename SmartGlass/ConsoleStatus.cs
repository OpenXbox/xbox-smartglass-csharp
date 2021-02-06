using System.Collections.Generic;

namespace SmartGlass
{
    public record ConsoleStatus
    {
        public ConsoleConfiguration Configuration { get; internal set; }
        public IReadOnlyCollection<ActiveTitle> ActiveTitles { get; internal set; }
    }
}