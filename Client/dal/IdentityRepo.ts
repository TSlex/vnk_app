import { CollectionDTO } from '~/models/Common/CollectionDTO';
import { EmptyResponseDTO } from '~/models/Responses/EmptyResponseDTO';
import { NuxtAxiosInstance } from '@nuxtjs/axios'
import { LoginDTO } from '~/models/Identity/LoginDTO';
import { UserGetDTO, UserPasswordPatchDTO, UserPatchDTO, UserPostDTO, UserRolePatchDTO } from '~/models/Identity/UserDTO';
import { ResponseDTO } from '~/models/Responses/ResponseDTO';
import BaseRepo from './BaseRepo';

export default class IdentityRepo extends BaseRepo {

  constructor(axios: NuxtAxiosInstance) {
    super(axios, "identity");
  }

  async login(loginDTO: LoginDTO) {
    return await this._post<ResponseDTO<string>>(`${this.baseURL}/login`, loginDTO)
  }

  async getAllUsers() {
    return await this._get<ResponseDTO<CollectionDTO<UserGetDTO>>>(`${this.baseURL}/users`);
  }

  async getCurrentUser() {
    return await this._get<ResponseDTO<UserGetDTO>>(`${this.baseURL}/users/current`);
  }

  async getUserById(id: number) {
    return await this._get<ResponseDTO<UserGetDTO>>(`${this.baseURL}/users/${id}`);
  }

  async addUser(model: UserPostDTO) {
    return await this._post<EmptyResponseDTO>(`${this.baseURL}/users`, model);
  }

  async updateUser(id: number, model: UserPatchDTO) {
    return await this._patch<EmptyResponseDTO>(`${this.baseURL}/users/${id}`, model);
  }

  async updateUserPassword(id: number, model: UserPasswordPatchDTO) {
    return await this._patch<EmptyResponseDTO>(`${this.baseURL}/users/${id}/password`, model);
  }

  async updateUserRole(id: number, model: UserRolePatchDTO) {
    return await this._patch<EmptyResponseDTO>(`${this.baseURL}/users/${id}/role`, model);
  }

  async deleteUser(id: number) {
    return await this._delete<EmptyResponseDTO>(`${this.baseURL}/users/${id}`);
  }
}

