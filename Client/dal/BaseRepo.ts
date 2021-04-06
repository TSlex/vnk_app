import { NuxtAxiosInstance } from "@nuxtjs/axios";
import { AxiosError, AxiosRequestConfig } from 'axios'

export type ErrorCallback = () => void

export default class BaseRepo {
  axios: NuxtAxiosInstance;
  baseURL = ""

  constructor(axios: NuxtAxiosInstance, resourseURL: string) {
    this.axios = axios
    this.baseURL = resourseURL
  }

  protected async _get<TKey>(url: string, onError?: ErrorCallback, config?: AxiosRequestConfig | undefined): Promise<TKey> {
    const response = await this.axios.$get<TKey>(url, config)
      .catch((err: AxiosError) => responseCatch(err, onError))

    return response
  }

  protected async _post<TKey>(url: string, data?: any, onError?: ErrorCallback, config?: AxiosRequestConfig | undefined): Promise<TKey> {
    const response = await this.axios.$post<TKey>(url, data, config)
      .catch((err: AxiosError) => responseCatch(err, onError))

    return response
  }

  protected async _patch<TKey>(url: string, data?: any, onError?: ErrorCallback, config?: AxiosRequestConfig | undefined): Promise<TKey> {
    const response = await this.axios.$patch<TKey>(url, data, config)
      .catch((err: AxiosError) => responseCatch(err, onError))

    return response
  }

  protected async _put<TKey>(url: string, data?: any, onError?: ErrorCallback, config?: AxiosRequestConfig | undefined): Promise<TKey> {
    const response = await this.axios.$put<TKey>(url, data, config)
      .catch((err: AxiosError) => responseCatch(err, onError))

    return response
  }

  protected async _delete<TKey>(url: string, onError?: ErrorCallback, config?: AxiosRequestConfig | undefined): Promise<TKey> {
    const response = await this.axios.$delete<TKey>(url, config)
      .catch((err: AxiosError) => responseCatch(err, onError))

    return response
  }
}

function responseCatch<TKey>(err: AxiosError, onError?: ErrorCallback) {
  if (onError) {
    onError()
  }
  return err.response?.data as TKey
}
