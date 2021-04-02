import { NuxtAxiosInstance } from '@nuxtjs/axios'
import BaseRepo from './BaseRepo';

export default class IdentityRepo extends BaseRepo {

  constructor(axios: NuxtAxiosInstance) {
    super(axios, "identity");
  }

  async login(){

  }

  async getAllUsers(){

  }

  async getUserById(){

  }

  async addUser(){

  }

  async updateUser(){

  }

  async deleteUser(){

  }
}

