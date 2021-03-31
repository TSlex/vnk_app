<template>
  <v-app-bar app flat>
    <NavAuthInfo />
    <v-spacer></v-spacer>
    <NavButton to="/">Календарь</NavButton>
    <NavButton to="/orders">Заказы</NavButton>
    <NavButton to="/attributes">Атрибуты</NavButton>
    <NavButton>Шаблоны</NavButton>
    <NavButton>Типы атрибутов</NavButton>
    <NavButton>Единицы измерения</NavButton>
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
