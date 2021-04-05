<template>
  <v-row justify="center" align-content="center" dense class="fill-height">
    <v-col lg="4" md="6" sm="6">
      <v-card class="text-center pa-10">
        <h1 class="text-h4 font-weight-bold mb-4">Войти в систему</h1>
        <span class="info--text">
          <v-icon color="info" size="16">mdi-information-outline</v-icon>
          Для регистрации обратитесь к администратору
        </span>
        <v-form class="mt-6" @submit.prevent="login()">
          <v-text-field
            label="Эл. адрес"
            v-model="model.email"
            required
          ></v-text-field>
          <v-text-field
            :type="showPassword ? 'text' : 'password'"
            :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
            label="Пароль"
            v-model="model.password"
            required
            @click:append="showPassword = !showPassword"
          ></v-text-field>
          <v-btn rounded block large color="primary mb-6" type="submit"
            >Войти</v-btn
          >
          <nuxt-link to="/" class="text-decoration-none">На главную</nuxt-link>
        </v-form>
      </v-card>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { identityStore } from "~/store";
import { LoginDTO } from "~/types/Identity/LoginDTO";
import { ResponseAnyDTO } from "~/types/Responses/ResponseDTO";

@Component({})
export default class Login extends Vue {
  showPassword = false;

  model: LoginDTO = {
    email: "aaaa",
    password: "ccc",
  };

  login() {
    identityStore.login(this.model).then((response: ResponseAnyDTO) => {
      // if (!response.errorMessage){
      //   this.$router.push("/")
      // }
    })
  }

  layout() {
    return "auth";
  }
}
</script>

<style>
</style>
