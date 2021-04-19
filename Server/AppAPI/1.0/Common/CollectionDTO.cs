using System.Collections.Generic;

namespace AppAPI._1._0.Common
{
    public class CollectionDTO<TKey>
    {
        public long TotalCount { get; set; }
        public IEnumerable<TKey> Items { get; set; } = default!;
    }
}