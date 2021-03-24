<template>
  <v-row justify="center" align-content="center" dense class="fill-height">
    <v-col lg="4" md="6" sm="6">
      <v-card class="text-center pa-10">
        <h1 class="text-h4 font-weight-bold mb-4">Войти в систему</h1>
        <span class="info--text">
          <v-icon color="info" size="16">mdi-information-outline</v-icon>
          Для регистрации обратитесь к администратору
          </span>
        <v-form class="mt-6">
          <v-text-field label="Эл. адрес" required></v-text-field>
          <v-text-field label="Пароль" required></v-text-field>
          <v-checkbox label="Запомнить меня"></v-checkbox>
          <v-btn rounded block large color="primary mb-6">Войти</v-btn>
          <nuxt-link to="/" class="text-decoration-none">На главную</nuxt-link>
        </v-form>
      </v-card>
    </v-col>
  </v-row>
</template>

<script>
export default {
  layout: "auth",
};
</script>

<style>
</style>
