import { NuxtAxiosInstance } from "@nuxtjs/axios";

export default class BaseRepo {
  axios: NuxtAxiosInstance;
  resourseURL = ""

  constructor(axios: NuxtAxiosInstance, resourseURL: string) {
    this.axios = axios
    this.resourseURL = resourseURL
  }
}
