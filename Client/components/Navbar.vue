<template>
  <v-app-bar app flat>
    <v-btn depressed tile large>Календарь</v-btn>
    <v-btn depressed tile large>Шаблоны</v-btn>
    <v-btn depressed tile large>Аттрибуты</v-btn>
    <v-spacer></v-spacer>
    <NavAuth />
  </v-app-bar>
</template>

<script>
import NavAuth from "@/components/NavAuth";

export default {
  components: {
    NavAuth,
  },
};
</script>

<style>
</style>
