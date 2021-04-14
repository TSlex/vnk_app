import { EmptyResponseDTO } from '~/models/Responses/EmptyResponseDTO';
import { NuxtAxiosInstance } from '@nuxtjs/axios'
import { ResponseDTO } from '~/models/Responses/ResponseDTO';
import {BaseRepo} from './BaseRepo';
import { TemplateAttributePostDTO, TemplateAttributeGetDTO, TemplateAttributePatchDTO } from '~/models/TemplateDTO';

export class TemplateAttributesRepo extends BaseRepo {

  constructor(axios: NuxtAxiosInstance) {
    super(axios, "templates/attributes");
  }

  async add(templateId: number, model: TemplateAttributePostDTO) {
    return await this._post<ResponseDTO<TemplateAttributeGetDTO>>(`templates/${templateId}/attributes`, model);
  }

  async update(id: number, model: TemplateAttributePatchDTO) {
    return await this._patch<EmptyResponseDTO>(`${this.baseURL}/${id}`, model);
  }

  async delete(id: number) {
    return await this._delete<EmptyResponseDTO>(`${this.baseURL}/${id}`);
  }
}
