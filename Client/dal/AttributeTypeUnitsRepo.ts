import { EmptyResponseDTO } from './../types/Responses/EmptyResponseDTO';
import { CollectionDTO } from './../types/Common/CollectionDTO';
import { NuxtAxiosInstance } from '@nuxtjs/axios'
import { ResponseDTO } from '~/models/Responses/ResponseDTO';
import BaseRepo from './BaseRepo';
import { AttributeTypeGetDetailsDTO, AttributeTypeGetDTO, AttributeTypePatchDTO, AttributeTypePostDTO } from '~/models/AttributeTypeDTO';

export default class AttributeTypeUnitsRepo extends BaseRepo {

  constructor(axios: NuxtAxiosInstance) {
    super(axios, "attributetypesunits");
  }

  async add(model: AttributeTypePostDTO) {
    return await this._post<ResponseDTO<AttributeTypeGetDTO>>(this.baseURL, model);
  }

  async update(id: number, model: AttributeTypePatchDTO) {
    return await this._patch<EmptyResponseDTO>(`${this.baseURL}/${id}`, model);
  }

  async delete() {

  }
}
