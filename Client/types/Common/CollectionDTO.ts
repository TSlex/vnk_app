export interface CollectionDTO<Tkey> {
  pageIndex: number;
  totalCount: number;
  items: Tkey[];
}
