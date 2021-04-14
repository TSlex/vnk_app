import { EmptyResponseDTO } from '~/models/Responses/EmptyResponseDTO';
import { CollectionDTO } from '~/models/Common/CollectionDTO';
import { NuxtAxiosInstance } from '@nuxtjs/axios'
import { ResponseDTO } from '~/models/Responses/ResponseDTO';
import {BaseRepo} from './BaseRepo';
import { AttributeTypeDetailsGetDTO, AttributeTypeGetDTO, AttributeTypePatchDTO, AttributeTypePostDTO } from '~/models/AttributeTypeDTO';

export class AttributeTypesRepo extends BaseRepo {

  constructor(axios: NuxtAxiosInstance) {
    super(axios, "attributetypes");
  }

  async getAll(pageIndex: number, itemsOnPage: number, orderReversed: boolean, searchKey: string | null) {
    return await this._get<ResponseDTO<CollectionDTO<AttributeTypeGetDTO>>>(this.baseURL, undefined, {
      params: {
        pageIndex: pageIndex,
        itemsOnPage: itemsOnPage,
        orderReversed: orderReversed,
        searchKey: searchKey
      }
    });
  }

  async getById(id: number) {
    return await this._get<ResponseDTO<AttributeTypeDetailsGetDTO>>(`${this.baseURL}/${id}`, undefined, {
      params: {
        valuesCount: 100,
        unitsCount: 100,
      }
    });
  }

  async add(model: AttributeTypePostDTO) {
    return await this._post<ResponseDTO<AttributeTypeGetDTO>>(this.baseURL, model);
  }

  async update(id: number, model: AttributeTypePatchDTO) {
    return await this._patch<EmptyResponseDTO>(`${this.baseURL}/${id}`, model);
  }

  // async delete() {

  // }
}
