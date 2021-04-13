import { NuxtAxiosInstance } from "@nuxtjs/axios";
import { BaseRepo } from "./BaseRepo";

export class AttributesRepo extends BaseRepo {
  constructor(axios: NuxtAxiosInstance) {
    super(axios, "attributes");
  }
}
