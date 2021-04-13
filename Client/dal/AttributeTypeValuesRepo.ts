
import { EmptyResponseDTO } from '~/models/Responses/EmptyResponseDTO';
import { NuxtAxiosInstance } from '@nuxtjs/axios'
import { ResponseDTO } from '~/models/Responses/ResponseDTO';
import BaseRepo from './BaseRepo';
import { AttributeTypeValuePostDTO, AttributeTypeValueGetDTO, AttributeTypeValuePatchDTO } from '~/models/AttributeTypeValueDTO';

export default class AttributeTypeValuesRepo extends BaseRepo {

  constructor(axios: NuxtAxiosInstance) {
    super(axios, "attributetypesvalues");
  }

  async add(model: AttributeTypeValuePostDTO) {
    return await this._post<ResponseDTO<AttributeTypeValueGetDTO>>(this.baseURL, model);
  }

  async update(id: number, model: AttributeTypeValuePatchDTO) {
    return await this._patch<EmptyResponseDTO>(`${this.baseURL}/${id}`, model);
  }

  async delete(id: number) {
    return await this._delete<EmptyResponseDTO>(`${this.baseURL}/${id}`);
  }
}
