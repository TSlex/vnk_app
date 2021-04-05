import { NuxtAxiosInstance } from '@nuxtjs/axios'
import BaseRepo from './BaseRepo';

export default class AttributeTypesRepo extends BaseRepo {

  constructor(axios: NuxtAxiosInstance) {
    super(axios, "attributetypes");
  }

  async getAll() {
    return await this.axios.$get(this.baseURL);
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
