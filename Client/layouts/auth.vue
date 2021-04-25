<template>
  <v-app>
    <v-main class="grey lighten-3">
      <v-container fluid class="fill-height">
        <Nuxt />
      </v-container>
    </v-main>
  </v-app>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";

@Component({
  middleware: ["server"],
})
export default class AuthLayout extends Vue {}
</script>


