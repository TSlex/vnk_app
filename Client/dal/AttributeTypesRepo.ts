import { NuxtAxiosInstance } from '@nuxtjs/axios'

export default class AttributeTypesRepo{
  axios: NuxtAxiosInstance;
  resourseURL = "attributetypes"

  constructor(axios: NuxtAxiosInstance){
    this.axios = axios;
  }

  async getAll() {
    return this.axios.$get(this.resourseURL);
  }

  async getById() {

  }

  async add() {

  }

  async edit() {

  }

  async delete() {

  }
}
