<template>
  <v-row justify="center" align-content="center" dense class="fill-height">
    <v-col lg="4" md="6" sm="6">
      <v-card class="text-center pa-10">
        <h1 class="text-h4 font-weight-bold mb-4">Войти в систему</h1>
        <span class="info--text">
          <v-icon color="info" size="16">mdi-information-outline</v-icon>
          Для регистрации обратитесь к администратору
        </span>
        <v-form class="mt-6" @submit.prevent="login()" ref="form">
          <v-alert dense text type="error" v-if="showError">{{ loginError }}</v-alert>
          <v-text-field
            label="Эл.адрес"
            v-model.trim="model.email"
            :rules="rules.email"
            required
          ></v-text-field>
          <v-text-field
            :type="showPassword ? 'text' : 'password'"
            :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
            label="Пароль"
            v-model.trim="model.password"
            :rules="rules.password"
            required
            @click:append="showPassword = !showPassword"
          ></v-text-field>
          <v-btn rounded block large color="primary mb-6 mt-4" type="submit"
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
import { LoginDTO } from "~/models/Identity/LoginDTO";
import { validate, required, email, password } from "~/utils/form-validation";

@Component({})
export default class Login extends Vue {

  isMounted = false;
  showPassword = false;
  showError = false;

  model: LoginDTO = {
    email: "",
    password: "",
  };

  rules = {
    email: validate(required, email),
    password: validate(required, password),
  };

  get loginError() {
    return identityStore.loginError;
  }

  login() {
    if ((this.$refs.form as any).validate()) {
      identityStore.login(this.model).then((response) => {
        if (response) {
          this.$router.push("/");
        } else {
          this.showError = true;
        }
      });
    }
  }

  layout() {
    return "auth";
  }

  mounted() {
    this.isMounted = true;
  }
}
</script>

<style>
</style>
