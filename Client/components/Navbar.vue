<template>
  <v-app-bar app flat>
    <NavAuthInfo />
    <v-spacer></v-spacer>
    <NavButton>Календарь</NavButton>
    <NavButton>Структура</NavButton>
    <NavButton>Настройки</NavButton>
    <v-spacer></v-spacer>
    <NavAuth />
  </v-app-bar>
</template>

<script>
import NavAuth from "@/components/NavAuth";
import NavAuthInfo from "@/components/NavAuthInfo";
import NavButton from "@/components/NavButton";

export default {
  components: {
    NavAuth,
    NavAuthInfo,
    NavButton,
  },
};
</script>

<style>
</style>
