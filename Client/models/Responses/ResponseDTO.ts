export interface ResponseDTO<TKey> {
  data: TKey;
  error?: string;
}
