<template>
  <v-col>
    <v-toolbar flat>
      <v-btn text>Добавить пользователя</v-btn>
    </v-toolbar>
    <v-divider></v-divider>
    <div class="d-flex justify-space-between pa-2" v-for="n in 10" :key="n">
      <span>Александр Иванов</span>
      <v-chip small>
        <template v-if="n % 3 == 0">Системный администратор</template>
        <template v-else>Пользователь</template>
      </v-chip>
    </div>
  </v-col>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";

@Component({})
export default class UsersList extends Vue {}
</script>

