import { NuxtAxiosInstance } from "@nuxtjs/axios";
import { BaseRepo } from "./BaseRepo";

export class TemplatesRepo extends BaseRepo {
  constructor(axios: NuxtAxiosInstance) {
    super(axios, "templates");
  }
}
