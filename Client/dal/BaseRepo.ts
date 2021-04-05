import { NuxtAxiosInstance } from "@nuxtjs/axios";

export default class BaseRepo {
  axios: NuxtAxiosInstance;
  baseURL = ""

  constructor(axios: NuxtAxiosInstance, resourseURL: string) {
    this.axios = axios
    this.baseURL = resourseURL
  }
}
