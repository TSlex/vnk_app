<template>
  <v-app>
    <Navbar />

    <v-main class="grey lighten-3">
      <v-container fluid>
        <Nuxt />
      </v-container>
    </v-main>
  </v-app>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";

@Component({
  middleware: ["server", "auth"],
})
export default class DafultLayout extends Vue {}
</script>
