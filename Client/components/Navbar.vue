<template>
  <v-app-bar app flat>
    <NavAuthInfo />
    <template v-if="isAdministrator">
      <v-spacer></v-spacer>
      <NavButton to="/">Календарь</NavButton>
      <NavButton to="/orders">Заказы</NavButton>
      <NavButton to="/attributes">Атрибуты</NavButton>
      <NavButton to="/types">Типы атрибутов</NavButton>
      <NavButton to="/templates">Шаблоны</NavButton>
      <NavButton to="/users">Пользователи</NavButton>
    </template>
    <!-- <template v-else>
      <NavButton to="/">Календарь</NavButton>
    </template> -->

    <!-- <NavButton to="/settings">Настройки</NavButton> -->

    <v-spacer></v-spacer>
    <NavAuth />
  </v-app-bar>
</template>

<script lang="ts">
import NavAuth from "@/components/NavAuth.vue";
import NavAuthInfo from "@/components/NavAuthInfo.vue";
import NavButton from "@/components/NavButton.vue";

import { Component, Vue } from "nuxt-property-decorator";
import { identityStore } from "~/store";

@Component({
  components: {
    NavAuth,
    NavAuthInfo,
    NavButton,
  },
})
export default class NavBar extends Vue {
  get isAdministrator() {
    return identityStore.isAdministrator || identityStore.isRoot;
  }
}
</script>

