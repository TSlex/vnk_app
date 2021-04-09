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
import { commonStore, identityStore } from "~/store"

@Component({})
export default class DafultLayout extends Vue {
  mounted() {
    commonStore.checkServer().then((online) => {
      if (online){
        identityStore.initializeIdentity()
      }
      else{
        this.$router.push("/offline")
      }
    })
  }
}
</script>
