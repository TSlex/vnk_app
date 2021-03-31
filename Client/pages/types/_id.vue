<template>
  <v-row justify="center" class="text-center">
    <v-col cols="4" class="mt-4">
      <v-sheet rounded="lg" class="pa-3">
        <div class="d-flex justify-space-between">
          <span>Название:</span
          ><span>ALL<v-icon dense>mdi-pencil</v-icon></span>
        </div>
        <div class="d-flex justify-space-between">
          <span>Количество использований:</span><span>23</span>
        </div>
        <div class="d-flex justify-space-between">
          <span>Значение по умолчанию:</span><span>BADABUI</span>
        </div>
      </v-sheet>
      <v-sheet rounded="lg" class="mt-4 pt-1">
        <v-toolbar flat dense>
          <v-btn text>Добавить</v-btn>
          <v-spacer></v-spacer>
          <v-toolbar-title>Значения</v-toolbar-title>
          <v-spacer></v-spacer>
        </v-toolbar>
        <v-divider></v-divider>
        <div class="d-flex justify-space-between pa-2">
          <span>Москва</span><v-chip small>по-умолчанию</v-chip>
        </div>
        <v-divider></v-divider>
        <div class="d-flex justify-space-between pa-2" v-for="n in 4" :key="n">
          <span>Питер</span>
        </div>
      </v-sheet>
      <v-sheet rounded="lg" class="mt-4 pt-1">
        <v-toolbar flat>
          <v-btn text>Добавить</v-btn>
          <v-spacer></v-spacer>
          <v-toolbar-title>Единицы измерения</v-toolbar-title>
          <v-spacer></v-spacer>
        </v-toolbar>
        <v-divider></v-divider>
        <div class="d-flex justify-space-between pa-2">
          <span>Тонна</span><v-chip small>по-умолчанию</v-chip>
        </div>
        <v-divider></v-divider>
        <div class="d-flex justify-space-between pa-2" v-for="n in 4" :key="n">
          <span>Грамм</span>
        </div>
      </v-sheet>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";

@Component({})
export default class extends Vue {}
</script>


