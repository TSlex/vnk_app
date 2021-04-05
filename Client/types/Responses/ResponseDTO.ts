export interface ResponseDTO<TKey> {
  data: TKey;
  message: string;
  errorMessage: string;
  errorKeys: {
    key: string,
    value: string
  }[]
}
