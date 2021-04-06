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
    return await this._post<ResponseDTO<string>>(`${this.baseURL}/login`, loginDTO)
  }

  async getAllUsers() {
    return await this._get<ResponseDTO<string>>(`${this.baseURL}/users`);
  }

  async getCurrentUser() {
    return await this._get<ResponseDTO<string>>(`${this.baseURL}/users/current`);
  }

  async getUserById(id: number) {
    return await this._get<ResponseDTO<string>>(`${this.baseURL}/users/${id}`);
  }

  async addUser() {
    return await this._post<ResponseDTO<string>>(`${this.baseURL}/users`);
  }

  async updateUser(id: number) {
    return await this._patch<ResponseDTO<string>>(`${this.baseURL}/users/${id}`);
  }

  async deleteUser(id: number) {
    return await this._delete<ResponseDTO<string>>(`${this.baseURL}/users/${id}`);
  }
}

