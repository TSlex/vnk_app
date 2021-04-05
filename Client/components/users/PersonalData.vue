<template>
  <v-col>
    <div class="py-6">
      <div class="d-flex justify-space-between">
        <span>Имя:</span><span>Александр</span>
      </div>
      <div class="d-flex justify-space-between">
        <span>Фамилия:</span><span>Иванов</span>
      </div>
      <div class="d-flex justify-space-between">
        <span>Эл.адрес:</span><span>aleksi@vnk.ee</span>
      </div>
      <div class="d-flex justify-space-between">
        <span>Роль:</span><span>Системный администратор</span>
      </div>
    </div>
    <v-divider></v-divider>
    <v-btn class="mt-3 mr-2">Изменить</v-btn>
    <v-btn class="mt-3">Удалить</v-btn>
  </v-col>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";

@Component({})
export default class PersonalData extends Vue {}
</script>

