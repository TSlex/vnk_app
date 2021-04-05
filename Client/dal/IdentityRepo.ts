import { NuxtAxiosInstance } from '@nuxtjs/axios'
import { AxiosError } from 'axios'
import { LoginDTO } from '~/types/Identity/LoginDTO';
import { ResponseDTO } from '~/types/Responses/ResponseDTO';
import BaseRepo from './BaseRepo';

export default class IdentityRepo extends BaseRepo {

  constructor(axios: NuxtAxiosInstance) {
    super(axios, "identity");
  }

  async login(loginDTO: LoginDTO) {
    return this._post<ResponseDTO<string>>(`${this.baseURL}/login`, loginDTO)
  }

  async getAllUsers() {
    return await this.axios.$get(`${this.baseURL}/users`);
  }

  async getUserById(id: number) {
    return await this.axios.$get(`${this.baseURL}/users/${id}`);
  }

  async addUser() {
    return await this.axios.$post(`${this.baseURL}/users`);
  }

  async updateUser(id: number) {
    return await this.axios.$patch(`${this.baseURL}/users/${id}`);
  }

  async deleteUser(id: number) {
    return await this.axios.$delete(`${this.baseURL}/users/${id}`);
  }
}

