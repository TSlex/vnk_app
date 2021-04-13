import { NuxtAxiosInstance } from "@nuxtjs/axios";
import { BaseRepo } from "./BaseRepo";

export class OrdersRepo extends BaseRepo {
  constructor(axios: NuxtAxiosInstance) {
    super(axios, "orders");
  }
}
