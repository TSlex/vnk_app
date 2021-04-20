import { NuxtAxiosInstance } from "@nuxtjs/axios";
import { AttributeGetDTO, AttributePostDTO, AttributePatchDTO, AttributeDetailsGetDTO } from "~/models/AttributeDTO";
import { CollectionDTO } from "~/models/Common/CollectionDTO";
import { SortOption } from "~/models/Enums/SortOption";
import { EmptyResponseDTO } from "~/models/Responses/EmptyResponseDTO";
import { ResponseDTO } from "~/models/Responses/ResponseDTO";
import { BaseRepo } from "./BaseRepo";

export class AttributesRepo extends BaseRepo {
  constructor(axios: NuxtAxiosInstance) {
    super(axios, "attributes");
  }

  async getAll(pageIndex: number, itemsOnPage: number, byName: SortOption, byType: SortOption, searchKey: string | null) {
    return await this._get<ResponseDTO<CollectionDTO<AttributeGetDTO>>>(this.baseURL, undefined, {
      params: {
        pageIndex: pageIndex,
        itemsOnPage: itemsOnPage,
        byName: byName,
        byType: byType,
        searchKey: searchKey
      }
    });
  }

  async getById(id: number) {
    return await this._get<ResponseDTO<AttributeDetailsGetDTO>>(`${this.baseURL}/${id}`);
  }

  async add(model: AttributePostDTO) {
    return await this._post<ResponseDTO<AttributeGetDTO>>(this.baseURL, model);
  }

  async update(id: number, model: AttributePatchDTO) {
    return await this._patch<EmptyResponseDTO>(`${this.baseURL}/${id}`, model);
  }
}
