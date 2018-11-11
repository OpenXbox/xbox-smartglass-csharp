using System.Collections.Generic;

namespace SmartGlass
{
    public class ConsoleStatus
    {
        public ConsoleConfiguration Configuration { get; internal set; }
        public IReadOnlyCollection<ActiveTitle> ActiveTitles { get; internal set; }
    }
}