import { NuxtAxiosInstance } from '@nuxtjs/axios'
import BaseRepo from './BaseRepo';

export default class AttributeTypesRepo extends BaseRepo {

  constructor(axios: NuxtAxiosInstance) {
    super(axios, "attributetypes");
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
