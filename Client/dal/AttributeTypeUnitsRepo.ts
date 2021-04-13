import { EmptyResponseDTO } from '~/models/Responses/EmptyResponseDTO';
import { NuxtAxiosInstance } from '@nuxtjs/axios'
import { ResponseDTO } from '~/models/Responses/ResponseDTO';
import {BaseRepo} from './BaseRepo';
import { AttributeTypeUnitGetDTO, AttributeTypeUnitPatchDTO, AttributeTypeUnitPostDTO } from '~/models/AttributeTypeUnitDTO';

export class AttributeTypeUnitsRepo extends BaseRepo {

  constructor(axios: NuxtAxiosInstance) {
    super(axios, "attributetypes/units");
  }

  async add(model: AttributeTypeUnitPostDTO) {
    return await this._post<ResponseDTO<AttributeTypeUnitGetDTO>>(this.baseURL, model);
  }

  async update(id: number, model: AttributeTypeUnitPatchDTO) {
    return await this._patch<EmptyResponseDTO>(`${this.baseURL}/${id}`, model);
  }

  async delete(id: number) {
    return await this._delete<EmptyResponseDTO>(`${this.baseURL}/${id}`);
  }
}
