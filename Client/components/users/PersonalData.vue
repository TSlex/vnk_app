<template>
  <v-col>
    <div class="py-6" v-if="fetched">
      <div class="d-flex justify-space-between">
        <span>Имя:</span><span>{{ personalData.firstName }}</span>
      </div>
      <div class="d-flex justify-space-between">
        <span>Фамилия:</span><span>{{ personalData.lastName }}</span>
      </div>
      <div class="d-flex justify-space-between">
        <span>Эл.адрес:</span><span>{{ personalData.email }}</span>
      </div>
      <div class="d-flex justify-space-between">
        <span>Роль:</span><span>{{ personalData.role }}</span>
      </div>
    </div>
    <v-divider></v-divider>
    <v-btn class="mt-3 mr-2">Изменить данные</v-btn>
  </v-col>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { identityStore } from "~/store";

@Component({})
export default class PersonalData extends Vue {
  fetched = false;

  get personalData() {
    return identityStore.userData;
  }

  mounted() {
    identityStore.fetchData().then((_) => (this.fetched = true));
  }
}
</script>

