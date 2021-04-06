export interface ResponseDTO<TKey> {
  data: TKey;
  error: string;
}

export interface ResponseAnyDTO extends ResponseDTO<any>{
}
