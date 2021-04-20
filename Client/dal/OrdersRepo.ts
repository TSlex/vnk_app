import { NuxtAxiosInstance } from "@nuxtjs/axios";
import { CollectionDTO } from "~/models/Common/CollectionDTO";
import { SortOption } from "~/models/Enums/SortOption";
import { OrderCompletionPatchDTO, OrderGetDTO, OrderPatchDTO, OrderPostDTO } from "~/models/OrderDTO";
import { EmptyResponseDTO } from "~/models/Responses/EmptyResponseDTO";
import { ResponseDTO } from "~/models/Responses/ResponseDTO";
import { BaseRepo } from "./BaseRepo";

export class OrdersRepo extends BaseRepo {
  constructor(axios: NuxtAxiosInstance) {
    super(axios, "orders");
  }

  async getAllWithDate(
    pageIndex: number, itemsOnPage: number, byName: SortOption, completed: boolean | null,
    searchKey: string | null, startDatetime: Date | null, endDatetime: Date | null,
    checkDatetime: Date | null
  ) {
    return await this._get<ResponseDTO<CollectionDTO<OrderGetDTO>>>(this.baseURL, undefined, {
      params: {
        pageIndex: pageIndex,
        itemsOnPage: itemsOnPage,
        byName: byName,
        completed: completed,
        searchKey: searchKey,
        startDatetime: startDatetime,
        endDatetime: endDatetime,
        checkDatetime: checkDatetime
      }
    });
  }

  async getAllWithoutDate(
    pageIndex: number, itemsOnPage: number, byName: SortOption, completed: boolean | null,
    searchKey: string | null) {
    return await this._get<ResponseDTO<CollectionDTO<OrderGetDTO>>>(`${this.baseURL}/nodate`, undefined, {
      params: {
        pageIndex: pageIndex,
        itemsOnPage: itemsOnPage,
        byName: byName,
        completed: completed,
        searchKey: searchKey
      }
    });
  }

  async getHistory(id: number, pageIndex: number, itemsOnPage: number) {
    return await this._get<ResponseDTO<CollectionDTO<OrderGetDTO>>>(`${this.baseURL}/${id}/history`, undefined, {
      params: {
        pageIndex: pageIndex,
        itemsOnPage: itemsOnPage,
      }
    });
  }

  async getById(id: number, checkDatetime: Date | null) {
    return await this._get<ResponseDTO<OrderGetDTO>>(`${this.baseURL}/${id}`, undefined, {
      params: {
        checkDatetime: checkDatetime
      }
    });
  }

  async add(model: OrderPostDTO) {
    return await this._post<ResponseDTO<OrderGetDTO>>(this.baseURL, model);
  }

  async update(id: number, model: OrderPatchDTO) {
    return await this._patch<EmptyResponseDTO>(`${this.baseURL}/${id}`, model);
  }

  async updateCompletion(id: number, model: OrderCompletionPatchDTO) {
    return await this._patch<EmptyResponseDTO>(`${this.baseURL}/${id}/completion`, model);
  }

  async delete(id: number) {
    return await this._delete<ResponseDTO<OrderGetDTO>>(`${this.baseURL}/${id}`);
  }
}
