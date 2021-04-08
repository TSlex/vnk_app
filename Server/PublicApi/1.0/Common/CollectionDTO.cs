using System.Collections.Generic;

namespace PublicApi.v1.Common
{
    public class CollectionDTO<TKey>
    {
        public long TotalCount { get; set; }
        public IEnumerable<TKey> Items { get; set; } = default!;
    }
}