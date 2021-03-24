<template>
  <v-app>
    <Navbar />

    <v-main class="grey lighten-3">
      <v-container fluid>
        <Nuxt />
      </v-container>
    </v-main>
    <!-- <v-footer></v-footer> -->
  </v-app>
</template>

<script>
import Navbar from "@/components/Navbar";

export default {
  comments: {
    Navbar,
  },
  data: () => ({}),
};
</script>
