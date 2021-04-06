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
import { Component, Vue } from "nuxt-property-decorator"
import { identityStore } from "~/store"

@Component({})
export default class DafultLayout extends Vue {
  mounted() {
    identityStore.initializeIdentity()
  }
}
</script>
