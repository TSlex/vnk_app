import { SortOption } from './../models/Enums/SortOption';
import { NuxtAxiosInstance } from "@nuxtjs/axios";
import { CollectionDTO } from "~/models/Common/CollectionDTO";
import { EmptyResponseDTO } from "~/models/Responses/EmptyResponseDTO";
import { ResponseDTO } from "~/models/Responses/ResponseDTO";
import { TemplateGetDTO, TemplatePostDTO, TemplatePatchDTO } from "~/models/TemplateDTO";
import { BaseRepo } from "./BaseRepo";

export class TemplatesRepo extends BaseRepo {
  constructor(axios: NuxtAxiosInstance) {
    super(axios, "templates");
  }

  async getAll(pageIndex: number, itemsOnPage: number, byName: SortOption, searchKey: string | null) {
    return await this._get<ResponseDTO<CollectionDTO<TemplateGetDTO>>>(this.baseURL, undefined, {
      params: {
        pageIndex: pageIndex,
        itemsOnPage: itemsOnPage,
        byName: byName,
        searchKey: searchKey
      }
    });
  }

  async getById(id: number) {
    return await this._get<ResponseDTO<TemplateGetDTO>>(`${this.baseURL}/${id}`);
  }

  async add(model: TemplatePostDTO) {
    return await this._post<ResponseDTO<TemplateGetDTO>>(this.baseURL, model);
  }

  async update(id: number, model: TemplatePatchDTO) {
    return await this._patch<EmptyResponseDTO>(`${this.baseURL}/${id}`, model);
  }

  async delete(id: number) {
    return await this._delete<ResponseDTO<TemplateGetDTO>>(`${this.baseURL}/${id}`);
  }
}
