<template>
  <v-row justify="center" class="text-center">
    <v-col cols="4" class="mt-4">
      <v-sheet rounded="lg">
        <span>Название: ALL</span>
        <span>Количество использований: 23</span>
        <span>Значение по умолчанию: BADABUI</span>
      </v-sheet>
      <v-sheet rounded="lg" class="mt-4">
        <v-toolbar>
          <v-toolbar-title>Значения</v-toolbar-title>
        </v-toolbar>
        <span>Название: ALL</span>
        <span>Количество использований: 23</span>
        <span>Значение по умолчанию: BADABUI</span>
      </v-sheet>
      <v-sheet rounded="lg" class="mt-4">
        <v-toolbar>
          <v-toolbar-title>Единицы измерения</v-toolbar-title>
        </v-toolbar>
        <span>Название: ALL</span>
        <span>Количество использований: 23</span>
        <span>Значение по умолчанию: BADABUI</span>
      </v-sheet>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";

@Component({})
export default class extends Vue {}
</script>


