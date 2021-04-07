<template>
  <div class="text-h6 font-weight-medium" v-if="isAuthenticated">{{fullName}}
    <v-chip v-if="isAdministrator">Администратор</v-chip>
    <v-chip v-if="isRoot">Супер Администратор</v-chip>
    <v-chip v-if="!isAdministrator && !isRoot">Пользователь</v-chip>
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

  get fullName(){
    return identityStore.fullName
  }

  get isAdministrator(){
    return identityStore.isAdministrator
  }

  get isRoot(){
    return identityStore.isRoot
  }
}
</script>
