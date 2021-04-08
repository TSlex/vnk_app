<template>
  <div>
    <NavButton v-if="isAuthenticated" @click.native="onLogout()">Выйти</NavButton>
    <NavButton v-else to="/auth/login">Войти</NavButton>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { identityStore } from "~/store";

@Component({})
export default class NavAuth extends Vue {
  get isAuthenticated(){
    return identityStore.isAuthenticated
  }

  onLogout(){
    this.$router.push("/auth/login")
    identityStore.logout()
  }
}
</script>
